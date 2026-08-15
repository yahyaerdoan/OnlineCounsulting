using AutoMapper;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.TestimonialDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class TestimonialManager(IMapper mapper, IGenericRepository<Testimonial> repository, IStorageService storageService) : GenericService<Testimonial, IDto>(mapper, repository), ITestimonialService
{
    public async Task<IOperationResult> AddTestimonialAsync(CreateTestimonialDto dto)
    {
        IOperationResult<(string FileName, string FileExtension, string FullPath, string TargetFolderPathOrContainerName)> uploadedImageRoute;
        if (dto.Image is not null)
        {
            uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/Testimonial-Images", dto.Image);

            if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
            {
                dto.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";

                var result = await AddAsync(dto);
                return result;
            }
            else
                return new ErrorResult($"An error occurred while creating the testimonial image", ResultStatus.BadRequest);
        }
        else
        {
            dto.ImageUrl = "/resource/localstorage/defaultimages/default-testimonial.png";

            var result = await AddAsync(dto);
            return result;
        }

    }
    public async Task<IOperationResult> RemoveTestimonialByIdAsync(string id, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            var result = await RemoveByIdAsync(id, true);
            return result;
        }
        else
        {
            var TestimonialResult = await GetByIdAsync<ResultTestimonialDto>(id, false);
            if (!TestimonialResult.IsSuccessful)
                return new ErrorResult("Testimonial not found", ResultStatus.NotFound);
            else
            {
                var Testimonial = TestimonialResult.Data;
                if (Testimonial.ImageUrl != "/resource/localstorage/defaultimages/default-testimonial.png")
                {
                    await storageService.DeleteAsync(Testimonial.ImageUrl);
                }
                var result = await RemoveAsync(Testimonial, false);
                return result;
            }

        }
    }
    public async Task<IOperationResult> UpdateTestimonialImageAsync(string id, IFormFile image)
    {
        var testimonialResult = await GetByIdAsync<ResultTestimonialDto>(id, false);


        if (!testimonialResult.IsSuccessful)
        {
            return new ErrorResult("The requested 'testimonial' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        var testimonial = testimonialResult.Data;

        if (image is null || image.Length == 0)
        {
            return new ErrorResult("No image was provided. Please select a valid image file to update the photo.", ResultStatus.BadRequest);
        }

        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        if (!allowedMimeTypes.Contains(image.ContentType))
        {
            return new ErrorResult("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.", ResultStatus.BadRequest);
        }

        if (testimonial.ImageUrl != "/resource/localstorage/defaultimages/default-testimonial.png")
        {
            if (!string.IsNullOrEmpty(testimonial.ImageUrl))
            {
                var rootPath = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(rootPath, "wwwroot", testimonial.ImageUrl.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    await storageService.DeleteAsync(testimonial.ImageUrl);
                }
            }
        }
        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/Testimonial-Images", image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            testimonial.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        var result = await UpdateAsync(testimonial);
        return result.IsSuccessful
            ? new SuccessResult("The cover image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the testimonial image. Please try again later.", ResultStatus.InternalServerError);
    }
}
