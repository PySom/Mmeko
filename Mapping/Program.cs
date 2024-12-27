
using Mapping.Model;
using Microsoft.Extensions.DependencyInjection;
using Mmeko.Infrastructure;
using Mmeko.Service;

var services = ConfigureServices();

var mapper = services.GetRequiredService<IMapper>();

var a = new A { D = "Hello" };
var b = new B { D = "World", C = 1, F = 5, Items = [], G = 4 };

var b2 = mapper.Map<A, B>(a);
var a2 = mapper.Map<A, B>(b);

var k = mapper.Map<K, B>(b);
var b3 = mapper.Map<K, B>(k);

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