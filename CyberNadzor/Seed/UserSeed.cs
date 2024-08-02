using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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

        public async Task<IdentityUser?> CreateRandomUser()
        {
            StringBuilder builder = new StringBuilder();
            var name = Guid.NewGuid().ToString().ToUpper().Substring(20);
            var userEmail = name + "@mail.com";
            try
            {
                var result = await _userManager.CreateAsync(new(userEmail) { Email = userEmail }, userEmail);
            }
            catch (Exception e)
            {

                throw;
            }
            

           // _logger.LogInformation($"Создаём нового пользователя {result}");
            return await _userManager.FindByEmailAsync(userEmail);
        }
        public async Task SeedUsers(uint count = 2)
        {
            _logger.LogInformation("Начало наполнения людей");
                   
            string json = File.ReadAllText(Path.Combine( Environment.CurrentDirectory, "seedusers.json"));
            var document = JsonNode.Parse(json);
            var userList = document["Users"].Deserialize<List<UserModel>>() ?? new();
            foreach (var item in userList)
            {
                
                var result = await _userManager.CreateAsync(new(item.Email) { Email = item.Email}, item?.Password ?? Guid.NewGuid().ToString());
                if (!result.Succeeded)
                {
                    _logger.LogWarning($"Не удалось создать {result.Errors}");
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
            string path = "seedanswers";
            // if (true) return;
            if (Path.Exists(Path.Combine(Environment.CurrentDirectory, path + ".xlsx")))
            {
                path = Path.Combine(Environment.CurrentDirectory, path + ".xlsx");
            }
            else
            {
                _logger.LogInformation("Закончили пополнять людей");
                return;
            }
            IWorkbook workbook;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream);
            }
            ISheet sheet = workbook.GetSheetAt(0);
            for (int i = 0; i < sheet.LastRowNum - userList.Count; i++)
            {
                var name = Guid.NewGuid().ToString().ToUpper().Substring(20);
                var userEmail = name + "@mail.com";
                try
                {
                    var result = await _userManager.CreateAsync(new(userEmail) { Email = userEmail }, userEmail);
                }
                catch (Exception e)
                {

                    throw;
                }
            }
            _logger.LogInformation("Закончили пополнять людей");
        }
    }
}
