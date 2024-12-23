using Mmeko.Contants;

namespace Mapping.Model;

public record D;
public record B
{
    public int C { get; set; }
    public string D { get; set; }
    public int F { get; set; }
    public int G { get; set; }
    public required List<D> Items { get; set; }
};

[Mapper]
[Map<B>(Exclude = [nameof(B.F)])]
public partial record A 
{
    public A()
    {
        Transform = (a, b) =>
        {
            a.C = b.C * 5;
        };
    }
}

[Mapper]
[Map<B>]
public partial record K
{
    public int G { get; set; }
    public K()
    {
        Transform = (k, b) =>
        {
            k.C = b.C + 10;
            k.D = $"{b.D} Hello";
            k.F = k.C + b.F;
        };
    }
}