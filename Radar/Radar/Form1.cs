using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radar {
    public partial class Form1 : Form {

        private DrawOPGMap opgMap = new DrawOPGMap(800, 800);
        private RadarOPGMap ropg = new RadarOPGMap(800, 800, 400);
        private RadarZeroFilter zeroFilter = new RadarZeroFilter();
        private RadarKalmanFilter kalmFilter = new RadarKalmanFilter();
        public Form1() {
            InitializeComponent();

            UrgPort.processDel = new UrgPort.processData(processMethod);
            updateBoardDel = new updateBoard(update);


        }
        private UrgPort urg = UrgPort.getInstance("COM5");

        public delegate void updateBoard(String text);
        public updateBoard updateBoardDel;
        int cnt = 0;


        int drawCnt = 0;
        private DrawRadar urgImg = new DrawRadar(450, 450);
        private void update(String text) {
            this.label1.Text = text;
            //Console.WriteLine(++cnt + " " + text);

            List<long> l1 = UrgPort.getDis();
            List<long> l2 = zeroFilter.getFilterData(l1);
            List<long> l3 = kalmFilter.getFilterData(l2);
            this.pictureBox1.Image = urgImg.drawRadarImg(l3);

            //
            drawCnt++;
            if (drawCnt == 20) {
                drawCnt = 0;

                Smart_Car.Point p = new Smart_Car.Point();
                if (drport != null) {
                    p = drport.getPoint();
                    Console.WriteLine(p.x + " " + p.y);
                }
                
                ropg.updateMap((int)(p.x * 100), (int)(p.y * 100), p.w, l3);
                this.pictureBox2.Image = opgMap.drawOPGMapFast(ropg.PriMap, ropg.BinMap);
            }
        }

        public void processMethod(List<long> list) {
            this.Invoke(updateBoardDel, list.Count.ToString());
        }

        private Smart_Car.DrPort drport;
        private void button1_Click(object sender, EventArgs e) {
            if (drport == null) {
                drport = new Smart_Car.DrPort();
                Console.WriteLine(drport.OpenPort("COM4", "115200"));
            }
            if (!urg.isOpen) {
                urg.openPort();
                Console.WriteLine("open");
            }
            else {
                urg.closePort();
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            this.pictureBox2.Image.Save("D:\\map.png", System.Drawing.Imaging.ImageFormat.Png);
        }

    }
}
