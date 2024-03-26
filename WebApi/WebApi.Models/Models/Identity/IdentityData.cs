namespace WebApi.Models.Models.Identity
{
    public class IdentityData
    {
        // Claims. Used by system to define claims
        public const string AdminClaimName = "Admin";
        public const string CreatorClaimName = "Creator";
        public const string SponsorClaimName = "Sponsor";
        public const string MachineClaimName = "Machine";

        // Policies. Used by jwt token to represent user policies
        public const string AdminPolicyName = "admin";
        public const string SponsorPolicyName = "sponsor";
        public const string MachinePolicyName = "machine";
    }
}
