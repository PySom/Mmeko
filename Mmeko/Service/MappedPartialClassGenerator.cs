using Mmeko.Contants;
using Mmeko.Models;
using System.Text;

namespace Mmeko.Service;

public static class MappedPartialClassGenerator
{
    public static string GetImplementation(MappingItem mappingItem)
    {
        var genCode = new StringBuilder(500);
        string typeKind = mappingItem.IsRecord ? "record" : "class";
        genCode.Append($$"""
            {{AttributeGeneratorHelper.GeneratedHeaderComment}}
            using Mmeko.Models.Infrastructure;
            namespace {{mappingItem.Namespace}};
            {{AttributeGeneratorHelper.GeneratedCodeAttribute}}
            public partial {{typeKind}} {{mappingItem.ClassName}} : IMap<{{mappingItem.ClassName}}, {{mappingItem.MappingClassName}}>
            { 
                // create all properties
                {{DefineClassProperties(mappingItem.PropertiesInBoth)}}
                // define the default transform property
                private Action<{{mappingItem.ClassName}}, {{mappingItem.MappingClassName}}> Transform { get; set; } = (_, _) => { };

                // implement the interfaces
                public {{mappingItem.ClassName}} ToSelf({{mappingItem.MappingClassName}} @in)
                {
                    {{DoPropertyAssignment(mappingItem)}}
                    Transform(this, @in);
                    return this;
                }

                public {{mappingItem.MappingClassName}} ToIn()
                {
                    return new {{mappingItem.MappingClassName}}
                    {
                        {{DoMappingPropertyAssignment(mappingItem)}}
                    };
                }
            }
            """);
        return genCode.ToString();
    }

    private static string DefineClassProperties(EquatableList<Item> properties)
    {
        var genCode = new StringBuilder(200);
        foreach (var property in properties)
        {
            string defaultAssignment = property.IsRequired ? " = default;" : string.Empty;
            genCode.Append($$"""
            public {{property.Type}} {{property.Name}} { get; set; }{{defaultAssignment}}
            """);
            genCode.Append('\n');
        }

        return genCode.ToString();
    }

    private static string DoMappingPropertyAssignment(MappingItem mappingItem)
    {
        var genCode = new StringBuilder(200);
        foreach (var property in mappingItem.PropertiesInBoth)
        {
            genCode.Append($$"""
            {{property.Name}} = this.{{property.Name}},
            """);
            genCode.Append('\n');
        }
        foreach (var property in mappingItem.DuplicateProperties)
        {
            genCode.Append($$"""
            {{property.Name}} = this.{{property.Name}},
            """);
            genCode.Append('\n');
        }
        foreach (var property in mappingItem.PropertiesInMapOnly)
        {
            var propertiesInClass = mappingItem.Properties.Select(x => x.Name).ToList();
            var assignment = propertiesInClass.Contains(property.Name) ? $"this.{property.Name}" : "default";
            genCode.Append($$"""
            {{property.Name}} = {{assignment}},
            """);
            genCode.Append('\n');
        }
        
        return genCode.ToString();
    }

    private static string DoPropertyAssignment(MappingItem item)
    {
        var genCode = new StringBuilder(200);
        // Assign items in both
        foreach (var property in item.PropertiesInBoth)
        {
            genCode.Append($$"""
            {{property.Name}} = @in.{{property.Name}};
            """);
            genCode.Append('\n');
        }
        // Assign class properties
        foreach (var property in item.Properties)
        {
            var propertiesInMappingClass = item.PropertiesInMapOnly.Select(x => x.Name).ToList();
            var propertiesInDuplicate = item.DuplicateProperties.Select(x => x.Name).ToList();
            var assignment = (propertiesInMappingClass.Contains(property.Name) || propertiesInDuplicate.Contains(property.Name)) 
                ? $"@in.{property.Name}" 
                : "default";
            genCode.Append($$"""
            {{property.Name}} = {{assignment}};
            """);
            genCode.Append('\n');
        }
        return genCode.ToString();
    }
}
