using System.Diagnostics.CodeAnalysis;

namespace miniLang;

public class Environment
{
    private readonly Dictionary<string, object> _variables = new();
    private readonly Environment? _parent;

    public Environment(Environment? parent = null)
    {
        _parent = parent;
    }

    public void Define(string name, object value) => _variables[name] = value;

    public bool TryGet(string name, [NotNullWhen(true)] out object? value)
    {
        if (_variables.TryGetValue(name, out value))
        {
            return true;
        }
        if (_parent != null)
        {
            return _parent.TryGet(name, out value);
        }
        value = null;
        return false;
    }

    public object Get(string name)
    {
        if (TryGet(name, out var value))
        {
            return value;
        }
        throw new RuntimeError($"Undefined variable: {name}");
    }

    public object Assign(string name, object value)
    {
        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
            return value;
        }
        if (_parent != null)
        {
            return _parent.Assign(name, value);
        }
        throw new RuntimeError($"Undefined variable: {name}");
    }
}
