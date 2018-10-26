using FileTagger.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;

namespace FileTagger.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly string _XmlPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//FileTaggerData.xml";

        private FileModel _FileModel;
        public FileModel FileModel
        {
            get
            {
                return _FileModel;
            }
            set
            {
                SetProperty(ref _FileModel, value);
            }
        }

        private ObservableCollection<TagItem> _TagItems;
        public ObservableCollection<TagItem> TagItems
        {
            get
            {
                return _TagItems;
            }
            set
            {
                SetProperty(ref _TagItems, value);
            }
        }

        public ObservableCollection<TagItem> SelectedItags
        {
            get
            {
                return new ObservableCollection<TagItem>(TagItems.Where(x => x.IsSelected));
            }
        }

        public DelegateCommand AddFileCommand { get; set; }
        public DelegateCommand DeleteFileCommand { get; set; }
        public DelegateCommand ExecuteCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<string> SearchCommand { get; set; }

        public MainViewModel()
        {            
            AddFileCommand = new DelegateCommand(AddFile);
            DeleteFileCommand = new DelegateCommand(DeleteFile);
            ExecuteCommand = new DelegateCommand(Execute);
            SaveCommand = new DelegateCommand<object>(Save);
            SearchCommand = new DelegateCommand<string>(Search);
            InitData();
        }

        private void InitData()
        {
            if (File.Exists(_XmlPath))
            {
                using (FileStream fs = new FileStream(_XmlPath, FileMode.Open))
                {
                    using (XmlReader reader = XmlReader.Create(fs))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(FileModel));
                        FileModel = (FileModel)serializer.Deserialize(reader);
                    }
                }
            }
            else
            {
                FileModel = new FileModel();
            }

            foreach (var fileItem in FileModel.AllItems)
            {
                fileItem.Icon = CreateIcon(fileItem.FIleName);
            }

            TagItems = new ObservableCollection<TagItem>();
            List<string> list = new List<string>();

            foreach(var item in FileModel.AllItems)
            {
                list.AddRange(item.Tags);
            }
            var tags = list.Distinct();
            foreach(var item in tags)
            {
                TagItems.Add(new TagItem() { Name = item });
            }
        }

        private void AddFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "파일 추가";

            if (openFileDialog.ShowDialog() == true)
            {
                for(int i = 0; i < openFileDialog.FileNames.Count(); i++ )
                {
                    int count = FileModel.AllItems.Count(x => x.FIleName == openFileDialog.FileNames[i]);
                    if(count > 0)
                    {
                        MessageBox.Show("이미 등록된 파일입니다.");
                        return;
                    }
                }
                for(int i = 0; i< openFileDialog.FileNames.Count(); i++)
                {
                    FileItem file = new FileItem
                    {
                        FIleName = openFileDialog.FileNames[i]
                        ,SafeFileName = openFileDialog.SafeFileNames[i]
                        ,Icon = CreateIcon(openFileDialog.FileNames[i])
                    };
                    FileModel.AllItems.Add(file);
                }
                WriteXml();
            }
        }

        private BitmapSource CreateIcon(string fileName)
        {
            Icon icon = Icon.ExtractAssociatedIcon(fileName);
            BitmapSource bitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
            icon.Dispose();
            return bitmap;
        }

        private void DeleteFile()
        {
            var items = FileModel.SelectedItems;
            if(items.Count() > 0)
            {
                if (MessageBox.Show("파일을 삭제하시겠습니까?", "파일 제거", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach(var item in items)
                    {
                        FileModel.AllItems.Remove(item);
                        FileModel.DisplayItems.Remove(item);
                    }                    
                    WriteXml();
                }               
            }
        }

        private void Execute()
        {
            var item = FileModel.SelectedItem;
            if (item != null)
            {
                Process.Start(item.FIleName);
            }           
        }

        private void Search(string text)
        {
            List<string> tags = new List<string>();
            foreach (TagItem item in SelectedItags)
            {
                tags.Add(item.Name);
            }

            if (tags.Count > 0)
            {
                var items = FileModel.AllItems;
                var result = from item in items
                              where tags.All(x => item.Tags.Contains(x))
                              select item;

                if (!string.IsNullOrEmpty(text))
                {
                    result = result.Where(x => x.FIleName.Contains(text));
                }
                
                FileModel.DisplayItems = new ObservableCollection<FileItem>(result);
            }
            else if(!string.IsNullOrEmpty(text))
            {
                FileModel.DisplayItems = new ObservableCollection<FileItem>(FileModel.AllItems.Where(x => x.SafeFileName.Contains(text)));
            }
            else
            {
                FileModel.DisplayItems = FileModel.AllItems;
            }
        }

        private void Save(object parameter)
        {
            if(FileModel.SelectedItem != null)
            {
                var values = (object[])parameter;
                FileModel.SelectedItem.Tags = (List<string>)values[0];
                FileModel.SelectedItem.Description = (string)values[1];
                WriteXml();

                MessageBox.Show("저장되었습니다.");
            }           
        }

        private void WriteXml()
        {
            XmlSerializer writer = new XmlSerializer(typeof(FileModel));
            using (FileStream fs = File.Create(_XmlPath))
            {
                writer.Serialize(fs, FileModel);
            }
        }
    }
}
