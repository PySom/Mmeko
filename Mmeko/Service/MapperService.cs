using Mmeko.Contants;

public static class MapperService
{
    public const string Definition = $$"""
        {{AttributeGeneratorHelper.GeneratedHeaderComment}}
        using Mmeko.Infrastructure;
        using Mmeko.Models.Infrastructure;
        
        namespace Mmeko.Service;
        public class Mapper<TSelf, TIn> : IMapper<TSelf, TIn> where TSelf : IMap<TSelf, TIn>, new()
        {
            public TSelf Map(TIn value)
            {
                TSelf into = new();
                return into.ToSelf(value);
            }

            public TIn Map(TSelf self)
            {
                return self.ToIn();
            }
        }
        """;
}