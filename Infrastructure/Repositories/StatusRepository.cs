using Domain.Models;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StatusRepository : IStatusRepository
    {
    
        private readonly AppDbContext _context;

        public StatusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Status>> GetAllAsync()
        {
            return await _context.Statuses.ToListAsync();
        }

        public async Task<Status?> GetByIdAsync(int id)
        {
            return await _context.Statuses.FindAsync(id);
        }

        public async Task AddAsync(Status satus)
        {
            if (satus is null)
                throw new ArgumentNullException(nameof(satus));
            await _context.Statuses.AddAsync(satus);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(Status status)
        {
            if (status is null)
                throw new ArgumentNullException(nameof(status));
            _context.Entry(status).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status != null)
            {
                _context.Statuses.Remove(status);
                await _context.SaveChangesAsync();
            }
        }
    }
}