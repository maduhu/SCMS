using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Model
{
    public partial class CountryProgramme
    {
        public Location FirstLocation { get; set; }

        public CountrySubOffice FirstSubOffice { get; set; }

        public Designation FirstDesignation { get; set; }
    }
}
