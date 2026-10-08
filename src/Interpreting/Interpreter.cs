using System.Globalization;

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

    // Runs one statement. If it fails with a RuntimeError that has no line yet,
    // the error is re-thrown with this statement's line number attached.
    public void Execute(Statement statement)
    {
        try
        {
            ExecuteStatement(statement);
        }
        catch (RuntimeError error) when (error.Line is null)
        {
            throw new RuntimeError(error.Reason, statement.Line);
        }
    }

    private void ExecuteStatement(Statement statement)
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

        if (statement is WhileStatement whileStatement)
        {
            while (IsTruthy(Evaluate(whileStatement.Condition)))
            {
                Execute(whileStatement.Body);
            }
            return;
        }

        if (statement is PrintStatement printStatement)
        {
            Console.WriteLine(Stringify(Evaluate(printStatement.Value)));
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
            if (IsTruthy(Evaluate(ifStatement.Condition)))
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
    

    public object Evaluate(Expression expr)
    {
        if (expr is NumberExpression numberExpression)
        {
            return numberExpression.Value;
        }

        if (expr is StringExpression stringExpression)
        {
            return stringExpression.Value;
        }

        if (expr is VariableExpression variableExpression)
        {
            return _environment.Get(variableExpression.Name);
        }

        if (expr is BinaryExpression binaryExpression)
        {
            var leftValue = Evaluate(binaryExpression.Left);
            var rightValue = Evaluate(binaryExpression.Right);
            var op = binaryExpression.Operator;

            // + joins text when either side is a string: "x = " + 5 gives "x = 5"
            if (op == TokenType.Plus && (leftValue is string || rightValue is string))
            {
                return Stringify(leftValue) + Stringify(rightValue);
            }

            // == works for any two values; numbers and strings are never equal
            if (op == TokenType.EqualEqual)
            {
                return ToNumber(leftValue.Equals(rightValue));
            }

            // every other operator needs two numbers
            if (leftValue is not double left || rightValue is not double right)
            {
                throw new RuntimeError($"Operator '{op}' needs two numbers, got {TypeName(leftValue)} and {TypeName(rightValue)}");
            }

            return op switch
            {
                TokenType.Plus => left + right,
                TokenType.Minus => left - right,
                TokenType.Star => left * right,
                TokenType.Slash => right == 0 ? throw new RuntimeError("Division by zero") : left / right,
                TokenType.Less => ToNumber(left < right),
                TokenType.LessEqual => ToNumber(left <= right),
                TokenType.Greater => ToNumber(left > right),
                TokenType.GreaterEqual => ToNumber(left >= right),
                _ => throw new RuntimeError($"Unknown operator: {op}")
            };
        }

        throw new RuntimeError($"Unknown expression type: {expr.GetType().Name}");
    }

    // Comparisons still give 1 (true) or 0 (false). It must be a double,
    // not an int, so the result can be used in arithmetic: (1 < 2) + 1
    private static double ToNumber(bool value) => value ? 1 : 0;

    // What counts as "true" in if/while: any number except 0, any string except ""
    private static bool IsTruthy(object value) => value switch
    {
        double number => number != 0,
        string text => text.Length > 0,
        _ => true
    };

    private static string Stringify(object value) => value switch
    {
        double number => number.ToString(CultureInfo.InvariantCulture),
        _ => value.ToString() ?? ""
    };

    private static string TypeName(object value) => value switch
    {
        double => "a number",
        string => "a string",
        _ => value.GetType().Name
    };
}
