using System.Linq.Expressions;
using AzureCognitiveSearch.Abstractions;
using AzureCognitiveSearch.Applications.Runners;

namespace AzureCognitiveSearch.Applications.Models;

/// <summary>
/// Query provider implementation
/// </summary>
public readonly struct ValueAzureQueryProvider : IAzureQueryProvider
{
    private readonly IAzureQueryRunner _queryRunner;

    public ValueAzureQueryProvider(IAzureQueryRunner queryRunner)
    {
        _queryRunner = queryRunner ?? throw new ArgumentNullException(nameof(queryRunner));
    }
    
    /// <inheritdoc />
    public IAzureQueryable<TResult> CreateQuery<TResult>(Expression expression)
    {
        return new AzureQueryable<TResult>(this, expression);
    }
    
    /// <inheritdoc />
    public Task<IPaginationResult<TResult>> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
    {
        return _queryRunner.ExecuteAsync<TResult>(expression, token);
    }
}
