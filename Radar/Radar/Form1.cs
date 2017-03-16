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
        public Form1() {
            InitializeComponent();

            UrgPort.processDel = new UrgPort.processData(processMethod);
            updateBoardDel = new updateBoard(update);
        }
        private UrgPort urg = UrgPort.getInstance("COM5");

        public delegate void updateBoard(String text);
        public updateBoard updateBoardDel;
        int cnt = 0;

        private DrawRadar urgImg = new DrawRadar(450, 450);
        private void update(String text) {
            this.label1.Text = text;
            Console.WriteLine(++cnt + " " + text);
            this.pictureBox1.Image = urgImg.drawRadarImg(UrgPort.getDis());
        }

        public void processMethod(List<long> list) {
            this.Invoke(updateBoardDel, list.Count.ToString());
        }

        private void button1_Click(object sender, EventArgs e) {
            if (!urg.isOpen) {
                urg.openPort();
                Console.WriteLine("open");
            }
            else {
                urg.closePort();
            }
        }

    }
}
