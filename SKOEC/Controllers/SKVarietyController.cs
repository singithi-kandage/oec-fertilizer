/* SKVarietyController
 * Controller class that manages varieties of crops
 * 
 * Revision History:
 *      Singithi Kandage, 2017.10.02 : Started
 *      Singithi Kandage, 2017.10.08 : Completed
 * 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SKOEC.Models;
using Microsoft.AspNetCore.Http;

namespace SKOEC.Controllers
{
    public class SKVarietyController : Controller
    {
        private readonly OECContext _context;

        //Constructor for SKVarietyController, initializes OECConext
        public SKVarietyController(OECContext context)
        {
            _context = context;
        }

        // Lists all variety records
        public async Task<IActionResult> Index(Int32? cropId, Int32? varietyId, string name)
        {
            if (cropId != null)
            {
                HttpContext.Session.Remove("varietyId");

                name = (from record in _context.Crop.Where(a => a.CropId == cropId) select record.Name).FirstOrDefault();

                HttpContext.Session.SetString(nameof(cropId), cropId.ToString());
                HttpContext.Session.SetString(nameof(name), name);
            }
            else if (HttpContext.Session.GetString(nameof(cropId)) != null)
            {
                cropId = Convert.ToInt32(HttpContext.Session.GetString(nameof(cropId)));
                name = HttpContext.Session.GetString(nameof(name));
            }
            else if (HttpContext.Session.GetString(nameof(varietyId)) != null)
            {
                varietyId = Convert.ToInt32(HttpContext.Session.GetString(nameof(varietyId)));
                name = HttpContext.Session.GetString(nameof(name));
            }
            else
            {
                TempData["message"] = "Please select a crop before looking for their variety.";
                return Redirect("/SKCrop/Index");
            }

            ViewBag.cropName = name;

            if (HttpContext.Session.GetString(nameof(varietyId)) != null)
            {
                int? cId = (from record in _context.Variety.Where(a => a.VarietyId == varietyId) select record.CropId).FirstOrDefault();
                var vContext = _context.Variety
                        .Where(a => a.CropId == cId)
                        .OrderBy(a => a.Name);


                return View(await vContext.ToListAsync());
            }

            var varietyContext = _context.Variety
                .Where(a => a.CropId == cropId)
                .OrderBy(a => a.Name);

            return View(await varietyContext.ToListAsync());


        }

        // Displays details of selected variety record
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The variety is for a different variety ID than your asked for.");
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);

            if (variety == null)
            {
                ModelState.AddModelError("", "The variety is was not found.");
            }

            return View(variety);
        }

        // Gets form to create new variety record
        public IActionResult Create()
        {
            ViewData["CropId"] = new SelectList(_context.Crop, "CropId", "Name");
            return View();
        }

        // Posts new variety record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VarietyId,CropId,Name")] Variety variety)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(variety);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Variety {variety.Name} created.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception thrown on Create: {ex.GetBaseException().Message}");
            }

            Create();
            return View(variety);
        }

        // Gets view to edit selected variety record
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The variety is for a different variety ID than your asked for.");
            }

            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);

            if (variety == null)
            {
                ModelState.AddModelError("", "The variety is was not found.");
            }

            Create();
            return View(variety);
        }

        // Posts edited variety record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VarietyId,CropId,Name")] Variety variety)
        {
            if (id != variety.VarietyId)
            {
                ModelState.AddModelError("", "The variety is for a different variety ID than your asked for.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(variety);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Variety updated: {variety.Name}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;
                    ModelState.AddModelError("", $"exception updating album: {ex.Message}");
                }
            }

            Create();
            return View(variety);
        }

        // Gets view to delete selected variety record
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The variety is for a different variety ID than your asked for.");
            }

            var variety = await _context.Variety
                .Include(v => v.Crop)
                .SingleOrDefaultAsync(m => m.VarietyId == id);

            if (variety == null)
            {
                ModelState.AddModelError("", "The variety is was not found.");
            }

            return View(variety);
        }

        // Deletes selected variety record
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var variety = await _context.Variety.SingleOrDefaultAsync(m => m.VarietyId == id);
            _context.Variety.Remove(variety);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Returns variety record, based on id
        private bool VarietyExists(int id)
        {
            return _context.Variety.Any(e => e.VarietyId == id);
        }

    }
}
