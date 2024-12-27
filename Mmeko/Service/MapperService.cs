using Mmeko.Contants;

public static class MapperService
{
    public const string Definition = $$"""
        {{AttributeGeneratorHelper.GeneratedHeaderComment}}
        using Mmeko.Infrastructure;
        using Mmeko.Models.Infrastructure;
        
        namespace Mmeko.Service;
        public class Mapper : IMapper
        {
            public TSelf Map<TSelf, TIn>(TIn value) where TSelf : IMap<TSelf, TIn>, new()
            {
                TSelf into = new();
                return into.ToSelf(value);
            }

            public TIn Map<TSelf, TIn>(TSelf self) where TSelf : IMap<TSelf, TIn>, new()
            {
                return self.ToIn();
            }
        }
        """;
}