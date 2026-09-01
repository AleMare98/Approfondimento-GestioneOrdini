using System.ComponentModel.DataAnnotations;

namespace GestioneOrdini.Api.Contracts.Clienti;

public sealed record CreaClienteRequest(
    [property: Required]
    [property: StringLength(100, MinimumLength = 2)]
    string Nome,

    [property: Required]
    [property: EmailAddress]
    [property: StringLength(254)]
    string Email);