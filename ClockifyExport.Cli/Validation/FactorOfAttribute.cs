using System.ComponentModel.DataAnnotations;

namespace ClockifyExport.Cli.Validation;

/// <summary>
/// Validation attribute to indicate that a property, field or parameter must be a factor of a given number.
/// </summary>
/// <param name="n">The number the value must be a factor of.</param>
public sealed class FactorOfAttribute(int n)
    : ValidationAttribute(() => $$"""The {0} field must be a factor of {{n}}.""")
{
    /// <summary>
    /// The number the value must be a factor of.
    /// </summary>
    public int N => n;

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        return int.TryParse(value?.ToString() ?? string.Empty, out var intValue)
            && intValue > 0
            && intValue <= n
            && n % intValue == 0;
    }
}
