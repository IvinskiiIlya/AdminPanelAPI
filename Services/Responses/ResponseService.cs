using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Responses;
using Data.Models;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Responses
{
    public class ResponseService : IResponseService
    {
        private readonly IResponseRepository _responseRepository;

        public ResponseService(IResponseRepository responseRepository)
        {
            _responseRepository = responseRepository;
        }

        public async Task<List<DisplayResponseDto>> GetResponsesByFeedbackAsync(int feedbackId)
        {
            var responses = await _responseRepository.GetByFeedbackIdAsync(feedbackId);
            return responses.Select(r => new DisplayResponseDto
            {
                Id = r.Id,
                ResponseMessage = r.ResponseMessage,
                CreatedAt = r.CreatedAt,
                FeedbackId = r.FeedbackId,
                AdminUserId = r.AdminUserId
            }).ToList();
        }

        public async Task<DisplayResponseDto> CreateResponseAsync(CreateResponseDto createResponseDto)
        {
            var response = new Response
            {
                ResponseMessage = createResponseDto.ResponseMessage,
                FeedbackId = createResponseDto.FeedbackId,
                AdminUserId = createResponseDto.AdminUserId,
                CreatedAt = DateTime.UtcNow
            };
            
            await _responseRepository.AddAsync(response);
            
            return new DisplayResponseDto
            {
                Id = response.Id,
                ResponseMessage = response.ResponseMessage,
                CreatedAt = response.CreatedAt,
                FeedbackId = response.FeedbackId,
                AdminUserId = response.AdminUserId
            };
        }

        public async Task UpdateResponseAsync(int id, UpdateResponseDto updateResponseDto)
        {
            var response = await _responseRepository.GetByIdAsync(id);
            if (response == null)
            {
                throw new ArgumentException($"Response with id {id} not found.");
            }

            response.ResponseMessage = updateResponseDto.ResponseMessage!;

            await _responseRepository.UpdateAsync(response);
        }

        // Остальные методы остаются без изменений
        public async Task<IEnumerable<DisplayResponseDto>> GetAllResponsesAsync()
        {
            var responses = await _responseRepository.GetAllAsync();
            return responses.Select(r => new DisplayResponseDto
            {
                Id = r.Id,
                ResponseMessage = r.ResponseMessage,
                CreatedAt = r.CreatedAt,
                FeedbackId = r.FeedbackId,
                AdminUserId = r.AdminUserId
            });
        }

        public async Task<DisplayResponseDto?> GetResponseByIdAsync(int id)
        {
            var response = await _responseRepository.GetByIdAsync(id);
            return response == null ? null : new DisplayResponseDto
            {
                Id = response.Id,
                ResponseMessage = response.ResponseMessage,
                CreatedAt = response.CreatedAt,
                FeedbackId = response.FeedbackId,
                AdminUserId = response.AdminUserId
            };
        }

        public async Task DeleteResponseAsync(int id)
        {
            await _responseRepository.DeleteAsync(id);
        }
    }
}