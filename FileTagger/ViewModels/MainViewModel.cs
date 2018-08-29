using FileTagger.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public DelegateCommand AddFileCommand { get; set; }
        public DelegateCommand DeleteFileCommand { get; set; }

        public MainViewModel()
        {            
            AddFileCommand = new DelegateCommand(AddFile);
            DeleteFileCommand = new DelegateCommand(DeleteFile);
            LoadData();
        }

        private void LoadData()
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
                        FIleName = openFileDialog.FileNames[i],
                        SafeFileName = openFileDialog.SafeFileNames[i]
                    };
                    FileModel.AllItems.Add(file);
                }
                WriteXml();
            }
        }

        private void DeleteFile()
        {
            int index = FileModel.SelectedIndex;
            if(index > -1)
            {
                FileModel.AllItems.RemoveAt(index);
                WriteXml();

                if(index > 0)
                {
                    FileModel.SelectedIndex = index - 1;
                }
                else if(index == 0 && FileModel.AllItems.Count > 0)
                {
                    FileModel.SelectedIndex = 0;
                }
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
