using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Models.Models.Identity
{
    public static class JwtStorage
    {
        // TODO: Use cache, or something more secure
        private static readonly List<string> _validTokenIds = new List<string>();

        public static string GenerateTokenId()
        {
            var newId = Guid.NewGuid().ToString();
            _validTokenIds.Add(newId);

            return newId;
        }

        public static bool IsTokenValid(string tokenId)
        {
            return _validTokenIds.Contains(tokenId);
        }
    }
}
