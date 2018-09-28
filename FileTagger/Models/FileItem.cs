using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace FileTagger.Models
{
    public class FileItem : BindableBase
    {
        public string FIleName { get; set; }
        public string SafeFileName { get; set; }
        [XmlIgnore]
        public BitmapSource Icon { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string Description { get; set; }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                SetProperty(ref _IsSelected, value);
            }
        }
    }
}
