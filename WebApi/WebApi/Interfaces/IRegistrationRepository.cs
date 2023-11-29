using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<List<string>> SuggestLanguagesAsync(string incorrectLanguage);
        Task<List<string>> SuggestCountriesAsync(string incorrectCountry);
        Task<List<string>> SuggestCitiesAsync(string incorrectCity);
    }
}
