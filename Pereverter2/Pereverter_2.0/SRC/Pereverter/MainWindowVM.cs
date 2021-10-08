using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Pereverter.Annotations;

namespace Pereverter
{
    class MainWindowVM: INotifyPropertyChanged
    {
        private string _lengthPath;
        private string _dataPath;

        public string LengthPath
        {
            get => _lengthPath;
            set { _lengthPath = value; OnPropertyChanged();}
        }

        public string DataPath
        {
            get => _dataPath;
            set { _dataPath = value; OnPropertyChanged();}
        }
           
        public Cmd SetLengthPathCmd { get; } = new Cmd();
        public Cmd SetDataPathCmd { get; } = new Cmd();
        public Cmd CalculateCmd { get; } = new Cmd();

        public MainWindowVM()
        {
            SetDataPathCmd.Executed+= SetDataPathCmdOnExecuted;
            SetLengthPathCmd.Executed+= SetLengthPathCmdOnExecuted;
            CalculateCmd.Executed+= CalculateCmdOnExecuted;
        }

        private void CalculateCmdOnExecuted()
        {
            if (string.IsNullOrWhiteSpace(_lengthPath))
            {
                MessageBox.Show("Введите путь к файлу протяженностей");
                return;
            }

            if (!File.Exists(_lengthPath))
            {
                MessageBox.Show("Файл протяженностей не найден по указанному пути");
                return;
            }
            Lengths lengths;
            try {
                lengths = Lengths.ReadFromFile(_lengthPath);
            } catch (InvalidDataException e) {
                MessageBox.Show(e.Message);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(_dataPath))
            {
                MessageBox.Show("Введите путь к файлу c данными");
                return;
            }

            if (!File.Exists(_dataPath))
            {
                MessageBox.Show("Файл c данными не найден по указанному пути");
                return;
            }
            DataFile dataFile;
            try {
                dataFile = DataFile.ReadFromFile(_dataPath);
            } catch (InvalidDataException e) {
                MessageBox.Show(e.Message);
                return;
            }
            
            var resultString = new StringBuilder();
            foreach (var item in dataFile.Items)
            {
                var(km, m) =  lengths.GetReversed(item.Km, item.M);
                var newItem = new DataItem(km, m, item.Data);
                newItem.Append(resultString);
            }
            Clipboard.SetText(resultString.ToString());
            MessageBox.Show("Результат скопирован в буффер обмена");
        }

        private void SetLengthPathCmdOnExecuted()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true) 
                LengthPath = ofd.FileName;
        }

        private void SetDataPathCmdOnExecuted()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true) 
                DataPath = ofd.FileName;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
