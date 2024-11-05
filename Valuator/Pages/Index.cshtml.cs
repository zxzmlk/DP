using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Library;
using System.Text.Json;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        private readonly IMessageBroker _messageBroker;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IMessageBroker messageBroker)
        {
            _logger = logger;
            _storage = storage;
            _messageBroker = messageBroker;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            _storage.Store(Const.TextKey + id, text);

            CreateRankCalculatorTask(id);

            string similarity = GetSimilarity(text).ToString();
            PublishEventSimilarityCalculated(id, similarity);
            _storage.Store(Const.SimilarityKey + id, similarity);

            _storage.StoreToSet(Const.TextsSetKey, text);

            return Redirect($"summary?id={id}");
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
