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
            var query = statuses.AsQueryable();

            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(s => s.Name.Contains(filters.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var search = filters.SearchTerm.ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(search));
            }

            var totalRecords = query.Count();

            query = ApplySorting(query, filters.SortColumn, filters.SortOrder);

            var paged = query
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

        private IQueryable<Status> ApplySorting(IQueryable<Status> query, string? sortColumn, string? sortOrder)
        {
            if (string.IsNullOrEmpty(sortColumn))
                sortColumn = "Id";
            
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "asc";

            sortColumn = sortColumn.ToLower();
            sortOrder = sortOrder.ToLower();

            return sortColumn switch
            {
                "id" => sortOrder == "desc" ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id),
                "name" => sortOrder == "desc" ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                _ => query.OrderBy(s => s.Id)
            };
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