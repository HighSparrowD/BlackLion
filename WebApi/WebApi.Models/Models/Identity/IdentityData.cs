namespace WebApi.Models.Models.Identity
{
    public class IdentityData
    {

        // Claims. Used by system to define claims
        public const string MachineClaimName = "Machine";
        public const string JwtIdClaimName = "jwt_id";

        // Policies. Used by jwt token to represent user policies
        public const string MachinePolicyName = "machine";
    }
}
