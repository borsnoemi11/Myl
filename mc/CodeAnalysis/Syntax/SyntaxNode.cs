namespace Minsk.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        // 1 + 2 * 3
        //
        // this will be a tree
        // 
        //         +
        //        / \
        //       1   *
        //          / \
        //         2   3

        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}