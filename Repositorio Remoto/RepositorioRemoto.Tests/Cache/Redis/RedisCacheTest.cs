using FluentAssertions;
using RepositorioRemoto.Cache.Redis;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace RepositorioRemoto.Tests.Cache.Redis;

// patrong AAA
[TestFixture]
[TestOf(typeof(RedisCache<,>))]
public class RedisCacheTest {
    
    private RedisContainer _container = null!;
    private IConnectionMultiplexer _redis = null!;
    private RedisCache<int, string> _cache = null!;

    [OneTimeSetUp]
    public async Task Setup() {
        _container = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();

        await _container.StartAsync();
        
        _redis = await ConnectionMultiplexer.ConnectAsync(_container.GetConnectionString());
    }

    [OneTimeTearDown]
    public async Task TearDown() {
        await _redis.CloseAsync();
        await _container.DisposeAsync();
    }

    [SetUp]
    public void SetUp() {
        // prefijo unico por test (mismo contenedor)
        _cache = new RedisCache<int, string>(_redis, prefix: $"test:{Guid.NewGuid()}");
    }

    [TestFixture] public class CasosAdd : RedisCacheTest
    {
        [Test]
        public async Task Add_ConElementoValido_DeberiaGuardarElemento() {
            
            await _cache.Add(1, "uno");

            (await _cache.Get(1)).Should().Be("uno");
        }

        [Test]
        public async Task Add_ClaveExistente_DeberiaActualizarValor() {
            
            await _cache.Add(1, "uno");
            await _cache.Add(1, "uno editado");

            (await _cache.Get(1)).Should().Be("uno editado");
        }

        [Test]
        public async Task Add_ConExpiracion_DeberiaEliminarseTrasExpirar() {
            
            var cache = new RedisCache<int, string>(_redis, $"ttl:{Guid.NewGuid()}", TimeSpan.FromMilliseconds(300));

            await cache.Add(1, "temporal");
            (await cache.Get(1)).Should().Be("temporal");

            await Task.Delay(600);

            (await cache.Get(1)).Should().BeNull();
        }
    }

    [TestFixture] public class CasosGet : RedisCacheTest {
        
        [Test]
        public async Task Get_ClaveInexistente_DeberiaRetornarDefault() {
            (await _cache.Get(999)).Should().BeNull();
        }
    }

    [TestFixture] 
    public class CasosRemove : RedisCacheTest {
        
        [Test]
        public async Task Remove_ClaveInexistente_DeberiaDevolverFalse() {
            (await _cache.Remove(999)).Should().BeFalse();
        }

        [Test]
        public async Task Remove_ClaveExistente_DeberiaDevolverTrueYEliminarla() {
            
            await _cache.Add(1, "uno");

            (await _cache.Remove(1)).Should().BeTrue();
            (await _cache.Get(1)).Should().BeNull();
        }
    }

    [TestFixture] 
    public class CasosClear : RedisCacheTest {
        
        [Test]
        public async Task Clear_DeberiaEliminarSoloLasClavesDeEstaCacheNoLasDeOtroPrefijo() {
            
            var otraCache = new RedisCache<int, string>(_redis, $"otra:{Guid.NewGuid()}");

            await _cache.Add(1, "uno");
            await _cache.Add(2, "dos");
            await otraCache.Add(1, "no debe borrarse");

            await _cache.Clear();

            (await _cache.Get(1)).Should().BeNull();
            (await _cache.Get(2)).Should().BeNull();
            (await otraCache.Get(1)).Should().Be("no debe borrarse");
        }
    }

    [Test]
    public async Task DisplayStatus_DeberiaEjecutarseSinLanzarExcepciones() {
        
        await _cache.Add(1, "Uno");

        var act = async () => await _cache.DisplayStatus();

        await act.Should().NotThrowAsync();
    }
}