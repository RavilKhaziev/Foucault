using Microsoft.AspNetCore.Identity;
using static CyberNadzor.Seed.UserSeed;
using System.Text.Json.Nodes;
using CyberNadzor.Data;
using CyberNadzor.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using CyberNadzor.Entities.Survey;
using System.IO;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Text;
using static CyberNadzor.Seed.AnswerSeed.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CyberNadzor.Seed
{
    public class AnswerSeed
    {
        private readonly ILogger<AnswerSeed> _logger;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserSeed _userSeed;
        public AnswerSeed(ILogger<AnswerSeed> logger,
            ISurveyRepository surveyRepository,
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            UserSeed userSeed
            )
        {
            _logger = logger;
            _surveyRepository = surveyRepository;
            _db = db;
            _userManager = userManager;
            _userSeed = userSeed;
        }

        public class Model
        {
            [Required]
            public string User { get; set; }
            [Required]
            public string SurveyName { get; set; }
            public class Answer
            {
                [Required]
                public string TopicDescription { get; set; }
                public string Value { get; set; }
            }
            [Required]
            public List<Answer> Answers { get; set;}
            
        }

        delegate Task SeedWithAny(string path, string surveyName);

        public async Task SeedJson(string path, string surveyName)
        {
            string json = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, path));
            var document = JsonNode.Parse(json);
            var answerList = document["Answers"].Deserialize<List<Model>>() ?? new();
            foreach (var item in answerList)
            {
                var user = await _userManager.FindByEmailAsync(item.User);
                if (user == null)
                {
                    _logger.LogWarning($"Не удалось получить пользователя {item.User}");
                    continue;
                }
                var survey = _db.Surveys
                    .Include(x => x.Topics)
                    .Where(x => x.Name == item.SurveyName)
                    .FirstOrDefault();
                if (survey == null)
                {
                    _logger.LogWarning("Не удалось получить опрос");
                    continue;
                }
                var answer = _db.SurveyAnswers.Add(new()
                {
                    Survey = survey,
                    User = user
                });

                foreach (var itemAnswer in item.Answers)
                {
                    var topic = survey.Topics.Find(x => x.Description == itemAnswer.TopicDescription);
                    if (topic == null)
                    {
                        _logger.LogWarning($"Такого топика не существует {itemAnswer.TopicDescription}");
                        continue;
                    }
                    switch (topic.Type)
                    {
                        case Topic.TopicType.Text:
                            {
                                answer.Entity.TopicAnswers.Add(new TopicTextAsnwer() { Topic = topic, Value = itemAnswer.Value });
                                break;
                            }
                        case Topic.TopicType.Choise:
                            {
                                var value = ((TopicChoise)topic).Value.FindIndex(x => x == itemAnswer.Value);
                                if (value <= -1)
                                {
                                    _logger.LogWarning($"Такого ответа не существует {itemAnswer.TopicDescription}");
                                    continue;
                                }
                                answer.Entity.TopicAnswers.Add(new TopicChoiseAnswer() { Topic = topic, Value = (uint)value });
                                break;
                            }
                    }

                }
                _db.SaveChanges();
            }
        }

        public async Task SeedXLSX(string path, string surveyName)
        {
            var survey = _db.Surveys
                    .Include(x => x.Topics)
                    .Where(x => x.Name == surveyName)
                    .FirstOrDefault();
            if (survey == null)
            {
                _logger.LogWarning($"Опросника с именем {surveyName} не существует");
                return;
            }

            IWorkbook workbook;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(fileStream);
            }
            ISheet sheet = workbook.GetSheetAt(0);
            var rowEnum = sheet.GetRowEnumerator();
            rowEnum.MoveNext();
            var headers = ((IRow)rowEnum.Current).Cells.ConvertAll(x=>x.StringCellValue);
            var userEmail = await _db.Users
                    .FromSql($"SELECT ANU.\"Email\" FROM \"AspNetUsers\" AS ANU LEFT JOIN \"SurveyAnswers\" AS SA ON ANU.\"Id\" = SA.\"UserId\" WHERE SA.\"UserId\" IS NULL ")
                    .Select(x => x.Email).ToListAsync();
            
            while (rowEnum.MoveNext())
            {
                IRow rowRaw = (IRow)rowEnum.Current;
                var row = rowRaw.Cells.ConvertAll(x=>x.StringCellValue);
                
                IdentityUser? user = null;
                if (userEmail == null)
                {
                    _logger.LogInformation("не хватает пользователей создаём нового");
                   // user = await _userSeed.CreateRandomUser();
                }
                else
                {
                    if (userEmail.Count <= 0)
                    {
                        _logger.LogInformation("не хватает пользователей");
                        break;
                    }
                    var email = userEmail.Last();
                    user = await _userManager.FindByEmailAsync(email);
                    userEmail.Remove(email);
                }
                    
                if (user == null)
                    return;
                EntityEntry<SurveyAnswers> answer;
                try
                {
                    answer = await _db.SurveyAnswers.AddAsync(new()
                    {
                        Survey = survey,
                        User = user
                    });
                }
                catch (Exception e)
                {

                    throw;
                }


                foreach (var topic in survey.Topics)
                {
                    switch (topic.Type)
                    {
                        case Topic.TopicType.Text:
                        {
                            answer.Entity.TopicAnswers.Add(new TopicTextAsnwer() { Topic = topic, Value = row[headers.FindIndex(x=>x == topic.Description)] });
                            break;
                        }
                        case Topic.TopicType.Choise:
                        {
                            try
                            {
                                var field = row[headers.FindIndex(x => x.Trim() == topic.Description.Trim())];
                                var value = ((TopicChoise)topic).Value.FindIndex(x => x == field);
                                if (value <= -1)
                                {
                                    _logger.LogWarning($"Такого ответа не существует {field}");
                                    continue;
                                }
                                answer.Entity.TopicAnswers.Add(new TopicChoiseAnswer() { Topic = topic, Value = (uint)value });
                                        
                            }
                            catch (Exception e)
                            {
                                _logger.LogWarning($"Такого ответа не существует {e.Message}");
                            }
                            break;
                        }
                    }
                }
                
            }
            await _db.SaveChangesAsync();
        }

        public async Task SeedAnswers(List<string> surveys)
        {
            _logger.LogInformation("Начало наполнения ответов");
            SeedWithAny? seedWith = null;
            string path = "seedanswers";
           // if (true) return;
            if (Path.Exists(Path.Combine(Environment.CurrentDirectory, path + ".xlsx")))
            {
                seedWith = SeedXLSX;
                path = Path.Combine(Environment.CurrentDirectory, path + ".xlsx");
            }
            else if(Path.Exists(Path.Combine(Environment.CurrentDirectory, path + ".json")))
            {
                seedWith = SeedJson;
                path = Path.Combine(Environment.CurrentDirectory, path + ".json");
            }
            if (seedWith == null)
            {
                _logger.LogInformation("Закончили заполнять ответы: нечем заполнять");
                return;
            }
            foreach (var survey in surveys)
            {
                await seedWith(path, survey);
            }
            _logger.LogInformation("Закончили заполнять ответы");
        }
    }
}
