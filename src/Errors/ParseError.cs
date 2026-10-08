namespace miniLang;

public class ParseError : Exception
{
    public int Line { get; }

    public ParseError(string message, int line) : base($"[line {line}] {message}")
    {
        Line = line;
    }
}
