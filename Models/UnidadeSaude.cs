using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VaciSmart.Api.Models;

/// <summary>
/// Unidade de saúde que recebe as remessas de vacinas (lado "1" do relacionamento 1:N).
/// </summary>
[Table("VS_UNIDADE_SAUDE")]
public class UnidadeSaude
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da unidade é obrigatório.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 120 caracteres.")]
    [Column("NOME")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O município é obrigatório.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "O município deve ter entre 2 e 80 caracteres.")]
    [Column("MUNICIPIO")]
    public string Municipio { get; set; } = string.Empty;

    [Required(ErrorMessage = "A UF é obrigatória.")]
    [RegularExpression("^[A-Z]{2}$", ErrorMessage = "A UF deve ter exatamente 2 letras maiúsculas (ex.: SP).")]
    [StringLength(2)]
    [Column("UF")]
    public string Uf { get; set; } = string.Empty;

    // Navegação 1:N — uma unidade recebe vários transportes.
    // [JsonIgnore] evita ciclo na serialização e mantém o JSON de entrada simples.
    [JsonIgnore]
    public List<Transporte> Transportes { get; set; } = new();
}
