using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();

        public BoundScope(BoundScope? parent)
        {
            Parent = parent;
        }

        public BoundScope? Parent { get; }

        public bool TryDeclare(VariableSymbol variable)
        {
            if (variable is null)
            {
                throw new ArgumentNullException(nameof(variable));
            }
            
            // we want to support shadowing, so the parent's dictionary is not considered
            if (_variables.ContainsKey(variable.Name))
                return false;

            _variables.Add(variable.Name, variable);
            return true;
        }

        public bool TryLookup(string name, [NotNullWhen(true)] out VariableSymbol? variable)
        {
            if (_variables.TryGetValue(name, out variable))
            {
                return true;
            }

            if (Parent is null)
            {
                variable = null;
                return false;
            }

            return Parent.TryLookup(name, out variable);
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return _variables.Values.ToImmutableArray();
        }
    }
}