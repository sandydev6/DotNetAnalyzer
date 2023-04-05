using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace SN1CustomRules
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Analyzer1Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SN1001";//custom sn1 rule 1

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static DiagnosticDescriptor SyntaxRule = new DiagnosticDescriptor(DiagnosticId, "Insert braces", "the statement {0} does not contains braces", "syntax", DiagnosticSeverity.Warning, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(SyntaxRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.IfStatement, SyntaxKind.ForEachStatement);
        }

        //diagnostic braces missing in if, for, foreach and while
        private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            var body = default(StatementSyntax);
            var token = default(SyntaxToken);
            switch (context.Node)
            {
                case IfStatementSyntax ifstate:
                    body = ifstate.Statement;
                    token = ifstate.IfKeyword;
                    break;
                case ForEachStatementSyntax foreachstate:
                    body = foreachstate.Statement;
                    token = foreachstate.ForEachKeyword;
                    break;
                case ForStatementSyntax forstate:
                    body = forstate.Statement;
                    token = forstate.ForKeyword;
                    break;
                case WhileStatementSyntax whilestate:
                    body = whilestate.Statement;
                    token = whilestate.WhileKeyword;
                    break;
            }

            if (!body.IsKind(SyntaxKind.Block))
            {
                var diagnostic = Diagnostic.Create(SyntaxRule, token.GetLocation(), context.Node.Kind().ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
