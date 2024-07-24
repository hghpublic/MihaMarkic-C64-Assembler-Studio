﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using C64AssemblerStudio.Engine.Services.Abstract;

namespace C64AssemblerStudio.Engine.ViewModels;

public class StatusInfoViewModel : ViewModel
{
    private readonly IVice _vice;
    public BuildStatus BuildingStatus { get; set; } = BuildStatus.Idle;
    public bool IsBuildingStatusVisible => BuildingStatus != BuildStatus.Idle;
    public bool IsViceConnected => _vice.IsConnected;
    public string RunCommandTitle => _vice.IsDebugging ? "Continue" : "Run";

    public StatusInfoViewModel(IVice vice)
    {
        _vice = vice;
        _vice.PropertyChanged += ViceOnPropertyChanged;
    }

    private void ViceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IVice.IsConnected):
                OnPropertyChanged(nameof(IsViceConnected));
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _vice.PropertyChanged -= ViceOnPropertyChanged;
        }

        base.Dispose(disposing);
    }
}

public enum BuildStatus
{
    [Display(Description = "Building")] Building,
    [Display(Description = "Idle")] Idle,

    [Display(Description = "Build Success")]
    Success,

    [Display(Description = "Build Failure")]
    Failure
}