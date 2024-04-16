namespace WebApi.Enums.Enums.User;

public enum VerificationRequestStatus : short
{
    ToView = 1,
    InProcess = 2,
    Declined = 3,
    Approved = 4,
    Aborted = 5,
    Failed = 6
}

