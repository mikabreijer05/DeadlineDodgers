using KE03_INTDEV_SE_2_Base.DAL;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
   
    public class SupportController : Controller
    {
        private readonly SQLCustomerService _sqlCustomerService;
        public SupportController()
        {
            _sqlCustomerService = new SQLCustomerService();
        }
        public IActionResult Index()
        {
            var tickets = _sqlCustomerService.GetAllCustomerServiceTickets();

            return View(tickets);
        }

        public IActionResult Create()
        {
            // I return an empty Ticket object so my view has a model to bind to
            // This prevents null reference issues in the Razor view
            return View(new CustomerServiceTicket());
        }

        [HttpPost]
        public IActionResult Create(CustomerServiceTicket newTicket)
        {
            // I validate the incoming data before sending it to the database
            // This prevents invalid records from being stored

            if (string.IsNullOrWhiteSpace(newTicket.Title))
            {
                ModelState.AddModelError("TicketTitle", "Ticket title is required.");
                return View(newTicket);
            }

            if (newTicket.ContactDescriptions == null || !newTicket.ContactDescriptions.Any())
            {
                ModelState.AddModelError("ContactDescriptions", "At least one contact description is required.");
                return View(newTicket);
            }

            if (!ModelState.IsValid)
            {
                return View(newTicket);
            }

            // I insert the new ticket into the database using my DAL
            _sqlCustomerService.AddCustomerServiceTicket(newTicket);

            // After successful insert, I redirect back to the product overview
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            // I fetch a single product by id so I can edit it
            var product = _sqlCustomerService.GetTicketById(id);

            // I check if the product exists to avoid errors when an invalid id is passed
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(CustomerServiceTicket updatedTicket)
        {
            // I validate the model again before updating the database
            if (!ModelState.IsValid)
            {
                return View(updatedTicket);
            }

            // I update the product in the database using my DAL
            _sqlCustomerService.UpdateTicket(updatedTicket);

            // After updating, I go back to the product list
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            // I remove the product from the database using its id
            _sqlCustomerService.DeleteTicket(id);

            // After deletion, I return to the overview page
            return RedirectToAction("Index");
        }
    }
}
