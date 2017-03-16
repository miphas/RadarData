using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radar {
    interface IPort {

        /// <summary>
        /// 打开串口方法
        /// </summary>
        /// <returns>打开传感器串口是否成功</returns>
        bool openPort();


        /// <summary>
        /// 关闭串口方法
        /// </summary>
        /// <returns>关闭传感器串口是否成功</returns>
        bool closePort();

    }
}
