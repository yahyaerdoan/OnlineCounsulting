using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.WhatWeProvideDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IWhatWeProvideService : IGenericService<WhatWeProvide, IDto>
{
    Task<IOperationResult> AddWhatWeProvideAsync(CreateWhatWeProvideDto dto);
    Task<IOperationResult> UpdateWhatWeProvideImageAsync(string id, IFormFile image);
}
