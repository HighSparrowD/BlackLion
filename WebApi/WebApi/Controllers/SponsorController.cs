using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Main.Models.Sponsor;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorController : Controller
    {
        private ISponsorRepository _repository;

        //public SponsorController(ISponsorRepository repository, ILogger<SponsorController> logger)
        //{
        //    _repository = repository;
        //    _logger = logger;
        //}

        [HttpGet("advertisement")]
        public async Task<ActionResult<List<Ad>>> GetAdvertisementList([FromServices] ISponsorRepository sponsorRepo)
        {
            return null;
        }
    }
}
