using Repositories.Statuses;
using Data.Models;
using Services.DTO.Status;

namespace Services.Statuses
{
    public class StatusService : IStatusService
    {
        
        private readonly IStatusRepository _statusRepository;

        public StatusService(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }

        public async Task<IEnumerable<DisplayStatusDto>> GetAllStatusesAsync()
        {
            var statuses = await _statusRepository.GetAllAsync();
            return statuses.Select(s => new DisplayStatusDto
            {
                Id = s.Id,
                Name = s.Name
            });
        }

        public async Task<DisplayStatusDto?> GetStatusByIdAsync(int id)
        {
            var status = await _statusRepository.GetByIdAsync(id);
            return status == null ? null : new DisplayStatusDto
            {
                Id = status.Id,
                Name = status.Name
            };
        }

        public async Task<DisplayStatusDto> CreateStatusAsync(string name)
        {
            var status = new Status
            {
                Name = name ?? "Новый" 
            };
            
            await _statusRepository.AddAsync(status);
            
            return new DisplayStatusDto
            {
                Id = status.Id,
                Name = status.Name
            };
        }

        public async Task UpdateStatusAsync(int id, string name)
        {
            var status = await _statusRepository.GetByIdAsync(id);
            if (status == null)
                throw new ArgumentException($"Статус с id = {id} не найден.");

            status.Name = string.IsNullOrWhiteSpace(name) ? "Новый" : name;
            await _statusRepository.UpdateAsync(status);
        }

        public async Task DeleteStatusAsync(int id)
        {
            await _statusRepository.DeleteAsync(id);
        }
    }
}