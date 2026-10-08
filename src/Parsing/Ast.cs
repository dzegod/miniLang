namespace miniLang;

public abstract record Expression;

public record NumberExpression(double Value) : Expression;
public record BinaryExpression(Expression Left, TokenType Operator, Expression Right) : Expression;

public abstract record Statement;

public record LetStatement(string Name, Expression Value) : Statement;
public record PrintStatement(Expression Value) : Statement;

public record BlockStatement(List<Statement> Statements) : Statement;

public record IfStatement(Expression Condition, Statement ThenBranch, Statement? ElseBranch) : Statement;

public record WhileStatement(Expression Condition, Statement Body) : Statement;

public record VariableExpression(string Name) : Expression;

public record AssignmentStatement(string Name, Expression Value) : Statement;
