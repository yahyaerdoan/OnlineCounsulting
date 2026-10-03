using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.DeleteTestimonial;
using OnlineConsulting.Modules.SiteContent.Application.Features.Testimonials.UpdateTestimonial;

namespace OnlineConsulting.Api.Features.SiteContent.Testimonials;

public sealed class TestimonialLinks() : ManagedContentLinks<TestimonialResponse, UpdateTestimonialCommand, DeleteTestimonialCommand>("UpdateTestimonial", "DeleteTestimonial", resource => resource.Id);
