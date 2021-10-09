using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Dil.Core;
using Dil.Core.Entities;
using Microsoft.Win32;

namespace Pereverter
{
    class MainWindowVM : VMBase
    {
        private DataItems _data = null;
        private Lengths _lengths = null;

        private string _dataPath;
        private string _lengthPath;

        public string DataPath
        {
            get => _dataPath;
            set
            {
                _dataPath = value;
                OnPropertyChanged();
            }
        }

        public string LengthPath
        {
            get => _lengthPath;
            set
            {
                _lengthPath = value;
                OnPropertyChanged();
            }
        }

        public Cmd SetLengthPathCmd { get; } = new Cmd();
        public Cmd SetDataPathCmd { get; } = new Cmd();
        public Cmd CalculateCmd { get; } = new Cmd();
        public Cmd PasteDataPathCmd { get; } = new Cmd();
        public Cmd PasteLengthCmd { get; } = new Cmd();

        public MainWindowVM()
        {
            SetDataPathCmd.Executed += SetDataPathCmdOnExecuted;
            SetLengthPathCmd.Executed += SetLengthPathCmdOnExecuted;
            PasteDataPathCmd.Executed += PasteDataPathCmdOnExecuted;
            PasteLengthCmd.Executed += PasteLengthCmdOnExecuted;
            CalculateCmd.Executed += CalculateCmdOnExecuted;
        }

        private void PasteDataPathCmdOnExecuted()
        {
            _data = UiOperationsHelper.TryGetFromClipboard(DataItems.Parse);
            if (_data != null)
                DataPath = $"Вставлено из буффера {_data.Items.Length} строк ";
        }

        private void PasteLengthCmdOnExecuted()
        {
            _lengths = UiOperationsHelper.TryGetFromClipboard(Lengths.Parse);
            if (_lengths != null)
                LengthPath = $"Вставлено из буффера {_data.Items.Length} строк ";
        }

        private void SetDataPathCmdOnExecuted()
        {
            var (path, data) = UiOperationsHelper.TryReadDataFileWithOfd();
            if(data==null)
                return;
            _data = data;
            DataPath = path;
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

            try
            {
                _lengths = Lengths.ReadFromFile(lengthPath);
                LengthPath = lengthPath;
            }
            catch (InvalidDataException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void CalculateCmdOnExecuted()
        {
            if (_lengths == null)
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
                var (km, m) = _lengths.GetReversed(item.Km, item.M);
                var newItem = new DataItem(km, m, item.Data);
                newItem.AppendTo(resultString);
            }

            UiOperationsHelper.SetDataToClipboard(resultString.ToString());
        }
    }
}