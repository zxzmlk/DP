using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Library;
using System.Text.Json;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        private readonly IMessageBroker _messageBroker;
        public string[] Countries { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IMessageBroker messageBroker)
        {
            _logger = logger;
            _storage = storage;
            _messageBroker = messageBroker;
            Countries = Const.Countries;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text, string country)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string segmentId = GetSegmentIdByCountry(country);
            _logger.LogDebug($"LOOKUP: {id}, {segmentId}");
            // Добавляем запись в карту сегментирования
            _storage.Store(Const.SegmentKey + id, segmentId);

            // Сохраняем текст в соответствующий сегмент
            _storage.Store(segmentId, Const.TextKey + id, text);

            CreateRankCalculatorTask(id);

            string similarity = GetSimilarity(text).ToString();
            PublishEventSimilarityCalculated(id, similarity);

            // Сохраняем similarity в соответствующий сегмент
            _storage.Store(segmentId, Const.SimilarityKey + id, similarity);

            // Сохраняем значение текста в множество в соответствующем сегменте
            _storage.StoreToSet(segmentId, Const.TextsSetKey, text);

            return Redirect($"summary?id={id}");
        }

        private string GetSegmentIdByCountry(string country)
        {
            switch (country)
            {
                case "Russia":
                    return Const.SEGMENT_RUS;
                case "France":
                case "Germany":
                    return Const.SEGMENT_EU;
                case "USA":
                case "India":
                    return Const.SEGMENT_OTHER;
            }

            _logger.LogError($"Undefined country {country}");
            
            return "";
        }

        private void CreateRankCalculatorTask(string id)
        {
            _messageBroker.Send("valuator.processing.rank", id);
        }
        
        private void PublishEventSimilarityCalculated(string id, string similarity)
        {
            EventContainer eventData = new EventContainer { Name = "SimilarityCalculated", Id = id, Value = similarity };
            _messageBroker.Send("Events", JsonSerializer.Serialize(eventData));
        }

        private double GetSimilarity(string text)
        {
            return _storage.IsExistsInSet(Const.TextsSetKey, text) ? 1 : 0;
        }
    }
}
