﻿using System.Collections.Frozen;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using C64AssemblerStudio.Core;
using C64AssemblerStudio.Core.Common;
using C64AssemblerStudio.Core.Services.Abstract;
using C64AssemblerStudio.Engine.Common;
using C64AssemblerStudio.Engine.Messages;
using C64AssemblerStudio.Engine.Models;
using C64AssemblerStudio.Engine.Models.Configuration;
using C64AssemblerStudio.Engine.Services.Abstract;
using C64AssemblerStudio.Engine.Services.Implementation;
using C64AssemblerStudio.Engine.ViewModels.Projects;
using C64AssemblerStudio.Engine.ViewModels.Tools;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Righthand.MessageBus;
using Righthand.RetroDbgDataProvider.KickAssembler.Models;
using Righthand.ViceMonitor.Bridge.Responses;

namespace C64AssemblerStudio.Engine.ViewModels.Breakpoints;

using SourceLineBlockItemsMap = FrozenDictionary<string, FrozenDictionary<int, ImmutableArray<BlockItem>>>;
using LabelsNameMap = FrozenDictionary<string, Righthand.RetroDbgDataProvider.Models.Program.Label>;

public record AddressRange(ushort StartAddress, ushort Length)
{
    public static AddressRange FromRange(ushort startAddress, ushort endAddress)
        => new AddressRange(startAddress, (ushort)(endAddress - startAddress + 1));

    public ushort EndAddress => (ushort)(StartAddress + Length - 1);
    public bool IsAddressInRange(ushort address) => address >= StartAddress && address <= EndAddress;
}

public record BreakpointLineKey(string FilePath, int Line);

public class BreakpointsViewModel : NotifiableObject, IToolView
{
    public enum BreakPointContextColumn
    {
        Binding,
        Other
    }

    public record BreakPointContext(BreakpointViewModel ViewModel, BreakPointContextColumn Column);

    private const int BreakpointPropertiesDefaultHeight = 360;
    public string Header => "Breakpoints";

    private readonly ILogger<BreakpointsViewModel> _logger;
    private readonly IDispatcher _dispatcher;
    private readonly Globals _globals;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IVice _vice;
    private readonly ISettingsManager _settingsManager;
    private readonly CommandsManager _commandsManager;
    private readonly DebugOutputViewModel _debugOutput;
    private readonly IAddressEntryGrammarService _addressEntryGrammar;
    private readonly IOsDependent _osDependent;
    public ObservableCollection<BreakpointViewModel> Breakpoints { get; }

    /// <summary>
    /// Maps breakpoints by checkpoint number
    /// </summary>
    private readonly Dictionary<uint, BreakpointViewModel> _breakpointsMap = new();

    private readonly Dictionary<BreakpointLineKey, List<BreakpointViewModel>> _breakpointsLinesMap = new();

    private readonly TaskFactory _uiFactory;
    public RelayCommandWithParameterAsync<BreakpointViewModel> ToggleBreakpointEnabledCommand { get; }
    public RelayCommandWithParameterAsync<BreakpointViewModel> ShowBreakpointPropertiesCommand { get; }
    public RelayCommandWithParameterAsync<BreakPointContext> BreakPointContextCommand { get; }
    public RelayCommandWithParameterAsync<BreakpointViewModel> RemoveBreakpointCommand { get; }
    public RelayCommandAsync RemoveAllBreakpointsCommand { get; }
    public RelayCommandAsync CreateBreakpointCommand { get; }
    public bool IsWorking { get; private set; }
    public bool IsProjectOpen => _globals.IsProjectOpen;

    /// <summary>
    /// When true, it shouldn't update breakpoints settings
    /// </summary>
    private bool _suppressLocalPersistence;

    private SourceLineBlockItemsMap? _sourceFiles;
    private LabelsNameMap? _labels;

