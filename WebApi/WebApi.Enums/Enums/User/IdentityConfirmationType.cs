﻿namespace WebApi.Enums.Enums.User;

public enum IdentityConfirmationType
{
    None = 0,
    Awaiting = 1, //Request awaits confirmation
    Partial = 2,
    Full = 3
}
