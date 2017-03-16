using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radar {
    interface IRadarFilter {
        List<long> getFilterData(List<long> data);
    }
}
