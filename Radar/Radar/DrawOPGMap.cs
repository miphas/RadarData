using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Radar {
    public class DrawOPGMap {
        /// <summary>
        /// 定义图像宽高
        /// </summary>
        private int width;
        private int height;
        private Bitmap img;

        private Color colOcc = Color.Black;
        private Color colUnn = Color.Gray;
        private Color colFee = Color.White;
        public DrawOPGMap(int width, int height) {
            this.width = width;
            this.height = height;
            img = new Bitmap(width, height);
        }
        /// <summary>
        /// 画网格地图的变化情况
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public Image drawOPGMapChange(bool[][] map) {
            Graphics g = Graphics.FromImage(img);
            for (int i = 0; i < map.Length; ++i) {
                for (int j = 0; j < map[i].Length; ++j) {
                    img.SetPixel(i, j, map[i][j] ? colFee : colOcc);
                }
            }
            return img;
        }

        /// <summary>
        /// 画整个地图
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public Image drawOPGMap(double[][] map) {
            Graphics g = Graphics.FromImage(img);
            for (int i = 0; i < map.Length; ++i) {
                for (int j = 0; j < map[i].Length; ++j) {
                    img.SetPixel(i, j, (map[i][j] == 0.5) ? colUnn : ((map[i][j] > 0.5) ? colOcc : colFee));
                }
            }
            return img;
        }
        /// <summary>
        /// 更新改变的地图
        /// </summary>
        /// <param name="map"></param>
        /// <param name="change"></param>
        /// <returns></returns>
        public Image drawOPGMapFast(double[][] map, bool[][] change) {
            Graphics g = Graphics.FromImage(img);
            for (int i = 0; i < map.Length; ++i) {
                for (int j = 0; j < map[i].Length; ++j) {
                    if (!change[i][j]) {
                        continue;
                    }
                    img.SetPixel(i, this.height - j - 1, (map[i][j] == 0.5) ? colUnn : ((map[i][j] > 0.5) ? colOcc : colFee));
                }
            }
            return img;
        }


    }
}
