using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace MyWebApi.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private AdminContext _contx { get; set; }

        public AdminRepository(AdminContext context)
        {
            _contx = context;
        }

        public async Task<long> UploadCities(List<City> cities)
        {
            cities.ForEach(async c => await _contx.CITIES.AddAsync(c));
            await _contx.SaveChangesAsync();
            return cities.Count;
        }

        public async Task<long> UploadCountries(List<UpdateCountry> countries)
        {
            countries.ForEach(async c => await _contx.COUNTRIES.AddAsync(c));
            await _contx.SaveChangesAsync();

            //foreach (var item in countries)
            //{
            //    _contx.COUNTRIES.Add(item);
            //    await _contx.SaveChangesAsync();
            //}

            return countries.Count;
        }

        public async Task<long> UploadLanguages(List<Language> langs)
        {
            langs.ForEach(async l => await _contx.LANGUAGES.AddAsync(l));
            await _contx.SaveChangesAsync();

            return langs.Count;
        }

        public async Task<long> UploadFeedbackReasons(List<FeedbackReason> reasons)
        {
            reasons.ForEach(async r => await _contx.FEEDBACK_REASONS.AddAsync(r));
            await _contx.SaveChangesAsync();
            return reasons.Count;
        }

        public async Task<List<Feedback>> GetFeedbacks()
        {
            var reports = await _contx.SYSTEM_FEEDBACKS.Include(r => r.User)
                .Include(r => r.Reason).ToListAsync();

            return reports;
        }

        public async Task<bool> CheckUserIsAdmin(long userId)
        {
            var admin = await _contx.SYSTEM_ADMINS.FindAsync(userId);
            if (admin == null) { return false; }

            return admin.IsEnabled;
        }

        public async Task<byte> SwitchAdminStatus(long userId)
        {
            var admin = await _contx.SYSTEM_ADMINS.Where(a => a.Id == userId).SingleOrDefaultAsync();
            if (admin == null) { return 0; }

            admin.IsEnabled = admin.IsEnabled ? false : true;
            _contx.Update(admin);
            await _contx.SaveChangesAsync();
            return 1;
        }

        public async Task<bool?> GetAdminStatus(long userId)
        {
            var admin = await _contx.SYSTEM_ADMINS.Where(a => a.Id == userId).SingleOrDefaultAsync();
            if (admin == null){ return null; }

            return admin.IsEnabled;
        }
    }
}
