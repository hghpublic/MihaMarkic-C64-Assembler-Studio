//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from D:/Git/Righthand/C64/C64-Assembler-Studio/src/C64AssemblerStudio/C64AssemblerStudio.Engine/Grammars/AddressEntry/AddressEntryParser.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419


using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IAddressEntryParserListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.Diagnostics.DebuggerNonUserCode]
[System.CLSCompliant(false)]
public partial class AddressEntryParserBaseListener : IAddressEntryParserListener {
	/// <summary>
	/// Enter a parse tree produced by the <c>Multiplication</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMultiplication([NotNull] AddressEntryParser.MultiplicationContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Multiplication</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMultiplication([NotNull] AddressEntryParser.MultiplicationContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Parens</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParens([NotNull] AddressEntryParser.ParensContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Parens</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParens([NotNull] AddressEntryParser.ParensContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Arg</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArg([NotNull] AddressEntryParser.ArgContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Arg</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArg([NotNull] AddressEntryParser.ArgContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Division</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDivision([NotNull] AddressEntryParser.DivisionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Division</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDivision([NotNull] AddressEntryParser.DivisionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Plus</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPlus([NotNull] AddressEntryParser.PlusContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Plus</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPlus([NotNull] AddressEntryParser.PlusContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Minus</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMinus([NotNull] AddressEntryParser.MinusContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Minus</c>
	/// labeled alternative in <see cref="AddressEntryParser.arguments"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMinus([NotNull] AddressEntryParser.MinusContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>Label</c>
	/// labeled alternative in <see cref="AddressEntryParser.argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLabel([NotNull] AddressEntryParser.LabelContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>Label</c>
	/// labeled alternative in <see cref="AddressEntryParser.argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLabel([NotNull] AddressEntryParser.LabelContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>DecNumber</c>
	/// labeled alternative in <see cref="AddressEntryParser.argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDecNumber([NotNull] AddressEntryParser.DecNumberContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>DecNumber</c>
	/// labeled alternative in <see cref="AddressEntryParser.argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDecNumber([NotNull] AddressEntryParser.DecNumberContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>HexNumber</c>
	/// labeled alternative in <see cref="AddressEntryParser.argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterHexNumber([NotNull] AddressEntryParser.HexNumberContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>HexNumber</c>
	/// labeled alternative in <see cref="AddressEntryParser.argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitHexNumber([NotNull] AddressEntryParser.HexNumberContext context) { }
	/// <summary>
	/// Enter a parse tree produced by the <c>BinNumber</c>
	/// labeled alternative in <see cref="AddressEntryParser.argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBinNumber([NotNull] AddressEntryParser.BinNumberContext context) { }
	/// <summary>
	/// Exit a parse tree produced by the <c>BinNumber</c>
	/// labeled alternative in <see cref="AddressEntryParser.argument"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBinNumber([NotNull] AddressEntryParser.BinNumberContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}