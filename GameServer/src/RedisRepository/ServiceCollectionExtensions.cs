using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RedisRepository;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RedisRepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisRepository(this IServiceCollection services, Action<RedisRepositoryOptions> configure)
        {
            var options = new RedisRepositoryOptions();
            configure(options);
            services.AddSingleton(options);
            return services;
        }

        public static IServiceCollection AddRedisSetRepository<T>(this IServiceCollection services)
        {
            services.TryAddSingleton<RedisSetRepository<T>>();
            return services;
        }

        public static IServiceCollection AddRedisValueRepository<T>(this IServiceCollection services)
        {
            services.TryAddSingleton<RedisValueRepository<T>>();
            return services;
        }
    }
}