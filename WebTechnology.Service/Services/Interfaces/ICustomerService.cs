using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ServiceResponse<CustomerDTO>> GetCustomerInfo(string token);
        Task<ServiceResponse<string>> UpdateCustomerInfo(string token, JsonPatchDocument<Customer> patchDoc);
    }
}
