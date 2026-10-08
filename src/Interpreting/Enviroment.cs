namespace miniLang;

public class Environment
{
    private readonly Dictionary<string, object> _variables = new();

    public void Define(string name, object value) => _variables[name] = value;

    private readonly Environment? _parent;
    public Environment(Environment? parent = null)
    {
        _parent = parent;
    }

    public object Get(string name)
    {
        if (_variables.TryGetValue(name, out var value))
        {
            return value;
        }
        if (_parent != null)
        {
            return _parent.Get(name);
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
