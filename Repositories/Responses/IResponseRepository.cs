using Data.Models;

namespace Repositories.Responses
{
    public interface IResponseRepository
    {
        Task<IEnumerable<Response>> GetAllAsync();
        Task<Response?> GetByIdAsync(int id);
        Task<IEnumerable<Response>> GetByFeedbackIdAsync(int feedbackId);
        Task AddAsync(Response response);
        Task UpdateAsync(Response response);
        Task DeleteAsync(int id);
    }
}