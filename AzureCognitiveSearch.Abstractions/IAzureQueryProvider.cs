using System.Linq.Expressions;

namespace AzureCognitiveSearch.Abstractions;

public interface IAzureQueryProvider
{
    public IAzureQueryable<TResult> CreateQuery<TResult>(Expression expression);
    
    public Task<IPaginationResult<TResult>> ExecuteAsync<TResult>(Expression expression);
}
