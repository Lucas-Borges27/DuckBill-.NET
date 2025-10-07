using DuckBill.Application.DTOs;
using DuckBill.Application.Services;

namespace DuckBill.Api.Endpoints;

public static class TransacoesAtivoEndpoints
{
    public static IEndpointRouteBuilder MapTransacoesAtivo(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api/transacoes-ativo");

        grp.MapPost("", async (TransacaoCreateDto dto, TransacaoAtivoService service) =>
        {
            try
            {
                var result = await service.CreateAsync(dto);
                return Results.Created($"/api/transacoes-ativo/{result.Id}", result);
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

        grp.MapGet("", async (long usuarioId, TransacaoAtivoService service) =>
            await service.GetByUsuarioIdAsync(usuarioId));

        grp.MapGet("{id:long}", async (long id, TransacaoAtivoService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        grp.MapPut("{id:long}", async (long id, TransacaoCreateDto dto, TransacaoAtivoService service) =>
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

        grp.MapDelete("{id:long}", async (long id, TransacaoAtivoService service) =>
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
