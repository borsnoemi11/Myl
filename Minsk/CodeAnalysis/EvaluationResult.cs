using System.Collections.Immutable;

namespace Minsk.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, object? value)
        {
            if (value is null && !diagnostics.Any())
            {
                throw new ArgumentException($"Parameter '{nameof(diagnostics)}' cannot be empty if the value is null.");
            }

            Diagnostics = diagnostics;
            Value = value;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        
        public object? Value { get; }
    }
}