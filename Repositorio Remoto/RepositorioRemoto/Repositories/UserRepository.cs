using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Mappers;
using RepositorioRemoto.Models;

namespace RepositorioRemoto.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<List<User>> GetAllAsync() =>
        (await context.Users.AsNoTracking().ToListAsync()).Select(e => e.ToModel()).ToList();

    public async Task<User?> GetByIdAsync(int id) =>
        (await context.Users.FindAsync(id))?.ToModel();

    public async Task<User> AddAsync(User user)
    {
        var entity = user.ToEntity();
        context.Users.Add(entity);
        await context.SaveChangesAsync();
        return entity.ToModel();
    }

    public async Task<User?> UpdateAsync(int id, User user)
    {
        var entity = await context.Users.FindAsync(id);
        if (entity is null) return null;

        context.Entry(entity).CurrentValues.SetValues(user.ToEntity());
        await context.SaveChangesAsync();
        return entity.ToModel();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await context.Users.FindAsync(id);
        if (entity is null) return false;

        context.Users.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task ReplaceAllAsync(IEnumerable<User> users)
    {
        await context.Users.ExecuteDeleteAsync();
        context.ChangeTracker.Clear();
        context.Users.AddRange(users.Select(u => u.ToEntity()));
        await context.SaveChangesAsync();
    }
}