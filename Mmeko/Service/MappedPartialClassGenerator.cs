﻿using Mmeko.Contants;
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
                private Action<{{mappingItem.MappingClassName}}> Transform { get; set; } = (_) => { };

                // define the default transform back property
                private Func<{{mappingItem.MappingClassName}}, {{mappingItem.MappingClassName}}> TransformBack { get; set; } = (value) => value;

                // implement the interfaces
                public {{mappingItem.ClassName}} ToSelf({{mappingItem.MappingClassName}} @in)
                {
                    {{DoPropertyAssignment(mappingItem)}}
                    Transform(@in);
                    return this;
                }

                public {{mappingItem.MappingClassName}} ToIn()
                {
                    var result = new {{mappingItem.MappingClassName}}
                    {
                        {{DoMappingPropertyAssignment(mappingItem)}}
                    };
                    return TransformBack(result);
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
            var thereIsMappingInClassProperties = mappingItem.Properties.Any(x => x.Name == property.Name && x.Type == property.Type);
            var assignment = thereIsMappingInClassProperties ? $"this.{property.Name}" : "default";
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
            var thereIsMappingInMapClass = item.PropertiesInMapOnly.Any(x => x.Name == property.Name && x.Type == property.Type);
            var thereIsMappingInDuplicate = item.DuplicateProperties.Any(x => x.Name == property.Name && x.Type == property.Type);
            var assignment = thereIsMappingInMapClass || thereIsMappingInDuplicate ? $"@in.{property.Name}" : "default";
            genCode.Append($$"""
            {{property.Name}} = {{assignment}};
            """);
            genCode.Append('\n');
        }
        return genCode.ToString();
    }
}
