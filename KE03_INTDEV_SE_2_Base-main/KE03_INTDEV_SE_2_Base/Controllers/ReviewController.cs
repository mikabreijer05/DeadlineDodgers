using Microsoft.AspNetCore.Mvc;
using KE03_INTDEV_SE_2_Base.Models;
using KE03_INTDEV_SE_2_Base.DAL;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    // Controller voor het beheren van reviews
    public class ReviewController : Controller
    {
        // Object om database-acties voor reviews uit te voeren
        private SQLReview _sqlReview;

        // Constructor van de controller
        public ReviewController()
        {
            // Maak een nieuwe instantie van SQLReview aan
            _sqlReview = new SQLReview();
        }

        // Toont de overzichtspagina met reviews
        public IActionResult Index()
        {
            // Haal alle reviews op die nog goedgekeurd moeten worden
            var pending = _sqlReview.GetPendingReviews().ToList();

            // Haal alle reviews op
            var all = _sqlReview.GetAllReviews().ToList();

            // Maak een ViewModel aan en vul deze met de opgehaalde gegevens
            var vm = new ReviewsPageViewModel
            {
                PendingReviews = pending,
                AllReviews = all
            };

            // Stuur het ViewModel naar de Index-view
            return View("Index", vm);
        }

        // Wordt uitgevoerd wanneer een review wordt goedgekeurd
        [HttpPost]
        public IActionResult ApproveReview(int id)
        {
            // Zet de review met het opgegeven ID op goedgekeurd
            _sqlReview.ApproveReview(id);

            // Ga terug naar de overzichtspagina
            return RedirectToAction("Index");
        }

        // Wordt uitgevoerd wanneer een review wordt afgekeurd
        [HttpPost]
        public IActionResult RejectReview(int id)
        {
            // Wijs de review met het opgegeven ID af
            _sqlReview.RejectReview(id);

            // Ga terug naar de overzichtspagina
            return RedirectToAction("Index");
        }
    }
}