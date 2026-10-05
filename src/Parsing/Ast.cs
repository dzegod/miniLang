namespace miniLang;

public abstract record Expression;

public record NumberExpression(double Value) : Expression;
public record BinaryExpression(Expression Left, TokenType Operator, Expression Right) : Expression;

public abstract record Statement;

public record LetStatement(string Name, Expression Value) : Statement;
public record PrintStatement(Expression Value) : Statement;