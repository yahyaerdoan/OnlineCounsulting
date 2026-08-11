using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Api.Features.Auth;
using OnlineConsulting.Api.Features.Categories;
using OnlineConsulting.Api.Features.Flights;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.Extensions;
using OnlineConsulting.DataAccess.Concretions.Configurations.Extensions;
using OnlineConsulting.DataTransferObject.Concretions.Configurations.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccesssServiceRegistration(builder.Configuration);
builder.Services.AddBusinessLogicServiceRegistration(builder.Configuration);
builder.Services.AddDataTransferObjectServiceRegistration(builder.Configuration);
builder.Services.AddApiServiceRegistration(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoint<GetCategories>()
   .MapEndpoint<GetCategoriesEnveloped>()
   .MapEndpoint<GetCategoriesManualSuccess>()
   .MapEndpoint<GetCategoriesRawProblemDetails>()
   .MapEndpoint<GetCategoriesMvcStyle>()
   .MapEndpoint<Login>()
   .MapEndpoint<CreateFlight>()
   .MapEndpoint<GetFlightsByAirportAndDate>()
   .MapEndpoint<UpdateFlight>();

app.Run();
