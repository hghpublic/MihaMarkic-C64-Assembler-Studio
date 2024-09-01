//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from D:/Git/Righthand/C64/C64-Assembler-Studio/src/C64AssemblerStudio/C64AssemblerStudio.Engine/Grammars/BreakpointConditions/BreakpointConditionsParser.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="BreakpointConditionsParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface IBreakpointConditionsParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="BreakpointConditionsParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRoot([NotNull] BreakpointConditionsParser.RootContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BreakpointConditionsParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRoot([NotNull] BreakpointConditionsParser.RootContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ConditionArguments</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionArguments([NotNull] BreakpointConditionsParser.ConditionArgumentsContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ConditionArguments</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionArguments([NotNull] BreakpointConditionsParser.ConditionArgumentsContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ConditionParens</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionParens([NotNull] BreakpointConditionsParser.ConditionParensContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ConditionParens</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionParens([NotNull] BreakpointConditionsParser.ConditionParensContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ConditionOperation</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConditionOperation([NotNull] BreakpointConditionsParser.ConditionOperationContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ConditionOperation</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConditionOperation([NotNull] BreakpointConditionsParser.ConditionOperationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BreakpointConditionsParser.operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOperator([NotNull] BreakpointConditionsParser.OperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BreakpointConditionsParser.operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOperator([NotNull] BreakpointConditionsParser.OperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Bank</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBank([NotNull] BreakpointConditionsParser.BankContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Bank</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBank([NotNull] BreakpointConditionsParser.BankContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Memspace</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMemspace([NotNull] BreakpointConditionsParser.MemspaceContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Memspace</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMemspace([NotNull] BreakpointConditionsParser.MemspaceContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>Label</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLabel([NotNull] BreakpointConditionsParser.LabelContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>Label</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLabel([NotNull] BreakpointConditionsParser.LabelContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>ArgumentRegister</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArgumentRegister([NotNull] BreakpointConditionsParser.ArgumentRegisterContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>ArgumentRegister</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArgumentRegister([NotNull] BreakpointConditionsParser.ArgumentRegisterContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>HexNumber</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterHexNumber([NotNull] BreakpointConditionsParser.HexNumberContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>HexNumber</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitHexNumber([NotNull] BreakpointConditionsParser.HexNumberContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BreakpointConditionsParser.register"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRegister([NotNull] BreakpointConditionsParser.RegisterContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BreakpointConditionsParser.register"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRegister([NotNull] BreakpointConditionsParser.RegisterContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BreakpointConditionsParser.variable"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariable([NotNull] BreakpointConditionsParser.VariableContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BreakpointConditionsParser.variable"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariable([NotNull] BreakpointConditionsParser.VariableContext context);
}
