using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;

namespace Smart_Car
{
    class DrPort
    {
        //Distance数据结构
        public struct Distance
        {
            public double Sx;
            public double Sy;
            public double Sw;
            public double Angel_Serial;
            public int Speed1;
            public int Speed2;
            public int Speed3;
            public int Speed4;
            public int sign;
        }

        //里程计接收串口
        SerialPort port;
        bool IsPortOpen = false;
        public bool IsSaving = false;
        //存数据口
        public static Distance distance;
        int indexForSave;
        //定值参数
        const double R = 0.1015;
        const double pi = 3.1416;
        const double lx = 0.1825;
        const double ly = 0.2950;
        //每个车轮速度
        int SpeedA = 0;
        int SpeedB = 0;
        int SpeedC = 0;
        int SpeedD = 0;
        //存储输入数据
        public static List<Byte> DataReader = new List<byte>();
        public static List<Point> Data_Save = new List<Point>();
        public static List<Point> Data_Save_Key = new List<Point>();

        public DrPort()
        {
        }
        ~DrPort()
        { }
        //打开串口（成功返回true，失败返回false）
        public bool OpenPort(string portName, string portBundrate)
        {
            if (!IsPortOpen)
            {
                int bundrate = int.Parse(portBundrate);
                port = new SerialPort(portName, bundrate);
                try{
                    clearData();
                    port.Open();
                    port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                }
                catch(Exception ex){
                    Console.WriteLine(ex);
                    return false;
                }
                IsPortOpen = true;
                return true;
            }
            return false;
        }

        //关闭串口（成功返回true，失败返回false）
        public bool ClosePort()
        {
            if (IsPortOpen){
                try{
                    IsPortOpen = false;
                    port.DataReceived -= new SerialDataReceivedEventHandler(port_DataReceived);
                    port.Close();
                }
                catch{
                    IsPortOpen = true;
                    return false;
                }
                return true;
            }
            return false;
        }
        public void clearData()
        {
            distance.Sx = 0;
            distance.Sy = 0;
            distance.Sw = 0;
            distance.Angel_Serial = 0;

            Data_Save.Clear();
            Data_Save_Key.Clear();
            indexForSave = 0;
            IsSaving = true;
        }
        public void clear()
        {
            distance.Sx = 0;
            distance.Sy = 0;
            distance.Sw = 0;
        }
        //获取distance
        public Distance getDistance()
        {
            return distance;
        }
        public Point getPoint()
        {
            Point tmpPoint = new Point( );
            tmpPoint.x = distance.Sx;
            tmpPoint.y = distance.Sy;
            tmpPoint.w = distance.Sw;
            return tmpPoint;
        }
        private Distance FunDistance(Distance distance)
        {

            double x = 0.0;
            double y = 0.0;
            double w = 0.0;
            x = (double)(distance.Speed1 + distance.Speed4 - distance.Speed2 - distance.Speed3) / 1000 * 2 * pi * R / 4;
            y = (double)(distance.Speed1 + distance.Speed2 + distance.Speed3 + distance.Speed4) / 1000 * 2 * pi * R / 4;
            w = (double)(distance.Speed2 + distance.Speed4 - distance.Speed1 - distance.Speed3) / 1000 * 2 * pi * R / (4 * (lx + ly));

            x = x / 4.077;
            y = y / 4.077;
            w = w / 4.077;

            distance.Sx = distance.Sx + x * Math.Cos(distance.Sw) - y * Math.Sin(distance.Sw);
            distance.Sy = distance.Sy + x * Math.Sin(distance.Sw) + y * Math.Cos(distance.Sw);
            distance.Sw = distance.Sw + w;
            distance.Angel_Serial = distance.Sw;
            return distance;
        }

        //编码器数据获取
        public void port_DataReceived(object sender, EventArgs e) 
        {
            if (port.IsOpen)
            {
                bool IsReaded = true;
                #region 接收串口接收数据
                DataReader.Clear();

                for (int i = 0; i < 14; i++)
                {
                    try{
                        DataReader.Add((byte)port.ReadByte());
                    }
                    catch(Exception){
                        IsReaded = false;
                        break;
                    }
                }
                if (port.IsOpen)
                {
                    try
                    {
                        port.ReadExisting();
                    }
                    catch(Exception)
                    {
                        IsReaded = false;
                    }
                }
                if (IsReaded && DataReader[0] == 170 && DataReader[DataReader.Count() - 1] == 187)
                {
                    SpeedA = DataReader[2] * 256 + DataReader[3];
                    SpeedB = DataReader[5] * 256 + DataReader[6];
                    SpeedC = DataReader[8] * 256 + DataReader[9];
                    SpeedD = DataReader[11] * 256 + DataReader[12];

                    if (DataReader[1] == 0)
                        SpeedA = 0 - SpeedA;
                    if (DataReader[4] == 0)
                        SpeedB = 0 - SpeedB;
                    if (DataReader[7] == 0)
                        SpeedC = 0 - SpeedC;
                    if (DataReader[10] == 0)
                        SpeedD = 0 - SpeedD;

                    DataReader.Clear();
                }
                #endregion

                if (IsReaded)
                {
                    #region 算法处理
                    int FilterMark = 0;
                    if (SpeedA == 0)
                        FilterMark++;
                    if (SpeedB == 0)
                        FilterMark++;
                    if (SpeedC == 0)
                        FilterMark++;
                    if (SpeedD == 0)
                        FilterMark++;
                    if (FilterMark < 2)
                    {
                        distance.Speed1 = 0 - SpeedB;
                        distance.Speed2 = SpeedC;
                        distance.Speed3 = 0 - SpeedA;
                        distance.Speed4 = SpeedD;
                        distance = FunDistance(distance);
                    }
                    #endregion

                    if (IsSaving)
                    {
                        #region 数据记录
                        Point nowData = new Point();
                        nowData.x = distance.Sx;
                        nowData.y = distance.Sy;
                        nowData.w = distance.Sw;
                        nowData.type = indexForSave++;
                        Data_Save.Add(nowData);
                        #endregion
                    }
                }
            }
        }

