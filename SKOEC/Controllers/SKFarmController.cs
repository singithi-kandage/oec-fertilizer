/* SKFarmController.cs
 *      Controller class that manages farms, Assignment 2                 
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
    //Constructor for SKFarmController, initializes OECConext
    public class SKFarmController : Controller
    {
        private readonly OECContext _context;

        public SKFarmController(OECContext context)
        {
            _context = context;
        }

        // Lists all farm records, depending on what parameters are passed
        public async Task<IActionResult> Index()
        {
            var oECContext = _context.Farm.Include(f => f.ProvinceCodeNavigation).OrderBy(f => f.Name);
            return View(await oECContext.ToListAsync());
        }

        // Displays details of selected farm record
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The farm is for a different farmID than your asked for.");
            }

            var farm = await _context.Farm
                .Include(f => f.ProvinceCodeNavigation)
                .SingleOrDefaultAsync(m => m.FarmId == id);

            if (farm == null)
            {
                ModelState.AddModelError("", "The farm you asked for does not exist.");
            }

            return View(farm);
        }

        // Gets form to create new farm record
        public IActionResult Create()
        {
            return View();
        }

        // Posts new farm record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FarmId,Name,Address,Town,County,ProvinceCode,PostalCode,HomePhone,CellPhone,Email,Directions,DateJoined,LastContactDate")] Farm farm)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _context.Add(farm);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Farm created: {farm.Name}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {

                while (ex.InnerException != null) ex = ex.InnerException;
                ModelState.AddModelError("", $"Exception thrown on Create: {ex.Message}");
            }

            Create();
            return View(farm);
        }

        // Gets view to edit selected farm record
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The farm is for a different farmID than your asked for.");
            }

            var farm = await _context.Farm.SingleOrDefaultAsync(m => m.FarmId == id);

            if (farm == null)
            {
                ModelState.AddModelError("", "The farm you asked for does not exist.");
            }
           
            ViewData["Province"] = new SelectList(_context.Province.OrderBy(p => p.Name), "ProvinceCode", "Name", farm.FarmId);
            return View(farm);
        }

        // Posts edited farm record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FarmId,Name,Address,Town,County,ProvinceCode,PostalCode,HomePhone,CellPhone,Email,Directions,DateJoined,LastContactDate")] Farm farm)
        {
            if (id != farm.FarmId)
            {
                ModelState.AddModelError("", "The farm is for a different farmID than your asked for.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farm);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Farm updated: {farm.Name}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;
                    ModelState.AddModelError("", $"Exception occurred while updating farm: {ex.Message}.");
                }
            }

            await Edit(id);
            return View(farm);
        }

        // Gets view to delete selected farm record
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The farm is for a different farmID than your asked for.");
            }

            var farm = await _context.Farm
                .Include(f => f.ProvinceCodeNavigation)
                .SingleOrDefaultAsync(m => m.FarmId == id);

            if (farm == null)
            {
                ModelState.AddModelError("", "The farm you asked for does not exist.");
            }

            return View(farm);
        }

        // Deletes selected farm record
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var farm = await _context.Farm.SingleOrDefaultAsync(m => m.FarmId == id);
                _context.Farm.Remove(farm);
                await _context.SaveChangesAsync();
                TempData["message"] = "Farm successfully deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["message"] = $"exception no delete: {ex.GetBaseException().Message}";
                return await Delete(id);
            }

            
        }

        //Returns farm record, based on id
        private bool FarmExists(int id)
        {
            return _context.Farm.Any(e => e.FarmId == id);
        }
    }
}
