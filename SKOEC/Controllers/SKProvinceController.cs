/* SKProvinceController.cs
 *      Controller class that manages provinces/states, Assignment 2                 
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
    //Constructor for SKProvinceController, initializes OECConext
    public class SKProvinceController : Controller
    {
        private readonly OECContext _context;

        public SKProvinceController(OECContext context)
        {
            _context = context;
        }

        // Lists all province records, depending on what parameters are passed
        public async Task<IActionResult> Index()
        {
            var oECContext = _context.Province.Include(p => p.CountryCodeNavigation);
            return View(await oECContext.ToListAsync());
        }

        // Displays details of selected province record
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The province is for a different provinceID than your asked for.");
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .SingleOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                ModelState.AddModelError("", "The province does not exist.");
            }

            return View(province);
        }

        // Gets form to create new province record
        public IActionResult Create()
        {
            ViewData["CountryCode"] = new SelectList(_context.Country.OrderBy(c => c.Name), "CountryCode", "Name");
            return View();
        }

        // Posts new province record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProvinceCode,Name,CountryCode")] Province province)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(province);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Province created: {province.Name}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception thrown on Create: {ex.GetBaseException().Message}");
            }


            Create();
            return View(province);
        }

        // Gets view to edit selected province record
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The province is for a different provinceID than your asked for.");
            }

            var province = await _context.Province.SingleOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                ModelState.AddModelError("", "The province does not exist.");
            }

            Create();
            return View(province);
        }

        // Posts edited province record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProvinceCode,Name,CountryCode")] Province province)
        {
            if (id != province.ProvinceCode)
            {
                ModelState.AddModelError("", "The province is for a different provinceID than your asked for."); ;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(province);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Province updated: {province.Name}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;
                    ModelState.AddModelError("", $"Exception occurred while updating plot: {ex.Message}.");
                }
            }

            Create();
            return View(province);
        }

        // Gets view to delete selected province record
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The province is for a different provinceID than your asked for.");
            }

            var province = await _context.Province
                .Include(p => p.CountryCodeNavigation)
                .SingleOrDefaultAsync(m => m.ProvinceCode == id);

            if (province == null)
            {
                ModelState.AddModelError("", "The province does not exist.");
            }

            return View(province);
        }

        // Deletes selected province record
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var province = await _context.Province.SingleOrDefaultAsync(m => m.ProvinceCode == id);
                _context.Province.Remove(province);
                await _context.SaveChangesAsync();
                TempData["message"] = $"Province successfully deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Exception thrown, record not deleted: {ex.GetBaseException().Message}";
                return await Delete(id);
            }
        }

        //Returns province record, based on id
        private bool ProvinceExists(string id)
        {
            return _context.Province.Any(e => e.ProvinceCode == id);
        }
    }
}
