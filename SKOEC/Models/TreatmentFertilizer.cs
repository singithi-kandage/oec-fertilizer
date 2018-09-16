using System;
using System.Collections.Generic;

namespace SKOEC.Models
{
    public partial class TreatmentFertilizer
    {
        public int TreatmentFertilizerId { get; set; }
        public int? TreatmentId { get; set; }
        public string FertilizerName { get; set; }
        public double? RatePerAcre { get; set; }
        public string RateMetric { get; set; }

        public Fertilizer FertilizerNameNavigation { get; set; }
        public Treatment Treatment { get; set; }
    }
}
