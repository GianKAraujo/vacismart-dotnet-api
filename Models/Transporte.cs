using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VaciSmart.Api.Models;

/// <summary>
/// Remessa de vacinas destinada a uma unidade de saúde (lado "N" do relacionamento 1:N).
/// </summary>
[Table("VS_TRANSPORTE")]
public class Transporte
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O código do transporte é obrigatório.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "O código deve ter entre 3 e 20 caracteres.")]
    [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Use apenas letras maiúsculas, números e hífen (ex.: TRP-001).")]
    [Column("CODIGO")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O status é obrigatório.")]
    [AllowedValues(
        StatusTransporte.Cadastrado,
        StatusTransporte.EmTransporte,
        StatusTransporte.Recebido,
        StatusTransporte.Finalizado,
        ErrorMessage = "Status inválido. Use CADASTRADO, EM_TRANSPORTE, RECEBIDO ou FINALIZADO.")]
    [StringLength(20)]
    [Column("STATUS")]
    public string Status { get; set; } = StatusTransporte.Cadastrado;

    // Preenchido pelo servidor no POST; valores enviados pelo cliente são ignorados.
    [Column("CRIADO_EM")]
    public DateTimeOffset CriadoEm { get; set; }

    // Preenchido pelo servidor quando o status passa a RECEBIDO/FINALIZADO.
    [Column("RECEBIDO_EM")]
    public DateTimeOffset? RecebidoEm { get; set; }

    // Campo opcional (coluna anulável), adicionado pela migration AdicionaObservacaoTransporte.
    [StringLength(200, ErrorMessage = "A observação deve ter no máximo 200 caracteres.")]
    [Column("OBSERVACAO")]
    public string? Observacao { get; set; }

    // Chave estrangeira para a unidade de destino (obrigatória).
    [Range(1, int.MaxValue, ErrorMessage = "Informe o id da unidade de destino.")]
    [Column("UNIDADE_DESTINO_ID")]
    public int UnidadeDestinoId { get; set; }

    // Navegação N:1 — ignorada no JSON; o cliente trabalha só com UnidadeDestinoId.
    [JsonIgnore]
    public UnidadeSaude? UnidadeDestino { get; set; }
}
