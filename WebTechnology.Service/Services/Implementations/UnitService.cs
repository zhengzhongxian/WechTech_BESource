﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;

        public UnitService(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }

        public async Task<ServiceResponse<IEnumerable<Unit>>> GetAllUnitsAsync()
        {
            try
            {
                var units = await _unitRepository.GetAllAsync();
                return ServiceResponse<IEnumerable<Unit>>.SuccessResponse(units, "Lấy danh sách đơn vị thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<Unit>>.ErrorResponse($"Lỗi khi lấy danh sách đơn vị: {ex.Message}");
            }
        }
    }
}
