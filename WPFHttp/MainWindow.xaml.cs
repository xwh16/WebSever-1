﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
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
        public string Accept_Language { get; set; }
        public DataGrid_BD(int _id, string _ip, string _port)
        {
            Id = _id;
            IP = _ip;
            Port = _port;
        }
        public DataGrid_BD(int _id, string _ip, string _method, string _port, 
                           string _status, string _uri, string _version, string _host,
                           string _useragent, string _accept, string _acceptlanguage)
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
            Accept_Language = _acceptlanguage;
        }
    }

    public class DataGrid_IP
    {
        public string IPDisplay { set; get; }
        public string MACDisplay { set; get; }
        public string NameDisplay { set; get; }
        public DataGrid_IP(string ipdisplay, string macdisplay, string namedisplay)
        {
            IPDisplay = ipdisplay;
            MACDisplay = macdisplay;
            NameDisplay = namedisplay;
        }
    }

    public partial class MainWindow : Window
    {
        public HttpServer httpserver;
        Thread HttpThread;
        Thread HttpsThread;

        public bool flag_http_s=false;

        public MainWindow()
        {
            httpserver = new HttpServer(80, IPAddress.Any);
            //设置默认配置信息
            HttpServer.SERVER_MAX_THREADS = 10;
            HttpServer.PROTOCOL_VERSION = "HTTP/1.1";
            HttpServer.SITE_DEFAULT_PAGE = "index.html";
            HttpServer.SITE_PATH = @"D:\GitHub for windows\Git2\HttpServer\Resources";
            HttpServer.CERT_PATH = HttpServer.SITE_PATH + "\\key\\server.cer";
            HttpServer.PHP_PATH = @"C:\php\php-cgi.exe";

            httpserver.InnerException += ServerExceptionHandler;    /*订阅HttpServer中的异常事件*/

            InitializeComponent();
        }

        private void ServerExceptionHandler(object sender, string message)
        {
            MessageBox.Show(message, "服务器内部错误");
            if (flag_http_s == true)
            {
                HttpThread.Abort();
            }
            else
            {
                HttpsThread.Abort();
            }
            
            HttpServer.SERVER_STATUS = false;

            //设置开始/停止按钮样式
            this.btn_start2_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
            this.btn_start_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
            this.btn_stop_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
            this.server_config.IsEnabled = false;
        }
        
        private void start_server(object sender, RoutedEventArgs e)
        {
            if (this.tbx_server_ip.Text=="null")
            {
                MessageBox.Show("请先配置IP");
            }
            else
            {
                if (HttpServer.SERVER_STATUS == false)
                {
                    flag_http_s = true;
                    //创建Http服务器主线程
                    HttpThread = new Thread(httpserver.Start);
                    HttpThread.Name = "Http Server";
                    HttpThread.Start();

                    //设置开始/停止按钮样式
                    this.btn_start_server.Background = new SolidColorBrush(Colors.Blue);
                    this.btn_start2_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                    this.btn_stop_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                    this.server_config.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("已经有正在运行的服务器例程");
                }
            }
            
        }

        private void start_https_server(object sender, RoutedEventArgs e)
        {
            if (this.tbx_server_ip.Text == "null")
            {
                MessageBox.Show("请先配置IP");
            }
            else
            {
                if (HttpServer.SERVER_STATUS == false)
                {
                    flag_http_s = false;
                    //创建Https服务器主线程
                    HttpsThread = new Thread(httpserver.StartSSL);
                    HttpsThread.Name = "SSL Server";
                    HttpsThread.Start();

                    //设置开始/停止按钮样式
                    this.btn_start2_server.Background = new SolidColorBrush(Colors.Blue);
                    this.btn_start_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                    this.btn_stop_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                    this.server_config.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("已经有正在运行的服务器例程");
                }
            }

        }

        private void stop_server(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == true)
            {
                this.server_config.IsEnabled = true;
                this.btn_start_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                this.btn_start2_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                this.btn_stop_server.Background = new SolidColorBrush(Colors.Blue);


                if(flag_http_s==true)
                {
                    this.httpserver.Close();
                }
                else
                {
                    this.httpserver.CloseSSL();
                }
                flag_http_s = false;
            }
            else
            {
                MessageBox.Show("服务器例程已停止");
            }
        }
        
        private void show_server_config(object sender, RoutedEventArgs e)
        {
            DataGrid_IP[] datagrid_ip = new DataGrid_IP[IPConfig.networkCardIPs.Count];
            for (int i = 0; i < IPConfig.networkCardIPs.Count; i++)
            {
                datagrid_ip[i] =
                    new DataGrid_IP(
                        IPConfig.networkCardIPs[i].interfaceIP,
                        IPConfig.networkCardIPs[i].interfaceMAC,
                        IPConfig.networkCardIPs[i].interfaceName);
            }
            Interface_Display.ItemsSource = datagrid_ip;
            this.tbx_server_port.Text = HttpServer.SERVER_PORT.ToString();
            this.tbx_site_path.Text = HttpServer.SITE_PATH;
            this.tbx_thread_max.Text = HttpServer.SERVER_MAX_THREADS.ToString();
        }

        private void SelectIP_Click(object sender, SelectionChangedEventArgs e)
        {
            if (this.Interface_Display.SelectedIndex != -1)
            {
                this.tbx_server_port.Text = HttpServer.SERVER_PORT.ToString();
                this.tbx_site_path.Text = HttpServer.SITE_PATH;
                this.tbx_thread_max.Text = HttpServer.SERVER_MAX_THREADS.ToString();
                this.tbx_server_ip.Text = IPConfig.networkCardIPs[this.Interface_Display.SelectedIndex].interfaceIP;
            }
        }

        private void save_config(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == false)
            {
                HttpServer.SERVER_PORT = Convert.ToInt16(this.tbx_server_port.Text);
                HttpServer.SITE_PATH = this.tbx_site_path.Text;
                HttpServer.SERVER_MAX_THREADS = Convert.ToInt16(this.tbx_thread_max.Text);
                HttpServer.SERVER_ADDR = this.tbx_server_ip.Text;
                MessageBox.Show(
                    "服务器地址: " + HttpServer.SERVER_ADDR + "\n" +
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
                HttpServer.SERVER_MAX_THREADS = 10;
                MessageBox.Show(
                    "服务器地址: " + HttpServer.SERVER_ADDR + "\n" +
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

        private void show_connect(object sender, RoutedEventArgs e)
        {
            if (httpserver.PROC_RECORD.Count == 0)
            {
                MessageBox.Show("尚未有来自客户端的请求");
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
                    if (httpserver.PROC_RECORD[j].request == null)
                    {
                        //列表信息显示
                        datagrid_bd[j].Status = "N/A";
                        datagrid_bd[j].Method = "N/A";
                        datagrid_bd[j].URI = "N/A";
                        datagrid_bd[j].Version = "N/A";
                        datagrid_bd[j].Host = "N/A";
                        datagrid_bd[j].User_Agent = "N/A";
                        datagrid_bd[j].Accept = "N/A";
                        datagrid_bd[j].Accept_Language = "N/A";
                    }
                    else
                    {
                        //列表信息显示
                        datagrid_bd[j].Status = httpserver.PROC_RECORD[j].response.StatusCode + " " + httpserver.PROC_RECORD[j].response.ReasonPhrase;
                        datagrid_bd[j].Method = httpserver.PROC_RECORD[j].request.Method;
                        datagrid_bd[j].Version = httpserver.PROC_RECORD[j].request.Version;
                        datagrid_bd[j].URI = httpserver.PROC_RECORD[j].request.Uri;
                        datagrid_bd[j].Host = httpserver.PROC_RECORD[j].request.Header["Host"];
                        datagrid_bd[j].User_Agent = httpserver.PROC_RECORD[j].request.Header["User-Agent"];
                        datagrid_bd[j].Accept = httpserver.PROC_RECORD[j].request.Header["Accept"];
                        if(datagrid_bd[j].Accept_Language==null)
                        {
                            datagrid_bd[j].Accept_Language = "N/A";
                        }
                        else
                        {
                            datagrid_bd[j].Accept_Language = httpserver.PROC_RECORD[j].request.Header["Accept-Language"];
                        }
                        

                        //窗口信息显示
                        SummaryPanel moreinfo = new SummaryPanel();
                        moreinfo.RemoteIP.Text = httpserver.PROC_RECORD[j].RemoteIP + ":" + httpserver.PROC_RECORD[j].RemotePort;
                        moreinfo.RemoteMethod.Text = httpserver.PROC_RECORD[j].request.Method;
                        moreinfo.RemoteVersion.Text = httpserver.PROC_RECORD[j].request.Version;
                        moreinfo.RemoteStatue.Text = httpserver.PROC_RECORD[j].response.StatusCode + " " + httpserver.PROC_RECORD[j].response.ReasonPhrase;

                        //moreinfo.txb_useragent.Text = "User-Agent:  " + httpserver.PROC_RECORD[j].request.Header["User-Agent"];
                        //moreinfo.txb_uri.Text = "URI:  " + httpserver.PROC_RECORD[j].request.Uri;
                        //moreinfo.txb_accept.Text = "Accept:  " + httpserver.PROC_RECORD[j].request.Header["Accept"];
                        //moreinfo.txb_acceptlanguage.Text = "Accept-Language:  " + httpserver.PROC_RECORD[j].request.Header["Accept-Language"];
                        //moreinfo.txb_acceptencoding.Text = "Accept-Encoding:  " + httpserver.PROC_RECORD[j].request.Header["Accept-Encoding"];
                        //moreinfo.txb_connection.Text = "Connection:  " + httpserver.PROC_RECORD[j].response.Header["Connection"];
                        //moreinfo.txb_cachecontrol.Text = "Cache-control:  no-cache";
                        foreach (KeyValuePair<string, string> item in httpserver.PROC_RECORD[j].request.Header)
                        {
                            TextBlock txb = new TextBlock();
                            txb.Text = item.Key + ": " + item.Value;
                            txb.FontSize = 10;

                            moreinfo.Headers.Children.Add(txb);
                        }
                        this.connection_monitor.Children.Add(moreinfo);
                    }
                    dataGrid.ItemsSource = datagrid_bd;
                }
            }
        }
    }
}
