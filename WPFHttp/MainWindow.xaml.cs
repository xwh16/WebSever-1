﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace WebServer.App
{
    using WebServer.HttpServer;
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    
    //用来作为datagrid绑定的类
    public class DataGrid_BD
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public string Method { get; set; }
        public string Port { get; set; }
        public string Status { get; set; }
        public string URI { get; set; }
        public string Version { get; set; }
        public string Host { get; set; }
        public string User_Agent { get; set; }
        public string Accept { get; set; }
        public string Accept_Encoding { get; set; }
        public DataGrid_BD(int _id, string _ip, string _port)
        {
            Id = _id;
            IP = _ip;
            Port = _port;
        }
        public DataGrid_BD(int _id, string _ip, string _method, string _port, 
                           string _status, string _uri, string _version, string _host,
                           string _useragent, string _accept, string _acceptencoding)
        {
            Id = _id;
            IP = _ip;
            Method = _method;
            Port = _port;
            Status = _status;
            URI = _uri;
            Version = _version;
            Host = _host;
            User_Agent = _useragent;
            Accept = _accept;
            Accept_Encoding = _acceptencoding;
        }
    }

    public partial class MainWindow : Window
    {
        public HttpServer httpserver;

        

        public MainWindow()
        {  
            httpserver = new HttpServer(80, IPAddress.Any);

            //用来保存request.Header字典里的Value
            //Header_BD = new List<string>();

            InitializeComponent();
        }

        private void select_ip(object sender, RoutedEventArgs e)
        {
            List<string> listIP = WebServer.HttpServer.HttpServer.GetIP();
            if (HttpServer.SERVER_STATUS == false)
            {
                this.listip.Text = listIP[0];
                this.listip1.Text = listIP[1];

            }
        }
        private void save_ip(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == false)
            {
                HttpServer.SERVER_ADDR = this.listip2.Text;
            }
        }
        private void start_server(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == false)
            {
                this.btn_start_server.Background = new SolidColorBrush(Colors.Blue);
                this.btn_stop_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                this.server_config.IsEnabled = false;

                //Create the main server thread
                Thread ServerThread = new Thread(httpserver.Start);
                ServerThread.Name = "Main Server Thread";

                //Set the default server properties
                HttpServer.SITE_PATH = "..\\..\\..\\HttpServer\\Resources";
                HttpServer.PROTOCOL_VERSION = "HTTP/1.1";
                HttpServer.SERVER_THREAD = ServerThread;

                ServerThread.Start();
            }
            else
            {
                MessageBox.Show("已经有正在运行的服务器例程");
            }
        }

        private void stop_server(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == true)
            {
                this.server_config.IsEnabled = true;
                this.btn_start_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                this.btn_stop_server.Background = new SolidColorBrush(Colors.Blue);

                this.httpserver.Close();
            }
            else
            {
                MessageBox.Show("服务器例程已停止");
            }
        }

        private void show_connect(object sender, RoutedEventArgs e)
        {
            if (httpserver.PROC_RECORD.Count == 0)
            {
                MessageBox.Show("尚未有接入的浏览器");
            }
            else
            {
                int now_count = httpserver.PROC_RECORD.Count;
                //作为绑定数据的类的实例----作为数组进行创建
                DataGrid_BD[] datagrid_bd = new DataGrid_BD[now_count];
                for (int j = 0; j < now_count; j++)
                {
                    datagrid_bd[j] = new DataGrid_BD(j + 1,
                                                    httpserver.PROC_RECORD[j].RemoteIP,
                                                    httpserver.PROC_RECORD[j].RemotePort);
                    if (httpserver.PROC_RECORD[j].request == null || httpserver.PROC_RECORD[j].response == null)
                    {
                        datagrid_bd[j].Status = "N/A";
                        datagrid_bd[j].Method = "N/A";
                        datagrid_bd[j].URI = "N/A";
                        datagrid_bd[j].Version = "N/A";
                        datagrid_bd[j].Host = "N/A";
                        datagrid_bd[j].User_Agent = "N/A";
                        datagrid_bd[j].Accept = "N/A";
                        datagrid_bd[j].Accept_Encoding = "N/A";
                    }
                    else
                    {
                        datagrid_bd[j].Status = httpserver.PROC_RECORD[j].response.StatusCode + " " + httpserver.PROC_RECORD[j].response.ReasonPhrase;
                        datagrid_bd[j].Method = httpserver.PROC_RECORD[j].request.Method;
                        datagrid_bd[j].URI = httpserver.PROC_RECORD[j].request.Uri;
                        datagrid_bd[j].Version = httpserver.PROC_RECORD[j].request.Version;
                        datagrid_bd[j].Host = httpserver.PROC_RECORD[j].request.Header["Host"];
                        datagrid_bd[j].User_Agent = httpserver.PROC_RECORD[j].request.Header["User-Agent"];
                        datagrid_bd[j].Accept = httpserver.PROC_RECORD[j].request.Header["Accept"];
                        datagrid_bd[j].Accept_Encoding = httpserver.PROC_RECORD[j].request.Header["Accept-Encoding"];
                    }

                    dataGrid.ItemsSource = datagrid_bd;
                }
            }
        }

        private void save_config(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == false)
            {
                HttpServer.SERVER_PORT = Convert.ToInt16(this.tbx_server_port.Text);
                HttpServer.SITE_PATH = this.tbx_site_path.Text;
                HttpServer.SERVER_MAX_THREADS = Convert.ToInt16(this.tbx_thread_max.Text);
                MessageBox.Show(
                    "服务器端口: " + HttpServer.SERVER_PORT + "\n" +
                    "站点路径: " + HttpServer.SITE_PATH + "\n" +
                    "最大线程数: " + HttpServer.SERVER_MAX_THREADS,
                    "修改的服务器配置"
                    );
            }
            else
            {
                MessageBox.Show("服务器运行中");
            }
        }

        private void reset_config(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == false)
            {
                HttpServer.SERVER_PORT = 80;
                HttpServer.SITE_PATH = "..\\..\\..\\HttpServer\\Resources";
                HttpServer.SERVER_MAX_THREADS = 30;
                MessageBox.Show(
                    "服务器端口: " + HttpServer.SERVER_PORT + "\n" +
                    "站点路径: " + HttpServer.SITE_PATH + "\n" +
                    "最大线程数: " + HttpServer.SERVER_MAX_THREADS,
                    "重置的服务器配置"
                    );
            }
            else
            {
                MessageBox.Show("服务器运行中");
            }
        }


        //将16进制转化为Argb的颜色表示
        private Color ToColor(string colorName)
        {
            if (colorName.StartsWith("#"))
                colorName = colorName.Replace("#", string.Empty);
            int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
            return new Color()
            {
                A = Convert.ToByte((v >> 24) & 255),
                R = Convert.ToByte((v >> 16) & 255),
                G = Convert.ToByte((v >> 8) & 255),
                B = Convert.ToByte((v >> 0) & 255)
            };
        }
    }
}
