using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings (WITH FILTERING)
        public async Task<IActionResult> Index(
            string searchString,
            int? eventTypeId,
            DateTime? startDate,
            DateTime? endDate)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            // 🔍 SEARCH (Venue or Event name)
            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    b.Venue.Name.Contains(searchString) ||
                    b.Event.EventName.Contains(searchString));
            }

            // 🎭 FILTER BY EVENT TYPE
            if (eventTypeId.HasValue)
            {
                bookings = bookings.Where(b =>
                    b.Event.EventTypeId == eventTypeId.Value);
            }

            // 📅 FILTER BY EVENT START DATE
            if (startDate.HasValue)
            {
                bookings = bookings.Where(b =>
                    b.Event.StartDate.Date >= startDate.Value.Date);
            }

            // 📅 FILTER BY EVENT END DATE
            if (endDate.HasValue)
            {
                bookings = bookings.Where(b =>
                    b.Event.EndDate.Date <= endDate.Value.Date);
            }

            // dropdown for view
            ViewBag.EventTypes = new SelectList(
                _context.EventTypes,
                "EventTypeId",
                "TypeName"
            );

            return View(await bookings.ToListAsync());
        }

        // 📊 BOOKING REPORT
        public async Task<IActionResult> Report()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .OrderBy(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name");

            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,VenueId,EventId,BookingDate")] Booking booking)
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);

            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            // 🚫 Prevent double bookings
            bool alreadyBooked = await _context.Bookings.AnyAsync(b =>
                b.VenueId == booking.VenueId &&
                b.BookingDate.Date == booking.BookingDate.Date
            );

            if (alreadyBooked)
            {
                ModelState.AddModelError("", "This venue is already booked on the selected date.");
                return View(booking);
            }

            _context.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound();

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "Name", booking.VenueId);

            return View(booking);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,VenueId,EventId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}