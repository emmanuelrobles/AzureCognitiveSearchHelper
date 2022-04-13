using System.Linq.Expressions;
using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Application.Models;

public struct ValueAzureQueryable<TSource> : IAzureQueryable<TSource>
{
    public Expression Expression { get; }
    public IAzureQueryProvider Provider { get; }
    public Type Type => typeof(TSource);

    public ValueAzureQueryable(IAzureQueryProvider provider, Expression? expression)
    {
        Expression = expression ?? Expression.Constant(null);
        Provider = provider;
    }
}
