/* SKTreatmentFertilizerController.cs
 *      Controller class that manages fertilizers for selected treatments, Assignment 3                    
 * 
 * Revision History
 *      Singithi Kandage, 2017.10.14 : Started
 *      Singithi Kandage, 2017.10.22 : Completed
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
    public class SKTreatmentFertilizerController : Controller
    {
        private readonly OECContext _context;

        //Constructor for SKTreatmentFertilizerController, initializes OECConext
        public SKTreatmentFertilizerController(OECContext context)
        {
            _context = context;
        }

        // Lists all treatment-fertilizer records for selected plot
        public async Task<IActionResult> Index(Int32? treatmentId)
        {
            if (treatmentId != null)
            {
                HttpContext.Session.SetString(nameof(treatmentId), treatmentId.ToString());
            }
            else if (HttpContext.Session.GetString(nameof(treatmentId)) != null)
            {
                treatmentId = Convert.ToInt32(HttpContext.Session.GetString(nameof(treatmentId)));


            }
            else
            {
                TempData["message"] = "Please select a treatment before looking for its fertilizer composition.";
                return Redirect("/SKTreatment/Index");
            }

            var tFertilizer = _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .Where(t => t.TreatmentId == treatmentId);

            return View(await tFertilizer.ToListAsync());
        }

        // Displays details of selected treatment-fertilizer record
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The treatment is for a different treatmentID than your asked for.");
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);

            if (treatmentFertilizer == null)
            {
                ModelState.AddModelError("", "The treatment you asked for does not exist.");
            }

            return View(treatmentFertilizer);
        }

        // Gets form to create new treatment-fertilizer record
        public IActionResult Create()
        {
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(a => a.FertilizerName), "FertilizerName", "FertilizerName");
            ViewData["RateMetric"] = new SelectList(_context.Fertilizer.OrderBy(a => a.FertilizerName), "FertilizerName", "Liquid");
            return View();
        }

        // Posts new treatment-fertilizer record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentFertilizerId,TreatmentId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(treatmentFertilizer);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Fertilizer for treatment created: {treatmentFertilizer.FertilizerName}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception thrown on Create: {ex.GetBaseException().Message}");
            }

            Create();
            return View(treatmentFertilizer);
        }

        // Gets view to edit selected treatment-fertilizer record
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The fertilizer for the treatment is for a different treatmentFertilizerID than your asked for.");
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);

            if (treatmentFertilizer == null)
            {
                ModelState.AddModelError("", "The fertilizer for the treatment you asked for does not exist.");
            }

            Create();
            return View(treatmentFertilizer);
        }

        // Posts edited treatment-fertilizer record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentFertilizerId,TreatmentId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
            if (id != treatmentFertilizer.TreatmentFertilizerId)
            {
                ModelState.AddModelError("", "The fertilizer for the treatment is for a different treatmentFertilizerID than your asked for.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatmentFertilizer);
                    await _context.SaveChangesAsync();
                    treatmentFertilizer.FertilizerName = Request.Form["FertilizerName"];
                    TempData["message"] = $"Fertilizer for treatment updated: {treatmentFertilizer.FertilizerName}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;
                    ModelState.AddModelError("", $"Exception occurred while updating fertilizer for treatment: {ex.Message}.");
                }
            }

            Create();
            return View(treatmentFertilizer);
        }

        // Gets view to delete selected treatment-fertilizer record
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The treatment is for a different treatmentID than your asked for.");
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);

            if (treatmentFertilizer == null)
            {
                ModelState.AddModelError("", "The treatment you asked for does not exist.");
            }

            return View(treatmentFertilizer);
        }

        // Deletes selected treatment-fertilizer record
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            _context.TreatmentFertilizer.Remove(treatmentFertilizer);
            TempData["message"] = $"Fertilizer for treatment deleted.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Returns treatment-fertilizer record, based on id
        private bool TreatmentFertilizerExists(int id)
        {
            return _context.TreatmentFertilizer.Any(e => e.TreatmentFertilizerId == id);
        }

    }
}
