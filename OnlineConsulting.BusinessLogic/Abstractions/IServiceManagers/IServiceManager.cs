using OnlineConsulting.BusinessLogic.Abstractions.IServices;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;

public interface IServiceManager
{
    IAboutUsService AboutUsService { get; }
    IBreadcrumbService BreadcrumbService { get; }
    IBookService BookService { get; }
    ICategoryService CategoryService { get; }
    IContactService ContactService { get; }
    IFooterAboutService FooterAboutService { get; }
    IGalleryItemService GalleryItemService { get; }
    IGalleryCategoryService GalleryCategoryService { get; }
    IHowIGetServiceService HowIGetServiceService { get; }
    IMessageService MessageService { get; }
    INewsletterService NewsletterService { get; }
    IPartnershipService PartnershipService { get; }
    IPartnershipSocialMediaService PartnershipSocialMediaService { get; }
    IProvidedItemService ProvidedItemService { get; }
    IServiceImageService ServiceImageService { get; }
    IServiceService ServiceService { get; }
    ISliderItemService SliderItemService { get; }
    ISocialMediaService SocialMediaService { get; }
    ITestimonialService TestimonialService { get; }
    IWhatWeProvideService WhatWeProvideService { get; }
    IImgIconService ImgIconService { get; }
    IClassIconService ClassIconService { get; }
    ISystemUserService SystemUserService { get; }
    ISystemRoleService SystemRoleService { get; }
    IBasketItemService BasketItemService { get; }
    IBasketService BasketService { get; }
    IUserAddressService UserAddressService { get; }
    IOrderService OrderService { get; }
    IOrderItemService OrderItemService { get; }
    IAiContentService AiContentService { get; }
    IStripeService StripeService { get; }
    IFlightService FlightService { get; }
}
