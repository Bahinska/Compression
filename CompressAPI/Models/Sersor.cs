using System.ComponentModel.DataAnnotations;

namespace ServerAPI.Models
{
    public class Sensor
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
