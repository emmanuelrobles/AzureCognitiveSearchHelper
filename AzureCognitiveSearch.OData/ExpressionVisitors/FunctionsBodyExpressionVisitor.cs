using System.Linq.Expressions;

namespace AzureCognitiveSearch.OData.ExpressionVisitors;

/// <summary>
/// Visitor that will build body inside a function
/// </summary>
internal class FunctionsBodyExpressionVisitor : ExpressionVisitor
{
    /// <summary>
    /// Gets value for member access
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Not a valid expression if its a member access</exception>
    protected override Expression VisitMember(MemberExpression node)
    {
        var objectMember = Expression.Convert(node, typeof(object));
        var getterLambda = Expression.Lambda<Func<object>>(objectMember);
        var getter = getterLambda.Compile();
        return Visit(getter() as Expression ?? throw new ArgumentException("Not a valid expression"));
    }

    /// <summary>
    /// If Lambda just return
    /// </summary>
    /// <param name="node"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        return node;
    }
}
