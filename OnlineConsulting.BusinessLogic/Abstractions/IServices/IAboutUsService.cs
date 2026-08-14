using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.AboutUsDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IAboutUsService : IGenericService<AboutUs, IDto>
{
    Task<IOperationResult> AddAboutUsAsync(CreateAboutUsDto dto);
    Task<IOperationResult> UpdateAboutUsImageAsync(string id, IFormFile image);
}
