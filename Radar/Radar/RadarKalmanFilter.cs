using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radar {
    public class RadarKalmanFilter : IRadarFilter {
        // 起始滤波点和结束滤波点
        public static int start = RadarINFO.filtStart;
        public static int end   = RadarINFO.filtEnd;

        /// <summary>
        /// 获取滤波后的结果信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int cnt = 0;
        private KalmanSingle[] kalmanData;
        public List<long> getFilterData(List<long> data) {
            // 处理特殊情况
            if (data == null || data.Count < end) {
                return data;
            }
            // 创建卡尔曼滤波数组
            if (cnt == 0 && kalmanData == null) {
                kalmanData = new KalmanSingle[data.Count];
                // 初始化为第一次得到的值
                for (int i = 0; i < data.Count; ++i) {
                    kalmanData[i] = new KalmanSingle(data[i]);
                }
                return data;
            }
            // 进行滤波操作
            for (int i = start; i < end; ++i) {
                data[i] = kalmanData[i].getRealValue((int)data[i]);
            }
            return data;
        }


    }
}
