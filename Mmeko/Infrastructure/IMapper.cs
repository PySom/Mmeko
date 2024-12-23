using Mmeko.Contants;

public static class MapperInfrastructure
{
    public const string Definition = $$"""
        {{AttributeGeneratorHelper.GeneratedHeaderComment}}
        using Mmeko.Models.Infrastructure;
        namespace Mmeko.Infrastructure;
        public interface IMapper<TSelf, TIn> where TSelf : IMap<TSelf, TIn>, new()
        {
            TSelf Map(TIn value);
            TIn Map(TSelf self);
        }
        """;
}




