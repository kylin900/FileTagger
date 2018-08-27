﻿using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileTagger.Models
{
 
    public class FileModel : BindableBase
    {
        private ObservableCollection<FileItem> _Items;

        [XmlElement("FileItem")]
        public ObservableCollection<FileItem> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                SetProperty(ref _Items, value);
            }
        }

        public FileModel()
        {
            Items = new ObservableCollection<FileItem>();
        }
    }
}