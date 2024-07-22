using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CyberNadzor.Seed
{
    public class UserSeed
    {
        private readonly ILogger<UserSeed> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        public UserSeed(ILogger<UserSeed> logger, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {   
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
        }

        public class UserModel
        {
            public string Email { get; set; } = null!;
            public string? Password { get; set; } = null;
        }  

        public async Task SeedUsers(uint count = 2)
        {
            _logger.LogInformation("Начало наполнения людей");

            string json = File.ReadAllText(Path.Combine( Environment.CurrentDirectory, "seedusers.json"));
            var document = JsonNode.Parse(json);
            var userList = document["Users"].Deserialize<List<UserModel>>() ?? new();
            foreach (var item in userList)
            {
                
                var result = await _userManager.CreateAsync(new(item.Email), item?.Password ?? Guid.NewGuid().ToString());
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Не удалось создать");
                    continue;
                }
                var user = await _userManager.FindByNameAsync(item.Email);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                if (!(await _userManager.ConfirmEmailAsync(user, token)).Succeeded)
                {
                    _logger.LogWarning("Не удалось создать");
                    continue;
                }
                _logger.LogInformation($"Привет {user.Email} {user.Id}");
            }
            _logger.LogInformation("Закончили пополнять людей");
        }
    }
}
