using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Library;

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

            string rank = _storage.Load(Const.RankKey + id);
            IsRankEmpty = rank == null;
            Rank = Convert.ToDouble(rank);

            Similarity = Convert.ToDouble(_storage.Load(Const.SimilarityKey + id));
        }
    }
}

