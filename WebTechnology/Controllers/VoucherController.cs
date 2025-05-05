using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Vouchers;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucher(string id)
        {
            var response = await _voucherService.GetVoucherAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDTO createDto)
        {
            var response = await _voucherService.CreateVoucherAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }
        
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateVoucher(string id, [FromBody] JsonPatchDocument<Voucher> patchDoc)
        {
            var response = await _voucherService.UpdateVoucherAsync(id, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(string id)
        {
            var response = await _voucherService.DeleteVoucherAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}