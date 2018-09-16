using System;
using System.Collections.Generic;

namespace SKOEC.Models
{
    public partial class Province
    {
        public Province()
        {
            Farm = new HashSet<Farm>();
        }

        public string ProvinceCode { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }

        public Country CountryCodeNavigation { get; set; }
        public ICollection<Farm> Farm { get; set; }
    }
}
