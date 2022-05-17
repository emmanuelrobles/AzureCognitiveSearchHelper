using AzureCognitiveSearch.Abstractions;
using AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;
using Sample.AzureCognitiveSearch.DI.Contexts.Models;

namespace Sample.AzureCognitiveSearch.DI.Contexts;

/// <summary>
/// Azure Cotnext using model builder/fluent api
/// </summary>
public class AzureContextFluentApi : IAzureContextModelBuilder
{

    /// <summary>
    /// index name
    /// </summary>
    private readonly string _indexName = string.Empty;
    
    /// <summary>
    /// Azure set that you can query on
    /// </summary>
    public IAzureQueryable<AzureModel> Set { get; set; }

    
    /// <summary>
    /// ctor with an index name
    /// </summary>
    /// <param name="indexName"></param>
    public AzureContextFluentApi(string indexName)
    {
        _indexName = indexName;
    }
    
    /// <summary>
    /// Builds the model
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <returns>A list of properties to be used</returns>
    public IEnumerable<IProperty> BuildModel(IModelBuilder modelBuilder)
    {
        modelBuilder
            // property to modify      
            .SetPropertySettings<AzureContextFluentApi, AzureModel>(context => context.Set)
            // sets an index name
            .WithIndexName(_indexName);
        return modelBuilder.Properties;
    }
}
