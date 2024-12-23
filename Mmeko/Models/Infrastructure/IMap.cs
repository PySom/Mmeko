using Mmeko.Contants;

public static class MapModelInfrastructure
{
    public const string Definition = $$"""
        {{AttributeGeneratorHelper.GeneratedHeaderComment}}
        
        namespace Mmeko.Models.Infrastructure;
        public interface IMap<TSelf, TIn> where TSelf : IMap<TSelf, TIn>
        {
            TSelf ToSelf(TIn @in);
            TIn ToIn();
        }
        """;
}
