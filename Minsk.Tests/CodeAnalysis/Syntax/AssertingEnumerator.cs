using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Syntax;
using Xunit;

namespace Minsk.Tests.CodeAnalysis.Syntax;

internal sealed class AssertingEnumerator : IDisposable
{
    private readonly IEnumerator<SyntaxNode> _enumerator;
    private bool _hasErrors;

    public AssertingEnumerator(SyntaxNode node)
    {
        _enumerator = Flatten(node).GetEnumerator();   
    }

    public void Dispose()
    {
        if (!_hasErrors)
        {
            Assert.False(_enumerator.MoveNext());
        }

        _enumerator.Dispose();
    }

    public void AssertToken(SyntaxKind expectedKind, string expectedText)
    {
        try
        {
            Assert.True(_enumerator.MoveNext());
            Assert.Equal(expectedKind, _enumerator.Current.Kind);
            var token = Assert.IsType<SyntaxToken>(_enumerator.Current);
            Assert.Equal(expectedText, token.Text);
        }
        catch when (MarkFailed())
        {
            throw;
        }
    }

    public void AssertNode(SyntaxKind expectedKind)
    {
        try
        {
            Assert.True(_enumerator.MoveNext());
            Assert.Equal(expectedKind, _enumerator.Current.Kind);
            Assert.IsNotType<SyntaxToken>(_enumerator.Current);
        }
        catch when (MarkFailed())
        {
            throw;
        }
    }

    private bool MarkFailed()
    {
        _hasErrors = true;
        return false;
    }

    private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
    {
        var stack = new Stack<SyntaxNode>();
        stack.Push(node);

        while (stack.Count > 0)
        {
            var n = stack.Pop();
            yield return n;

            foreach (var child in n.GetChildren().Reverse())
            {
                stack.Push(child);
            }
        }
    }
}
