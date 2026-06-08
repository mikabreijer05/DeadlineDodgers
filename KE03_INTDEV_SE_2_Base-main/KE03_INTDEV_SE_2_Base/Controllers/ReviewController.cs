using Microsoft.AspNetCore.Mvc;
using KE03_INTDEV_SE_2_Base.Models;
using KE03_INTDEV_SE_2_Base.DAL;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class ReviewController : Controller
    {
        private SQLReview _sqlReview;

        public ReviewController()
        {
            _sqlReview = new SQLReview();
        }

        public IActionResult Index()
        {
            var pending = _sqlReview.GetPendingReviews().ToList();
            var all = _sqlReview.GetAllReviews().ToList();
            var vm = new ReviewsPageViewModel
            {
                PendingReviews = pending,
                AllReviews = all
            };
            return View("Index", vm);
        }

        [HttpPost]
        public IActionResult ApproveReview(int id)
        {
            _sqlReview.ApproveReview(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RejectReview(int id)
        {
            _sqlReview.RejectReview(id);
            return RedirectToAction("Index");
        }
    }
}
