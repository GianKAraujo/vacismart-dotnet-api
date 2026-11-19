namespace VaciSmart.Api.Models;

/// <summary>
/// Estados da remessa combinados no contrato do VaciSmart (os mesmos nomes usados
/// na API Java e no app Flutter). Nenhum estado representa localização/GPS.
/// </summary>
public static class StatusTransporte
{
    public const string Cadastrado = "CADASTRADO";
    public const string EmTransporte = "EM_TRANSPORTE";
    public const string Recebido = "RECEBIDO";
    public const string Finalizado = "FINALIZADO";

    /// <summary>Indica se a remessa já chegou à unidade de destino.</summary>
    public static bool JaFoiRecebido(string status) =>
        status == Recebido || status == Finalizado;
}
