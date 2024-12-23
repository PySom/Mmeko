using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mmeko.Contants;
using Mmeko.Models;
using Mmeko.Service;
using System.Collections.Immutable;

namespace Mmeko;

[Generator]
public class MapGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitializationOutputCallback);
        var syntaxProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "Mmeko.Contants.MapperAttribute",
            predicate: IsClassOrRecord,
            transform: GetMappedItem
        );

        context.RegisterSourceOutput(syntaxProvider, Execute);
    }

    private static void PostInitializationOutputCallback(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("Mmeko.Constants.MapperAttribute.g.cs", AttributeGeneratorHelper.MapperAttribute);
        context.AddSource("Mmeko.Constants.MapAttribute.g.cs", AttributeGeneratorHelper.MapAttribute);
        context.AddSource("Mmeko.Service.Mapper.g.cs", MapperService.Definition);
        context.AddSource("Mmeko.Models.Infrastructure.IMap.g.cs", MapModelInfrastructure.Definition);
        context.AddSource("Mmeko.Infrastructure.IMapper.g.cs", MapperInfrastructure.Definition);
        context.AddSource("Mmeko.Service.ServiceCollection.g.cs", MappedRegistration.Definition);
    }

    private static bool IsClassOrRecord(SyntaxNode syntaxNode, CancellationToken cancellationToken) => syntaxNode is ClassDeclarationSyntax or RecordDeclarationSyntax;
    private static MappingItem? GetMappedItem(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        var symbol = context.TargetSymbol;
        var mapAttribute = symbol.GetAttributes()
            .Where(static attribute => attribute?.AttributeClass?.Name.Contains("MapAttribute") ?? false &&
                 attribute.AttributeClass?.TypeArguments.Length == 1).FirstOrDefault();

        if (mapAttribute == null) return null;

        var classProperty = GetPropertiesFromAttributes(mapAttribute);
        var propertiesDefinedInThisCLass = (symbol as INamedTypeSymbol)
            ?.GetMembers()
            .Where(x => !x.IsVirtual)
            ?.OfType<IPropertySymbol>()
            ?.Select(static x => new Item
            {
                IsRequired = x.IsRequired,
                Name = x.Name,
                Type = x.Type.ToDisplayString()
            })?.ToEquatableList() ?? [];

        var propertiesDefinedInThisCLassNames = propertiesDefinedInThisCLass.Select(x => x.Name).ToList();

        var removedDuplicatePrpertiesInBoth = classProperty
            .PropertiesInBoth
            ?.Where(x => !propertiesDefinedInThisCLass.Any(y => y.Name == x.Name && y.Type == x.Type))
            ?.ToEquatableList() ?? [];

        var duplicatePrpertiesInBoth = classProperty
            .PropertiesInBoth
            ?.Where(x => propertiesDefinedInThisCLass.Any(y => y.Name == x.Name && y.Type == x.Type))
            ?.ToEquatableList() ?? [];

        var mappingItem = new MappingItem
        {
            Namespace = symbol.ContainingNamespace.IsGlobalNamespace
            ? null : symbol.ContainingNamespace.ToDisplayString(),
            ClassName = symbol.Name,
            IsRecord = (symbol as INamedTypeSymbol)?.IsRecord ?? false,
            PropertiesInBoth = removedDuplicatePrpertiesInBoth,
            PropertiesInMapOnly = classProperty.PropertiesInOnlyMap,
            MappingClassName = mapAttribute.AttributeClass?.TypeArguments[0]?.ToDisplayString(),
            Properties = propertiesDefinedInThisCLass,
            DuplicateProperties = duplicatePrpertiesInBoth
        };

        return mappingItem;
    }
    private static ClassProperty GetPropertiesFromAttributes(AttributeData mapAttribute)
    {
        var excludedProperties = mapAttribute.NamedArguments
            .Where(static x => x.Key == "Exclude")
            ?.Select(static x => x.Value.Values.Select(x => x.Value as string).ToArray())
            ?.FirstOrDefault() ?? [];

        var typeProperties = mapAttribute.AttributeClass?.TypeArguments[0]
            ?.GetMembers()
            .OfType<IPropertySymbol>();

        var extractItems = (IEnumerable<IPropertySymbol>? properties, Func<IPropertySymbol, bool> condition) =>
        properties?.Where(condition)?.Select(static x => new Item
        {
            IsRequired = x.IsRequired,
            Name = x.Name,
            Type = x.Type.ToDisplayString()
        })?.ToEquatableList() ?? [];

        var itemsInBoth = extractItems(typeProperties, x => !x.IsVirtual && !excludedProperties.Contains(x.Name));
            
        var itemsInOnlyMap = extractItems(typeProperties, x => !x.IsVirtual && excludedProperties.Contains(x.Name));
        

        return new ClassProperty { PropertiesInBoth = itemsInBoth, PropertiesInOnlyMap = itemsInOnlyMap};
    }

   

    private static void Execute(SourceProductionContext context, MappingItem? mappingItem)
    {
        if (mappingItem == null) return;
        context.AddSource($"Mmeko.Service.{mappingItem.ClassName}{mappingItem.MappingClassName}.g.cs", MappedPartialClassGenerator.GetImplementation(mappingItem));
    }
}
