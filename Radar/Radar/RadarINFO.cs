using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radar {
    public class RadarINFO {
        // 激光雷达数据起点、数据终点
        public static int dataStart = 44;
        public static int dataEnd = 725;
        // 激光雷达滤波起点(0°)、滤波终点（180°）
        public static int filtStart = 130;
        public static int filtEnd = 640;
        // 激光雷达测量距离最近点、最远点
        public static int minLim = 20;
        public static int maxLim = 5600;
        // Occupancy Grid Map使用的范围
        public static int showLim = 4000;
        // 数据起始角、滤波起始角
        public static double dataStartAngle = -Math.PI / 6.0;
        public static double filtStartAngle = 0;
        /// <summary>
        /// 180度中有多少测量点
        /// </summary>
        public static int p180 = 510;
    }
}
