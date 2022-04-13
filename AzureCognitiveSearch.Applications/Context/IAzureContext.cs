using System.Linq.Expressions;
using AzureCognitiveSearch.Abstractions;

namespace AzureCognitiveSearch.Applications.Context;

public interface IAzureContext
{
    Task<IPaginationResult<TResult>> ExecuteAsync<TResult>(Expression expression);
}
