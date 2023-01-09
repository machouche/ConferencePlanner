using Microsoft.EntityFrameworkCore;
using BackEnd.Models;
using Microsoft.AspNetCore.Http.HttpResults;
namespace BackEnd;

public static class SpeakerEndpoints
{
    public static void MapSpeakerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Speaker").WithTags(nameof(Speaker));

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            return await db.Speakers.ToListAsync();
        })
        .WithTags("Speaker").WithName("GetAllSpeakers")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Speaker>, NotFound>> (int id, ApplicationDbContext db) =>
        {
            return await db.Speakers.FindAsync(id)
                is Speaker model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithTags("Speaker").WithName("GetSpeakerById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<NotFound, NoContent>> (int id, Speaker speaker, ApplicationDbContext db) =>
        {
            var foundModel = await db.Speakers.FindAsync(id);

            if (foundModel is null)
            {
                return TypedResults.NotFound();
            }

            db.Update(speaker);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .WithTags("Speaker").WithName("UpdateSpeaker")
        .WithOpenApi();

        group.MapPost("/", async (Speaker speaker, ApplicationDbContext db) =>
        {
            db.Speakers.Add(speaker);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Speaker/{speaker.Id}", speaker);
        })
        .WithTags("Speaker").WithName("CreateSpeaker")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok<Speaker>, NotFound>> (int id, ApplicationDbContext db) =>
        {
            if (await db.Speakers.FindAsync(id) is Speaker speaker)
            {
                db.Speakers.Remove(speaker);
                await db.SaveChangesAsync();
                return TypedResults.Ok(speaker);
            }

            return TypedResults.NotFound();
        })
        .WithTags("Speaker").WithName("DeleteSpeaker")
        .WithOpenApi();
    }
}