    public BreakpointsViewModel(ILogger<BreakpointsViewModel> logger, IVice vice, IDispatcher dispatcher,
        Globals globals,
        IServiceScopeFactory serviceScopeFactory, ISettingsManager settingsManager, DebugOutputViewModel debugOutput,
        IAddressEntryGrammarService addressEntryGrammar, IOsDependent osDependent)
    {
        _logger = logger;
        _dispatcher = dispatcher;
        _globals = globals;
        _vice = vice;
        _serviceScopeFactory = serviceScopeFactory;
        _settingsManager = settingsManager;
        _debugOutput = debugOutput;
        _addressEntryGrammar = addressEntryGrammar;
        _osDependent = osDependent;
        _uiFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        _commandsManager = new CommandsManager(this, _uiFactory);
        Breakpoints = new ObservableCollection<BreakpointViewModel>();
        Breakpoints.CollectionChanged += Breakpoints_CollectionChanged;
        // breakpointsLinesMap = new Dictionary<PdbLine, List<BreakpointViewModel>>();
        // breakpointsMap = new Dictionary<uint, BreakpointViewModel>();
        ToggleBreakpointEnabledCommand =
            _commandsManager.CreateRelayCommandWithParameterAsync<BreakpointViewModel>(ToggleBreakpointEnabledAsync);
        ShowBreakpointPropertiesCommand =
            _commandsManager.CreateRelayCommandWithParameterAsync<BreakpointViewModel>(ShowBreakpointPropertiesAsync);
        BreakPointContextCommand =
            _commandsManager.CreateRelayCommandWithParameterAsync<BreakPointContext>(BreakPointContextAsync);
        RemoveBreakpointCommand =
            _commandsManager.CreateRelayCommandWithParameterAsync<BreakpointViewModel>(RemoveBreakpointAsync);
        RemoveAllBreakpointsCommand =
            _commandsManager.CreateRelayCommandAsync(RemoveAllBreakpointsAsync, () => IsProjectOpen);
        CreateBreakpointCommand = _commandsManager.CreateRelayCommandAsync(CreateBreakpoint, () => IsProjectOpen);
        _vice.CheckpointInfoUpdated += ViceOnCheckpointInfoUpdated;
        _vice.PropertyChanged += ViceOnPropertyChanged;
        globals.PropertyChanged += Globals_PropertyChanged;
    }

