using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Country
    {
        public int CountryId { get; set; }

        public string Name { get; set; }

        public string Continent { get; set; }

        public bool Important { get; set; }

        public string Iso { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
