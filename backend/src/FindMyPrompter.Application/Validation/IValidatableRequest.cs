using FluentValidation.Results;

namespace FindMyPrompter.Application.Validation;

/// <summary>
/// Contrato para requests que carregam a própria validação estrutural.
/// A implementação instancia seu validator específico e o invoca em <see cref="Validate"/>.
/// O filtro de validação chama este método antes da action executar e interrompe com
/// HTTP 400 quando o resultado é inválido.
/// </summary>
public interface IValidatableRequest
{
    ValidationResult Validate();
}
