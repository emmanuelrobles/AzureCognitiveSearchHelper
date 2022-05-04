using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace AzureCognitiveSearch.OData.Test;

public class FilterTest
{
    private static string FilterExpressionWrapper<T>(Expression<Func<T, bool>> expression)
        => Filters.TransformFilter(expression.Body);

    [Fact]
    public void BinaryOperators_Test()
    {
        var num = 4;
        var str = "a string";
        var obj = new { str, num };
        Assert.Equal("(TypeInt eq 4)", FilterExpressionWrapper<MyClass>(c => c.TypeInt == num));
        Assert.Equal("(i eq 4)", FilterExpressionWrapper<int>(i => i == num));
        Assert.Equal("(i eq 4)", FilterExpressionWrapper<int>(i => i == obj.num));
        Assert.Equal("(i eq 4)", FilterExpressionWrapper<int>(i => i == 4));
        Assert.Equal("(i ne 4)", FilterExpressionWrapper<int>(i => i != 4));
        Assert.Equal("(i gt 4)", FilterExpressionWrapper<int>(i => i > 4));
        Assert.Equal("(i ge 4)", FilterExpressionWrapper<int>(i => i >= 4));
        Assert.Equal("(i lt 4)", FilterExpressionWrapper<int>(i => i < 4));
        Assert.Equal("(i le 4)", FilterExpressionWrapper<int>(i => i <= 4));
        Assert.Equal("((i eq True) or i)", FilterExpressionWrapper<bool>(i => i == true || i));
        Assert.Equal("((i eq True) and i)", FilterExpressionWrapper<bool>(i => i == true && i));

        Assert.Equal("(i eq 'a string')", FilterExpressionWrapper<string>(i => i == str));
        Assert.Equal("(i ne 'a string')", FilterExpressionWrapper<string>(i => i != str));
    }

    [Fact]
    public void Property_Test()
    {
        Assert.Equal("(TypeClass/TypeInt eq 4)",
            FilterExpressionWrapper<MyClass>(i => i.TypeClass.TypeInt == 4));
        Assert.Equal("(TypeInt eq 4)", FilterExpressionWrapper<MyClass>(i => i.TypeInt == 4));
    }

    [Fact]
    public void String_Test()
    {
        Assert.Equal("(TypeString eq 'p')", FilterExpressionWrapper<MyClass>(i => i.TypeString == "p"));
    }

    [Fact]
    public void Func_SearchIn_Test()
    {
        var s = "S";
        Assert.Equal("search.in(TypeString, 'A,B', ',')",
            FilterExpressionWrapper<MyClass>(i =>
                i.TypeString.AzureSearchIn(new List<string>() { "A", "B" }, ",")));
        Assert.Equal("search.in(TypeString, 'A,B', ',')",
            FilterExpressionWrapper<MyClass>(i => i.TypeString.AzureSearchIn(new[] { "A", "B" }, ",")));
        Assert.Equal("search.in(TypeString, 'A,B,S', ',')",
            FilterExpressionWrapper<MyClass>(i => i.TypeString.AzureSearchIn(new[] { "A", "B", s }, ",")));
        Assert.Equal("search.in(TypeString, 'A,B,S', ',')",
            FilterExpressionWrapper<MyClass>(i =>
                i.TypeString.AzureSearchIn(new List<string>() { "A", "B", s }, ",")));


        Assert.Equal("search.in(TypeClass/TypeString, 'A,B', ',')",
            FilterExpressionWrapper<MyClass>(i =>
                i.TypeClass.TypeString.AzureSearchIn(new List<string>() { "A", "B" }, ",")));
        Assert.Equal("search.in(TypeClass/TypeString, 'A,B', ',')",
            FilterExpressionWrapper<MyClass>(
                i => i.TypeClass.TypeString.AzureSearchIn(new[] { "A", "B" }, ",")));
        Assert.Equal("search.in(TypeClass/TypeString, 'A,B,S', ',')",
            FilterExpressionWrapper<MyClass>(i =>
                i.TypeClass.TypeString.AzureSearchIn(new[] { "A", "B", s }, ",")));
        Assert.Equal("search.in(TypeClass/TypeString, 'A,B,S', ',')",
            FilterExpressionWrapper<MyClass>(i =>
                i.TypeClass.TypeString.AzureSearchIn(new List<string>() { "A", "B", s }, ",")));
    }

    [Fact]
    public void Func_Any_Test()
    {
        var s = "S";
        Assert.Equal("TypeArray/any()",
            FilterExpressionWrapper<MyClass>(i => i.TypeArray.Any()));
    }
}

class MyClass
{
    public string TypeString { get; set; }
    public int TypeInt { get; set; }
    public MyClassB TypeClass { get; set; }
    public IEnumerable<int> TypeArray { get; set; }
}

class MyClassB
{
    public string TypeString { get; set; }
    public int TypeInt { get; set; }
    public IEnumerable<int> TypeArray { get; set; }
}