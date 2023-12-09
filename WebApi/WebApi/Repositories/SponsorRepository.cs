using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApi.Data;
using WebApi.Entities.SponsorEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Entities.UserInfoEntities;
using WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Main.Models.User;
using WebApi.Main.Models.Sponsor;
using WebApi.Main.Enums.General;

namespace WebApi.Repositories
{
    public class SponsorRepository : ISponsorRepository
    {
        private UserContext _contx { get; set; }

        public SponsorRepository(UserContext context)
        {
            _contx = context;
        }

    }
}
