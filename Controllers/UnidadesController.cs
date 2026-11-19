using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaciSmart.Api.Data;
using VaciSmart.Api.Models;

namespace VaciSmart.Api.Controllers;

[ApiController]
[Route("api/unidades")]
public class UnidadesController : ControllerBase
{
    private readonly AppDbContext _context;

    public UnidadesController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/unidades -> 200
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnidadeSaude>>> Listar()
    {
        var unidades = await _context.Unidades
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .ToListAsync();

        return Ok(unidades);
    }

    // GET api/unidades/5 -> 200 ou 404
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnidadeSaude>> BuscarPorId(int id)
    {
        var unidade = await _context.Unidades.FindAsync(id);
        if (unidade is null)
        {
            return NaoEncontrada(id);
        }

        return Ok(unidade);
    }

    // GET api/unidades/5/transportes -> 200 ou 404 (mostra o relacionamento 1:N)
    [HttpGet("{id:int}/transportes")]
    public async Task<ActionResult<IEnumerable<Transporte>>> ListarTransportes(int id)
    {
        var unidadeExiste = await _context.Unidades.AnyAsync(u => u.Id == id);
        if (!unidadeExiste)
        {
            return NaoEncontrada(id);
        }

        var transportes = await _context.Transportes
            .AsNoTracking()
            .Where(t => t.UnidadeDestinoId == id)
            .OrderBy(t => t.Id)
            .ToListAsync();

        return Ok(transportes);
    }

    // POST api/unidades -> 201 (ou 400 automático do [ApiController] se a validação falhar)
    [HttpPost]
    public async Task<ActionResult<UnidadeSaude>> Criar(UnidadeSaude unidade)
    {
        unidade.Id = 0; // o id é sempre gerado pelo banco

        _context.Unidades.Add(unidade);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = unidade.Id }, unidade);
    }

    // PUT api/unidades/5 -> 204, 400 ou 404
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, UnidadeSaude dados)
    {
        if (dados.Id != 0 && dados.Id != id)
        {
            ModelState.AddModelError(nameof(UnidadeSaude.Id), "O id do corpo é diferente do id da rota.");
            return ValidationProblem(ModelState);
        }

        var unidade = await _context.Unidades.FindAsync(id);
        if (unidade is null)
        {
            return NaoEncontrada(id);
        }

        unidade.Nome = dados.Nome;
        unidade.Municipio = dados.Municipio;
        unidade.Uf = dados.Uf;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/unidades/5 -> 204, 404 ou 409 (unidade ainda tem transportes)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var unidade = await _context.Unidades.FindAsync(id);
        if (unidade is null)
        {
            return NaoEncontrada(id);
        }

        var temTransportes = await _context.Transportes.AnyAsync(t => t.UnidadeDestinoId == id);
        if (temTransportes)
        {
            return Problem(
                title: "Unidade com transportes vinculados",
                detail: $"A unidade {id} possui transportes vinculados. Exclua ou mova os transportes antes.",
                statusCode: StatusCodes.Status409Conflict);
        }

        _context.Unidades.Remove(unidade);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private ObjectResult NaoEncontrada(int id) =>
        Problem(
            title: "Unidade de saúde não encontrada",
            detail: $"Não existe unidade de saúde com id {id}.",
            statusCode: StatusCodes.Status404NotFound);
}
