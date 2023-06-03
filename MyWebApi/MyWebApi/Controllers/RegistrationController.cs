using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : Controller
    {
        public IStringLocalizer<RegistrationController> _localizer { get; set; }

        public RegistrationController(IStringLocalizer<RegistrationController> localizer)
        {
            _localizer = localizer;
        }

        [HttpGet("/get-registration-localization")]
        public async Task<Dictionary<string, string>> GetRegistrationLocalization()
        {
            var s = Request.Headers;

            var localizationDict = new Dictionary<string, string>();
            await Task.Run(() =>
            {
                var rawLocalization = _localizer.GetAllStrings();

                foreach (var item in rawLocalization)
                {
                    localizationDict.Add(item.Name, item.Value);
                }
            });

            return localizationDict;
        }
    }
}
