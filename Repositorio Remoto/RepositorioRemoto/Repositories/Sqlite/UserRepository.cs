using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Mappers;
using RepositorioRemoto.Models;

namespace RepositorioRemoto.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var entities = await context.Users
            .OrderBy(u => u.Id)
            .ToListAsync();

        return entities.Select(e => e.ToModel());
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        var entity = await context.Users.FindAsync(id);
        return entity?.ToModel();
    }

    public async Task<User> CreateAsync(User user)
    {
        var entity = user.ToEntity();
        context.Users.Add(entity);
        await context.SaveChangesAsync();
        return entity.ToModel();
    }

    public async Task<User?> UpdateAsync(long id, User user)
    {
        var entity = await context.Users.FindAsync(id);
        if (entity is null) return null;

        context.Entry(entity).CurrentValues.SetValues(user.ToEntity());
        await context.SaveChangesAsync();
        return entity.ToModel();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await context.Users.FindAsync(id);
        if (entity is null) return false;

        context.Users.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public Task DeleteAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task ReplaceAllAsync(IEnumerable<User> users)
    {
        await context.Users.ExecuteDeleteAsync();
        context.ChangeTracker.Clear();
        context.Users.AddRange(users.Select(u => u.ToEntity()));
        await context.SaveChangesAsync();
    }
}