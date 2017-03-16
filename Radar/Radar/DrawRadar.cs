using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Radar {
    public class DrawRadar {
        /// <summary>
        /// 保存当前图像
        /// </summary>
        private Image img;
        // 图像宽度以及高度
        private int width;
        private int height;
        // 图像缩放比例
        private double rate;

        /// <summary>
        /// 构造一个雷达图像类
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public DrawRadar(int width, int height) {
            this.width = width;
            this.height = height;
            // 长度的一半对应最远距离，防止绘出边界
            this.rate = Math.Min(width, height) / 2.0 / 6000.0;
        }

        private int start = 44;
        private int end = 725;
        public Image drawRadarImg(List<long> data) {
            if (data.Count < end) {
                return null;
            }
            Console.WriteLine("384: " + data[384]);
            data = new RadarZeroFilter().getFilterData(data);
            data = new RadarKalmanFilter().getFilterData(data);
            img = new Bitmap(this.width, this.height);
            Graphics g = Graphics.FromImage(img); int cnt = 0;
            for (int i = start; i < end; ++i) {
                double ang = ((i - start) / 510.0) * Math.PI - Math.PI / 6.0;
                if (data[i] <= 30) {
                    cnt++;
                    data[i] += 4400;
                }
                double x = data[i] * Math.Cos(ang) * rate;
                double y = data[i] * Math.Sin(ang) * rate;
                //Console.WriteLine(x + " " + y);
                if (data[i] >= 4400) {
                    g.DrawRectangle(Pens.Blue, (int)(width / 2 + x), (int)(height / 2 - y), 2, 2);
                }
                else {
                    g.DrawRectangle(Pens.Black, (int)(width / 2 + x), (int)(height / 2 - y), 2, 2);
                }
                
            }
            Console.WriteLine("CNT " + cnt);
            return img;
        }
    }
}
