using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace Smart_Car
{

    //class WorkMode {
    //    int Mode;//工作模式 1表示 手动   2表示自动
    //    int AutoMode;//  1表示处于推得状态   2 表示 自动运行
    //}

    /// <summary>
    /// struct point for save count information
    /// </summary>
    public struct Point {
        public double x;
        public double y;
        public double w;
        public int type;
        public bool direc;
    }

    /// <summary>
    /// struct line saving information between points
    /// </summary>
    public struct Line {
        public Point startpoint;
        public Point endpoint;
        public bool rightstate;
        public bool leftstate;
        public int num;
    }

    /// <summary>
    /// Fixed Rectangle
    /// </summary>
    public struct FixedRect {
        public int Shapenum;
        public int Size;
        public Point CenterPoint;
    }

    /// <summary>
    /// map information
    /// </summary>
    public struct Map {
        public List<Point> listPoint;
        public List<Line> listLine;
        public List<FixedRect> listFixedRect;
    }
    struct ActiveRect {
        int Shapenum;
        double Height;
        double Width;
        double Angle;
        Point CenterPoint;
    }
    class Route//路径类
    {
        public List<Point> pointcollection = new List<Point>();
        public Map map;//当前巡检地图
        public Point nowpoint; //当前对应节点
        public Point nextpoint;//目标对应节点
        public Line line; //当前所对应的线
        public int ControlRight; //控制权（仓库巡检或/点对点巡检）
        public DrPort drs; //编码器信息

        public void ControlRightChange()//切换巡检车的控制权 
        {

        }
    }

    class OperatingXML //提供路径XML的读取和写入操作
    {
        public void writeXML(Map map, String path) {
            XmlDocument xmldoc;
            XmlElement xmlelem;

            /*创建dom对象*/
            xmldoc = new XmlDocument();

            /*加入XML的声明段落,<?xml version="1.0" encoding="utf-8"?>*/
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmldoc.AppendChild(xmldecl);

            /*加入根节点Information*/
            xmlelem = xmldoc.CreateElement("", "Information", "");
            xmldoc.AppendChild(xmlelem);

            /*创建根节点下三个子节点：point、line、fixedret*/
            XmlNode root = xmldoc.SelectSingleNode("Information");//查找<Information> ,并把它作为根节点。
            XmlElement xe_point = xmldoc.CreateElement("point");//创建一个<point>节点
            XmlElement xe_line = xmldoc.CreateElement("line");//创建一个<line>节点
            XmlElement xe_fixedrect = xmldoc.CreateElement("fixedrect");//创建一个<fixedrect>节点


            /*创建point节点下面的子节点并加入数据*/
            for (int i = 0; i < map.listPoint.Count; i++) {
                string roadpoint_flag = "roadpoint";
                string num = Convert.ToString(i + 1);
                string roadpoint = roadpoint_flag + num;

                XmlElement xe_roadpoint = xmldoc.CreateElement(roadpoint);//创建一个<roadpoint>节点 

                XmlElement xe_x = xmldoc.CreateElement("x");
                XmlElement xe_y = xmldoc.CreateElement("y");
                XmlElement xe_w = xmldoc.CreateElement("w");
                XmlElement xe_type = xmldoc.CreateElement("type");
                XmlElement xe_direc = xmldoc.CreateElement("direc");

                xe_x.InnerText = Convert.ToString(map.listPoint[i].x);
                xe_y.InnerText = Convert.ToString(map.listPoint[i].y);
                xe_w.InnerText = Convert.ToString(map.listPoint[i].w);
                xe_type.InnerText = Convert.ToString(map.listPoint[i].type);
                xe_direc.InnerText = Convert.ToString(map.listPoint[i].direc);

                xe_roadpoint.AppendChild(xe_x);
                xe_roadpoint.AppendChild(xe_y);
                xe_roadpoint.AppendChild(xe_w);
                xe_roadpoint.AppendChild(xe_type);
                xe_roadpoint.AppendChild(xe_direc);

                xe_point.AppendChild(xe_roadpoint);

                root.AppendChild(xe_point);//添加到<Information>节点中 
            }

            /*创建line节点下面的子节点并加入数据*/
            for (int i = 0; i < map.listLine.Count; i++) {
                string line_flag = "l";
                string num = Convert.ToString(i + 1);
                string line = line_flag + num;

                XmlElement xe_l = xmldoc.CreateElement(line);//创建一个l节点

                XmlElement xe_startpoint = xmldoc.CreateElement("startpoint");//创建一个startpoint节点
                XmlElement xe_endpoint = xmldoc.CreateElement("endpoint");//创建一个endpoint节点
                XmlElement xe_rightstate = xmldoc.CreateElement("rightstate");//创建一个rightstate节点
                XmlElement xe_leftstate = xmldoc.CreateElement("leftstate");//创建一个leftstate节点
                XmlElement xe_num = xmldoc.CreateElement("num");//创建一个num节点

                XmlElement xe_x_startpoint = xmldoc.CreateElement("x");
                XmlElement xe_y_startpoint = xmldoc.CreateElement("y");
                XmlElement xe_w_startpoint = xmldoc.CreateElement("w");
                XmlElement xe_type_startpoint = xmldoc.CreateElement("type");
                XmlElement xe_direc_startpoint = xmldoc.CreateElement("direc");
                XmlElement xe_x_endpoint = xmldoc.CreateElement("x");
                XmlElement xe_y_endpoint = xmldoc.CreateElement("y");
                XmlElement xe_w_endpoint = xmldoc.CreateElement("w");
                XmlElement xe_type_endpoint = xmldoc.CreateElement("type");
                XmlElement xe_direc_endpoint = xmldoc.CreateElement("direc");

                xe_x_startpoint.InnerText = Convert.ToString(map.listLine[i].startpoint.x);
                xe_y_startpoint.InnerText = Convert.ToString(map.listLine[i].startpoint.y);
                xe_w_startpoint.InnerText = Convert.ToString(map.listLine[i].startpoint.w);
                xe_type_startpoint.InnerText = Convert.ToString(map.listLine[i].startpoint.type);
                xe_direc_startpoint.InnerText = Convert.ToString(map.listLine[i].startpoint.direc);

                xe_startpoint.AppendChild(xe_x_startpoint);
                xe_startpoint.AppendChild(xe_y_startpoint);
                xe_startpoint.AppendChild(xe_w_startpoint);
                xe_startpoint.AppendChild(xe_type_startpoint);
                xe_startpoint.AppendChild(xe_direc_startpoint);

                xe_l.AppendChild(xe_startpoint);

                xe_x_endpoint.InnerText = Convert.ToString(map.listLine[i].endpoint.x);
                xe_y_endpoint.InnerText = Convert.ToString(map.listLine[i].endpoint.y);
                xe_w_endpoint.InnerText = Convert.ToString(map.listLine[i].endpoint.w);
                xe_type_endpoint.InnerText = Convert.ToString(map.listLine[i].endpoint.type);
                xe_direc_endpoint.InnerText = Convert.ToString(map.listLine[i].endpoint.direc);

                xe_endpoint.AppendChild(xe_x_endpoint);
                xe_endpoint.AppendChild(xe_y_endpoint);
                xe_endpoint.AppendChild(xe_w_endpoint);
                xe_endpoint.AppendChild(xe_type_endpoint);
                xe_endpoint.AppendChild(xe_direc_endpoint);

                xe_l.AppendChild(xe_endpoint);

                xe_rightstate.InnerText = Convert.ToString(map.listLine[i].rightstate);
                xe_l.AppendChild(xe_rightstate);

                xe_leftstate.InnerText = Convert.ToString(map.listLine[i].leftstate);
                xe_l.AppendChild(xe_leftstate);

                xe_num.InnerText = Convert.ToString(map.listLine[i].num);
                xe_l.AppendChild(xe_num);

                //xe_l.AppendChild(xe_startpoint);
                //xe_l.AppendChild(xe_endpoint);
                xe_line.AppendChild(xe_l);
                root.AppendChild(xe_line);
            }

            /*创建fixedrect节点下面的子节点并加入数据*/
            for (int i = 0; i < map.listFixedRect.Count; i++) {
                string fixedrect_flag = "fr";
                string num = Convert.ToString(i + 1);
                string fixedrect = fixedrect_flag + num;

                XmlElement xe_fr = xmldoc.CreateElement(fixedrect);//创建一个fr节点

                XmlElement xe_shapenum = xmldoc.CreateElement("shapenum");//创建一个shapenum节点
                XmlElement xe_size = xmldoc.CreateElement("size");//创建一个size节点
                XmlElement xe_centerpoint = xmldoc.CreateElement("centerpoint");//创建一个size节点

                xe_shapenum.InnerText = Convert.ToString(map.listFixedRect[i].Shapenum);
                xe_fr.AppendChild(xe_shapenum);

                xe_size.InnerText = Convert.ToString(map.listFixedRect[i].Size);
                xe_fr.AppendChild(xe_size);

                XmlElement xe_x_centerpoint = xmldoc.CreateElement("x");
                XmlElement xe_y_centerpoint = xmldoc.CreateElement("y");
                XmlElement xe_w_centerpoint = xmldoc.CreateElement("w");
                XmlElement xe_type_centerpoint = xmldoc.CreateElement("type");
                XmlElement xe_direc_centerpoint = xmldoc.CreateElement("direc");

                xe_x_centerpoint.InnerText = Convert.ToString(map.listFixedRect[i].CenterPoint.x);
                xe_y_centerpoint.InnerText = Convert.ToString(map.listFixedRect[i].CenterPoint.y);
                xe_w_centerpoint.InnerText = Convert.ToString(map.listFixedRect[i].CenterPoint.w);
                xe_type_centerpoint.InnerText = Convert.ToString(map.listFixedRect[i].CenterPoint.type);
                xe_direc_centerpoint.InnerText = Convert.ToString(map.listFixedRect[i].CenterPoint.direc);

                xe_centerpoint.AppendChild(xe_x_centerpoint);
                xe_centerpoint.AppendChild(xe_y_centerpoint);
                xe_centerpoint.AppendChild(xe_w_centerpoint);
                xe_centerpoint.AppendChild(xe_type_centerpoint);
                xe_centerpoint.AppendChild(xe_direc_centerpoint);

                xe_fr.AppendChild(xe_centerpoint);

                xe_fixedrect.AppendChild(xe_fr);
                root.AppendChild(xe_fixedrect);
            }

            xmldoc.Save(path);//保存文件
        }
        public Map readXML(String path) {
            Point p = new Point();
            Line l = new Line();
            FixedRect fr = new FixedRect();
            List<Point> listPoint = new List<Point>();
            List<Line> listLine = new List<Line>();
            List<FixedRect> listFixedRect = new List<FixedRect>();

            Map map = new Map();


            /*创建xml读取流对象，并加载文件*/
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            /*获取根节点*/
            XmlNode root = doc.SelectSingleNode("Information");
            //string name = root.Name;
            //Console.WriteLine(name);
            //Console.ReadKey();

            XmlNodeList xnl_1 = root.ChildNodes;//xnl_1节点为point、line、fixedline。
            foreach (XmlNode xn1_1 in xnl_1)//xnl_1为xnl_1中的一个具体的节点。
            {
                XmlElement xe_1 = (XmlElement)xn1_1;//将该节点转化为元素。
                if (xe_1.Name == "point")//检测到point节点
                {
                    XmlNodeList xnl_2 = xe_1.ChildNodes;//xnl_2节点为roadpoint1、roadpoint2、roadpoint3。
                    foreach (XmlNode xn1_2 in xnl_2)//xn1_2为xn1_2的一个具体的节点。
                    {
                        XmlNodeList xnl_3 = xn1_2.ChildNodes;//xnl_3是xnl_2的子节点，包括x、y、w等等。
                        /*将xml中点的值读入到point变量中，并存入到list集合中。*/
                        p.x = Convert.ToDouble(xnl_3.Item(0).InnerText);
                        p.y = Convert.ToDouble(xnl_3.Item(1).InnerText);
                        p.w = Convert.ToDouble(xnl_3.Item(2).InnerText);
                        p.type = Convert.ToInt32(xnl_3.Item(3).InnerText);
                        p.direc = Convert.ToBoolean(xnl_3.Item(4).InnerText);
                        listPoint.Add(p);
                    }
                } else if (xe_1.Name == "line")//检测到line节点。
                {
                    XmlNodeList xnl_2 = xe_1.ChildNodes;//xnl_2节点为l1、l2。
                    foreach (XmlNode xn1_2 in xnl_2)//xn1_2为xn1_2的一个具体的节点。
                    {
                        XmlNodeList xnl_3 = xn1_2.ChildNodes;//xnl_3是xnl_2的子节点，包括startpoint、endpoint、rightstate、leftstate.
                        foreach (XmlNode xn1_3 in xnl_3) {
                            XmlElement xe3 = (XmlElement)xn1_3;//xn1_3为xn1_3的一个具体的节点。
                            if (xe3.Name == "startpoint")//检测到startpoint节点。
                            {
                                XmlNodeList xnl_4 = xn1_3.ChildNodes;
                                /*将startpoint中的值读入到Line结构体变量l的startpoint中*/
                                l.startpoint.x = Convert.ToDouble(xnl_4.Item(0).InnerText);
                                l.startpoint.y = Convert.ToDouble(xnl_4.Item(1).InnerText);
                                l.startpoint.w = Convert.ToDouble(xnl_4.Item(2).InnerText);
                                l.startpoint.type = Convert.ToInt32(xnl_4.Item(3).InnerText);
                                l.startpoint.direc = Convert.ToBoolean(xnl_4.Item(4).InnerText);
                            } else if (xe3.Name == "endpoint")//检测到endpoint节点。
                            {
                                XmlNodeList xnl_4 = xn1_3.ChildNodes;
                                /*将endpoint中的值读入到Line结构体变量l的endpoint中*/
                                l.endpoint.x = Convert.ToDouble(xnl_4.Item(0).InnerText);
                                l.endpoint.y = Convert.ToDouble(xnl_4.Item(1).InnerText);
                                l.endpoint.w = Convert.ToDouble(xnl_4.Item(2).InnerText);
                                l.endpoint.type = Convert.ToInt32(xnl_4.Item(3).InnerText);
                                l.endpoint.direc = Convert.ToBoolean(xnl_4.Item(4).InnerText);
                            } else if (xe3.Name == "rightstate")//检测到rightstate节点。
                            {
                                /*将rightstate中的值读入到Line结构体变量l的rightstate中*/
                                l.rightstate = Convert.ToBoolean(xn1_3.InnerText);
                            } else if (xe3.Name == "leftstate")//检测到leftstate节点。
                            {
                                /*将endstate中的值读入到Line结构体变量l的endstate中*/
                                l.leftstate = Convert.ToBoolean(xn1_3.InnerText);
                            } else if (xe3.Name == "num")
                            {

                                l.num = Convert.ToInt32(xn1_3.InnerText);
                            }
                        }
                        listLine.Add(l);//将Line结构体变量添加入list集合中。                     
                    }
                } else if (xe_1.Name == "fixedrect")//检测到fixedrect节点。
                {
                    //Console.WriteLine("fixedrect");
                    XmlNodeList xnl_2 = xe_1.ChildNodes;//xnl_2节点为fr1、fr2。
                    foreach (XmlNode xn1_2 in xnl_2)//xn1_2为xn1_2的一个具体的节点。
                    {
                        XmlNodeList xnl_3 = xn1_2.ChildNodes;//xnl_3是xnl_2的子节点，包括shapenum、size、centerpoint.
                        foreach (XmlNode xn1_3 in xnl_3) {
                            XmlElement xe3 = (XmlElement)xn1_3;//xn1_3为xn1_3的一个具体的节点。
                            if (xe3.Name == "shapenum")//检测到shapenum节点。
                            {
                                /*将shapenum中的值读入到FixedRect结构体变量fr的shapenum中*/
                                fr.Shapenum = Convert.ToInt32(xn1_3.InnerText);
                            } else if (xe3.Name == "size") {
                                /*将size中的值读入到FixedRect结构体变量fr的size中*/
                                fr.Size = Convert.ToInt32(xn1_3.InnerText);
                            } else if (xe3.Name == "centerpoint") {
                                XmlNodeList xnl_4 = xn1_3.ChildNodes;
                                /*将centerpoint中的值读入到FixedRect结构体变量fr的centerpoint中*/
                                fr.CenterPoint.x = Convert.ToDouble(xnl_4.Item(0).InnerText);
                                fr.CenterPoint.y = Convert.ToDouble(xnl_4.Item(1).InnerText);
                                fr.CenterPoint.w = Convert.ToDouble(xnl_4.Item(2).InnerText);
                                fr.CenterPoint.type = Convert.ToInt32(xnl_4.Item(3).InnerText);
                                fr.CenterPoint.direc = Convert.ToBoolean(xnl_4.Item(4).InnerText);
                            }
                        }
                        listFixedRect.Add(fr);//将FixedRect结构体变量fr添加入list集合中。
                    }
                }
            }
            /*将三个集合分别添加到map的集合中*/
            map.listPoint = listPoint;
            map.listLine = listLine;
            map.listFixedRect = listFixedRect;
            return map;
        }
    }
}
