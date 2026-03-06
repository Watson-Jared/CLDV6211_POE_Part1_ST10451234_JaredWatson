using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Event
        public async Task<IActionResult> Index()
        {
            var events = await _context.Event.ToListAsync();
            return View(events);
        }

        // GET: Event/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Event.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(@event);
        }

        // GET: Event/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();

            return View(@event);
        }

        // POST: Event/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.EventID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Event.Update(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(@event);
        }

        // GET: Event/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return NotFound();

            // Check if event has bookings
            var isBooked = await _context.Booking.AnyAsync(b => b.EventID == id);

            if (isBooked)
            {
                ModelState.AddModelError("", "Cannot delete event with existing bookings.");
                return RedirectToAction(nameof(Index));
            }

            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Event/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (@event == null) return NotFound();

            return View(@event);
        }
    }
}