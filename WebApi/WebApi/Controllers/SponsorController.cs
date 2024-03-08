using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Enums.Enums.Sponsor;
using WebApi.Models.Models.Sponsor;
using WebApi.Models.Models.User;
using System;

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
            var advertisement = await sponsorRepo.AddAdvertisementAsync(model);
            return NoContent(); // TODO: Ask how is handled and return OK instead
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

        [HttpPost("/statistics")]
        public async Task<ActionResult<List<AdvertisementStats>>> GetAdvertisementsStatistics([FromServices] ISponsorRepository sponsorRepo, 
            [FromQuery] long userId, [FromBody] AdvertisementStatsRequest request)
        {
            var stats = await sponsorRepo.GetAllAdvertisementsStatsAsync(userId, request);
            return Ok(stats);
        }

        [HttpPost("/statistics/{advertisementId}")]
        public async Task<ActionResult<List<AdvertisementStats>>> GetAdvertisementStatistics([FromServices] ISponsorRepository sponsorRepo, 
            [FromRoute] int advertisementId, [FromBody] AdvertisementStatsRequest request)
        {
            var stats = await sponsorRepo.GetAdvertisementStatsAsync(advertisementId, request);
            return Ok(stats);
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
