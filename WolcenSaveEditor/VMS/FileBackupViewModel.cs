﻿using GalaSoft.MvvmLight;
using System;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using WolcenFileManagers;

namespace WolcenSaveEditor.VMS
{
    public class FileBackupViewModel : ViewModelBase
    {
        private readonly IFileBackup _fileBackup;
        private bool _isDirectoryValid;
        private string _saveDirectory;

        public FileBackupViewModel(IFileBackup fileBackup)
        {
            _fileBackup = fileBackup;
            fileBackup.SaveDirectory = GetDefaultSaveDirectory();

            SaveDirectory = fileBackup.SaveDirectory.FullName;
            IsDirectoryValid = fileBackup.LoadDirectory();
        }

        public string SaveDirectory
        {
            get => _saveDirectory;
            set
            {
                _saveDirectory = value;
                _fileBackup.SaveDirectory = new DirectoryInfo(value);
                IsDirectoryValid = _fileBackup.LoadDirectory();
                RaisePropertyChanged();
            }
        }

        public bool IsDirectoryValid
        {
            get => _isDirectoryValid;
            set
            {
                _isDirectoryValid = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SaveCommand => new RelayCommand(PerformSave);

        public RelayCommand RestoreCommand => new RelayCommand(PerformRestore);

        private void PerformRestore()
        {
            if (!IsDirectoryValid) return;

            var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "zip files (*.zip)|*.zip"
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                _fileBackup.SaveDirectory.Delete(true);
                var (result, message) = _fileBackup.RestoreDirectory(openFileDialog.FileName);
                if (!result)
                {
                    MessageBox.Show(message);
                }
            }
        }

        public void PerformSave()
        {
            if (!IsDirectoryValid) return;

            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "zip files (*.zip)|*.zip"
            };

            if (saveFileDialog.ShowDialog().GetValueOrDefault())
            {
                var (result, message) = _fileBackup.BackUpDirectory(saveFileDialog.FileName);
                if (!result)
                {
                    MessageBox.Show(message);
                }
            }
        }
        private DirectoryInfo GetDefaultSaveDirectory()
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return new DirectoryInfo(@$"{basePath}/Saved Games/wolcen/savegames");
        }
    }
}
