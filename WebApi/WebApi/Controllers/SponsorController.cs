using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Enums.Enums.Sponsor;
using WebApi.Models.Models.Sponsor;
using WebApi.Models.Models.User;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorController : Controller
    {
        [HttpGet("/advertisement-list/{sponsorId}")]
        public async Task<ActionResult<List<AdvertisementItem>>> GetAdvertisementList(
            [FromServices] ISponsorRepository sponsorRepo, 
            [FromRoute] int sponsorId)
        {
            var advertisements = await sponsorRepo.GetAdvertisementListAsync(sponsorId);
            return Ok(advertisements);
        }

        [HttpGet("/advertisement/{id}")]
        public async Task<ActionResult<Advertisement>> GetAdvertisement(
            [FromServices] ISponsorRepository sponsorRepo, 
            [FromRoute] int id)
        {
            var advertisement = await sponsorRepo.GetAdvertisementAsync(id);
            return Ok(advertisement); 
        }

        [HttpPost("/advertisement")]
        public async Task<ActionResult> AddAdvertisement(
            [FromServices] ISponsorRepository sponsorRepo, 
            [FromBody] AdvertisementNew model)
        {
            await sponsorRepo.AddAdvertisementAsync(model);
            return NoContent();
        }

        [HttpPut("/advertisement")]
        public async Task<ActionResult> UpdateAdvertisement(
            [FromServices] ISponsorRepository sponsorRepo,
            [FromBody] AdvertisementUpdate model)
        {
            await sponsorRepo.UpdateAdvertisementAsync(model);
            return NoContent();
        }

        [HttpDelete("/advertisement/{id}")]
        public async Task<ActionResult> DeleteAdvertisement(
            [FromServices] ISponsorRepository sponsorRepo, 
            [FromRoute] int id)
        {
            await sponsorRepo.DeleteAdvertisementAsync(id);
            return NoContent();
        }

        [HttpGet("/statistics")]
        public async Task<ActionResult> GetAdvertisementsStatistics([FromServices] ISponsorRepository sponsorRepo, 
            [FromQuery] long userId)
        {
            return Ok(await sponsorRepo.GetAllAdvertisementsStatsAsync(userId));
        }

        [HttpGet("/statistics/{id}")]
        public async Task<ActionResult> GetAdvertisementStatistics([FromServices] ISponsorRepository sponsorRepo, 
            [FromRoute] int id)
        {
            return Ok(await sponsorRepo.GetAdvertisementStatsAsync(id));
        }

        [HttpGet("/advertisement/switch-status/{id}")]
        public async Task<ActionResult<List<Advertisement>>> SwitchShowStatus(
            [FromServices] ISponsorRepository sponsorRepo,
            [FromRoute] int id)
        {
            await sponsorRepo.SwitchShowStatusAsync(id);
            return NoContent();
        }

        [HttpGet("/priorities/{id}")]
        public async Task<ActionResult> SetPriority(
            [FromServices] ISponsorRepository sponsorRepo,
            [FromRoute] int id,
            [FromQuery] AdvertisementPriority priority)
        {
            await sponsorRepo.SetAdvertisementPriorityAsync(id, priority);
            return NoContent();
        }

		[HttpGet("/priorities")]
		public ActionResult<List<GetLocalizedEnum>> GetPriorities(
	        [FromServices] ISponsorRepository sponsorRepo)
		{
			var priorities = sponsorRepo.GetPriorities();
			return Ok(priorities);
		}
	}
}
