using System.Linq.Expressions;
using System.Reflection;
using Azure.Search.Documents.Indexes;
using AzureCognitiveSearch.Abstractions;
using AzureCognitiveSearch.Applications.Contexts;
using AzureCognitiveSearch.Applications.Models;
using AzureCognitiveSearch.Applications.Runners;
using AzureCognitiveSearch.Context.FromNugetPackage;
using AzureCognitiveSearch.DI.FromNugetPackage.Attributes;
using AzureCognitiveSearch.OData;
using AzureCognitiveSearch.QueryRunnerV1;
using Microsoft.Extensions.DependencyInjection;

namespace AzureCognitiveSearch.DI.FromNugetPackage;

/// <summary>
/// Default options
/// </summary>
class AzureSearchOptions : IAzureSearchOptions
{
    
    private static Func<uint, (uint take, uint skip)> PaginationHelper(ushort qty) =>
        page => (take: qty, skip: (page - 1) * qty);

    public Func<Expression, string> FilterToString { get; set; } = Filters.TransformFilter;
    public Func<ushort, Func<uint, (uint take, uint skip)>> Pagination { get; set; } = PaginationHelper;
    public Func<Expression, string> OrderByExpression { get; set; } = Filters.TransformFilter;
}

/// <summary>
/// Dependency injection
/// </summary>
public static class DI
{
    /// <summary>
    /// Adds a context
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="indexClient"></param>
    /// <param name="optionsCallBack"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <exception cref="ArgumentException"></exception>
    public static void AddAzureSearchContext<TContext>(this IServiceCollection collection, SearchIndexClient indexClient,
        Action<IAzureSearchOptions>? optionsCallBack = null)
        where TContext : class, IAzureContext, new()
    {
        // set default options
        var options = new AzureSearchOptions();
        
        // if there is a func invoke it
        optionsCallBack?.Invoke(options);

        TContext Builder()
        {
            var contextObject = (TContext)Activator.CreateInstance(typeof(TContext));
            
            foreach (var propertyInfo in typeof(TContext).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var ignore = propertyInfo.GetCustomAttribute<AzureIndexIgnoreAttribute>();
            
                // check if the property is different for IAureQueryable or is set to ignore
                if (propertyInfo.PropertyType.GetGenericTypeDefinition() != typeof(IAzureQueryable<>) || ignore is not null)
                {
                    continue;
                }
            
                // Get Index name attribute
                var indexNameAttribute = propertyInfo.GetCustomAttribute<AzureIndexNameAttribute>();
            
                // if not found return
                // TODO: throw an exception
                if (indexNameAttribute is null)
                {
                    continue;
                }

                // get model type
                var indexModel = propertyInfo.PropertyType.GenericTypeArguments.Single();

                // gets the search client
                var client = indexClient.GetSearchClient(indexNameAttribute.IndexName);
            
                // create query runner
                var azureQueryRunnerGeneric = typeof(ValueAzureSearchQueryRunner<>).MakeGenericType(indexModel);
                if (Activator.CreateInstance(azureQueryRunnerGeneric, client, options) is not IAzureQueryRunner azureQueryRunner)
                {
                    throw new ArgumentException("Cannot create an Azure query runner instance");
                }
            
                // create a provider
                var azureQueryProvider = new ValueAzureQueryProvider(azureQueryRunner);
            
                // create queryable
                var azureQueryableGeneric = typeof(AzureQueryable<>).MakeGenericType(indexModel);
                var azureQueryable = Activator.CreateInstance(azureQueryableGeneric, azureQueryProvider,null);

                // set it on the object
                propertyInfo.SetValue(contextObject,azureQueryable);
            }

            return contextObject;
        }
        
        // add client as scoped
        collection.AddScoped(_ => Builder());
    }
}
