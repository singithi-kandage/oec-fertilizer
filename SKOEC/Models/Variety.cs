using System;
using System.Collections.Generic;

namespace SKOEC.Models
{
    public partial class Variety
    {
        public Variety()
        {
            Plot = new HashSet<Plot>();
        }

        public int VarietyId { get; set; }
        public int? CropId { get; set; }
        public string Name { get; set; }

        public Crop Crop { get; set; }
        public ICollection<Plot> Plot { get; set; }
    }
}
