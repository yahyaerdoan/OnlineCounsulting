using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;

namespace OnlineConsulting.BusinessLogic.Concretions.ServiceManagers;

public class ServiceManager(
    IAboutUsService aboutUsService,
    IBreadcrumbService breadcrumbService,
    IBookService bookService,
    ICategoryService categoryService,
    IContactService contactService,
    IFooterAboutService footerAboutService,
    IGalleryItemService galleryItemService,
    IGalleryCategoryService galleryCategoryService,
    IHowIGetServiceService howIGetServiceService,
    IMessageService messageService,
    INewsletterService newsletterService,
    IPartnershipService partnershipService,
    IPartnershipSocialMediaService partnershipSocialMediaService,
    IProvidedItemService providedItemService,
    IServiceImageService serviceImageService,
    IServiceService serviceService,
    ISliderItemService sliderItemService,
    ISocialMediaService socialMediaService,
    ITestimonialService testimonialService,
    IWhatWeProvideService whatWeProvideService,
    IImgIconService imgIconService,
    IClassIconService classIconService,
    ISystemUserService systemUserService,
    ISystemRoleService systemRoleService,
    IBasketItemService basketItemService,
    IBasketService basketService,
    IUserAddressService userAddressService,
    IOrderService orderService,
    IOrderItemService orderItemService,
    IAiContentService aiContentService,
    IStripeService stripeService,
    IFlightService flightService) : IServiceManager
{
    public IAboutUsService AboutUsService => aboutUsService;
    public IBreadcrumbService BreadcrumbService => breadcrumbService;
    public IBookService BookService => bookService;
    public ICategoryService CategoryService => categoryService;
    public IContactService ContactService => contactService;
    public IFooterAboutService FooterAboutService => footerAboutService;
    public IGalleryItemService GalleryItemService => galleryItemService;
    public IHowIGetServiceService HowIGetServiceService => howIGetServiceService;
    public IMessageService MessageService => messageService;
    public INewsletterService NewsletterService => newsletterService;
    public IPartnershipService PartnershipService => partnershipService;
    public IPartnershipSocialMediaService PartnershipSocialMediaService => partnershipSocialMediaService;
    public IProvidedItemService ProvidedItemService => providedItemService;
    public IServiceImageService ServiceImageService => serviceImageService;
    public IServiceService ServiceService => serviceService;
    public ISliderItemService SliderItemService => sliderItemService;
    public ISocialMediaService SocialMediaService => socialMediaService;
    public ITestimonialService TestimonialService => testimonialService;
    public IWhatWeProvideService WhatWeProvideService => whatWeProvideService;
    public IGalleryCategoryService GalleryCategoryService => galleryCategoryService;
    public IImgIconService ImgIconService => imgIconService;
    public IClassIconService ClassIconService => classIconService;
    public ISystemUserService SystemUserService => systemUserService;
    public ISystemRoleService SystemRoleService => systemRoleService;
    public IBasketItemService BasketItemService => basketItemService;
    public IBasketService BasketService => basketService;
    public IUserAddressService UserAddressService => userAddressService;
    public IOrderService OrderService => orderService;
    public IOrderItemService OrderItemService => orderItemService;
    public IAiContentService AiContentService => aiContentService;
    public IStripeService StripeService => stripeService;
    public IFlightService FlightService => flightService;
}
