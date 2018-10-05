using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTagger.Models
{
    public class TagItem : BindableBase
    {
        public string Name { get; set; }
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
