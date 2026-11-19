using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VaciSmart.Api.Data;
using VaciSmart.Api.Models;

namespace VaciSmart.Api.Controllers;

[ApiController]
[Route("api/transportes")]
public class TransportesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransportesController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/transportes -> 200
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transporte>>> Listar()
    {
        var transportes = await _context.Transportes
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToListAsync();

        return Ok(transportes);
    }

    // GET api/transportes/5 -> 200 ou 404
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Transporte>> BuscarPorId(int id)
    {
        var transporte = await _context.Transportes.FindAsync(id);
        if (transporte is null)
        {
            return NaoEncontrado(id);
        }

        return Ok(transporte);
    }

    // POST api/transportes -> 201, 400 ou 409
    [HttpPost]
    public async Task<ActionResult<Transporte>> Criar(Transporte transporte)
    {
        var erro = await ValidarRegrasAsync(transporte, idAtual: null);
        if (erro is not null)
        {
            return erro;
        }

        // Campos controlados pelo servidor: o que vier do cliente é ignorado.
        transporte.Id = 0;
        transporte.CriadoEm = DateTimeOffset.UtcNow;
        transporte.RecebidoEm = StatusTransporte.JaFoiRecebido(transporte.Status)
            ? DateTimeOffset.UtcNow
            : null;

        _context.Transportes.Add(transporte);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarPorId), new { id = transporte.Id }, transporte);
    }

    // PUT api/transportes/5 -> 204, 400, 404 ou 409
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, Transporte dados)
    {
        if (dados.Id != 0 && dados.Id != id)
        {
            ModelState.AddModelError(nameof(Transporte.Id), "O id do corpo é diferente do id da rota.");
            return ValidationProblem(ModelState);
        }

        var transporte = await _context.Transportes.FindAsync(id);
        if (transporte is null)
        {
            return NaoEncontrado(id);
        }

        var erro = await ValidarRegrasAsync(dados, idAtual: id);
        if (erro is not null)
        {
            return erro;
        }

        transporte.Codigo = dados.Codigo;
        transporte.UnidadeDestinoId = dados.UnidadeDestinoId;
        transporte.Status = dados.Status;
        transporte.Observacao = dados.Observacao;

        // A data de recebimento acompanha o status: marca na primeira vez que a
        // remessa é recebida e limpa se o status voltar para antes do recebimento.
        if (!StatusTransporte.JaFoiRecebido(dados.Status))
        {
            transporte.RecebidoEm = null;
        }
        else if (transporte.RecebidoEm is null)
        {
            transporte.RecebidoEm = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/transportes/5 -> 204 ou 404
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var transporte = await _context.Transportes.FindAsync(id);
        if (transporte is null)
        {
            return NaoEncontrado(id);
        }

        _context.Transportes.Remove(transporte);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Regras que dependem do banco (as de formato já foram validadas pelo [ApiController]).
    // Devolve null quando está tudo certo.
    private async Task<ActionResult?> ValidarRegrasAsync(Transporte dados, int? idAtual)
    {
        var unidadeExiste = await _context.Unidades.AnyAsync(u => u.Id == dados.UnidadeDestinoId);
        if (!unidadeExiste)
        {
            ModelState.AddModelError(
                nameof(Transporte.UnidadeDestinoId),
                $"Não existe unidade de saúde com id {dados.UnidadeDestinoId}.");
            return ValidationProblem(ModelState);
        }

        var codigoEmUso = await _context.Transportes
            .AnyAsync(t => t.Codigo == dados.Codigo && t.Id != idAtual);
        if (codigoEmUso)
        {
            return Problem(
                title: "Código de transporte já cadastrado",
                detail: $"Já existe um transporte com o código {dados.Codigo}.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return null;
    }

    private ObjectResult NaoEncontrado(int id) =>
        Problem(
            title: "Transporte não encontrado",
            detail: $"Não existe transporte com id {id}.",
            statusCode: StatusCodes.Status404NotFound);
}
