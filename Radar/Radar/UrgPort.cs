using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Ports;
using SCIP_library;
using System.Collections.Concurrent;

namespace Radar {
    class UrgPort : IPort {
        
        // 基本串口实例、激光雷达实例
        private static SerialPort basePort;
        private static UrgPort urgPort;
        // 定时接收数据
        private static System.Timers.Timer timer;
        //起始和终止位置
        private const int startStep = 0;
        private const int endStep = 760;
        //数据记录
        private static long timeStamp;
        private static List<long> distance = new List<long>();
        /// <summary>
        /// 串口状态（正在关闭、正在读取）
        /// </summary>
        public static bool isClosing = false;
        public static bool isReading = false;
        /// <summary>
        /// 获取激光雷达串口是否打开的方法
        /// </summary>
        public bool isOpen {
            get {
                if (basePort != null) {
                    return basePort.IsOpen;
                }
                return false;
            }
        }
        /// <summary>
        /// 处理数据绑定委托
        /// </summary>
        /// <param name="list"></param>
        public delegate void processData(List<long> list);
        public static processData processDel;

        /// <summary>
        /// 单例模式，使用getInstance获取实例
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        private UrgPort(String portName, int baudRate = 115200) {
            if (urgPort != null && urgPort.isOpen) {
                urgPort.closePort();
            }
            // 初始化串口
            basePort = new SerialPort(portName, baudRate);
            // 绑定读取数据定时器
            timer    = new System.Timers.Timer(100);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;
        }
        /// <summary>
        /// 获取激光雷达实例
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public static UrgPort getInstance(string portName) {
            if (urgPort == null) {
                urgPort = new UrgPort(portName);
            }
            return urgPort;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            // 尝试关闭或者已经关闭则返回
            if (isClosing || !urgPort.isOpen) {
                return;
            }
            try {
                isReading = true;
                // 接收数据
                string receiveData = basePort.ReadLine();// basePort.ReadExisting();
                // 清除缓存
                basePort.DiscardInBuffer();
                lock (distance) {
                    SCIP_Reader.MD(receiveData, ref timeStamp, ref distance);
                }

                // 执行处理数据程序
                if (processDel != null) {
                    processDel(getDis());
                }
                isReading = false;
            }
            catch { }
            if (isClosing) {
                timer.Enabled = false;
            }
        }

        // 返回可操作的distance数组
        private static List<long> disCopy;
        public static List<long> getDis() {
            lock (distance) {
                disCopy = new List<long>(distance);
            }
            return disCopy;
        }


        

        /// <summary>
        /// 打开串口方法
        /// </summary>
        /// <returns>串口打开是否成功</returns>
        public bool openPort() {
            if (urgPort == null) {
                return false;
            }
            try {
                // 串口打开返回成功
                basePort.NewLine = "\n\n";
                if (urgPort.isOpen) {
                    isClosing = false;
                    timer.Enabled = true;
                    return true;
                }
                // 尝试打开串口
                basePort.Open();
                // 写入读取数据命令
                basePort.Write(SCIP_Writer.SCIP2());
                basePort.ReadLine();
                basePort.Write(SCIP_Writer.MD(startStep, endStep));
                basePort.ReadLine();
                isClosing = false;
                timer.Enabled = true;
            }
            catch {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 关闭激光雷达串口
        /// </summary>
        /// <returns>返回关闭激光雷达是否成功</returns>
        public bool closePort() {
            // 串口已经关闭返回成功
            if (!urgPort.isOpen || urgPort == null) {
                return true;
            }
            try {
                isClosing = true;
                // 等待读取串口程序完成
                while (isReading) {
                    System.Windows.Forms.Application.DoEvents();
                }
                // 停止读取数据命令
                // timer.Enabled = false;
                // 发送停止命令并清除缓存
                basePort.Write(SCIP_Writer.QT());
                basePort.DiscardInBuffer();
                //basePort.ReadLine();
                // 尝试关闭串口
                basePort.Close();
            }
            catch {
                return false;
            }
            return true;
        }


    }
}