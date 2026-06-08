using System;
using System.Collections.Generic;

namespace KE03_INTDEV_SE_2_Base.Models
{
    public class ReviewsPageViewModel
    {
        public IEnumerable<Review> PendingReviews { get; set; }
        public IEnumerable<Review> AllReviews { get; set; }

        // Optional filter fields
        public string Search { get; set; }
        public string Status { get; set; }
        public int? ProductId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
