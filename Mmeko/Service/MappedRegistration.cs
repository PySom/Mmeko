﻿using Mmeko.Contants;

public static class MappedRegistration
{
    public const string Definition = $$"""
        {{AttributeGeneratorHelper.GeneratedHeaderComment}}
        using Mmeko.Infrastructure;
        using Mmeko.Service;
        using Microsoft.Extensions.DependencyInjection;
        
        namespace Mmeko.Service;
        public static class MmekoExtensions
        {
            public static IServiceCollection AddTransientMmekoMappings(this IServiceCollection services)
            {
                services.AddTransient(typeof(IMapper<,>), typeof(Mapper<,>));
                return services;
            }

            public static IServiceCollection AddScopedMmekoMappings(this IServiceCollection services)
            {
                services.AddScoped(typeof(IMapper<,>), typeof(Mapper<,>));
                return services;
            }
            
            public static IServiceCollection AddSingletonMmekoMappings(this IServiceCollection services)
            {
                services.AddSingleton(typeof(IMapper<,>), typeof(Mapper<,>));
                return services;
            }
        }
        """;
}