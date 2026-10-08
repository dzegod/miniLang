namespace miniLang;

public class Interpreter
{
    private Environment _environment = new();

    public void Interpret(List<Statement> statements)
    {
        foreach (var statement in statements)
        {
            Execute(statement);
        }
    }

    public void Execute(Statement statement)
    {
        if (statement is LetStatement letStatement)
        {
            var value = Evaluate(letStatement.Value);
            _environment.Define(letStatement.Name, value);
            return;
        }

        if (statement is AssignmentStatement assignmentStatement)
        {
            var value = Evaluate(assignmentStatement.Value);
            _environment.Assign(assignmentStatement.Name, value);
            return;
        }

        if (statement is PrintStatement printStatement)
        {
            Console.WriteLine(Evaluate(printStatement.Value));
            return;
        }

        if (statement is BlockStatement blockStatement)
        {
            var previousEnvironment = _environment;
            _environment = new Environment(previousEnvironment);
            try
            {
                foreach (var stmt in blockStatement.Statements)
                {
                    Execute(stmt);
                }
            }
            finally
            {
                _environment = previousEnvironment;
            }
            return;
        }

        if (statement is IfStatement ifStatement)
        {
            if (Evaluate(ifStatement.Condition) != 0)
            {
                Execute(ifStatement.ThenBranch);
            }
            else if (ifStatement.ElseBranch != null)
            {
                Execute(ifStatement.ElseBranch);
            }
            return;
        }

        throw new RuntimeError($"Unknown statement type: {statement.GetType().Name}");
    }
    

    public double Evaluate(Expression expr)
    {
        if (expr is NumberExpression numberExpression)
        {
            return numberExpression.Value;
        }

        if (expr is VariableExpression variableExpression)
        {
            return _environment.Get(variableExpression.Name);
        }

        if (expr is BinaryExpression binaryExpression)
        {
            var leftValue = Evaluate(binaryExpression.Left);
            var rightValue = Evaluate(binaryExpression.Right);

            return binaryExpression.Operator switch
            {
                TokenType.Plus => leftValue + rightValue,
                TokenType.Minus => leftValue - rightValue,
                TokenType.Star => leftValue * rightValue,
                TokenType.Slash => rightValue == 0 ? throw new RuntimeError("Division by zero") : leftValue / rightValue,
                TokenType.Less => leftValue < rightValue ? 1 : 0,
                TokenType.LessEqual => leftValue <= rightValue ? 1 : 0,
                TokenType.Greater => leftValue > rightValue ? 1 : 0,
                TokenType.GreaterEqual => leftValue >= rightValue ? 1 : 0,
                TokenType.EqualEqual => leftValue == rightValue ? 1 : 0,
                _ => throw new RuntimeError($"Unknown operator: {binaryExpression.Operator}")
            };
        }

        throw new RuntimeError($"Unknown expression type: {expr.GetType().Name}");
    }
}