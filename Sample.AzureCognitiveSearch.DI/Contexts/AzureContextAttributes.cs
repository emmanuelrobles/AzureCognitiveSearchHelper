using AzureCognitiveSearch.Abstractions;
using AzureCognitiveSearch.Applications.Contexts;
using AzureCognitiveSearch.DI.FromNugetPackage.Attributes;
using Sample.AzureCognitiveSearch.DI.Contexts.Models;

namespace Sample.AzureCognitiveSearch.DI.Contexts;

public class AzureContextAttributes : IAzureContext
{
    /// <summary>
    /// Azure set that you can query on
    /// </summary>
    [AzureIndexName("__INDEX_NAME__")]
    public IAzureQueryable<AzureModel> Set { get; set; }
}
