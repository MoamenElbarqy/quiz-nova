using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Security;

public static class PermissionErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Permission_Name_Required", "Permission name is required.");

    public static readonly Error DescriptionRequired =
        Error.Validation("Permission_Description_Required", "Permission description is required.");
}
