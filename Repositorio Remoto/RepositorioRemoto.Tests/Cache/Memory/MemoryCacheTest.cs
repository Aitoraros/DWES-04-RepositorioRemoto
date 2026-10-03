using Microsoft.Extensions.Caching.Memory;
using FluentAssertions;
using RepositorioRemoto.Cache.Memory;

namespace RepositorioRemoto.Tests.Cache.Memory;

[TestFixture]
[TestOf(typeof(MemoryCache<,>))]
public class MemoryCacheTest
{
    private MemoryCache<int, string> _cache = null!;

    [SetUp]
    public void SetUp()
    {
        _cache = new MemoryCache<int, string>(new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions()));
    }

    [TestFixture] public class CasosAdd : MemoryCacheTest
    {
        [Test]
        public async Task Add_ConElementoValido_DeberiaGuardarElemento()
        {
            await _cache.Add(1, "uno");
            (await _cache.Get(1)).Should().Be("uno");
        }

        [Test]
        public async Task Add_ClaveExistente_DeberiaActualizarValor()
        {
            await _cache.Add(1, "uno");
            await _cache.Add(1, "uno editado");

            (await _cache.Get(1)).Should().Be("uno editado");
        }

        [Test]
        public async Task Add_ConExpiracion_DeberiaEliminarseTrasExpirar()
        {
            var cache = new MemoryCache<int, string>(
                new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMilliseconds(200));

            await cache.Add(1, "temporal");
            (await cache.Get(1)).Should().Be("temporal");

            await Task.Delay(400);

            (await cache.Get(1)).Should().BeNull();
        }
    }

    [TestFixture] public class CasosGet : MemoryCacheTest
    {
        [Test]
        public async Task Get_ClaveInexistente_DeberiaRetornarDefault()
        {
            (await _cache.Get(999)).Should().BeNull();
        }
    }

    [TestFixture] public class CasosRemove : MemoryCacheTest
    {
        [Test]
        public async Task Remove_ClaveInexistente_DeberiaDevolverFalse()
        {
            (await _cache.Remove(999)).Should().BeFalse();
        }

        [Test]
        public async Task Remove_ClaveExistente_DeberiaDevolverTrue()
        {
            await _cache.Add(1, "uno");
            (await _cache.Remove(1)).Should().BeTrue();
            (await _cache.Get(1)).Should().BeNull();
        }
    }

    [TestFixture] public class CasosClear : MemoryCacheTest
    {
        [Test]
        public async Task Clear_DeberiaEliminarTodo()
        {
            await _cache.Add(1, "uno");
            await _cache.Add(2, "dos");

            await _cache.Clear();

            (await _cache.Get(1)).Should().BeNull();
            (await _cache.Get(2)).Should().BeNull();
        }
    }

    [Test]
    public async Task DisplayStatus_DeberiaEjecutarseSinLanzarExcepciones()
    {
        await _cache.Add(1, "Uno");
        var act = async () => await _cache.DisplayStatus();
        await act.Should().NotThrowAsync();
    }
}