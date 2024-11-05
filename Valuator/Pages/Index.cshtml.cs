using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;
        private readonly string _textsSetKey = "TEXTS-SET";

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string rankKey = "RANK-" + id;
            string rank = GetRank(text).ToString("0.##");
            _storage.Store(rankKey, rank);

            string similarityKey = "SIMILARITY-" + id;
            string similarity = GetSimilarity(text).ToString();
            _storage.Store(similarityKey, similarity);

            string textKey = "TEXT-" + id;
            _storage.Store(textKey, text);
            _storage.StoreToSet(_textsSetKey, text);

            return Redirect($"summary?id={id}");
        }

        private double GetRank(string text)
        {
            if (text == null) {
                return 0;
            }
            int notLetterCharsCount = text.Where(ch => !char.IsLetter(ch)).Count();
            return notLetterCharsCount / (double) text.Length;
        }

        private double GetSimilarity(string text)
        {
            return _storage.IsExistsInSet(_textsSetKey, text) ? 1 : 0;
        }
    }
}

