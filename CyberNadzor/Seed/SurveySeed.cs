using CyberNadzor.Repositories.Interfaces;
using System.Text.Json.Nodes;
using System.Text.Json;
using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Survey;

namespace CyberNadzor.Seed
{
    public class SurveySeed
    {
        private readonly ILogger<SurveySeed> _logger;
        private readonly ISurveyRepository _surveyRepository;
        public SurveySeed(ILogger<SurveySeed> logger, ISurveyRepository surveyRepository)
        {
            _logger = logger;
            _surveyRepository = surveyRepository;
            
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
            foreach ( var surveyModel in surveyModels)
            {
                var topics = new List<TopicModel>();
                foreach (var topicModel in surveyModel["Topics"].AsArray())
                {
                    var type = topicModel["Type"].Deserialize<Topic.TopicType>();
                    switch (type)
                    {
                        case Topic.TopicType.Text:
                            {
                                topics.Add(new TopicTextModel() { 
                                    Description = topicModel["Description"]?.ToString() ?? "",
                                    Value = topicModel["Value"]?.ToString() ?? "" 
                                });
                                break;
                            }
                        case Topic.TopicType.Choise:
                            {
                                topics.Add(new TopicChoiseModel()
                                {
                                    Description = topicModel["Description"]?.ToString() ?? "",
                                    Value = topicModel["Value"]?.Deserialize<List<string>>() ?? new()
                                });
                                break;
                            }
                    }
                }
                surveys.Add( 
                    new SurveyCreateDto() {
                        Name = surveyModel["Name"].ToString(),
                        Description = surveyModel["Description"].ToString(),
                        IsEnable = surveyModel["IsEnable"].Deserialize<bool>(),
                        Topics = topics
                    });
            }
            _logger.LogInformation($"{surveyModels.Count}");
            await _surveyRepository.AddManyAsync(surveys);
        }
    }
}
