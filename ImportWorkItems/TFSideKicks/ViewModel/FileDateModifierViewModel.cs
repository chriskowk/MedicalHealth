using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Globalization;
using System.Threading;
using SharpCompress.Writer;
using SharpCompress.Common;
using System.Diagnostics;
using TFSideKicks.Helpers;

namespace TFSideKicks
{
    /// <summary>
    /// The MainViewModel. Provides interaction logic for the View.
    /// </summary>
    public class FileDateModifierViewMode : INotifyPropertyChanged
    {
        #region Fields
        /// <summary>
        /// Backing field for the BrowseCommand property.
        /// </summary>
        private RelayCommand _browseCommand;
        /// <summary>
        /// Backing field for the UpdateDateCommand property.
        /// </summary>
        private RelayCommand _updateDateCommand;
        /// <summary>
        /// 
        /// </summary>
        private RelayCommand _execWinRARCommand;
        /// <summary>
        /// 
        /// </summary>
        private RelayCommand _TFGetLatestCommand;
        /// <summary>
        /// Backing field for the EncryptCommand property.
        /// </summary>
        private RelayCommand _encryptCommand;
        /// <summary>
        /// Backing field for the SelectedPath property.
        /// </summary>
        private string _selectedPath;
        /// <summary>
        /// Keeps track of the number of files updated.
        /// </summary>
        private int _fileUpdatedCount = 0;
        #endregion // Fields

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDateModifierViewMode"/> class.
        /// </summary>
        public FileDateModifierViewMode()
        {
            DirPathSelected = true;
            SelectedPath = "F:\\MedicalHealthSYCode";
            CultureInfo = CultureInfo.CurrentCulture;

            var defaultTime = DateTime.Now;
            CreatedDateTime = defaultTime;
            ModifiedDateTime = defaultTime;
            AccessedDateTime = defaultTime;

            _browseCommand = new RelayCommand(obj =>
            {
                // Gets/updates the selected path based on path type
                //
                if (DirPathSelected)
                {
                    var dialog = new FolderBrowserDialog();
                    var result = dialog.ShowDialog();

                    if (DialogResult.OK == result)
                    {
                        SelectedPath = dialog.SelectedPath;
                    }
                }
                else if (FilePathSelected)
                {
                    var dialog = new OpenFileDialog();
                    var result = dialog.ShowDialog();

                    if (DialogResult.OK == result)
                    {
                        SelectedPath = dialog.FileName;
                    }
                }
            });

            _updateDateCommand = new RelayCommand(obj =>
            {
                // Updates the file(s) based on selected dates
                //
                if (!String.IsNullOrEmpty(SelectedPath))
                {
                    if (File.Exists(SelectedPath))
                    {
                        updateFileDateInfo(SelectedPath, CreatedDateTime, AccessedDateTime, ModifiedDateTime);
                        MessageBox.Show("Successfully updated!", "Update Successful");
                        resetControls();
                    }
                    else if (Directory.Exists(SelectedPath))
                    {
                        foreach (var f in Directory.GetFiles(SelectedPath))
                        {
                            _fileUpdatedCount += 1;
                            updateFileDateInfo(f, CreatedDateTime, AccessedDateTime, ModifiedDateTime);
                        }

                        updateDirDateInfo(SelectedPath);
                        MessageBox.Show(string.Format("Successfully updated {0} file(s)!", _fileUpdatedCount), "Update Successful");

                        _fileUpdatedCount = 0;
                        resetControls();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a file or directory", "Update");
                }
            });

            _execWinRARCommand = new RelayCommand(obj =>
             {
                 if (!String.IsNullOrEmpty(SelectedPath))
                 {
                     if (File.Exists(SelectedPath))
                     {
                         WinRARHelper.Compress(SelectedPath, Path.Combine("F:\\FDiskTemp", $"{Path.GetFileNameWithoutExtension(SelectedPath)}.rar"), true);
                     }
                     else if (Directory.Exists(SelectedPath))
                     {
                         TFGetLatestVersion();
                         WinRARHelper.Compress(SelectedPath, Path.Combine("F:\\FDiskTemp", $"{Path.GetFileNameWithoutExtension(SelectedPath)}.rar"));
                     }
                 }
                 else
                 {
                     MessageBox.Show("Please select a file or directory", "Compress");
                 }
             });

            _TFGetLatestCommand = new RelayCommand(obj =>
            {
                TFGetLatestVersionWithConsole();
            });

            _encryptCommand = new RelayCommand(obj =>
            {
                DoEncrypt();
            });
        }
        #endregion // Ctor

        #region Properties
        /// <summary>
        /// Gets the browse command.
        /// </summary>
        public ICommand BrowseCommand => _browseCommand;

        /// <summary>
        /// Gets the encrypt command.
        /// </summary>
        public ICommand EncryptCommand => _encryptCommand;

        /// <summary>
        /// Gets the update date command.
        /// </summary>
        public ICommand UpdateDateCommand => _updateDateCommand;

        /// <summary>
        /// Gets the ExecWinRAR command.
        /// </summary>
        public ICommand ExecWinRARCommand => _execWinRARCommand;
        /// <summary>
        /// 执行当前运行路径下的批命令（TF_GET_MedicalHealth.bat）
        /// </summary>
        public ICommand TFGetLatestCommand => _TFGetLatestCommand;

        /// <summary>
        /// Gets the current culture info.
        /// </summary>
        public CultureInfo CultureInfo { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [created date checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [created date checked]; otherwise, <c>false</c>.
        /// </value>
        public bool CreatedChecked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [accessed date checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [accessed date checked]; otherwise, <c>false</c>.
        /// </value>
        public bool AccessedChecked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [modified date checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [modified date checked]; otherwise, <c>false</c>.
        /// </value>
        public bool ModifiedChecked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [file path selected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [file path selected]; otherwise, <c>false</c>.
        /// </value>
        public bool FilePathSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [dir path selected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [dir path selected]; otherwise, <c>false</c>.
        /// </value>
        public bool DirPathSelected { get; set; }

        /// <summary>
        /// Gets or sets the created date time.
        /// </summary>
        /// <value>
        /// The created date time.
        /// </value>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the modified date time.
        /// </summary>
        /// <value>
        /// The modified date time.
        /// </value>
        public DateTime ModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the accessed date time.
        /// </summary>
        /// <value>
        /// The accessed date time.
        /// </value>
        public DateTime AccessedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the selected file/dir path.
        /// </summary>
        /// <value>
        /// The selected path.
        /// </value>
        public string SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                _selectedPath = value;
                OnPropertyChanged("SelectedPath");
            }
        }

        private string _plainText;
        public string PlainText
        {
            get { return _plainText; }
            set
            {
                _plainText = value;
                OnPropertyChanged("PlainText");
            }
        }

        private string _commandText = "Encrypt";
        public string CommandText
        {
            get { return _commandText; }
            set
            {
                _commandText = value;
                OnPropertyChanged("CommandText");
            }
        }

        #endregion // Properties

        #region Methods
        /// <summary>
        /// Resets the UI controls.
        /// </summary>
        private void resetControls()
        {
            SelectedPath = string.Empty;
        }

        /// <summary>
        /// Updates the date attribute information for a file.
        /// </summary>
        /// <param name="file">The file path</param>
        /// <param name="createdTime">The created time.</param>
        /// <param name="accessedTime">The accessed time.</param>
        /// <param name="modifiedTime">The modified time.</param>
        private void updateFileDateInfo(string file, DateTime createdTime, DateTime accessedTime, DateTime modifiedTime)
        {
            if (CreatedChecked)
            {
                File.SetCreationTime(file, createdTime);
            }

            if (AccessedChecked)
            {
                File.SetLastAccessTime(file, accessedTime);
            }

            if (ModifiedChecked)
            {
                File.SetLastWriteTime(file, modifiedTime);
            }
        }

        /// <summary>
        /// Updates the directory date infomation.
        /// </summary>
        /// <param name="dir">The directory</param>
        private void updateDirDateInfo(string dir)
        {
            foreach (var d in Directory.GetDirectories(dir))
            {
                foreach (var f in Directory.GetFiles(d))
                {
                    _fileUpdatedCount += 1;
                    updateFileDateInfo(f, CreatedDateTime, AccessedDateTime, ModifiedDateTime);
                }

                updateDirDateInfo(d);
            }
        }

        private void DoEncrypt()
        {
            bool isEncrypt = CommandText == "Encrypt";
            try
            {
                PlainText = isEncrypt ? EncryptHepler.Encrypt(PlainText) : EncryptHepler.Decrypt(PlainText);
            }
            catch (Exception)
            {
            }
            finally
            {
                isEncrypt = !isEncrypt;
                CommandText = isEncrypt ? "Encrypt" : "Decrypt";
            }
        }

        private static string _filename = Path.Combine(Environment.CurrentDirectory, "TF_GET_MedicalHealth.bat");
        /// <summary>
        /// 
        /// </summary>
        private void TFGetLatestVersionWithConsole()
        {
            if (!File.Exists(_filename)) return;

            string errMsg = string.Empty;
            string filename = $"{_filename} pause";
            Utility.Execute(filename, 60, false, false, ref errMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        private void TFGetLatestVersion()
        {
            if (!File.Exists(_filename)) return;

            string errMsg = string.Empty;
            string output = Utility.Execute(_filename, 60, true, true, ref errMsg);
            string logPath = Path.Combine(Environment.CurrentDirectory, "Log");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            FileStream fs = new FileStream(Path.Combine(logPath, string.Format("TFGetLog{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"))), FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(string.Format("Output: {0}\r\nErrorMsg:\r\n{1}", output, errMsg));
            sw.Close();
            fs.Close();
        }

        #endregion // Methods

        #region INotifyPropertyChanged
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion // INotifyPropertyChanged
    }
}