        public void calKeyPoint()
        {
            IsSaving = false;
            Data_Save_Key.Clear();
            Point p = new Point( );
            double Angle_Total = 0;//累计角度
            int Point_Key = 0;//关键点是否统计标志,0为未统计，1为正在统计，2为统计完成
            //数据滤波
            for (int i = 1; i < Data_Save.Count() - 1; i++)
            {
                p.x = Data_Save[i].x * 0.6 + Data_Save[i - 1].x * 0.2 + Data_Save[i + 1].x * 0.2;
                p.y = Data_Save[i].y * 0.6 + Data_Save[i - 1].y * 0.2 + Data_Save[i + 1].y * 0.2;
                p.w = Data_Save[i].w * 0.6 + Data_Save[i - 1].w * 0.2 + Data_Save[i + 1].w * 0.2;
                p.type = Data_Save[i].type;
                Data_Save[i] = p;
            }
            #region 关键点坐标计算
            //记录起始点
            Data_Save_Key.Add(Data_Save[0]);
            for (int i = 1; i < Data_Save.Count(); i++)
            {
                double Position_Change = Math.Sqrt((Data_Save[i].x - Data_Save[i - 1].x) * (Data_Save[i].x - Data_Save[i - 1].x) + (Data_Save[i].y - Data_Save[i - 1].y) * (Data_Save[i].y - Data_Save[i - 1].y));
                double Angle_Change = Data_Save[i].w - Data_Save[i - 1].w;
                if (Position_Change > 0.004)
                {
                    Point_Key = 0;
                    Angle_Total = 0;
                }
                else
                {
                    if (Angle_Change > 0.0015 || Angle_Change < -0.0015)
                    {
                        if (Point_Key == 0)
                        {
                            Point_Key = 1;
                        }
                        Angle_Total = Angle_Total + Angle_Change;
                    }
                }
                if (Point_Key == 1 && (Angle_Total > 0.1 || Angle_Total < -0.1))
                {
                    Point_Key = 2;
                    Data_Save_Key.Add(Data_Save[i]);
                }
            }
            //记录终止点
            Data_Save_Key.Add(Data_Save[Data_Save.Count() - 1]);
            #endregion 

            #region 平移点推算
            for (int i = 0; i < Data_Save_Key.Count() - 1; ++i)
            {
                if (addKeyPoint(Data_Save_Key[i], Data_Save_Key[i + 1], i, 0.02))
                    ++i;
            }
            #endregion

            #region 关键点角度重计算
            for (int i = 0; i < Data_Save_Key.Count() - 1; ++i)
            {
                Point tmp = Data_Save_Key[i];
                if (Math.Abs(Data_Save_Key[i].x - Data_Save_Key[i + 1].x) >
                    Math.Abs(Data_Save_Key[i].y - Data_Save_Key[i + 1].y))
                {
                    tmp.w = Search(Data_Save_Key[i], Data_Save_Key[i + 1],
                                                     0.5 * (Data_Save_Key[i].x + Data_Save_Key[i + 1].x), true, 0.05);
                }
                else
                {
                    tmp.w = Search(Data_Save_Key[i], Data_Save_Key[i + 1],
                                                    0.5 * (Data_Save_Key[i].y + Data_Save_Key[i + 1].y), false, 0.05);
                }
                Data_Save_Key[i] = tmp;
            }
            #endregion

        }

        private double Search(Point left, Point right, double X, bool IsX, double range)
        {
            int index = left.type;
            while (index < right.type - 1)
            {
                if (IsX && Math.Abs(Data_Save[index].x - X) < range)
                    break;
                if (!IsX && Math.Abs(Data_Save[index].y - X) < range)
                    break;
                ++index;
            }
            return Data_Save[index].w;
        }

        //补遗关键点
        private bool addKeyPoint(Point left, Point right, int index, double range)
        {
            double shitting = 0.1;
            if(Math.Abs(left.x - right.x) > Math.Abs(left.y - right.y))
            {
                if (Math.Abs(left.y - right.y) > shitting)
                {
                    int oldIndex = left.type;
                    while (Math.Abs(Data_Save[oldIndex++].x - right.x) > range)
                        ;
                    if (Math.Abs(Data_Save[oldIndex].x - right.x) + Math.Abs(Data_Save[oldIndex].y - right.y) > 3 * range)
                    {
                        Data_Save_Key.Insert(index + 1, Data_Save[oldIndex]);
                        return true;
                    }
                }
            }
            else{
                if (Math.Abs(left.x - right.x) > shitting)
                {
                    int oldIndex = left.type;
                    while (Math.Abs(Data_Save[oldIndex++].y - right.y) > range)
                        ;
                    if (Math.Abs(Data_Save[oldIndex].x - right.x) + Math.Abs(Data_Save[oldIndex].y - right.y) > 3 * range)
                    {
                        Data_Save_Key.Insert(index + 1, Data_Save[oldIndex]);
                        return true;
                    }
                }
            }
            return false;
        }


    }
    
}
