using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SKOEC.Models
{
    public class PlotViewModel
    {
        public PlotViewModel()
        {
            Treatment = new HashSet<Treatment>();
        }

        public int PlotId { get; set; }
        public string Farm { get; set; }
        public string Crop { get; set; }
        public string Variety { get; set; }
        public DateTime? DatePlanted { get; set; }
        public double? Cec { get; set; }

        public ICollection<Treatment> Treatment { get; set; }

    }
}
