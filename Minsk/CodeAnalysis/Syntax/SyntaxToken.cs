namespace Minsk.CodeAnalysis.Syntax
{
    public class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string? text, object? value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
            var length = Text is null ? 0 : Text.Length;
            Span = new TextSpan(Position, length);
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string? Text { get; }
        public object? Value { get; }
        public TextSpan Span { get; }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}