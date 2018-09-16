/* SKPlotController.cs
 *      Controller class that manages plots, Assignment 2                    
 * 
 * Revision History
 *      Singithi Kandage, 2017.10.02 : Started
 *      Singithi Kandage, 2017.10.08 : Completed
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
    public class SKPlotController : Controller
    {
        private readonly OECContext _context;

        //Constructor for SKPlotController, initializes OECConext
        public SKPlotController(OECContext context)
        {
            _context = context;
        }

        // Lists all plot records, depending on what parameters are passed
        public async Task<IActionResult> Index(Int32? cropId, Int32? varietyId, Int32? plotId, string name = "", string filter = "", string farmName = "")
        {

            if (cropId != null)
            {
                HttpContext.Session.Remove("varietyId");
                HttpContext.Session.Remove("name");
                HttpContext.Session.Remove("plotId");

                name = (from record in _context.Crop.Where(a => a.CropId == cropId) select record.Name).FirstOrDefault();

                HttpContext.Session.SetString(nameof(cropId), cropId.ToString());
                HttpContext.Session.SetString(nameof(name), name.ToString());
                HttpContext.Session.SetString(nameof(filter), filter.ToString());
            }
            else if (varietyId != null)
            {
                HttpContext.Session.Remove("cropId");
                HttpContext.Session.Remove("name");
                HttpContext.Session.Remove("plotId");

                name = (from record in _context.Variety.Where(a => a.VarietyId == varietyId) select record.Name).FirstOrDefault();

                HttpContext.Session.SetString(nameof(varietyId), varietyId.ToString());
                HttpContext.Session.SetString(nameof(name), name.ToString());
                HttpContext.Session.SetString(nameof(filter), filter.ToString());
            }
            else if (plotId != null)
            {
                HttpContext.Session.SetString(nameof(plotId), plotId.ToString());
            }
            else if (HttpContext.Session.GetString(nameof(cropId)) != null)
            {
                cropId = Convert.ToInt32(HttpContext.Session.GetString(nameof(cropId)));
                name = HttpContext.Session.GetString(nameof(name));

                if (filter != "")
                {
                    HttpContext.Session.SetString(nameof(filter), filter.ToString());
                }
                else
                {
                    filter = HttpContext.Session.GetString(nameof(filter));
                }
            }
            else if (HttpContext.Session.GetString(nameof(varietyId)) != null)
            {
                varietyId = Convert.ToInt32(HttpContext.Session.GetString(nameof(varietyId)));
                name = HttpContext.Session.GetString(nameof(name));

                if (filter != "")
                {
                    HttpContext.Session.SetString(nameof(filter), filter.ToString());
                }
                else
                {
                    filter = HttpContext.Session.GetString(nameof(filter));
                }
            }
            else if (HttpContext.Session.GetString(nameof(plotId)) != null)
            {
                plotId = Convert.ToInt32(HttpContext.Session.GetString(nameof(plotId)));
            }

            var plots = from record in _context.Plot
                          .Include(a => a.Farm)
                          .Include(a => a.Variety)
                          .ThenInclude(a => a.Crop)
                          .Include(a => a.Treatment) 
                        select new PlotViewModel
                            {
                                PlotId = record.PlotId,
                                Farm = record.Farm.Name,
                                Crop = record.Variety.Crop.Name,
                                Variety = record.Variety.Name,
                                DatePlanted = record.DatePlanted,
                                Cec = record.Cec,
                                Treatment = record.Treatment,
                            };


            if (plotId != null || HttpContext.Session.GetString(nameof(plotId)) != null)
            {
                return View(await plots.Where(p => p.PlotId == plotId).ToListAsync());
            }

            string filterName;

            if (cropId != null || HttpContext.Session.GetString(nameof(cropId)) != null)
            {
                filterName = HttpContext.Session.GetString(nameof(filter));

                if (filterName == "DatePlanted" || filterName == null)
                {
                    return View(await plots.Where(a => a.Crop == name)
                   .OrderByDescending(a => a.DatePlanted).ToListAsync());
                }

                if (filterName == "Farm")
                {
                    return View(await plots.Where(a => a.Crop == name)
                    .OrderBy(a => a.Farm).ToListAsync());
                }

                if (filterName == "Variety")
                {
                    return View(await plots.Where(a => a.Crop == name)
                    .OrderBy(a => a.Variety).ToListAsync());
                }

                if (filterName == "Cec")
                {
                    return View(await plots.Where(a => a.Crop == name)
                    .OrderBy(a => a.Cec).ToListAsync());
                }
               
            }
            else if (varietyId != null || HttpContext.Session.GetString(nameof(varietyId)) != null)
            {
                filterName = HttpContext.Session.GetString(nameof(filter));

                if (filterName == "DatePlanted")
                {
                    return View(await plots.Where(a => a.Variety == name)
                    .OrderByDescending(a => a.DatePlanted).ToListAsync());
                }
                else if (filterName == "Farm")
                {
                    return View(await plots.Where(a => a.Variety == name)
                    .OrderByDescending(a => a.Farm).ToListAsync());
                }
                else if (filterName == "Variety")
                {
                    return View(await plots.Where(a => a.Variety == name)
                    .OrderByDescending(a => a.Variety).ToListAsync());
                }
                else if (filterName == "Cec")
                {
                    return View(await plots.Where(a => a.Variety == name)
                    .OrderByDescending(a => a.Cec).ToListAsync());
                }      
            }
            else
            {
                filterName = HttpContext.Session.GetString(nameof(filter));

                if (filterName == "DatePlanted")
                {
                    return View(await plots.OrderByDescending(a => a.DatePlanted).ToListAsync());
                }
                else if (filterName == "Farm")
                {
                    return View(await plots.OrderBy(a => a.Farm).ToListAsync());
                }
                else if (filterName == "Variety")
                {
                    return View(await plots.OrderBy(a => a.Variety).ToListAsync());
                }
                else if (filterName == "Cec")
                {
                    return View(await plots.OrderBy(a => a.Cec).ToListAsync());
                }
            }

                return View(await plots.OrderByDescending(a => a.DatePlanted).ToListAsync());
        }

        // Displays details of selected plot record
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The plot is for a different plotID than your asked for.");
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);

            if (plot == null)
            {
                ModelState.AddModelError("", "The plot you asked for does not exist.");
            }

            return View(plot);
        }

        // Gets form to create new plot record
        public IActionResult Create()
        {
            int cropId;
            int varietyId;

            if (HttpContext.Session.GetString(nameof(cropId)) != null)
            {
                cropId = Convert.ToInt32(HttpContext.Session.GetString(nameof(cropId)));

                ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(a => a.Name), "FarmId", "Name");
                ViewData["VarietyId"] = new SelectList(_context.Variety
                    .Where(a => a.CropId == cropId)
                    .OrderBy(a => a.Name)
                    , "VarietyId", "Name");
            }
            else if (HttpContext.Session.GetString(nameof(varietyId)) != null)
            {
                varietyId = Convert.ToInt32(HttpContext.Session.GetString(nameof(varietyId)));

                ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(a => a.Name), "FarmId", "Name");
                ViewData["VarietyId"] = new SelectList(_context.Variety
                    .Where(a => a.VarietyId == varietyId)
                    .OrderBy(a => a.Name)
                    , "VarietyId", "Name"); 
            }

            return View();
        }

        // Posts new plot record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(plot);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Plot created: {plot.Farm}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception thrown on Create: {ex.GetBaseException().Message}");
            }

            Create();
            return View(plot);
        }

        // Gets view to edit selected plot record
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The plot is for a different plotID than your asked for.");
            }

            var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);

            if (plot == null)
            {
                ModelState.AddModelError("", "The plot does not exist.");
            }

            Create();
            return View(plot);
        }

        // Posts edited plot record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (id != plot.PlotId)
            {
                ModelState.AddModelError("", "The plot is for a different plotID than your asked for.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plot);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Plot updated: {plot.Farm}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null) ex = ex.InnerException;
                    ModelState.AddModelError("", $"Exception occurred while updating plot: {ex.Message}.");
                }
            }

            Create();
            return View(plot);
        }

        // Gets view to delete selected plot record
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "The plot is for a different plotID than your asked for.");
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);

            if (plot == null)
            {
                ModelState.AddModelError("", "The plot does not exist.");
            }

            return View(plot);
        }

        // Deletes selected plot record
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);
                _context.Plot.Remove(plot);
                await _context.SaveChangesAsync();
                TempData["message"] = $"Plot deleted: {plot.Farm}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Exception thrown, record not deleted: {ex.GetBaseException().Message}";
                return await Delete(id);
            }
        }

        //Returns plot record, based on id
        private bool PlotExists(int id)
        {
            return _context.Plot.Any(e => e.PlotId == id);
        }
     
    }
}
