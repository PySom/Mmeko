using Mmeko.Contants;

public static class MapperInfrastructure
{
    public const string Definition = $$"""
        {{AttributeGeneratorHelper.GeneratedHeaderComment}}
        using Mmeko.Models.Infrastructure;
        namespace Mmeko.Infrastructure;
        public interface IMapper
        {
            TSelf Map<TSelf, TIn>(TIn value) where TSelf : IMap<TSelf, TIn>, new();
            TIn Map<TSelf, TIn>(TSelf self) where TSelf : IMap<TSelf, TIn>, new();
        }
        """;
}




