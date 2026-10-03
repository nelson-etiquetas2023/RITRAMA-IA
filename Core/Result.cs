namespace Ritrama2025.Core;

/// <summary>
/// Resultado genérico para operaciones que pueden fallar por reglas de negocio previsibles.
/// Úsalo en services para validaciones (stock insuficiente, dato no encontrado, validación fallida)
/// reservando excepciones para fallos de infraestructura/inesperados.
/// </summary>
/// <typeparam name="T">Tipo del valor de éxito.</typeparam>
/// <param name="IsSuccess">True si la operación fue exitosa.</param>
/// <param name="Value">Valor cuando <see cref="IsSuccess"/> es true.</param>
/// <param name="Error">Mensaje de error cuando <see cref="IsSuccess"/> es false.</param>
/// <param name="ErrorCode">Código opcional para discriminar el error en la UI.</param>
public sealed record Result<T>(bool IsSuccess, T? Value, string? Error, string? ErrorCode)
{
    /// <summary>
    /// Crea un resultado exitoso.
    /// </summary>
    /// <param name="value">Valor de éxito.</param>
    /// <returns>Resultado con <see cref="IsSuccess"/> = true.</returns>
    public static Result<T> Success(T value) => new(true, value, null, null);

    /// <summary>
    /// Crea un resultado fallido.
    /// </summary>
    /// <param name="error">Mensaje descriptivo.</param>
    /// <param name="code">Código opcional (ej. VALIDATION_CATEGORY).</param>
    /// <returns>Resultado con <see cref="IsSuccess"/> = false.</returns>
    public static Result<T> Failure(string error, string? code = null) => new(false, default, error, code);
}

/// <summary>
/// Resultado sin valor para operaciones que solo indican éxito/fallo (ej. Anular).
/// </summary>
/// <param name="IsSuccess">True si la operación fue exitosa.</param>
/// <param name="Error">Mensaje de error cuando <see cref="IsSuccess"/> es false.</param>
/// <param name="ErrorCode">Código opcional.</param>
public sealed record Result(bool IsSuccess, string? Error, string? ErrorCode)
{
    /// <summary>
    /// Crea un resultado exitoso sin valor.
    /// </summary>
    public static Result Success() => new(true, null, null);

    /// <summary>
    /// Crea un resultado fallido sin valor.
    /// </summary>
    public static Result Failure(string error, string? code = null) => new(false, error, code);

    /// <summary>
    /// Convierte a <see cref="Result{T}"/> con valor booleano para compatibilidad con firmas bool.
    /// </summary>
    public Result<bool> ToBoolResult() => IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(Error ?? "Operación fallida", ErrorCode);
}
