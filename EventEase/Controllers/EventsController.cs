using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Youre getting events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // Youre getting details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // get create
        public IActionResult Create()
        {
            ViewData["EventTypeId"] =
                new SelectList(_context.EventTypes, "EventTypeId", "TypeName");

            return View();
        }

        // creating post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,StartDate,EndDate,Description,ImageUrl,EventTypeId")] Event @event)
        {
            // Fixing navigation validation Issue
            ModelState.Remove("EventType");

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventTypeId"] =
                new SelectList(_context.EventTypes, "EventTypeId", "TypeName", @event.EventTypeId);

            return View(@event);
        }

        // Getting edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);

            if (@event == null) return NotFound();

            ViewData["EventTypeId"] =
                new SelectList(_context.EventTypes, "EventTypeId", "TypeName", @event.EventTypeId);

            return View(@event);
        }

        // post edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,StartDate,EndDate,Description,ImageUrl,EventTypeId")] Event @event)
        {
            if (id != @event.EventId) return NotFound();

            ModelState.Remove("EventType");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["EventTypeId"] =
                new SelectList(_context.EventTypes, "EventTypeId", "TypeName", @event.EventTypeId);

            return View(@event);
        }

        // Get delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // Psot delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);

            bool hasBookings = await _context.Bookings
                .AnyAsync(b => b.EventId == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] =
                    "This event cannot be deleted because it has active bookings.";

                return RedirectToAction(nameof(Index));
            }

            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}