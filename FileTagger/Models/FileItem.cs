﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileTagger.Models
{
    public class FileItem
    {
        public string FIleName { get; set; }
        public string SafeFileName { get; set; }
    }
}