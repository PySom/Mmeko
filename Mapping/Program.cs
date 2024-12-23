
using Mapping.Model;
using Microsoft.Extensions.DependencyInjection;
using Mmeko.Infrastructure;
using Mmeko.Service;

var services = ConfigureServices();

var mapper = services.GetRequiredService<IMapper<A, B>>();
var mapper2 = services.GetRequiredService<IMapper<K, B>>();

var a = new A { D = "Hello" };
var b = new B { D = "World", C = 1, F = 5, Items = [], G = 4 };

var b2 = mapper.Map(a);
var a2 = mapper.Map(b);

var k = mapper2.Map(b);
var b3 = mapper2.Map(k);

Console.WriteLine(b2);
Console.WriteLine(a2);
Console.WriteLine(k);
Console.WriteLine(b3);

static IServiceProvider ConfigureServices()
{
    IServiceCollection services = new ServiceCollection();
    
    services.AddTransientMmekoMappings();

    return services.BuildServiceProvider();
}