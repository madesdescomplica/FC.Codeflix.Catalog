using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.Domain.Validation;

public class DomainValidation
{
    public static void NotNull(object? target, string fieldname)
    {
        if (target == null)
        {
            throw new EntityValidationException(
                $"{fieldname} should not be null"
            );
        }
    }

    public static void NotNullOrEmpty(string? target, string fieldname)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            throw new EntityValidationException(
                $"{fieldname} should not be empty or null"
            );
        }
    }

    public static void MinLength(string target, int minLength, string fieldname)
    {
        if (target.Length < minLength)
        {
            throw new EntityValidationException(
                $"{fieldname} should be at least {minLength} characters long"
            );
        }
    }

    public static void MaxLength(string target, int maxLength, string fieldname)
    {
        if (target.Length > maxLength)
        {
            throw new EntityValidationException(
                $"{fieldname} should be less or equal {maxLength} characters long"
            );
        }
    }
}
