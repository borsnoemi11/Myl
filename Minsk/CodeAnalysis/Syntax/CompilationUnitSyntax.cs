namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        // this basically means one file of the source code.
        public CompilationUnitSyntax(ExpressionSyntax expression, SyntaxToken endOfFileToken)
        {
            Expression = expression;
            EndOfFileToken = endOfFileToken;
        }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public ExpressionSyntax Expression { get; }
        
        public SyntaxToken EndOfFileToken { get; }
    }
}