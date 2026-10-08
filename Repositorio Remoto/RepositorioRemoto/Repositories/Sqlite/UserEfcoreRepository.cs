using Microsoft.EntityFrameworkCore;
using RepositorioRemoto.Entity;
using RepositorioRemoto.Repositories.Common;

namespace RepositorioRemoto.Repositories.Sqlite;

public class UserEfcoreRepository(AppDbContext context) : IUserRepository
{
    public async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        return await context.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .ToListAsync();
    }

    public async Task<UserEntity?> GetByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<UserEntity> CreateAsync(UserEntity user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<UserEntity?> UpdateAsync(UserEntity user)
    {
        var entity = await context.Users.FindAsync(user.Id);
        if (entity is null) return null;

        context.Entry(entity).CurrentValues.SetValues(user);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await context.Users.FindAsync(id);
        if (entity is null) return false;

        context.Users.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task InsertRangeAsync(IEnumerable<UserEntity> users)
    {
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        await context.Users.ExecuteDeleteAsync();
        context.ChangeTracker.Clear();
    }
}