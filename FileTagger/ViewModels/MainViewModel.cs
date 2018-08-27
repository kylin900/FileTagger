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
using System.Xml;
using System.Xml.Serialization;

namespace FileTagger.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly string xmlPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//FileTaggerData.xml";
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

        public MainViewModel()
        {            
            AddFileCommand = new DelegateCommand(AddFile);

            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(xmlPath))
            {
                using (FileStream fs = new FileStream(xmlPath, FileMode.Open))
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

        public void AddFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {              
                Models.FileItem file = new Models.FileItem();
                file.FIleName = openFileDialog.FileName;
                file.SafeFileName = openFileDialog.SafeFileName;

                FileModel.Items.Add(file);

                XmlSerializer writer = new XmlSerializer(typeof(FileModel));
                using (FileStream fs = File.Create(xmlPath))
                {
                    writer.Serialize(fs, FileModel);
                }
            }             
        }
    }
}
