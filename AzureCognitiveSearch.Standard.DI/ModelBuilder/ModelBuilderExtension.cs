using System.Linq.Expressions;
using System.Reflection;
using AzureCognitiveSearch.Abstractions;
using AzureCognitiveSearch.Applications.Contexts;

namespace AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;

/// <summary>
/// Extensions to build the model
/// </summary>
public static class ModelBuilderExtension
{
    public static IPropertySettings SetPropertySettings<TContext,TEntity>(this IModelBuilder propertySettings, 
        Expression<Func<TContext,IAzureQueryable<TEntity>>> selector)
    {

        var memberExpression = selector.Body as MemberExpression;
        var propInfo = (PropertyInfo)memberExpression.Member;
        var settings = new PropertySettings();
        propertySettings.Properties = propertySettings.Properties.Append(new ValueProperty(propInfo,settings));
        return settings;
    }
    
    public static IPropertySettings WithIndexName(this IPropertySettings propertySettings, string indexName)
    {
        propertySettings.IndexName = indexName;
        return propertySettings;
    }
    
    public static IPropertySettings Ignore(this IPropertySettings propertySettings)
    {
        propertySettings.IndexName = string.Empty;
        return propertySettings;
    }
}
