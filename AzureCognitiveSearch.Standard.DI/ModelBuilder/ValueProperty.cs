using System.Reflection;

namespace AzureCognitiveSearch.DI.FromNugetPackage.ModelBuilder;

public struct ValueProperty : IProperty
{
    public ValueProperty(PropertyInfo propertyInfo, IPropertySettings propertySettings)
    {
        PropertyInfo = propertyInfo;
        PropertySettings = propertySettings;
    }

    public PropertyInfo PropertyInfo { get; }
    public IPropertySettings PropertySettings { get; }
}
