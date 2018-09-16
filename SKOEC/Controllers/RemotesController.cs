using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SKOEC.Models;
using System.Text.RegularExpressions;

namespace SKOEC.Controllers
{
    public class RemotesController : Controller
    {
        private readonly OECContext _context;
        public RemotesController(OECContext context)
        {
            _context = context;
        }
        public JsonResult ValidateProvinceCode(string provinceCode)
        {
            //Checks if province code contains digits
            if (Regex.IsMatch(provinceCode, @"^[a-zA-Z]+$") == false)
            {
                return Json("Please enter letters only.");
            }
            else
            {
                //If province code contains contains only letters, check if it is 2 letters long
                if (provinceCode.Length != 2)
                {
                    return Json("Province Code must be 2 letters long");
                }

             
                try
                {
                    var province = _context.Province.SingleOrDefault(a => a.ProvinceCode == provinceCode.ToUpper());

                    if (province == null)
                    {
                        return Json("Province Code is not on file, please enter an existing Province Code.");
                    }
                    
                }
                catch (Exception ex)
                {
                    return Json($"Exception thrown on validating Province Code: {ex.GetBaseException().Message}");
                }
            }
    
            return Json(true);
        }
    }
}