using AdminPanelAPI.Data.Models;

namespace AdminPanelAPI.Repositories.Responses
{
    public interface IResponseRepository
    {
        Task<IEnumerable<Response>> GetAllAsync();
        Task<Response?> GetByIdAsync(int id);
        Task AddAsync(Response response);
        Task UpdateAsync(Response response);
        Task DeleteAsync(int id);
    }
}