using Application.DTO;
using Application.DTO.Status;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class StatusService : IStatusService
    {
        
        private readonly IStatusRepository _statusRepository;

        public StatusService(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }

        public async Task<PagedResponse<DisplayStatusDto>> GetAllStatusesAsync(FilterStatusDto filters)
        {
            var statuses = await _statusRepository.GetAllAsync();
            var filtered = statuses.AsQueryable();

            if (!string.IsNullOrEmpty(filters.SearchTerm))
                filtered = filtered.Where(s => s.Name.Contains(filters.SearchTerm));
            else
                if (!string.IsNullOrEmpty(filters.Name))
                    filtered = filtered.Where(s => s.Name.Contains(filters.Name));

            var totalRecords = filtered.Count();

            var paged = filtered
                .OrderBy(s => s.Id)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            var dtos = paged.Select(s => new DisplayStatusDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

            return new PagedResponse<DisplayStatusDto>(
                dtos,
                filters.PageNumber,
                filters.PageSize,
                totalRecords
            );
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

        public async Task<DisplayStatusDto> CreateStatusAsync(CreateStatusDto createStatusDto)
        {
            var status = new Status
            {
                Id = createStatusDto.Id,
                Name = createStatusDto.Name ?? "Новый" 
            };
            
            await _statusRepository.AddAsync(status);
            
            return new DisplayStatusDto
            {
                Id = status.Id,
                Name = status.Name
            };
        }

        public async Task UpdateStatusAsync(int id, UpdateStatusDto updateStatusDto) 
        {
            var status = await _statusRepository.GetByIdAsync(id);
            if (status == null)
                throw new ArgumentException($"Статус с id = {id} не найден.");

            status.Name = string.IsNullOrWhiteSpace(updateStatusDto.Name) ? "Новый" : updateStatusDto.Name;
            await _statusRepository.UpdateAsync(status);
        }

        public async Task DeleteStatusAsync(int id)
        {
            await _statusRepository.DeleteAsync(id);
        }
    }
}