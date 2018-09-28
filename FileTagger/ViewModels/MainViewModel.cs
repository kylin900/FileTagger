﻿using FileTagger.Models;
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

            foreach (var fileItem in FileModel.AllItems)
            {
                fileItem.Icon = CreateIcon(fileItem.FIleName);
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
            var items = FileModel.SelectedItems;
            if (items.Count() > 0)
            {
                if (File.Exists(FileModel.SelectedItem.FIleName))
                {
                    foreach (var item in items)
                    {
                        Process.Start(item.FIleName);
                    }                    
                }
            }           
        }

        private void Search(string text)
        {
            if(!string.IsNullOrEmpty(text))
            {
                string[] tags = text.Trim().Split(' ');
                var items = FileModel.AllItems;
                var result = from item in items
                             where item.Tags.Intersect(tags).Any()
                             select item;
                FileModel.DisplayItems = new ObservableCollection<FileItem>(result);
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
            }           
        }

        private void SelectAllFiles()
        {
            return;
            var items = FileModel.DisplayItems;
            foreach(var item in items)
            {
                item.IsSelected = true;
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
