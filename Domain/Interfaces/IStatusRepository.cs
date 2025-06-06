using Domain.Models;

namespace Domain.Interfaces
{
    public interface IStatusRepository
    {
        Task<IEnumerable<Status>> GetAllAsync();
        Task<Status?> GetByIdAsync(int id);
        Task AddAsync(Status status);
        Task UpdateAsync(Status status);
        Task DeleteAsync(int id);
    }
}

