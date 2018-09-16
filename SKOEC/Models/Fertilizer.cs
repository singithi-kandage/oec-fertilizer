using System;
using System.Collections.Generic;

namespace SKOEC.Models
{
    public partial class Fertilizer
    {
        public Fertilizer()
        {
            TreatmentFertilizer = new HashSet<TreatmentFertilizer>();
        }

        public string FertilizerName { get; set; }
        public bool Oecproduct { get; set; }
        public bool Liquid { get; set; }

        public ICollection<TreatmentFertilizer> TreatmentFertilizer { get; set; }
    }
}
