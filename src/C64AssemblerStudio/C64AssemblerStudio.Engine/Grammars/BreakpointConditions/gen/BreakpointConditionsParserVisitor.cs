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
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="BreakpointConditionsParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface IBreakpointConditionsParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="BreakpointConditionsParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRoot([NotNull] BreakpointConditionsParser.RootContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ConditionArguments</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConditionArguments([NotNull] BreakpointConditionsParser.ConditionArgumentsContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ConditionParens</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConditionParens([NotNull] BreakpointConditionsParser.ConditionParensContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ConditionOperation</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConditionOperation([NotNull] BreakpointConditionsParser.ConditionOperationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="BreakpointConditionsParser.operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOperator([NotNull] BreakpointConditionsParser.OperatorContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>Bank</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBank([NotNull] BreakpointConditionsParser.BankContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>Memspace</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMemspace([NotNull] BreakpointConditionsParser.MemspaceContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>Label</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLabel([NotNull] BreakpointConditionsParser.LabelContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>ArgumentRegister</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArgumentRegister([NotNull] BreakpointConditionsParser.ArgumentRegisterContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>HexNumber</c>
	/// labeled alternative in <see cref="BreakpointConditionsParser.argument"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitHexNumber([NotNull] BreakpointConditionsParser.HexNumberContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="BreakpointConditionsParser.register"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRegister([NotNull] BreakpointConditionsParser.RegisterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="BreakpointConditionsParser.variable"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVariable([NotNull] BreakpointConditionsParser.VariableContext context);
}
