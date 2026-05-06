using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.Services;

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public VenuesController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create (WITH AZURITE UPLOAD)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Upload image to Azurite
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageUrl = await _blobService.UploadFileAsync(imageFile);
                    venue.ImageUrl = imageUrl;
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);

            if (venue != null)
            {
                _context.Venues.Remove(venue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}