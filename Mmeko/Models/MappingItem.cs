namespace Mmeko.Models;

public record ClassProperty
{
    public EquatableList<Item> PropertiesInBoth { get; set; } = [];
    public EquatableList<Item> PropertiesInOnlyMap { get; set; } = [];
}

public record Item
{
    public string? Name { get; set; }
    public bool IsRequired { get; set; }
    public string? @Type { get; set; }
}
public record MappingItem
{
    public string? ClassName { get; set; }
    public string? Namespace { get; set; }
    public string? MappingClassName { get; set; }
    public bool IsRecord { get; set; }
    public EquatableList<Item> PropertiesInBoth { get; set; } = [];
    public EquatableList<Item> PropertiesInMapOnly { get; set; } = [];
    public EquatableList<Item> Properties { get; set; } = [];
    public EquatableList<Item> DuplicateProperties { get; set; } = [];
}
