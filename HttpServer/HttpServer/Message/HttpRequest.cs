﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.HttpServer
{
    public class HttpRequest
    {
        public string Method { get; set; }
        public string Uri { get; set; }
        public string Ver { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
    }
}