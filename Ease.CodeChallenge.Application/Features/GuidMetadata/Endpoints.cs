using Carter;
using Ease.CodeChallenge.Application.Features.GuidMetadata.Commands;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CodeSkills.Firmas.Application.Features.Eventos
{
    public class Endpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/guid", async (IMediator mediator) =>
            {
                return await mediator.Send(new GetAllUserMetadataQuery());
            }).WithTags("Guid");


            app.MapGet("/guid/{guid}", async (IMediator mediator, string guid) =>
            {
                return await mediator.Send(new GetUserMetadataQuery { Guid = guid });
            }).WithTags("Guid");


            app.MapPost("/guid/{guid?}", async (IMediator mediator, string guid, [FromBody] CreateUserMetadataCommand command) =>
            {
                if (!string.IsNullOrEmpty(guid))
                    command.Guid = guid;

                return await mediator.Send(command);
            }).WithTags("Guid");


            app.MapPut("/guid/{guid}", async (IMediator mediator, string guid, [FromBody] UpdateUserMetadataCommand command) =>
            {
                if (!string.IsNullOrEmpty(guid))
                    command.Guid = guid;

                return await mediator.Send(command);
            }).WithTags("Guid");


            app.MapDelete("/guid/{guid}", async (IMediator mediator, string guid) =>
            {
                var result = await mediator.Send(new DeleteUserMetadataCommand { Guid = guid });
                return result ? Results.NoContent() : Results.BadRequest();

            }).WithTags("Guid");
        }
    }
}
