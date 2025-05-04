﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        /// <summary>
        /// Get all units
        /// </summary>
        /// <returns>List of all units</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUnits()
        {
            var result = await _unitService.GetAllUnitsAsync();
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
