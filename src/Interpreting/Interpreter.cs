namespace miniLang;

public class Interpreter
{
    public double Evaluate(Expression expr)
    {
        if (expr is NumberExpression numberExpression)
        {
            return numberExpression.Value;
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
                TokenType.Slash => leftValue / rightValue,
                _ => throw new Exception($"Unknown operator: {binaryExpression.Operator}")
            };
        }

        throw new Exception($"Unknown expression type: {expr.GetType().Name}");
    }
}