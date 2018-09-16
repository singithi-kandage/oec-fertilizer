/* SKCountryController.cs
 *      Controller class that manages country records, Assignment 3                    
 * 
 * Revision History
 *      Singithi Kandage, 2017.12.10 : Started
 *      Singithi Kandage, 2017.12.18 : Completed
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SKOEC.Models;

namespace SKOEC.Controllers
{
    //Constructor for SKCountryController, initializes OECConext
    public class SKCountryController : Controller
    {
        private readonly OECContext _context;

        public SKCountryController(OECContext context)
        {
            _context = context;
        }

        // Lists all country records, depending on what parameters are passed
        public async Task<IActionResult> Index()
        {
            return View(await _context.Country.ToListAsync());
        }

        // Displays details of selected country record
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The country is for a different countryID than your asked for.");
            }

            var country = await _context.Country
                .SingleOrDefaultAsync(m => m.CountryCode == id);

            if (country == null)
            {
                ModelState.AddModelError("", "The country you asked for does not exist.");
            }

            return View(country);
        }

        // Gets form to create new country record
        public IActionResult Create()
        {
            return View();
        }

        // Posts new country record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CountryCode,Name,PostalPattern,PhonePattern")] Country country)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(country);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Country created: {country.Name}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception thrown on Create: {ex.GetBaseException().Message}");
            }

            Create();
            return View(country);
        }

        // Gets view to edit selected country record
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The country is for a different countryID than your asked for.");
            }

            var country = await _context.Country.SingleOrDefaultAsync(m => m.CountryCode == id);

            if (country == null)
            {
                ModelState.AddModelError("", "The country you asked for does not exist.");
            }

            Create();
            return View(country);
        }

        // Posts edited country record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CountryCode,Name,PostalPattern,PhonePattern")] Country country)
        {
            if (id != country.CountryCode)
            {
                ModelState.AddModelError("", "The country is for a different countryID than your asked for.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Country updated: {country.Name}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;
                    ModelState.AddModelError("", $"Exception occurred while updating plot: {ex.Message}.");
                }
            }

            Create();
            return View(country);
        }

        // Gets view to delete selected country record
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The country is for a different countryID than your asked for.");
            }

            var country = await _context.Country
                .SingleOrDefaultAsync(m => m.CountryCode == id);

            if (country == null)
            {
                ModelState.AddModelError("", "The country you asked for does not exist.");
            }

            return View(country);
        }

        // Deletes selected country record
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            try
            {
                var country = await _context.Country.SingleOrDefaultAsync(m => m.CountryCode == id);
                _context.Country.Remove(country);
                await _context.SaveChangesAsync();
                TempData["message"] = $"Country successfully deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Exception thrown, record not deleted: {ex.GetBaseException().Message}";
                return await Delete(id);
            }
        }

        //Returns country record, based on id
        private bool CountryExists(string id)
        {
            return _context.Country.Any(e => e.CountryCode == id);
        }
    }
}
