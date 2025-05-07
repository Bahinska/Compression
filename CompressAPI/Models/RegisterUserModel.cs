using System.ComponentModel.DataAnnotations;

namespace ServerAPI.Models
{
    public class RegisterUserModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string SensorId { get; set; }
    }
}
