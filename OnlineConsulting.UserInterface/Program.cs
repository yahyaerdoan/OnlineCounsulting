using Core.SecurityLayer.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using OnlineConsulting.ServiceDefaults;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.UserInterface.Areas.Admin.Features.AboutUs;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Appointment;
using OnlineConsulting.UserInterface.Areas.Admin.Features.AvailabilityRule;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Breadcrumb;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Bundle;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Category;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Equipment;
using OnlineConsulting.UserInterface.Areas.Admin.Features.FeatureFlag;
using OnlineConsulting.UserInterface.Areas.Admin.Features.FooterAbout;
using OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryCategory;
using OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryItem;
using OnlineConsulting.UserInterface.Areas.Admin.Features.HowIGetService;
using OnlineConsulting.UserInterface.Areas.Admin.Features.MembershipPlan;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Message;
using OnlineConsulting.UserInterface.Areas.Admin.Features.ModuleOffering;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Newsletter;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Order;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Partnership;
using OnlineConsulting.UserInterface.Areas.Admin.Features.PartnershipSocialLink;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Promotion;
using OnlineConsulting.UserInterface.Areas.Admin.Features.ProvidedItem;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Referral;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Service;
using OnlineConsulting.UserInterface.Areas.Admin.Features.ServiceArea;
using OnlineConsulting.UserInterface.Areas.Admin.Features.SliderItem;
using OnlineConsulting.UserInterface.Areas.Admin.Features.SocialMedia;
using OnlineConsulting.UserInterface.Areas.Admin.Features.SystemRole;
using OnlineConsulting.UserInterface.Areas.Admin.Features.SystemUser;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Tenant;
using OnlineConsulting.UserInterface.Areas.Admin.Features.Testimonial;
using OnlineConsulting.UserInterface.Areas.Admin.Features.WhatWeProvide;
using OnlineConsulting.UserInterface.Areas.User.Features.Appointment;
using OnlineConsulting.UserInterface.Areas.User.Features.Equipment;
using OnlineConsulting.UserInterface.Areas.User.Features.Membership;
using OnlineConsulting.UserInterface.Areas.User.Features.Order;
using OnlineConsulting.UserInterface.Areas.User.Features.Referral;
using OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;
using OnlineConsulting.UserInterface.Configurations.Extensions;
using OnlineConsulting.UserInterface.Features.Account;
using OnlineConsulting.UserInterface.Features.Appointment;
using OnlineConsulting.UserInterface.Features.Cart;
using OnlineConsulting.UserInterface.Features.Category;
using OnlineConsulting.UserInterface.Features.Checkout;
using OnlineConsulting.UserInterface.Features.Gallery;
using OnlineConsulting.UserInterface.Features.Home;
using OnlineConsulting.UserInterface.Features.Membership;
using OnlineConsulting.UserInterface.Features.Search;
using OnlineConsulting.UserInterface.Features.Service;
using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;
using AdminContact = OnlineConsulting.UserInterface.Areas.Admin.Features.Contact;
using PublicContact = OnlineConsulting.UserInterface.Features.Contact;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddHttpContextAccessor();


builder.Services.AddTransient<BearerTokenHandler>();
builder.Services.AddTransient<GuestIdHandler>();
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"] ?? "https+http://api"))
    .AddHttpMessageHandler<BearerTokenHandler>()
    .AddHttpMessageHandler<GuestIdHandler>();


builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection(StripeOptions.SectionName));
builder.Services.Configure<RecaptchaOptions>(builder.Configuration.GetSection(RecaptchaOptions.SectionName));
builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IPartnershipService, PartnershipService>();
builder.Services.AddScoped<IPartnershipSocialLinkService, PartnershipSocialLinkService>();
builder.Services.AddScoped<ITestimonialService, TestimonialService>();
builder.Services.AddScoped<ISocialMediaService, SocialMediaService>();
builder.Services.AddScoped<ISliderItemService, SliderItemService>();
builder.Services.AddScoped<IBreadcrumbService, BreadcrumbService>();
builder.Services.AddScoped<IFooterAboutService, FooterAboutService>();
builder.Services.AddScoped<IWhatWeProvideService, WhatWeProvideService>();
builder.Services.AddScoped<IHowIGetServiceService, HowIGetServiceService>();
builder.Services.AddScoped<IProvidedItemService, ProvidedItemService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IServiceAreaService, ServiceAreaService>();
builder.Services.AddScoped<IFeatureFlagService, FeatureFlagService>();
builder.Services.AddScoped<IAvailabilityRuleService, AvailabilityRuleService>();
builder.Services.AddScoped<IAppointmentDispatchService, AppointmentDispatchService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IMembershipPlanService, MembershipPlanService>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IModuleOfferingService, ModuleOfferingService>();
builder.Services.AddScoped<IBundleService, BundleService>();
builder.Services.AddScoped<AdminContact.IContactService, AdminContact.ContactService>();
builder.Services.AddScoped<PublicContact.IContactService, PublicContact.ContactService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ISystemRoleService, SystemRoleService>();
builder.Services.AddScoped<ISystemUserService, SystemUserService>();
builder.Services.AddScoped<IAboutUsService, AboutUsService>();
builder.Services.AddScoped<IAdminCategoryService, AdminCategoryService>();
builder.Services.AddScoped<IAdminServiceCatalogService, AdminServiceCatalogService>();
builder.Services.AddScoped<IAdminGalleryCategoryService, AdminGalleryCategoryService>();
builder.Services.AddScoped<IAdminGalleryItemService, AdminGalleryItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IServiceCatalogService, ServiceCatalogService>();
builder.Services.AddScoped<IAppointmentBookingService, AppointmentBookingService>();
builder.Services.AddScoped<IMembershipPlanCatalogService, MembershipPlanCatalogService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IUserAddressService, UserAddressService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IHomeContentService, HomeContentService>();
builder.Services.AddScoped<IGalleryContentService, GalleryContentService>();
builder.Services.AddScoped<IServiceCatalogPageService, ServiceCatalogPageService>();
builder.Services.AddScoped<ICartPageService, CartPageService>();
builder.Services.AddScoped<IUserAddressPageService, UserAddressPageService>();
builder.Services.AddScoped<IUserOrderPageService, UserOrderPageService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IUserAppointmentPageService, UserAppointmentPageService>();
builder.Services.AddScoped<IUserEquipmentService, UserEquipmentService>();
builder.Services.AddScoped<IUserEquipmentPageService, UserEquipmentPageService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IUserReferralService, UserReferralService>();
builder.Services.AddScoped<IUserReferralPageService, UserReferralPageService>();

builder.Services.AddUserInterfaceServiceRegistration(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminAreaAccessPolicy", policy => policy.RequireAssertion(context =>
        context.User.HasClaim(c => c.Type == PermissionClaimTypes.Type)))
    .AddPolicy("RequirePlatformOwnerAccessPolicy", policy => policy.RequireRole(GlobalOperationClaims.SuperAdmin));

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Home/Error");
    _ = app.UseHsts();
}

//Nice error pages for error status codes (401/403/404/405/500 etc.)
app.UseStatusCodePagesWithReExecute("/errorpage/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();
    if (!string.IsNullOrEmpty(path) &&
    context.User.Identity?.IsAuthenticated == true &&
    context.Request.Method == HttpMethods.Get &&
    !path.Contains("/account/login") &&
    !path.Contains("/account/logout") &&
    !path.Contains("/account/register"))
    {
        context.Session.SetString("last-visited-url", context.Request.Path + context.Request.QueryString);
    }

    await next();
});

app.UseNToastNotify();

//Legacy static entry point some browsers/tools still request
app.MapGet("/index.html", () => Results.Redirect("/"));

app.MapControllerRoute(
name: "areas",
pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();
