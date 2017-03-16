using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radar {
    public class RadarZeroFilter : IRadarFilter {

        // 起始滤波点和结束滤波点
        public int start = RadarINFO.filtStart;
        public int end   = RadarINFO.filtEnd;
        // 门限值
        public int minLim = RadarINFO.minLim;
        public int maxLim = RadarINFO.maxLim;
        public int points = 5;

        /// <summary>
        /// 获取滤波后的结果信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<long> getFilterData(List<long> data) {
            // 处理特殊情况
            if (data == null || data.Count < end) {
                return data;
            }
            // 寻找第一个符合条件的点
            int first = start;
            for (; first < end; ++first) {
                if (data[first] <= minLim) {
                    break;
                }
            }
            // 没有合适数据则返回
            if (first == end) {
                return data;
            }
            // 处理编号大的节点
            for (int i = first; i < end; ++i) {
                if (data[i] <= minLim) {
                    // data[i] = data[i - 1];
                    int len = zeroLen(data, i, true);
                    if (len > points) {
                        dealLargeRange(data, i, len, true);
                    }
                    else {
                        dealSmallRange(data, i, len, true);
                    }
                    i += len - 1;
                }
            }
            // 处理编号小的节点
            for (int i = first; i >= start; --i) {
                if (data[i] <= minLim) {
                    // data[i] = data[i - 1];
                    int len = zeroLen(data, i, false);
                    if (len > points) {
                        dealLargeRange(data, i, len, false);
                    }
                    else {
                        dealSmallRange(data, i, len, false);
                    }
                    i += len - 1;
                }
            }
            return data;
        }
        /// <summary>
        /// 范围内0的个数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pos"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        private int zeroLen(List<long> data, int pos, bool add) {
            int cnt = 1;
            if (add) {
                for (; pos + cnt < end && data[pos + cnt] <= minLim; ++cnt) ;
            }
            else {
                for (; pos - cnt > start && data[pos - cnt] <= minLim; ++cnt) ;
            }
            return cnt;
        }
        /// <summary>
        /// 处理小范围内都是0的情况
        /// data = 附近点的值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pos"></param>
        /// <param name="cnt"></param>
        /// <param name="add"></param>
        private void dealSmallRange(List<long> data, int pos, int cnt, bool add) {
            if (add) {
                for (int i = pos; i < pos + cnt; ++i) {
                    data[i] = data[i - 1];
                }
            }
            else {
                for (int i = pos; i > pos - cnt; --i) {
                    data[i] = data[i + 1];
                }
            }
        }
        /// <summary>
        /// 处理大范围内都是0的情况
        /// data = 最大
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pos"></param>
        /// <param name="cnt"></param>
        /// <param name="add"></param>
        private void dealLargeRange(List<long> data, int pos, int cnt, bool add) {
            if (add) {
                for (int i = pos; i < pos + cnt; ++i) {
                    data[i] = maxLim;
                }
            }
            else {
                for (int i = pos; i > pos - cnt; --i) {
                    data[i] = maxLim;
                }
            }
        }



    }
}
