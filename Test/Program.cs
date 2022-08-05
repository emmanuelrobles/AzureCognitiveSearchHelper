// See https://aka.ms/new-console-template for more information
//
// using AzureCognitiveSearch.SourceGenerator.Models.Generators;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp;
// using System.Reflection;
//
// Compilation inputCompilation = MyClass.CreateCompilation(@"
// namespace MyCode
// {
//     public class Program
//     {
//         public static void Main(string[] args)
//         {
//         }
//     }
// }
// ");
// var generator = new GeneratorV1();
//
// GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
//
// driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
//
// Console.WriteLine();
//
// public static class  MyClass
// {
//     public static Compilation CreateCompilation(string source)
//         => CSharpCompilation.Create("compilation",
//             new[] { CSharpSyntaxTree.ParseText(source) },
//             new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
//             new CSharpCompilationOptions(OutputKind.ConsoleApplication));
//     
// }



Console.WriteLine("Hello, World!");

