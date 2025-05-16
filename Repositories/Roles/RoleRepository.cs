using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Roles
{
    public class RoleRepository : IRoleRepository
    {
    
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task AddAsync(Role role)
        {
            if (role is null)
                throw new ArgumentNullException(nameof(role));
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(Role role)
        {
            if (role is null)
                throw new ArgumentNullException(nameof(role));
            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }
    }
}
