using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.TestimonialDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface ITestimonialService : IGenericService<Testimonial, IDto>
{
    Task<IOperationResult> AddTestimonialAsync(CreateTestimonialDto dto);
    Task<IOperationResult> UpdateTestimonialImageAsync(string id, IFormFile image);
    Task<IOperationResult> RemoveTestimonialByIdAsync(string id, bool isSoftDelete = true);
}
