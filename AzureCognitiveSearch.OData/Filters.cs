using System.Linq.Expressions;
using AzureCognitiveSearch.OData.ExpressionVisitors;

namespace AzureCognitiveSearch.OData;

public static class Filters
{
    public static string TransformFilter(Expression filter)
    {
        //Handle expression types
        static string Transform(Expression expression,ref ValueTransformOptions options)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return GetValueFromConstant(expression as ConstantExpression);
                case ExpressionType.Parameter:
                    return expression.ToString();
                case ExpressionType.MemberAccess:
                    var memberExpression = expression as MemberExpression;
                    return MemberToString(memberExpression, options.MemberAccessIgnoreParent);
                case ExpressionType.Call:
                    return CallToString(expression as MethodCallExpression);
                case ExpressionType.Lambda:
                    var lambdaExpression = expression as LambdaExpression;
                    var valueTransformOptions = (new ValueTransformOptions { MemberAccessIgnoreParent = false });
                    return $"{lambdaExpression.Parameters[0]}:{Transform(lambdaExpression.Body, ref valueTransformOptions)}";
                case ExpressionType.Convert:
                    var convertUnary = expression as UnaryExpression;
                    return Transform(convertUnary.Operand, ref options);
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
                        $"({Transform(be.Left, ref options)} {BinaryExpressionToString(expression.NodeType)} {Transform(be.Right, ref options)})";
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
            var valueTransformOptions = new ValueTransformOptions();

            // header visitor
            var expressionHeaderVisitor = new FunctionHeaderExpressionVisitor();
            var expressionBodyVisitor = new FunctionsBodyExpressionVisitor();
            
            var callExpression = expression as MethodCallExpression;
            switch (expression.Method.Name)
            {
                case nameof(Enumerable.Any):
                    var callerAny = expressionHeaderVisitor.Visit(callExpression.Arguments[0]) as MemberExpression;
                    var bodyAny = callExpression.Arguments.Count > 1
                        ? expressionBodyVisitor.Visit(callExpression.Arguments[1])
                        : null;
                    return $"{MemberToString(callerAny)}/any({(bodyAny != null ? Transform(bodyAny, ref valueTransformOptions) : "")})";
                case nameof(Enumerable.All):
                    var callerAll = callExpression.Arguments[0] as MemberExpression;
                    var bodyAll = callExpression.Arguments.Count > 1
                        ? new FunctionsBodyExpressionVisitor().Visit(callExpression.Arguments[1])
                        : null;
                    return $"{MemberToString(callerAll)}/any({(bodyAll != null ? Transform(bodyAll,ref valueTransformOptions) : "")})";
                case nameof(AzureFunctions.AzureSearchIn):

                    T CompileLambda<T>(Expression e)
                    {
                        var objectMember = Expression.Convert(e, typeof(T));
                        var getterLambda = Expression.Lambda<Func<T>>(objectMember);
                        return getterLambda.Compile()();
                    }

                    var value_to_compare = callExpression.Arguments[0] as MemberExpression;
                    var array = CompileLambda<IEnumerable<string>>(callExpression.Arguments[1]);
                    var delimiter = CompileLambda<string>(callExpression.Arguments[2]);
                    return $"search.in({Transform(value_to_compare,ref valueTransformOptions)}, " +
                           $"'{String.Join(delimiter, array)}', " +
                           $"'{delimiter}')";
            }

            throw new ArgumentException("Method not supported");
        }
        
        // member function to string
        static string MemberToString(MemberExpression expression, bool ignoreParent = true)
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
                    return (GetValueFromConstant(Expression.Constant(InvokeExpression(expression))), true);
                }

                var childExpression = MemberStringHelper(subExpression.Expression as MemberExpression);
                if (childExpression.compiled)
                {
                    return (childExpression.expression, true);
                }

                var parentExpression = subExpression.Expression.NodeType switch
                {
                    ExpressionType.Parameter => ignoreParent ? "" : (subExpression.Expression as ParameterExpression)?.Name +"/",
                    ExpressionType.MemberAccess => childExpression.expression+ "/",
                    _ => throw new ArgumentException("Expression not handle in member access")
                };

                return ($"{parentExpression}{subExpression.Member.Name}", false);
            }

            return MemberStringHelper(expression).expression;
        }

        // invokes an expression dynamically
        static object? InvokeExpression(Expression expression) => Expression.Lambda(expression).Compile().DynamicInvoke();

        var valueTransformOptions = new ValueTransformOptions();
        return Transform(filter, ref valueTransformOptions);
    }
    
    //Process constant values
    private static string GetValueFromConstant(ConstantExpression expression)
    {
        Dictionary<Type, Func<string>> dictionary = new()
        {
            { typeof(string), () => $"'{expression.Value}'" },
            { typeof(bool), () => $"{expression.Value.ToString().ToLower()}" },
        };
        var expressionType = expression.Value.GetType();
        //handle special case
        if (dictionary.ContainsKey(expressionType))
        {
            return dictionary[expressionType]();
        }

        return expression.ToString();
    }
    /// <summary>
    /// Private structure for odata parser
    /// </summary>
    private struct ValueTransformOptions
    {
        public ValueTransformOptions()
        {
        }

        /// <summary>
        /// Set the member access to no parse the first parent if true
        /// </summary>
        public bool MemberAccessIgnoreParent { get; init; } = true;
    }
}
