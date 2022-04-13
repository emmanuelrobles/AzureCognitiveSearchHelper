using System.Linq.Expressions;
using AzureCognitiveSearch.Abstractions;
using AzureCognitiveSearch.Application.Models;
using AzureCognitiveSearch.Applications.Context;

namespace AzureCognitiveSearch.Applications.Models;

public readonly struct ValueAzureQueryProvider : IAzureQueryProvider
{
    private readonly IAzureContext _context;

    public ValueAzureQueryProvider(IAzureContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IAzureQueryable<TResult> CreateQuery<TResult>(Expression expression)
    {
        return new ValueAzureQueryable<TResult>(this, expression);
    }

    public Task<IPaginationResult<TResult>> ExecuteAsync<TResult>(Expression expression)
    {
        return _context.ExecuteAsync<TResult>(expression);
    }
}
