using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Responses
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly AppDbContext _context;

        public ResponseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Response>> GetAllAsync()
        {
            return await _context.Responses.ToListAsync();
        }

        public async Task<Response?> GetByIdAsync(int id)
        {
            return await _context.Responses.FindAsync(id);
        }

        public async Task AddAsync(Response response)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }
            await _context.Responses.AddAsync(response);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Response response)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }
            _context.Entry(response).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _context.Responses.FindAsync(id);
            if (response != null)
            {
                _context.Responses.Remove(response);
                await _context.SaveChangesAsync();
            }
        }
    }
}