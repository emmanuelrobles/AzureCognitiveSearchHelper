using System.Linq.Expressions;

namespace AzureCognitiveSearch.OData.ExpressionVisitors;

/// <summary>
/// Function Header visitor
/// </summary>
internal class FunctionHeaderExpressionVisitor : ExpressionVisitor
{
    /// <summary>
    /// Check when a method is call
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        // check if its an IQueryable
        return node.Method.Name == (nameof(Queryable.AsQueryable)) ? base.Visit(node.Arguments[0]) : base.VisitMethodCall(node);
    }
}
