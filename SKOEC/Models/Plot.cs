using System;
using System.Collections.Generic;

namespace SKOEC.Models
{
    public partial class Plot
    {
        public Plot()
        {
            Treatment = new HashSet<Treatment>();
        }

        public int PlotId { get; set; }
        public int? FarmId { get; set; }
        public int? VarietyId { get; set; }
        public DateTime? DatePlanted { get; set; }
        public DateTime? DateHarvested { get; set; }
        public int? PlantingRate { get; set; }
        public bool PlantingRateByPounds { get; set; }
        public int? RowWidth { get; set; }
        public int? PatternRepeats { get; set; }
        public double? OrganicMatter { get; set; }
        public double? BicarbP { get; set; }
        public double? Potassium { get; set; }
        public double? Magnesium { get; set; }
        public double? Calcium { get; set; }
        public double? PHsoil { get; set; }
        public double? PHbuffer { get; set; }
        public double? Cec { get; set; }
        public double? PercentBaseSaturationK { get; set; }
        public double? PercentBaseSaturationMg { get; set; }
        public double? PercentBaseSaturationCa { get; set; }
        public double? PercentBaseSaturationH { get; set; }
        public string Comments { get; set; }

        public Farm Farm { get; set; }
        public Variety Variety { get; set; }
        public ICollection<Treatment> Treatment { get; set; }
    }
}
