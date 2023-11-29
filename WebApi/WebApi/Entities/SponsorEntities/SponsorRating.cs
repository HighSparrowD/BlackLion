using WebApi.Entities.UserInfoEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.SponsorEntities
{
	public class SponsorRating
	{
		[Key]
		public long Id { get; set; }
		public long SponsorId { get; set; }
		public long UserId { get; set; }
		public short Rating { get; set; }
		public string Comment { get; set; }
		public DateTime CommentTime { get; set; }
		public Sponsor Sponsor { get; set; }
		public User Commentator{ get; set; }
	}
}
