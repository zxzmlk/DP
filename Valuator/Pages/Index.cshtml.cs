using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        private readonly IMessageBroker _messageBroker;
        private readonly string _textsSetKey = "TEXTS-SET";

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

            CreateRankCalculatorTask(id);

            string similarityKey = "SIMILARITY-" + id;
            string similarity = GetSimilarity(text).ToString();
            _storage.Store(similarityKey, similarity);

            string textKey = "TEXT-" + id;
            _storage.Store(textKey, text);
            _storage.StoreToSet(_textsSetKey, text);

            return Redirect($"summary?id={id}");
        }
        
        private void CreateRankCalculatorTask(string id)
        {
            _messageBroker.Send("valuator.processing.rank", id);
        }

        private double GetSimilarity(string text)
        {
            return _storage.IsExistsInSet(_textsSetKey, text) ? 1 : 0;
        }
    }
}
