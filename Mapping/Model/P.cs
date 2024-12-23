﻿using Mmeko.Contants;

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
        Transform = (b) =>
        {
            C = b.C * 5;
        };
    }
}

[Mapper]
[Map<B>(Exclude = [nameof(B.G)])]
public partial record K
{
    public string G { get; set; }
    public K()
    {
        Transform = (b) =>
        {
            C = b.C + 10;
            D = $"{b.D} Hello";
            F = b.C + F;
        };

        TransformBack = (b) =>
        {
            b.C = C * 1000;
            return b;
        };
    }
}