using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.Reports.Utilities
{
    public class TableOptions
    {
        public Boolean StripeRows;
        public List<int> InternalRowsForHeadingFormat;
        public double FontSizeCm = 0;

        public TableOptions() { }
    }
}
