using Prism.Mvvm;
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
        private ObservableCollection<FileItem> _AllItems;
        [XmlElement("FileItem")]
        public ObservableCollection<FileItem> AllItems
        {
            get
            {
                return _AllItems;
            }
            set
            {
                SetProperty(ref _AllItems, value);
            }
        }

        private ObservableCollection<FileItem> _DisplayItems;
        [XmlIgnore]
        public ObservableCollection<FileItem> DisplayItems
        {
            get
            {
                return _DisplayItems;
            }
            set
            {
                SetProperty(ref _DisplayItems, value);
            }
        }

        private int _SelectedIndex;
        [XmlIgnore]
        public int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                SetProperty(ref _SelectedIndex, value);
            }
        }

        private FileItem _SelectedItem;
        [XmlIgnore]
        public FileItem SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                SetProperty(ref _SelectedItem, value);
            }
        }

        public FileModel()
        {
            AllItems = new ObservableCollection<FileItem>();
            DisplayItems = AllItems;
        }
    }
}