    private void ViceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IVice.IsPaused):
                if (!_vice.IsPaused)
                {
                    ClearHitBreakpoint();
                }
                break;
        }
    }

    private void ViceOnCheckpointInfoUpdated(object? sender, CheckpointInfoEventArgs e)
    {
        if (e.Response?.StopWhenHit == true)
        {
            UpdateBreakpointDataFromVice(e.Response);
        }
    }

    async void Breakpoints_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        await SaveLocalSettingsAsync();
    }

    /// <summary>
    /// Returns all breakpoints associated with given line.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <returns>An array of breakpoint viewmodel associated with line</returns>
    /// <remarks>While a line can have only one line breakpoint, it can have multiple unbound ones (TBD)</remarks>
    public ImmutableArray<BreakpointViewModel> GetBreakpointsAssociatedWithLine(string filePath, int lineNumber)
    {
        if (_breakpointsLinesMap.TryGetValue(new BreakpointLineKey(filePath, lineNumber), out var breakpoints))
        {
            return [..breakpoints];
        }

        return ImmutableArray<BreakpointViewModel>.Empty;
    }

    public BreakpointViewModel? GetLineBreakpointForLine(string filePath, int lineNumber)
    {
        var breakpoints = GetBreakpointsAssociatedWithLine(filePath, lineNumber);
        return breakpoints.SingleOrDefault(b => b.Bind is BreakpointLineBind);
    }

    async void Globals_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Globals.Project):
                _ = RemoveAllBreakpointsAsync(false);
                OnPropertiesChanged(nameof(IsProjectOpen));
                if (_globals.IsProjectOpen)
                {
                    await LoadBreakpointsAsync(CancellationToken.None);
                }

                break;
        }
    }

    internal async Task CreateBreakpoint()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var detailViewModel = scope.CreateScopedBreakpointDetailViewModel(
                new BreakpointViewModel
                {
                    IsEnabled = true,
                    BindMode = BreakpointBindMode.None,
                    Mode = BreakpointMode.Load,
                    StopWhenHit = true
                },
                BreakpointDetailDialogMode.Create);
            var message =
                new ShowModalDialogMessage<BreakpointDetailViewModel, SimpleDialogResult>(
                    "Breakpoint properties",
                    DialogButton.OK | DialogButton.Cancel,
                    detailViewModel)
                {
                    MinSize = new Size(400, BreakpointPropertiesDefaultHeight),
                    DesiredSize = new Size(600, BreakpointPropertiesDefaultHeight),
                };
            _dispatcher.DispatchShowModalDialog(message);
            var result = await message.Result;
        }
    }

    async Task SaveLocalSettingsAsync(CancellationToken ct = default)
    {
        if (!_suppressLocalPersistence)
        {
            await SaveBreakpointsAsync(ct);
        }
    }

    internal async Task RemoveBreakpointAsync(BreakpointViewModel? breakpoint)
    {
        if (breakpoint is not null)
        {
            try
            {
                await RemoveBreakpointAsync(breakpoint, forceRemove: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove breakpoint");
            }
        }
    }

    internal async Task RemoveAllBreakpointsAsync()
    {
        await RemoveAllBreakpointsAsync(removeFromLocalStorage: true);
    }

    internal async Task ShowBreakpointPropertiesAsync(BreakpointViewModel? breakpoint)
    {
        if (breakpoint is not null)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var detailViewModel =
                    scope.CreateScopedBreakpointDetailViewModel(breakpoint, BreakpointDetailDialogMode.Update);
                var message =
                    new ShowModalDialogMessage<BreakpointDetailViewModel, SimpleDialogResult>(
                        "Breakpoint properties",
                        DialogButton.OK | DialogButton.Cancel,
                        detailViewModel)
                    {
                        MinSize = new Size(400, BreakpointPropertiesDefaultHeight),
                        DesiredSize = new Size(600, BreakpointPropertiesDefaultHeight),
                    };
                _dispatcher.DispatchShowModalDialog(message);
                var result = await message.Result;
            }
        }
    }

    internal Task BreakPointContextAsync(BreakPointContext? context)
    {
        if (context is not null)
        {
            // if (context.Column == BreakPointContextColumn.Binding)
            // {
            //     var binding = context.ViewModel.Bind;
            //     if (binding is BreakpointLineBind lineBind)
            //     {
            //         _dispatcher.Dispatch(new OpenSourceLineNumberFileMessage(
            //             lineBind.File, lineBind.LineNumber + 1, Column: 0, MoveCaret: true));
            //         return Task.CompletedTask;
            //     }
            // }
            // if (ShowBreakpointPropertiesCommand.CanExecute(context.ViewModel))
            // {
            //     ShowBreakpointPropertiesCommand.Execute(context.ViewModel);
            // }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes all breakpoints from VICE and locally
    /// </summary>
    /// <param name="removeFromLocalStorage">When true, breakpoints are removed from persistence, left otherwise.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <remarks>
    /// <paramref name="removeFromLocalStorage"/> is used when app is cleaning breakpoints but they shouldn't be removed
    /// from persistence.
    /// </remarks>
    internal async Task RemoveAllBreakpointsAsync(bool removeFromLocalStorage, CancellationToken ct = default)
    {
        _suppressLocalPersistence = true;
        try
        {
            while (Breakpoints.Count > 0)
            {
                await RemoveBreakpointAsync(Breakpoints[0], true, ct);
            }
        }
        finally
        {
            _suppressLocalPersistence = false;
            if (removeFromLocalStorage)
            {
                await SaveLocalSettingsAsync(ct);
            }
        }
    }

    internal void ClearHitBreakpoint()
    {
        foreach (var breakpoint in Breakpoints)
        {
            breakpoint.IsCurrentlyHit = false;
        }
    }

    /// <summary>
    /// Arms all breakpoints.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <remarks>This method is required when lost contact with VICE or when debugging symbols change.</remarks>
    public async Task ArmBreakpointsAsync(CancellationToken ct = default)
    {
        Debug.Assert(Breakpoints.All(bp => !bp.CheckpointNumbers.Any()));
        Debug.Assert(_vice.IsConnected);
        var project = (KickAssProjectViewModel)_globals.Project.ValueOrThrow();
        _sourceFiles = MapDebugSourceFileNameToBlockItems(project);
        _labels = MapLabelsToName(project);
        foreach (var b in Breakpoints)
        {
            b.Error = await ArmBreakpointAsync(_sourceFiles, _labels, b, ct);
        }
        _logger.LogDebug("Breakpoints armed");
    }

    internal LabelsNameMap MapLabelsToName(KickAssProjectViewModel project)
    {
        Guard.IsNotNull(project.DbgData);
        return project.AppInfo!.SourceFiles.SelectMany(s => s.Value.Labels.Values)
            .ToFrozenDictionary(l => l.Name);
    }

    /// <summary>
    /// Creates a map of relative file name that contains map of line indexes against <see cref="BlockItem"/>.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    internal SourceLineBlockItemsMap MapDebugSourceFileNameToBlockItems(
        KickAssProjectViewModel project)
    {
        Guard.IsNotNull(project.DbgData);
        Guard.IsNotNull(project.Directory);
        var map = project.DbgData.Sources.Where(s => s.Origin == SourceOrigin.User).ToFrozenDictionary(s =>
            {
                string? relativePath = s.GetRelativePath(project.Directory);
                if (relativePath is not null)
                {
                    return relativePath;
                }

                _logger.LogError("Failed to create relative path for {SourceFile} in {ProjectDirectory}", s.FullPath,
                    project.Directory);
                return s.FullPath;
            },
            s =>
            {
                var blockItems = project.DbgData.Segments.SelectMany(s => s.Blocks).SelectMany(b => b.Items)
                    .Where(bi => bi.FileLocation.SourceIndex == s.Index);
                var builder = new Dictionary<int, ImmutableArray<BlockItem>.Builder>();
                foreach (var bi in blockItems)
                {
                    for (int line = bi.FileLocation.Line1; line <= bi.FileLocation.Line2; line++)
                    {
                        if (!builder.TryGetValue(line, out var items))
                        {
                            items = ImmutableArray.CreateBuilder<BlockItem>();
                            builder[line] = items;
                        }

                        items.Add(bi);
                    }
                }

                return builder.ToFrozenDictionary(p => p.Key, p => p.Value.ToImmutable());
            },
            _osDependent.FileStringComparer);
        return map;
    }

    public async Task<BreakpointError> ArmBreakpointAsync(SourceLineBlockItemsMap sourceFiles, LabelsNameMap labels,
        BreakpointViewModel breakpoint, CancellationToken ct)
    {
        breakpoint.AddressRanges = breakpoint.Bind switch
        {
            BreakpointLineBind lineBind => GetAddressRangesForLineBreakpoint(sourceFiles, lineBind),
            BreakpointNoBind noBind => GetAddressRangesForUnboundBreakpoint(labels, noBind),
            _ => null,
        };
        if (breakpoint.AddressRanges is null || breakpoint.AddressRanges.Count == 0)
        {
            _logger.LogWarning("Couldn't bind breakpoint because no address ranges");
            return BreakpointError.NoAddressRange;
        }

        try
        {
            var result = await _vice.ArmBreakpointAsync(breakpoint, resumeOnStop: false, ct);
            if (result == BreakpointError.None)
            {
                AddBreakpointToMap(breakpoint);
            }

            return result;
        }
        catch (TimeoutException)
        {
            _debugOutput.AddLine("Failed arming a breakpoint due to timeout");  
            _logger.LogError("Failed arming a breakpoint due to timeout");
        }

        return BreakpointError.ViceFailure;
    }

    /// <summary>
    /// Retrieves address ranges for given line in a source file.
    /// </summary>
    /// <param name="bind"></param>
    /// <param name="sourceFiles">Sources file map with relative paths as key</param>
    /// <returns></returns>
    internal HashSet<BreakpointAddressRange>? GetAddressRangesForLineBreakpoint(
        SourceLineBlockItemsMap sourceFiles, BreakpointLineBind bind)
    {
        if (sourceFiles.TryGetValue(bind.FilePath, out var source))
        {
            if (source.TryGetValue(bind.LineNumber + 1, out var blockItems))
            {
                return blockItems.Select(bi => new BreakpointAddressRange(bi.Start, bi.End)).ToHashSet();
            }
            else
            {
                _logger.LogWarning("No {Line} addresses for {SourceFile}", bind.LineNumber, bind.FilePath);
            }
        }
        else
        {
            _logger.LogWarning("Failed to find {SourceFile}", bind.FilePath);
        }
        return null;
    }

    internal HashSet<BreakpointAddressRange>? GetAddressRangesForUnboundBreakpoint(LabelsNameMap labels,
        BreakpointNoBind bind)
    {
        try
        {
            ushort? start = _addressEntryGrammar.CalculateAddress(labels, bind.StartAddress);
            if (!start.HasValue)
            {
                _debugOutput.AddLine($"Couldn't parse breakpoints {bind.StartAddress}");
                return null;
            }
            ushort end = _addressEntryGrammar.CalculateAddress(labels, bind.StartAddress) ?? (ushort)(start.Value + 1);
            return new HashSet<BreakpointAddressRange> { new (start.Value, end) };
        }
        catch (Exception e)
        {
            _debugOutput.AddLine($"Failed parsing breakpoints start {bind.StartAddress} or end {bind.EndAddress} address: {e.Message}");
            return null;
        }
    }

    public async Task DisarmAllBreakpointsAsync(CancellationToken ct = default)
    {
        if (_vice.IsConnected)
        {
            // collects all applied check points in VICE
            var checkpointsList = await _vice.GetCheckpointsListAsync(ct);
            if (checkpointsList is not null)
            {
                // builds map of CheckpointNumber->breakpoint
                var map = new Dictionary<uint, BreakpointViewModel>();
                foreach (var b in Breakpoints)
                {
                    foreach (var cn in b.CheckpointNumbers)
                    {
                        map.Add(cn, b);
                    }
                }

                foreach (var ci in checkpointsList.Info)
                {
                    if (map.TryGetValue(ci.CheckpointNumber, out var breakpoint))
                    {
                        // deletes only those that are part of breakpoints
                        await _vice.DeleteCheckpointAsync(ci.CheckpointNumber, ct);
                        breakpoint.RemoveCheckpointNumber(ci.CheckpointNumber);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Warning,
                            "Breakpoint with checkpoint number {CheckpointNumber} not found when disarming",
                            ci.CheckpointNumber);
                    }
                }
            }
        }

        foreach (var b in Breakpoints)
        {
            b.MarkDisarmed();
        }

        _sourceFiles = null;
        _labels = null;
        _breakpointsMap.Clear();
    }

    void UpdateBreakpointDataFromVice(CheckpointInfoResponse checkpointInfo)
    {
        if (_breakpointsMap.TryGetValue(checkpointInfo.CheckpointNumber, out var breakpoint))
        {
            breakpoint.IsCurrentlyHit = checkpointInfo.CurrentlyHit;
            breakpoint.IsEnabled = checkpointInfo.Enabled;
            breakpoint.HitCount = checkpointInfo.HitCount;
            breakpoint.IgnoreCount = checkpointInfo.IgnoreCount;
        }
    }

    /// <summary>
    /// Adds breakpoint linked to line in the file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <param name="condition"></param>
    /// <param name="ct"></param>
    public async Task AddLineBreakpointAsync(string filePath, int lineNumber,
        string? condition, CancellationToken ct = default)
    {
        var bind = new BreakpointLineBind(filePath, lineNumber, null);
        ImmutableArray<AddressRange> addresses;
        // address can be retrieved only when running using debug information
        if (_sourceFiles is not null)
        {
            var breakPointAddresses = GetAddressRangesForLineBreakpoint(_sourceFiles, bind);
            addresses =
                breakPointAddresses?.Select(b => AddressRange.FromRange(b.Start, b.End)).ToImmutableArray() ??
                ImmutableArray<AddressRange>.Empty;
        }
        else
        {
            addresses = ImmutableArray<AddressRange>.Empty;
        }

        await AddBreakpointAsync(true, true, BreakpointMode.Exec, bind, addresses, null,
            ct);
    }

    internal async Task ToggleBreakpointEnabledAsync(BreakpointViewModel? breakpoint)
    {
        if (breakpoint is not null)
        {
            var checkpointNumbers = breakpoint.CheckpointNumbers.ToImmutableArray();
            if (!checkpointNumbers.IsEmpty)
            {
                bool allRemoved = true;
                foreach (var cn in checkpointNumbers)
                {
                    bool success = await _vice.ToggleCheckpointAsync(cn, !breakpoint.IsEnabled, CancellationToken.None);
                    if (!success)
                    {
                        allRemoved = false;
                        _logger.Log(LogLevel.Error,
                            "Failed toggling breakpoint for CheckpointNumber {CheckpointNumber}", cn);
                    }
                }

                if (allRemoved)
                {
                    breakpoint.IsEnabled = !breakpoint.IsEnabled;
                }
            }
        }
    }

    /// <summary>
    /// Adds breakpoint to VICE
    /// </summary>
    /// <param name="stopWhenHit"></param>
    /// <param name="isEnabled"></param>
    /// <param name="mode"></param>
    /// <param name="bind"></param>
    /// <param name="addressRanges"></param>
    /// <param name="condition"></param>
    /// <param name="ct"></param>
    /// <exception cref="Exception"></exception>
    internal async Task AddBreakpointAsync(bool stopWhenHit, bool isEnabled, BreakpointMode mode,
        BreakpointBind bind, ImmutableArray<AddressRange> addressRanges, string? condition,
        CancellationToken ct = default)
    {
        foreach (var range in addressRanges)
        {
            if (range.EndAddress < range.StartAddress)
            {
                throw new Exception($"Invalid breakpoint address range {range.StartAddress} to {range.EndAddress}");
            }
        }

        var breakpointAddressRanges = addressRanges
            .Select(ar => new BreakpointAddressRange(ar.StartAddress, ar.EndAddress))
            .ToHashSet();
        var breakpoint = new BreakpointViewModel(stopWhenHit, isEnabled, mode, bind, condition)
            { AddressRanges = breakpointAddressRanges };
        await AddBreakpointAsync(breakpoint, ct);
    }

    internal async Task AddBreakpointAsync(BreakpointViewModel breakpoint, CancellationToken ct)
    {
        Breakpoints.Add(breakpoint);
        if (_vice.IsDebugging)
        {
            await _vice.ArmBreakpointAsync(breakpoint, resumeOnStop: !_vice.IsPaused, ct);
            AddBreakpointToMap(breakpoint);
        }

        if (breakpoint.Bind is BreakpointLineBind lineBind)
        {
            var key = new BreakpointLineKey(lineBind.FilePath, lineBind.LineNumber);
            if (!_breakpointsLinesMap.TryGetValue(key, out var breakpoints))
            {
                breakpoints = new List<BreakpointViewModel> { breakpoint };
                _breakpointsLinesMap.Add(key, breakpoints);
            }
            else
            {
                _breakpointsLinesMap[key].Add(breakpoint);
            }
        }
    }

    internal void AddBreakpointToMap(BreakpointViewModel breakpoint)
    {
        foreach (var cp in breakpoint.CheckpointNumbers)
        {
            _breakpointsMap.Add(cp, breakpoint);
        }
    }

    public async Task<bool> RemoveBreakpointAsync(BreakpointViewModel breakpoint, bool forceRemove,
        CancellationToken ct = default)
    {
        var checkpointNumbers = breakpoint.CheckpointNumbers.ToImmutableArray();
        if (!checkpointNumbers.IsEmpty)
        {
            bool allRemoved = true;
            foreach (var cn in checkpointNumbers)
            {
                // delete from VICE only when connected
                bool success = !_vice.IsConnected || await _vice.DeleteCheckpointAsync(cn, ct);
                if (!success)
                {
                    // TODO what to do if some fails?
                    allRemoved = false;
                }

                _breakpointsMap.Remove(cn);
            }

            if (allRemoved || forceRemove)
            {
                if (breakpoint.Bind is BreakpointLineBind lineBind)
                {
                    RemoveBreakpointFromLinesMap(breakpoint, lineBind.FilePath, lineBind.LineNumber);
                }

                Breakpoints.Remove(breakpoint);
                return true;
            }

            return false;
        }
        else
        {
            if (breakpoint.Bind is BreakpointLineBind lineBind)
            {
                if (!RemoveBreakpointFromLinesMap(breakpoint, lineBind.FilePath, lineBind.LineNumber))
                {
                    _logger.LogWarning("Failed to remove breakpoint from line map");
                }
            }

            return Breakpoints.Remove(breakpoint);
        }
    }

    internal bool RemoveBreakpointFromLinesMap(BreakpointViewModel breakpoint, string filePath, int lineNumber)
    {
        var key = new BreakpointLineKey(filePath, lineNumber);
        if (_breakpointsLinesMap.TryGetValue(key, out var breakpoints))
        {
            return breakpoints.Remove(breakpoint);
        }

        return false;
    }

    /// <summary>
    /// Updates an existing breakpoint. Will throw if problems with communication or condition fails.
    /// When breakpoint is armed, it will be replaced with new one, nothing is done otherwise.
    /// </summary>
    /// <param name="breakpoint"></param>
    /// <param name="sourceBreakpoint"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <remarks>Breakpoint might be a clone and thus equality on <see cref="BreakpointViewModel"/> can not be used.</remarks>
    public async Task UpdateBreakpointAsync(BreakpointViewModel breakpoint, BreakpointViewModel sourceBreakpoint,
        CancellationToken ct = default)
    {
        if (_vice.IsDebugging)
        {
            var checkpointNumbers = breakpoint.CheckpointNumbers.ToImmutableHashSet();
            if (!checkpointNumbers.IsEmpty)
            {
                foreach (uint cn in checkpointNumbers)
                {
                    bool result = await _vice.DeleteCheckpointAsync(cn, ct);
                    if (!result)
                    {
                        _logger.Log(LogLevel.Error,
                            "Failed to remove checkpoint number {CheckpointNumber} from the list", cn);
                    }
                }
            }
        }

        sourceBreakpoint.CopyFrom(breakpoint);
        // clears configuration errors if any
        sourceBreakpoint.Error = BreakpointError.None;
        sourceBreakpoint.ErrorText = null;
        // arm breakpoint only when debugging
        if (_vice.IsDebugging)
        {
            await _vice.ArmBreakpointAsync(sourceBreakpoint, resumeOnStop: !_vice.IsPaused, ct);
        }

        await SaveLocalSettingsAsync(ct);
    }

    /// <summary>
    /// Creates a <see cref="BreakpointsViewModel"/> for a file exec breakpoint.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="bind"></param>
    /// <returns></returns>
    internal BreakpointViewModel? LoadLineBreakpoint(BreakpointInfo b, BreakpointInfoLineBind bind)
    {
        return new BreakpointViewModel
        {
            StopWhenHit = b.StopWhenHit,
            IsEnabled = b.IsEnabled,
            Mode = b.Mode,
            Condition = b.Condition,
            Bind = new BreakpointLineBind(bind.FilePath, bind.LineNumber, null),
        };
    }

    /// <summary>
    /// Loads an unbound breakpoint - one not tied to either file or label.
    /// </summary>
    /// <param name="b"></param>
    /// <param name="bind"></param>
    /// <returns></returns>
    internal BreakpointViewModel? LoadUnboundBreakpoint(BreakpointInfo b, BreakpointInfoNoBind bind)
    {
        return new BreakpointViewModel
        {
            StopWhenHit = b.StopWhenHit,
            IsEnabled = b.IsEnabled,
            Mode = b.Mode,
            Bind = new BreakpointNoBind(bind.StartAddress, bind.EndAddress),
            Condition = b.Condition,
        };
    }

    public Task AddBreakpointsFromCodeAsync(CancellationToken ct = default)
    {
        var project = (KickAssProjectViewModel)_globals.Project.ValueOrThrow();
        foreach (var b in project.DbgData.ValueOrThrow().Breakpoints)
        {
            // TODO add kickass breakpoints and watchpoints
        }

        return Task.CompletedTask;
    }

    public async Task LoadBreakpointsFromSettings(BreakpointsSettings settings, CancellationToken ct = default)
    {
        _suppressLocalPersistence = true;
        try
        {
            foreach (var b in settings.Breakpoints)
            {
                BreakpointViewModel? breakpoint = b.Bind switch
                {
                    BreakpointInfoLineBind lineBind => LoadLineBreakpoint(b, lineBind),
                    BreakpointInfoNoBind noBind => LoadUnboundBreakpoint(b, noBind),
                    _ => throw new Exception($"Unknown breakpoint bind type {b.Bind?.GetType().Name}"),
                };
                if (breakpoint is not null)
                {
                    await AddBreakpointAsync(breakpoint, ct);
                }
            }
        }
        finally
        {
            _suppressLocalPersistence = false;
        }
    }

    public async Task SaveBreakpointsAsync(CancellationToken ct = default)
    {
        var project = _globals.Project;
        if (project.BreakpointsSettingsPath is not null)
        {
            var items = Breakpoints
                .Where(b => b.Bind is not null)
                .Select(b =>
                    new BreakpointInfo(b.StopWhenHit, b.IsEnabled, b.Mode, b.Condition, b.Bind!.ConvertFromModel())
                ).ToImmutableArray();
            var settings = new BreakpointsSettings(items);
            try
            {
                _logger.LogDebug("Saving breakpoints settings");
                await _settingsManager.SaveAsync(settings, project.BreakpointsSettingsPath, false, ct);
                _logger.LogDebug("Saved breakpoints settings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed saving breakpoints settings");
            }
        }
    }

    public async Task LoadBreakpointsAsync(CancellationToken ct = default)
    {
        var project = _globals.Project;
        if (project?.BreakpointsSettingsPath is not null)
        {
            var settings = await _settingsManager.LoadAsync<BreakpointsSettings>(project.BreakpointsSettingsPath, ct);
            if (settings is not null)
            {
                await LoadBreakpointsFromSettings(settings, ct);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _vice.CheckpointInfoUpdated -= ViceOnCheckpointInfoUpdated;
            _globals.PropertyChanged -= Globals_PropertyChanged;
            _commandsManager.Dispose();
        }

        base.Dispose(disposing);
    }
}