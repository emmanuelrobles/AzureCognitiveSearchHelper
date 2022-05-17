using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Azure.Search.Documents.Indexes;
using AzureCognitiveSearch.Abstractions;
using AzureCognitiveSearch.Applications.Contexts;
using AzureCognitiveSearch.Applications.Models;
using AzureCognitiveSearch.Applications.Runners;
using AzureCognitiveSearch.Context.FromNugetPackage;
using AzureCognitiveSearch.DI.FromNugetPackage.Attributes;
using AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;
using AzureCognitiveSearch.OData;
using AzureCognitiveSearch.QueryRunnerV1;
using Microsoft.Extensions.DependencyInjection;

namespace AzureCognitiveSearch.DI.FromNugetPackage;

/// <summary>
/// Default options
/// </summary>
class AzureSearchOptions : IAzureSearchOptions
{
    /// <summary>
    /// Default pagination func
    /// </summary>
    /// <param name="qty">how many items per page</param>
    /// <returns>Func that takes the page and returns tuple with items to skip and items to return</returns>
    private static Func<uint, (uint take, uint skip)> PaginationHelper(ushort qty) =>
        page => (take: qty, skip: (page - 1) * qty);

    /// <summary>
    /// Func to be call to transform filter expression to string
    /// </summary>
    public Func<Expression, string> FilterExpression { get; set; } = Filters.TransformFilter;
    
    /// <summary>
    /// Function that solve pagination logic
    /// </summary>
    public Func<ushort, Func<uint, (uint take, uint skip)>> Pagination { get; set; } = PaginationHelper;
    
    /// <summary>
    /// Function that transform order by expression to string
    /// </summary>
    public Func<MemberExpression, string> OrderByExpression { get; set; } = Filters.TransformFilter;
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
    /// <param name="instanceBuilder"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <exception cref="ArgumentException"></exception>
    public static void AddAzureSearchContext<TContext>(this IServiceCollection collection, SearchIndexClient indexClient,
        Action<IAzureSearchOptions>? optionsCallBack = null,
        Func<TContext>? instanceBuilder = null)
        where TContext : class, IAzureContext
    {
        // set default options
        var options = new AzureSearchOptions();
        
        // if there is a func invoke it
        optionsCallBack?.Invoke(options);

        TContext Builder()
        {
            var contextObject = instanceBuilder is null ? (TContext)Activator.CreateInstance(typeof(TContext)) : instanceBuilder();
            
            // Get the properties from assembly
            var properties =
                GetModelFromPropertiesAttributes(
                    typeof(TContext).GetProperties(BindingFlags.Instance | BindingFlags.Public));

            // Get properties from fluent api
            //check if the class implements IAzureContext API
            if (typeof(TContext).IsAssignableTo(typeof(IAzureContextModelBuilder)))
            {
                // context is a model builder
                var contextModelBuilder = contextObject as IAzureContextModelBuilder;
                Debug.Assert(contextModelBuilder != null, nameof(contextModelBuilder) + " != null");
                
                // concat properties
                properties = properties.Concat(contextModelBuilder.BuildModel(new ValueModelBuilder()));
            }
            

            // Sets the properties into the instance
            foreach (var property in properties)
            {
                // get model type
                var indexModel = property.PropertyInfo.PropertyType.GenericTypeArguments.Single();
                
                // gets the search client
                var client = indexClient.GetSearchClient(property.PropertySettings.IndexName);
            
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
                property.PropertyInfo.SetValue(contextObject,azureQueryable);
            }
            
            return contextObject;
        }
        
        // add client as scoped
        collection.AddScoped(_ => Builder());
    }

    private static IEnumerable<IProperty> GetModelFromPropertiesAttributes(IEnumerable<PropertyInfo> propertyInfos)
    {
        foreach (var propertyInfo in propertyInfos)
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
            if (indexNameAttribute is null)
            {
                continue;
            }

            yield return new ValueProperty(propertyInfo, new PropertySettings(indexNameAttribute.IndexName));
        }
    }
}
