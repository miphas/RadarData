using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radar {
    public class RadarINFO {
        // 激光雷达数据起点、数据终点
        public static int dataStart = 44;
        public static int dataEnd = 725;
        // 激光雷达滤波起点、滤波终点
        public static int filtStart = 130;
        public static int filtEnd = 640;
        // 激光雷达测量距离最近点、最远点
        public static int minLim = 30;
        public static int maxLim = 5000;
    }
}
