using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Pereverter.Annotations;

namespace Pereverter
{
    class MainWindowVM: INotifyPropertyChanged
    {
        private DataFile _data = null;
        private Lengths _lengths = null;

        private string _dataPath;
        private string _lengthPath;
        public string DataPath
        {
            get => _dataPath;
            set { _dataPath = value; OnPropertyChanged();}
        }
        public string LengthPath
        {
            get => _lengthPath;
            set { _lengthPath = value; OnPropertyChanged();}
        }
           
        public Cmd SetLengthPathCmd { get; } = new Cmd();
        public Cmd SetDataPathCmd { get; } = new Cmd();
        public Cmd CalculateCmd { get; } = new Cmd();
        public Cmd PasteDataPathCmd { get; } = new Cmd();
        public Cmd PasteLengthCmd { get; } = new Cmd();
     
        public MainWindowVM()
        {
            SetDataPathCmd.Executed  += SetDataPathCmdOnExecuted;
            SetLengthPathCmd.Executed+= SetLengthPathCmdOnExecuted;
            PasteDataPathCmd.Executed+= PasteDataPathCmdOnExecuted;
            PasteLengthCmd.Executed  += PasteLengthCmdOnExecuted;
            CalculateCmd.Executed    += CalculateCmdOnExecuted;
        }
        private void PasteDataPathCmdOnExecuted() {
            try {
                var clipboard =Clipboard.GetText();
                if(string.IsNullOrWhiteSpace(clipboard))
                    throw new InvalidOperationException("Содержимое буфера обмена пусто");
                _data = DataFile.Parse(clipboard);
                DataPath = $"Вставлено из буффера {_data.Items.Length} строк ";
            }
            catch (Exception e) {
                MessageBox.Show($"Ошибка вставки из буфера: {e.Message}");
            }
        }
        
        private void PasteLengthCmdOnExecuted() {
            try {
                var clipboard =Clipboard.GetText();
                if(string.IsNullOrWhiteSpace(clipboard))
                    throw new InvalidOperationException("Содержимое буфера обмена пусто");
                _lengths = Lengths.Parse(clipboard);
                LengthPath = $"Вставлено из буффера {_lengths.Count} строк ";
            }
            catch (Exception e) {
                MessageBox.Show($"Ошибка вставки из буфера: {e.Message}");
            }
        }
       
        private void SetDataPathCmdOnExecuted()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != true)
                return;
            var dataPath = ofd.FileName;
            if (string.IsNullOrWhiteSpace(dataPath))
            {
                MessageBox.Show("Введите путь к файлу с данными");
                return;
            }
            if (!File.Exists(dataPath))
            {
                MessageBox.Show("Файл с данными не найден по указанному пути");
                return;
            }
            try {
                _data = DataFile.ReadFromFile(dataPath);
                DataPath = dataPath;
            }
            catch (InvalidDataException e) {
                MessageBox.Show(e.Message);
            }
        }
        
        private void SetLengthPathCmdOnExecuted()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != true)
                return;
            var lengthPath = ofd.FileName;
            if (string.IsNullOrWhiteSpace(lengthPath))
            {
                MessageBox.Show("Введите путь к файлу протяженностей");
                return;
            }

            if (!File.Exists(lengthPath))
            {
                MessageBox.Show("Файл протяженностей не найден по указанному пути");
                return;
            }
            try {
                _lengths = Lengths.ReadFromFile(lengthPath);
                LengthPath = lengthPath;
            } catch (InvalidDataException e) {
                MessageBox.Show(e.Message);
            }
        }

        private void CalculateCmdOnExecuted()
        {
            if (_lengths==null)
            {
                MessageBox.Show("Введите путь к файлу протяженностей или вставьте их из буффера обмена");
                return;
            }
            if (_data == null)
            {
                MessageBox.Show("Введите путь к файлу c данными или вставьте их из буффера обмена");
                return;
            }

            var resultString = new StringBuilder();
            foreach (var item in _data.Items.Reverse())
            {
                var(km, m) =  _lengths.GetReversed(item.Km, item.M);
                var newItem = new DataItem(km, m, item.Data);
                newItem.Append(resultString);
            }
            Clipboard.SetText(resultString.ToString());
            MessageBox.Show("Результат скопирован в буффер обмена");
        }

    

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
