using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Models;

namespace SensorApi.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> RegisterUserAsync(RegisterUserModel newUser)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == newUser.Email);
            if (existingUser != null)
            {
                return false;
            }
            var userId = Guid.NewGuid();
            Guid sensoreId;
            Guid.TryParse(newUser.SensorId, out sensoreId);

            var user = new User()
            {
                Id = userId,
                Username = newUser.Email,
                Password = newUser.Password,
                Sensors = new List<ServerAPI.Models.Sensor>()
            };

            var sensor = new ServerAPI.Models.Sensor
            {
                Id = sensoreId,
                UserId = userId,
                Users = new List<ServerAPI.Models.User>()
            };

            user.Sensors.Add(sensor);
            sensor.Users.Add(user);

            _dbContext.Sensors.AddRange(user.Sensors);

            await _dbContext.SaveChangesAsync();

            return true;
        }

    }
}