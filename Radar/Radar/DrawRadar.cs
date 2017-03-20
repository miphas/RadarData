using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Radar {
    public class DrawRadar {
        // 保存当前图像
        private Image img;
        // 图像宽度以及高度
        private int width;
        private int height;
        private int halfWidth;
        private int halfHeight;
        // 图像缩放比例（自动计算）
        private double rate;
        // 附加放大镜比例
        private double enlarge = 1;
        // 画点的大小（PSize设置）
        private int pSize = 2;
        private int pHalfSize = 1;
        // 画点的颜色（PenStyle设置）
        private Pen penStyle = Pens.Black;
        private Color color = Color.Black;
        // 绘图起始数据点、结束数据点
        private int start = RadarINFO.filtStart;
        private int end =   RadarINFO.filtEnd;
        // 画图的起始点和终点
        private double[] angs;
        // 设置字体
        private Font font = new Font("宋体", 9);
        private int fontSize;
        // 设置间隔
        private int interval = 20;
        private String tittle = "激光雷达数据图";

        private String tag = "1m";
        /// <summary>
        /// 设置或获取点的宽度大小
        /// </summary>
        public int PSize {
            get { return pSize; }
            set { pSize = Math.Max(1, value); pHalfSize = pSize / 2; }
        }
        /// <summary>
        /// 设置或获取画笔颜色
        /// </summary>
        public Pen PenStyle {
            get { return penStyle; }
            set { penStyle = value == null ? Pens.Black : null; }
        }
        /// <summary>
        /// 设置或获取放大比例
        /// </summary>
        public double Enlarge {
            get { return enlarge; }
            set { enlarge = Math.Max(0.1, value); }
        }
        /// <summary>
        /// 设置或获取标题
        /// </summary>
        public String Tittle {
            get { return tittle; }
            set { tittle = value; }
        }


        /// <summary>
        /// 构造一个雷达图像类
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public DrawRadar(int width, int height) {
            this.width = width;
            this.height = height;
            this.halfWidth = width / 2;
            this.halfHeight = height / 2;
            // 长度的一半对应最远距离，防止绘出边界
            this.rate = Math.Min(width, height) / 2.0 / RadarINFO.maxLim;
            // 初始化角度数组
            this.angs = new double[RadarINFO.filtEnd];
            for (int i = start; i < end; ++i) {
                angs[i] = ((i - start) / (double)RadarINFO.p180) * Math.PI - RadarINFO.filtStartAngle;
            }
        }

        
        /// <summary>
        /// 绘制激光雷达数据信息图
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Image drawRadarImg(List<long> data) {
            // 数据不足则不进行绘制
            if (data.Count < end) {
                return null;
            }
            data = new RadarZeroFilter().getFilterData(data);
            data = new RadarKalmanFilter().getFilterData(data);
            img = new Bitmap(this.width, this.height);
            Graphics g = Graphics.FromImage(img);
            for (int i = start; i < end; ++i) {
                double x = data[i] * Math.Cos(angs[i]) * rate * enlarge;
                double y = data[i] * Math.Sin(angs[i]) * rate * enlarge;
                g.FillRectangle(new SolidBrush(color), 
                                (int)(halfWidth + x - pHalfSize), 
                                (int)(halfHeight - (y - pHalfSize)), 
                                pSize, pSize);
            }
            // 一米长
            int meterLen = (int)(1000 * this.rate * enlarge);
            int posH = this.height - interval;
            int posW = this.width - interval;
            g.DrawString("1m", font, Brushes.Black, posW - g.MeasureString(tag, font).Width / 2 - meterLen / 2, posH - interval);
            g.DrawLine(penStyle, posW - meterLen, posH, posW, posH);
            // 图片标题
            g.DrawString(tittle, font, Brushes.Black, halfWidth - g.MeasureString(tittle, font).Width / 2, interval);
            return img;
        }
    }
}
