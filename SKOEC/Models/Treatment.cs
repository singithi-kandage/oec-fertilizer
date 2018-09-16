using System;
using System.Collections.Generic;

namespace SKOEC.Models
{
    public partial class Treatment
    {
        public Treatment()
        {
            TreatmentFertilizer = new HashSet<TreatmentFertilizer>();
        }

        public int TreatmentId { get; set; }
        public string Name { get; set; }
        public int PlotId { get; set; }
        public float? Moisture { get; set; }
        public double? Yield { get; set; }
        public double? Weight { get; set; }

        public Plot Plot { get; set; }
        public ICollection<TreatmentFertilizer> TreatmentFertilizer { get; set; }
    }
}
