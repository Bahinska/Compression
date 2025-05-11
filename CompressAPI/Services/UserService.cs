using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Models;

namespace SensorApi.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<User> GetUserAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public List<UserModel> GetAllUsers()
        {
            var dbUsers = _dbContext.Users.Include(u => u.Sensors).ToList();
            var result = new List<UserModel>();

            dbUsers.ForEach(dbUser =>
            {
                var userModel = new UserModel
                {
                    Id = dbUser.Id.ToString(),
                    Username = dbUser.Username,
                    Password = dbUser.Password,
                    SensorIds = dbUser.Sensors.Select(sensor => sensor.Id.ToString()).ToList()
                };
                result.Add(userModel);
            });

            return result;
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

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _dbContext.Users
                .Include(u => u.Sensors)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return false;
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserAsync(Guid id, UserModel updatedUser)
        {
            var user = await _dbContext.Users
                .Include(u => u.Sensors)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return false;
            }

            user.Username = updatedUser.Username;
            user.Password = updatedUser.Password;

            // Оновимо сенсори
            user.Sensors.Clear();
            foreach (var sensorId in updatedUser.SensorIds)
            {
                Guid sensorGuid;
                if (Guid.TryParse(sensorId, out sensorGuid))
                {
                    var sensor = await _dbContext.Sensors.FirstOrDefaultAsync(s => s.Id == sensorGuid);
                    if (sensor != null)
                    {
                        user.Sensors.Add(sensor);
                    }
                    else
                    {
                        var newSensor = new ServerAPI.Models.Sensor
                        {
                            Id = sensorGuid,
                            Users = new List<User>() { user }
                        };
                        _dbContext.Sensors.Add(newSensor);
                        user.Sensors.Add(sensor);
                    }
                }
            }

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}