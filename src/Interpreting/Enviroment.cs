namespace miniLang;

public class Environment
{
    private readonly Dictionary<string, double> _variables = new();

    public void Define(string name, double value) => _variables[name] = value;

    private readonly Environment? _parent;
    public Environment(Environment? parent = null)
    {
        _parent = parent;
    }

    public double Get(string name)
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

    public double Assign(string name, double value)
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
