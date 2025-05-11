namespace ServerAPI.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public IList<string> SensorIds { get; set; }
    }
}
