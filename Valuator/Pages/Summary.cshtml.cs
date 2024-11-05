using System;
using Library;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private readonly IStorage _storage;

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }
        public bool IsRankEmpty { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug(id);

            string segmentId = _storage.GetSegmentId(id);

            string rank = _storage.Load(segmentId, Const.RankKey + id);
            IsRankEmpty = rank == null;
            Rank = Convert.ToDouble(rank);

            Similarity = Convert.ToDouble(_storage.Load(segmentId, Const.SimilarityKey + id));
        }
    }
}
