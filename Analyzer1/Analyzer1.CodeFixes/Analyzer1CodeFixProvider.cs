using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SN1CustomRules
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Analyzer1CodeFixProvider)), Shared]
    public class Analyzer1CodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(Analyzer1Analyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

                switch (diagnostic.Id)
                {
                    case Analyzer1Analyzer.DiagnosticId:
                        context.RegisterCodeFix(
                          CodeAction.Create(
                               "Insert braces",
                               c => InsertCurlyBraces(context.Document, root.FindToken(diagnosticSpan.Start).Parent, c),
                               "Insert braces"),
                           diagnostic);
                        break;
                }
            }
        }

        private async Task<Document> InsertCurlyBraces(Document document, SyntaxNode oldStatemnt, CancellationToken c)
        {
            var newStatement = oldStatemnt;

            switch (oldStatemnt)
            {
                case IfStatementSyntax ifstate:
                    newStatement = ifstate.WithStatement(SyntaxFactory.Block(ifstate.Statement));
                    break;
                case ForEachStatementSyntax foreachstate:
                    newStatement = foreachstate.WithStatement(SyntaxFactory.Block(foreachstate.Statement));
                    break;
            }

            newStatement = newStatement.WithAdditionalAnnotations();
            var root = await document.GetSyntaxRootAsync(c).ConfigureAwait(false);
            var newRoot = root.ReplaceNode(oldStatemnt, newStatement);
            var newdoc = document.WithSyntaxRoot(newRoot);

            return newdoc;
        }
    }
}
