﻿namespace WebApi.Models.Models.User;

public enum SwitchBusyStatusResult
{
    Success = 1,
    IsBusy = 2,
    DoesNotExist = 3,
    IsDeleted = 4,
    IsBanned = 5
}
