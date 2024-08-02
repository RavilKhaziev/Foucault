using CyberNadzor.Repositories.Interfaces;
using System.Text.Json.Nodes;
using System.Text.Json;
using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Survey;
using CyberNadzor.Data;
using Microsoft.AspNetCore.Identity;

namespace CyberNadzor.Seed
{
    public class SurveySeed
    {
        private readonly ILogger<SurveySeed> _logger;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserSeed _userSeed;
        public SurveySeed(ILogger<SurveySeed> logger,
            ISurveyRepository surveyRepository,
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            UserSeed userSeed)
        {
            _logger = logger;
            _surveyRepository = surveyRepository;
            _db = db;
            _userManager = userManager;
            _userSeed = userSeed;
        }

        public async Task SeedSurvey()
        {
            _logger.LogInformation("Начало наполнения опросников");
            string json = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "seedsurvey.json"));
            var document = JsonNode.Parse(json);
            var surveyModels = document["Survey"].AsArray();
            var surveys = new List<SurveyCreateDto>(surveyModels.Count);
            var options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true
            };
            foreach (var surveyModel in surveyModels)
            {
                var user = await _userManager.FindByEmailAsync(surveyModel["UserOnwerEmail"]!.ToString());
                if(user == null)
                {
                    _logger.LogWarning($"Такого пользователя {surveyModel["UserOnwerEmail"]!.ToString()} не существует" +
                        $"\nСоздаём случайного пользователя");
                    user = await _userSeed.CreateRandomUser();
                    _logger.LogInformation($"Создали пользователя с email {user.Email}");
                } 
                var survey = _db.Add(new Survey() {
                    Name = surveyModel["Name"].ToString(),
                    Description = surveyModel["Description"].ToString(),
                    IsEnable = surveyModel["IsEnable"].Deserialize<bool>(),
                    UserOwner = user,
                    Topics = new()
                });
                foreach (var topicModel in surveyModel["Topics"].AsArray())
                {
                    var type = topicModel["Type"].Deserialize<Topic.TopicType>();
                    switch (type)
                    {
                        case Topic.TopicType.Text:
                            {
                                survey.Entity.Topics.Add(new TopicText()
                                {
                                    Description = topicModel["Description"]?.ToString() ?? "",
                                    Value = topicModel["Value"]?.ToString() ?? ""
                                });
                                break;
                            }
                        case Topic.TopicType.Choise:
                            {
                                survey.Entity.Topics.Add(new TopicChoise()
                                {
                                    Description = topicModel["Description"]?.ToString() ?? "",
                                    Value = topicModel["Value"]?.Deserialize<List<string>>() ?? new()
                                });
                                break;
                            }
                    }
                }


            }
            await _db.SaveChangesAsync();
            //foreach ( var surveyModel in surveyModels)
            //{

            //    _surveyRepository.AddMany(surveys);
            //    var topics = new List<TopicModel>();
            //    foreach (var topicModel in surveyModel["Topics"].AsArray())
            //    {
            //        var type = topicModel["Type"].Deserialize<Topic.TopicType>();
            //        switch (type)
            //        {
            //            case Topic.TopicType.Text:
            //                {
            //                    topics.Add(new TopicTextModel() { 
            //                        Description = topicModel["Description"]?.ToString() ?? "",
            //                        Value = topicModel["Value"]?.ToString() ?? "" 
            //                    });
            //                    break;
            //                }
            //            case Topic.TopicType.Choise:
            //                {
            //                    topics.Add(new TopicChoiseModel()
            //                    {
            //                        Description = topicModel["Description"]?.ToString() ?? "",
            //                        Value = topicModel["Value"]?.Deserialize<List<string>>() ?? new()
            //                    });
            //                    break;
            //                }
            //        }
            //    }
            //    surveys.Add( 
            //        new SurveyCreateDto() {
            //            Name = surveyModel["Name"].ToString(),
            //            Description = surveyModel["Description"].ToString(),
            //            IsEnable = surveyModel["IsEnable"].Deserialize<bool>(),
            //            Topics = topics
            //        });
            //}
            //_logger.LogInformation($"{surveyModels.Count}");
            //_surveyRepository.AddMany(surveys);
        }
    }
}
