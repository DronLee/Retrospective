using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RetrospectiveUnitTests
{
    public static class InMemoryDbContextOptionsFactory
    {
        public static DbContextOptions<T> Create<T>() where T : DbContext
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<T>();
            builder.UseInMemoryDatabase("Retrospective").UseInternalServiceProvider(serviceProvider);
            return builder.Options;
        }
    }
}