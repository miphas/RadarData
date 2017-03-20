using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radar {
    public class RadarOPGMap {
        // 图像的宽度和高度
        private int width;
        private int height;
        // 其实坐标X（对应width） 和 Y（对应height）
        private int startX;
        private int startY;
        // 全局地图信息、全局地图true/false形式
        // 第一维为X，第二维为Y
        private double[][] map;
        private double[][] priMap;
        private bool[][] binMap;
        private double[][] curMap;
        // 0度起始位置、180度包含点的数量
        private int zerop = RadarINFO.filtStart;
        private int points = RadarINFO.p180;
        // 设置每个点所占的范围
        private double[] angs;
        // 每个像素归属角度点 长度值
        private int[][] corrAng;
        private int[][] corrLen;
        // 计算四米以内的点（一个单位显示1cm X 1cm）
        private int radarRange = RadarINFO.showLim;
        private int showRange = 400;
        // 可信范围(mm)
        private int showAlpha = 10;
        // p(mi)取0.5即未知区域是否为障碍概率为0.5
        private double L0 = 0;//Math.Log(1);
        private double Locc = 2.2;// Math.Log(9);
        private double Lfree = -2.2;//Math.Log(0.11);

        private Object priMapLock = new object();
        private Object binMapLock = new object();
        public bool[][] BinMap {
            get {
                lock (binMapLock) {
                    return binMap;
                }
            }
            set {
                lock (binMapLock) {
                    binMap = value;
                }
            }
        }

        public double[][] PriMap {
            get {
                lock (priMapLock) {
                    return priMap;
                }
            }
            set {
                lock (priMapLock) {
                    priMap = value;
                }
            }
        }

        /// <summary>
        /// Occupancy Grid Map
        /// </summary>
        public RadarOPGMap(int width, int height, int startX, int startY = 0) {
            // 创建对应大小的矩阵值
            this.width = width;
            this.height = height;

            map = new double[width][];
            priMap = new double[width][];
            binMap = new bool[width][];
            for (int i = 0; i < width; ++i) {
                map[i] = new double[height];
                priMap[i] = new double[height];
                binMap[i] = new bool[height];
            }
            // 赋初始值
            this.startX = startX;
            this.startY = startY;

            // 当前显示地图对应大小矩阵
            // 像素点对应角度、距离
            curMap = new double[showRange * 2][];
            corrAng = new int[showRange * 2][];
            corrLen = new int[showRange * 2][];
            for (int i = 0; i < curMap.Length; ++i) {
                curMap[i] = new double[showRange];
                corrAng[i] = new int[showRange];
                corrLen[i] = new int[showRange];
            }
            
            // 初始化角度信息
            angs = new double[points];
            for (int i = 0; i < points; ++i) {
                angs[i] = ((i + 1) / (double)points) * Math.PI;
            }
            // 计算每个点对应角度
            int showWidth = 2 * showRange;
            for (int i = 0; i < showWidth; ++i) {
                for (int j = 0; j < showRange; ++j) {
                    double tmp = Math.Atan2(j, i - showRange) / Math.PI * points;
                    // 取整得到对应角度(取showRange-1防止数组越界)
                    corrAng[i][j] = Math.Min((int)tmp, points - 1);
                }
            } Console.WriteLine(corrAng[0][0]);
        }

        public double[][] updateMap(int offX, int offY, double offW, List<long> dis) {
            if (updateCurMap(dis)) {
                updateGMapFromCurMap(offX, offY, offW);
            }
            return PriMap;
        }

        /// <summary>
        /// 更新当前程序
        /// </summary>
        /// <param name="dis"></param>
        private bool updateCurMap(List<long> dis) {
            // 不符合条件则返回
            if (dis == null || dis.Count < zerop + points) {
                return false;
            }
            // 更新curMap
            int curWidth = 2 * showRange;
            for (int i = 0; i < curWidth; ++i) {
                for (int j = 0; j < showRange; ++j) {
                    // 取得对应角度
                    int ang = corrAng[i][j];
                    int len = (int)Math.Sqrt((i - showRange) * (i - showRange) + j * j) * 10;
                    corrLen[i][j] = len;
                    // 超出距离范围
                    if (radarRange < len) {
                        continue;
                    }
                    // 在边界附近 
                    else if (Math.Abs(len - dis[zerop + ang]) <= showAlpha) {
                        curMap[i][j] = Locc - L0;
                    }
                    // 在可测距离以内
                    else if (len < dis[zerop + ang] - showAlpha) {
                        curMap[i][j] = Lfree - L0;
                    }
                    else {
                        curMap[i][j] = Locc - L0;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 从当前地图更新全局地图(之前需要调用updateCurMap更新corrLen信息)
        /// </summary>
        /// <param name="offX"></param>
        /// <param name="offY"></param>
        /// <param name="offW"></param>
        /// <returns></returns>
        private bool updateGMapFromCurMap(int offX, int offY, double offW) {
            for (int i = 0; i < BinMap.Length; ++i) {
                for (int j = 0; j < BinMap[i].Length; ++j) {
                    BinMap[i][j] = false;
                }
            }
            for (int i = 0; i < curMap.Length; ++i) {
                for (int j = 0; j < curMap[i].Length; ++j) {
                    // 计算当前点对应激光雷达原点偏移角
                    double angle = angs[corrAng[i][j]];
                    // 计算全局地图中的位置
                    int realX = offX + (int)(Math.Cos(angle + offW) * corrLen[i][j] / 10) + startX;
                    int realY = offY + (int)(Math.Sin(angle + offW) * corrLen[i][j] / 10) + startY;
                    // 更新全局地图
                    if (0 <= realX && realX < width &&
                        0 <= realY && realY < height) {
                        map[realX][realY] = curMap[i][j];
                        double val = 1 - 1 / (1.0 + Math.Exp(map[realX][realY]));
                        // 变化设置为true,否则为false
                        BinMap[realX][realY] = true;
                        PriMap[realX][realY] = val;
                    }
                }
            }
            return true;
        }


    }
}
