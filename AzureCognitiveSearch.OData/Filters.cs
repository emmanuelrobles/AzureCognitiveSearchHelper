using System.Linq.Expressions;

namespace AzureCognitiveSearch.OData;

public static class Filters
{
    public static string TransformFilter(Expression filter)
    {
        //Handle expression types
        static string Transform(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return GetValueFromConstant(expression as ConstantExpression);
                case ExpressionType.Parameter:
                    return expression.ToString();
                case ExpressionType.MemberAccess:
                    var memberExpression = expression as MemberExpression;
                    return MemberToString(memberExpression);
                case ExpressionType.Call:
                    return CallToString(expression as MethodCallExpression);
                case ExpressionType.Lambda:
                    var lambdaExpression = expression as LambdaExpression;
                    return $"{lambdaExpression.Parameters[0]}:{Transform(lambdaExpression.Body)}";
                case ExpressionType.Convert:
                    var convertUnary = expression as UnaryExpression;
                    return Transform(convertUnary.Operand);
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    var be = expression as BinaryExpression;
                    return
                        $"({Transform(be.Left)} {BinaryExpressionToString(expression.NodeType)} {Transform(be.Right)})";
                default:
                    throw new ArgumentException($"Expression expression not define: {expression.NodeType}");
            }
        }

        // cast binary operators to azure string
        static string BinaryExpressionToString(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal: return "eq";
                case ExpressionType.NotEqual: return "ne";
                case ExpressionType.GreaterThan: return "gt";
                case ExpressionType.GreaterThanOrEqual: return "ge";
                case ExpressionType.LessThan: return "lt";
                case ExpressionType.LessThanOrEqual: return "le";
                case ExpressionType.AndAlso: return "and";
                case ExpressionType.OrElse: return "or";
                default:
                    throw new ArgumentException();
            }
        }

        //Handles methods
        static string CallToString(MethodCallExpression? expression)
        {
            var callExpression = expression as MethodCallExpression;
            switch (expression.Method.Name)
            {
                case "Any":
                    var callerAny = callExpression.Arguments[0] as MemberExpression;
                    var bodyAny = callExpression.Arguments.Count > 1
                        ? callExpression.Arguments[1] as LambdaExpression
                        : null;
                    return $"{Transform(callerAny)}/any({(bodyAny != null ? Transform(bodyAny) : "")})";
                case "All":
                    var callerAll = callExpression.Arguments[0] as MemberExpression;
                    var bodyAll = callExpression.Arguments.Count > 1
                        ? callExpression.Arguments[1] as LambdaExpression
                        : null;
                    return $"{Transform(callerAll)}/any({(bodyAll != null ? Transform(bodyAll) : "")})";
                case "AzureSearchIn":

                    T CompileLambda<T>(Expression e)
                    {
                        var objectMember = Expression.Convert(e, typeof(T));
                        var getterLambda = Expression.Lambda<Func<T>>(objectMember);
                        return getterLambda.Compile()();
                    }

                    var value_to_compare = callExpression.Arguments[0] as MemberExpression;
                    var array = CompileLambda<IEnumerable<string>>(callExpression.Arguments[1]);
                    var delimiter = CompileLambda<string>(callExpression.Arguments[2]);
                    return $"search.in({Transform(value_to_compare)}, " +
                           $"'{String.Join(delimiter, array)}', " +
                           $"'{delimiter}')";
            }

            throw new ArgumentException("Method not supported");
        }
        
        // member function to string
        static string MemberToString(MemberExpression expression)
        {
            (string expression, bool compiled) MemberStringHelper(MemberExpression? subExpression)
            {
                //base case
                if (subExpression == null)
                {
                    return ("", false);
                }

                // if subexpression is a constant compile whole expression
                if (subExpression.Expression.NodeType is ExpressionType.Constant)
                {
                    return (Filters.GetValueFromConstant(Expression.Constant(InvokeExpression(expression))), true);
                }

                var childExpression = MemberStringHelper(subExpression.Expression as MemberExpression);
                if (childExpression.compiled)
                {
                    return (childExpression.expression, true);
                }

                var parentExpression = "";
                if (subExpression.Expression.NodeType is ExpressionType.MemberAccess)
                {
                    parentExpression = $"{childExpression.expression}/";
                }

                return ($"{parentExpression}{subExpression.Member.Name}", false);
            }

            return MemberStringHelper(expression).expression;
        }

        // invokes an expression dynamically
        static object? InvokeExpression(Expression expression) => Expression.Lambda(expression).Compile().DynamicInvoke();
        
        return Transform(filter);
    }
    
    //Process constant values
    private static string GetValueFromConstant(ConstantExpression expression)
    {
        Dictionary<Type, Func<string>> dictionary = new()
        {
            { typeof(string), () => $"'{expression.Value}'" },
        };
        var expressionType = expression.Value.GetType();
        //handle special case
        if (dictionary.ContainsKey(expressionType))
        {
            return dictionary[expressionType]();
        }

        return expression.ToString();
    }
}
