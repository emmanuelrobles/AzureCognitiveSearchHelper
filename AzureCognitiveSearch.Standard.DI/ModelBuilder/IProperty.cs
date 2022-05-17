using System.Reflection;

namespace AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;

public interface IProperty
{
    public PropertyInfo PropertyInfo { get; }
    public IPropertySettings PropertySettings { get; }
}
