/* SKTreatmentController.cs
 *      Controller class that manages treatments, Assignment 3                    
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
    public class SKTreatmentController : Controller
    {
        private readonly OECContext _context;

        //Constructor for SKTreatmentController, initializes OECConext
        public SKTreatmentController(OECContext context)
        {
            _context = context;
        }

        // Lists all treatment records for selected plot
        public async Task<IActionResult> Index(Int32? plotId, string farmName = "")
        {
            if (plotId != null)
            {
                HttpContext.Session.SetString(nameof(plotId), plotId.ToString());
                HttpContext.Session.SetString(nameof(farmName), farmName.ToString());
            }
            else if (HttpContext.Session.GetString(nameof(plotId)) != null)
            {
                plotId = Convert.ToInt32(HttpContext.Session.GetString(nameof(plotId)));
                farmName = HttpContext.Session.GetString(nameof(farmName));

                var updatedTreatments = await _context.Treatment
                            .Include(t => t.TreatmentFertilizer)
                            .ThenInclude(t => t.FertilizerNameNavigation)
                            .Where(t => t.PlotId == plotId)
                            .ToListAsync();

                foreach (var treatment in updatedTreatments)
                {
                    string treatmentName = "";
                    int counter = 1;

                    var treatmentFertilizers = await _context.TreatmentFertilizer
                                                     .Where(tf => tf.TreatmentId == treatment.TreatmentId)
                                                     .ToListAsync();

                    foreach (var tf in treatmentFertilizers.OrderBy(t => t.FertilizerName))
                    {
                        treatmentName += tf.FertilizerName;

                        if (counter < treatmentFertilizers.Count())
                        {
                            treatmentName += " + ";
                        }
                        counter++;
                    }

                    if (treatmentFertilizers.Count() == 0)
                    {
                        treatmentName = "no fertilizer";
                    }

                    treatment.Name = treatmentName;

                    await Edit(treatment.TreatmentId, treatment);
                }

                return View(updatedTreatments.OrderBy(t => t.Name));

            }
            else
            {
                TempData["message"] = "Please select a plot before looking for a treatment.";
                return Redirect("/SKPlot/Index");
            }

            var treatments = await _context.Treatment
               .Where(t => t.PlotId == plotId)
               .ToListAsync();

            return View(treatments.OrderBy(t => t.Name));
        }

        // Displays details of selected treatment record
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The treatment is for a different treatmentID than your asked for.");
            }

            var treatment = await _context.Treatment
                .Include(t => t.Plot)
                .SingleOrDefaultAsync(m => m.TreatmentId == id);

            if (treatment == null)
            {
                ModelState.AddModelError("", "The treatment you asked for does not exist.");
            }

            return View(treatment);
        }

        // Gets form to create new treatment record
        public IActionResult Create()
        {
            return View();
        }

        // Posts new treatment record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentId,Name,PlotId,Moisture,Yield,Weight")] Treatment treatment)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if(treatment.Name == "")
                    {
                        treatment.Name = "no fertilizer";
                    }

                    _context.Add(treatment);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Treatment created: {treatment.Name}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception thrown on Create: {ex.GetBaseException().Message}");
            }

            Create();
            return View(treatment);
        }

        // Gets view to edit selected treatment record
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The treatment is for a different treatmentID than your asked for.");
            }

            var treatment = await _context.Treatment.SingleOrDefaultAsync(m => m.TreatmentId == id);          

            if (treatment == null)
            {
                ModelState.AddModelError("", "The treatment you asked for does not exist.");
            }

            return View(treatment);
        }

        // Posts edited treatment record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentId,Name,PlotId,Moisture,Yield,Weight")] Treatment treatment)
        {
            if (id != treatment.TreatmentId)
            {
                ModelState.AddModelError("", "The treatment is for a different treatmentID than your asked for.");
            }

            if (ModelState.IsValid)
            {

                try
                {
                    _context.Update(treatment);
                    await _context.SaveChangesAsync();
                    treatment.Name = Request.Form["Name"];
                    TempData["message"] = $"Treatment updated: {treatment.Name}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;
                    ModelState.AddModelError("", $"Exception occurred while updating treatment: {ex.Message}.");
                }
            }

            return View(treatment);
        }

        // Gets view to delete selected treatment record
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The treatment is for a different treatmentID than your asked for.");
            }

            var treatment = await _context.Treatment
                .Include(t => t.Plot)
                .SingleOrDefaultAsync(m => m.TreatmentId == id);

            if (treatment == null)
            {
                ModelState.AddModelError("", "The treatment you asked for does not exist.");
            }

            return View(treatment);
        }

        // Deletes selected treatment record
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatment = await _context.Treatment.SingleOrDefaultAsync(m => m.TreatmentId == id);
            TempData["message"] = $"Treatment deleted.";
            _context.Treatment.Remove(treatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Returns treatment record, based on id
        private bool TreatmentExists(int id)
        {
            return _context.Treatment.Any(e => e.TreatmentId == id);
        }
    }
}
