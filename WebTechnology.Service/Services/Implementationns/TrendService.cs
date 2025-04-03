using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Trends;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class TrendService : ITrendService
    {
        private readonly ITrendRepository _trendRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TrendService> _logger;
        public TrendService(ITrendRepository trendRepository, IMapper mapper, 
            IUnitOfWork unitOfWork, ILogger<TrendService> logger)
        {
            _trendRepository = trendRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ServiceResponse<Trend>> CreateTrendAsync(CreateTrendDTO createDto)
        {
            try
            {
                var trend = _mapper.Map<Trend>(createDto);
                trend.Trend1 = Guid.NewGuid().ToString();
                trend.CreatedAt = DateTime.UtcNow;
                trend.IsActive = true;
                await _trendRepository.AddAsync(trend);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<Trend>.SuccessResponse("Tạo xu hướng thành công nhé FE");
            }
            catch (Exception ex)
            {
                return ServiceResponse<Trend>.ErrorResponse($"Lỗi khi tạo xu hướng nhé FE: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<Trend>>> GetTrendsAsync()
        {
            try
            {
                var trends = await _trendRepository.GetAllAsync();
                return ServiceResponse<IEnumerable<Trend>>.SuccessResponse(trends, "Lấy danh sách xu hướng thành công nhé các FE");
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<Trend>>.ErrorResponse($"Lỗi khi lấy dữ liệu nhé các FE: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<Trend>> PatchTrendAsync(string id, JsonPatchDocument<Trend> patchDoc)
        {
            try
            {
                var exist = await _trendRepository.GetByIdAsync(id);
                if (exist == null)
                {
                    return ServiceResponse<Trend>.NotFoundResponse("Trend không tồn tại nhé FE");
                }
                patchDoc.ApplyTo(exist);
                exist.UpdatedAt = DateTime.UtcNow;
                await _trendRepository.UpdateAsync(exist);
                return ServiceResponse<Trend>.SuccessResponse("Cập nhật xu hướng thành công nhé FE");
            }
            catch (Exception ex)
            {
                return ServiceResponse<Trend>.ErrorResponse($"Lỗi khi tạo xu hướng nhé FE: {ex.Message}");
            }
        }
    }
}
