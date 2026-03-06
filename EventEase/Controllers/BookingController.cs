using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace EventEase.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .ToListAsync();

            return View(bookings);
        }

        public IActionResult Create()
        {
            ViewData["Event"] = _context.Event.ToList();
            ViewData["Venue"] = _context.Venue.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            var EventStartDate = _context.Event.FirstOrDefault(e => e.EventID == booking.EventID)?.EventStartDate;

            var conflict = await _context.Booking
                .AnyAsync(b => b.VenueID == booking.VenueID &&
                               _context.Event.Any(e =>
                                   e.EventID == b.EventID &&
                                   e.EventStartDate == EventStartDate));

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Event"] = _context.Event.ToList();
            ViewData["Venue"] = _context.Venue.ToList();
            return View(booking);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Venue.FindAsync(id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
    }
}