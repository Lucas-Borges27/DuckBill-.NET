using DuckBill.Application.DTOs;
using DuckBill.Application.Services;

namespace DuckBill.Api.Endpoints;

public static class CategoriasEndpoints
{
    public static IEndpointRouteBuilder MapCategorias(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api/categorias");

        grp.MapPost("", async (CategoriaCreateDto dto, CategoriaService service) =>
        {
            try
            {
                var result = await service.CreateAsync(dto);
                return Results.Created($"/api/categorias/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(ex.Message);
            }
        });

        grp.MapGet("search", async (CategoriaService service, string? filter, string? sort, int page = 1, int size = 10) =>
        {
            if (page < 1) return Results.BadRequest("Page must be greater than 0.");
            if (size < 1 || size > 100) return Results.BadRequest("Size must be between 1 and 100.");
            var result = await service.SearchAsync(filter, sort, page, size);
            return Results.Ok(result);
        });

        grp.MapGet("", async (CategoriaService service) =>
            await service.GetAllAsync());

        grp.MapGet("{id:long}", async (long id, CategoriaService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        grp.MapPut("{id:long}", async (long id, CategoriaCreateDto dto, CategoriaService service) =>
        {
            try
            {
                await service.UpdateAsync(id, dto);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(ex.Message);
            }
        });

        grp.MapDelete("{id:long}", async (long id, CategoriaService service) =>
        {
            try
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        });

        return app;
    }
}
