using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Text
{
    public sealed class SourceText
    {
        private readonly string _text;

        private SourceText(string text)
        {
            Lines = ParseLines(this, text);
            _text = text;
        }

        public ImmutableArray<TextLine> Lines { get; private set; }

        public char this[int index] => _text[index];

        public int Length => _text.Length;

        public static SourceText From(string text)
        {
            return new SourceText(text);
        }

        public override string ToString() => _text;

        public string ToString(int start, int length) => _text.Substring(start, length);

        public string ToString(TextSpan span) => _text.Substring(span.Start, span.Length);

        public int GetLineIndex(int position)
        {
            var lower = 0;
            var upper = Lines.Length - 1;
            var lineIndex = (upper + lower) / 2;

            while (lower <= upper)
            {
                lineIndex = (upper + lower) / 2;
                var lineStart = Lines[lineIndex].Start;
                var lineEnd = Lines[lineIndex].End;

                if (position >= lineStart && position <= lineEnd)
                {
                    return lineIndex;
                }

                if (position < lineStart)
                {
                    upper = lineIndex - 1;
                    continue;
                }

                if (position > lineEnd)
                {
                    lower = lineIndex + 1;
                    continue;
                }
            }

            return lineIndex;
        }

        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();
            var position = 0;
            var lineStart = 0;

            while (position < text.Length)
            {
                var lineBreakWidth = GetLineBreakWidth(text, position);
                if (lineBreakWidth == 0)
                {
                    position++;
                    continue;
                }

                AddLine(result, sourceText, position, lineStart, lineBreakWidth);

                position += lineBreakWidth;
                lineStart = position;
            }

            if (position > lineStart)
            {
                AddLine(result, sourceText, position, lineStart, 0);
            }

            return result.ToImmutable();
        }

        private static void AddLine(ImmutableArray<TextLine>.Builder result, SourceText sourceText, int position, int lineStart, int lineBreakWidth)
        {
            var lineLength = position - lineStart;
            var lineLengthIncludingLineBreak = lineLength + lineBreakWidth;
            var line = new TextLine(sourceText, lineStart, lineLength, lineLengthIncludingLineBreak);
            result.Add(line);
        }

        private static int GetLineBreakWidth(string text, int position)
        {
            var current = text[position];
            var lookahead = position + 1 >= text.Length ? '\0' : text[position + 1];

            if (current == '\r' && lookahead == '\n')
                return 2;

            if (current == '\r' || lookahead == '\n')
                return 1;

            return 0;
        }
    }
}