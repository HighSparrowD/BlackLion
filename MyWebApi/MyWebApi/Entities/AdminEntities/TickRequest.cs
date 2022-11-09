using MyWebApi.Entities.UserInfoEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.AdminEntities
{
    #nullable enable
    public class TickRequest
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long? AdminId { get; set; }
        public bool? State { get; set; }
        public string? Video { get; set; }
        public string? Photo { get; set; }
        public string? Circle { get; set; }
        public virtual User? User{ get; set; }
    }
}
