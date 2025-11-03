using DuckBill.Application.DTOs;
using DuckBill.Application.Services;

namespace DuckBill.Api.Endpoints;

public static class DespesasEndpoints
{
    public static IEndpointRouteBuilder MapDespesas(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api/despesas");

        grp.MapPost("", async (DespesaCreateDto dto, DespesaService service) =>
        {
            try
            {
                var result = await service.CreateAsync(dto);
                return Results.Created($"/api/despesas/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        grp.MapGet("{id:long}", async (long id, DespesaService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        grp.MapGet("search", async (DespesaService service, long? usuarioId, string? filter, string? sort, int page = 1, int size = 10) =>
        {
            if (page < 1) return Results.BadRequest("Page must be greater than 0.");
            if (size < 1 || size > 100) return Results.BadRequest("Size must be between 1 and 100.");
            var result = await service.SearchAsync(usuarioId, filter, sort, page, size);
            return Results.Ok(result);
        });

        grp.MapGet("", async (long usuarioId, int? mes, int? ano, DespesaService service) =>
        {
            IEnumerable<DespesaDto> list;
            if (mes.HasValue && ano.HasValue)
            {
                list = await service.GetByUsuarioIdAndMesAnoAsync(usuarioId, mes.Value, ano.Value);
            }
            else
            {
                list = await service.GetByUsuarioIdAsync(usuarioId);
            }
            return Results.Ok(list);
        });

        grp.MapPut("{id:long}", async (long id, DespesaCreateDto dto, DespesaService service) =>
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
        });

        grp.MapDelete("{id:long}", async (long id, DespesaService service) =>
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
