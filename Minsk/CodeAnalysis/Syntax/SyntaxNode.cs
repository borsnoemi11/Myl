using System.Reflection;

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

        public IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode)property.GetValue(this)!;
                    yield return child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this)!;
                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}