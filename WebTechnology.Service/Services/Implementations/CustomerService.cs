using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerService(ICustomerRepository customerRepository,
            IMapper mapper,
            IUserRepository userRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<CustomerDTO>> GetCustomerInfo(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return ServiceResponse<CustomerDTO>.FailResponse("Token không hợp lệ");
                }
                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<CustomerDTO>.FailResponse("Token đã hết hạn");
                }

                var userId = _tokenService.GetUserIdFromToken(token);

                if (userId == null)
                {
                    return ServiceResponse<CustomerDTO>.FailResponse("Không tìm thấy thông tin người dùng");
                }
                var customer = await _customerRepository.GetByIdAsync(userId);
                if (customer == null)
                {
                    return ServiceResponse<CustomerDTO>.NotFoundResponse("Không tìm thấy thông tin người dùng");
                }
                var customerDto = _mapper.Map<CustomerDTO>(customer);
                var email = await _userRepository.GetByIdAsync(userId);
                customerDto.Email = email.Email ?? "Khong co email";
                return ServiceResponse<CustomerDTO>.SuccessResponse(customerDto, "Lấy thông tin người dùng thành công");

            }
            catch (Exception ex)
            {
                return ServiceResponse<CustomerDTO>.ErrorResponse($"Có lỗi phía server nhé FE {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> UpdateCustomerInfo(string token, JsonPatchDocument<Customer> patchDoc)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return ServiceResponse<string>.FailResponse("Token không hợp lệ");
                }
                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<string>.FailResponse("Token đã hết hạn");
                }
                var userId = _tokenService.GetUserIdFromToken(token);
                if (userId == null)
                {
                    return ServiceResponse<string>.FailResponse("Không tìm thấy thông tin người dùng");
                }
                var customer = await _customerRepository.GetByIdAsync(userId);
                if (customer == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy thông tin người dùng");
                }
                patchDoc.ApplyTo(customer);
                await _customerRepository.UpdateAsync(customer);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Cập nhật thông tin người dùng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi phía server nhé FE {ex.Message}");
            }
        }
    }
}
