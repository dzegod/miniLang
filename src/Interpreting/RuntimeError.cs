namespace miniLang;

public class RuntimeError : Exception
{
    public int? Line { get; }
    public string Reason { get; }

    public RuntimeError(string message, int? line = null)
        : base(line is null ? message : $"[line {line}] {message}")
    {
        Reason = message;
        Line = line;
    }
}
