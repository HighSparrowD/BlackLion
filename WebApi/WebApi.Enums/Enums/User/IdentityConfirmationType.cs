namespace WebApi.Enums.Enums.User;

public enum IdentityConfirmationType : short
{
    None = 1,
    Awaiting = 2, //Request awaits confirmation
    Partial = 3,
    Full = 4
}

