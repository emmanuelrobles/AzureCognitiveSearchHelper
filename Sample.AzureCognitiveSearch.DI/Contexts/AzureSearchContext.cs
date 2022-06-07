using Sample.AzureCognitiveSearch.DI.Contexts.Models;

/// <summary>
/// Azure Context using contructor injection 
/// </summary>
public class AzureSearchContext
{
    /// <summary>
    /// Azure Model that you can query on
    /// </summary>
    public IQueryable<AzureModel> QueryModel { get; private set; }

    /// <summary>
    /// ctor with queryable model
    /// </summary>
    /// <param name="indexName"></param>
    public AzureSearchContext(IQueryable<AzureModel> query)
	{
		QueryModel = query;
	}
}
