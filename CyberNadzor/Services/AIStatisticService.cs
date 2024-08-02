using CyberNadzor.Data;
using CyberNadzor.Entities.Statistic;
using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Statistic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;
using static CyberNadzor.Extensions.Extension;
using System.Net.Http;
using System.Text;
using System.Net.Http.Json;
using System.Net.Http.Formatting;
using System.Text.Encodings.Web;

namespace CyberNadzor.Services
{
    public class AIStatisticService
    {
        private readonly ILogger<AIStatisticService> _logger;
        private readonly IOptions<AISummarizationOptions> _options;
        private readonly ApplicationDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        public AIStatisticService(ILogger<AIStatisticService> logger,
            IOptions<AISummarizationOptions> options,
            ApplicationDbContext applicationDbContext,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _options = options;
            _db = applicationDbContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<AISummarization?> CreateAISummarizationAsync(Survey survey)
        {
            var result = await _db.AISummarizations.AddAsync(new()
            {
                Survey = survey
            });
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<bool> ProcessingWasFinishAsync(Guid IK, string summ)
        {
            //var batch = await _db.AISummarizationBatches
            //    .Include(x=>x.AISummarization)
            //    .Where(x=>x.AISummarizationBatchId == IK)
            //    .FirstOrDefaultAsync();
            //if(batch == null)
            //{
            //    _logger.LogWarning("Такого batch-а не существует");
            //    return false;
            //}
            //batch.Result = summ;
            //batch.IsProcessed = false;
            return true;
        }
        public async Task<bool> StartProcessingSurveyAsync(Survey survey)
        {
            var curentSumResult = await _db.AISummarizations.Where(x=>x.Survey == survey).FirstOrDefaultAsync();
            if (curentSumResult == null)
            {
                curentSumResult = await CreateAISummarizationAsync(survey);
            }
           
            var answersBatch = await _db.TopicTextAnswers.FromSql($"SELECT \"TopicAnswers\".\"TopicAnswerId\", \"TopicAnswers\".\"Discriminator\", \r\n\"TopicAnswers\".\"SurveyAnswersId\", \"TopicAnswers\".\"TopicId\", \"TopicAnswers\".\"AISummarizationBatchId\",\r\n\"TopicAnswers\".\"TopicTextAsnwer_Value\" \r\nFROM \"TopicAnswers\" \r\nLEFT JOIN \"AISummarizationBatches\" AS AISB ON AISB.\"AISummarizationBatchId\" = \"TopicAnswers\".\"AISummarizationBatchId\" \r\nWHERE \"TopicAnswers\".\"TopicTextAsnwer_Value\" IS NOT NULL AND LENGTH(\"TopicAnswers\".\"TopicTextAsnwer_Value\") > 5 AND AISB.\"AISummarizationBatchId\" IS NULL")
                .Include(x => x.Topic)
                .ToListAsync();
            if (answersBatch == null)
            {
                _logger.LogInformation("Ответы у опросников закончились");
                return false;
            }
            var batchToSend = answersBatch.GroupBy(x => x.Topic.Description).MaxBy(x => x.Count())
                .ToList();
            curentSumResult!.Batches.Add(new() { });
            var batch = curentSumResult.Batches.Last();
            batch.TopicAnswers.AddRange(answersBatch);
            var AIClient = _httpClientFactory.CreateClient();
            var count = 1;
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            HttpResponseMessage httpResponseMessage = null;
            string jsonString = JsonSerializer.Serialize(new AISummarizationDto()
            {
                Answers = batchToSend.ConvertAll(x => x.Value).ToList(),
                Question = batchToSend.First().Topic.Description,
                IK = batch.AISummarizationBatchId.ToString()

            }, options);

            _logger.LogInformation(jsonString);

            while (httpResponseMessage == null && count <= 10)
            {
                try
                {
                    httpResponseMessage = await AIClient.PostAsync(_options.Value.Url,
                        new StringContent(jsonString, Encoding.Unicode, "application/json"));
                }
                catch (Exception e)
                {
                    _logger.LogError($"При отправке на обработку возникла ошибка\nПопытка: {count++}");
                }
            }
            
            if (httpResponseMessage != null && httpResponseMessage.IsSuccessStatusCode)
            {
                curentSumResult!.Batches.Remove(batch);
                await _db.SaveChangesAsync();
            }
            else if(httpResponseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError($"Невозможно отправить на обработку из-за ошибки сервера обработки");
            }
            else 
            {
                _logger.LogError($"Невозможно отправить на обработку: {count++}");
                return false;
            }
            return true;
        }
    }
}
