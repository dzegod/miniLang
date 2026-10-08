namespace miniLang;

public class RuntimeError : Exception
{
    public RuntimeError(string message) : base(message) { }
}
