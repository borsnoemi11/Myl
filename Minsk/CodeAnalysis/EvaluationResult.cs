namespace Minsk.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(IEnumerable<string> diagnostics, object? value)
        {
            if (value is null && (diagnostics is null || !diagnostics.Any()))
            {
                throw new ArgumentException($"Parameter '{nameof(diagnostics)}' cannot be empty if the value is null.");
            }

            Diagnostics = diagnostics.ToArray();
            Value = value;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        
        public object? Value { get; }
    }
}