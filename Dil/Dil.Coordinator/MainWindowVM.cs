using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Dil.Core;
using Dil.Core.Entities;
using Microsoft.Win32;

namespace Dil.Coordinator
{
    class MainWindowVM : VMBase
    {
        private DataItems _data = null;

        private string _dataPath;
        private string _coordinatePath;
        private Coordinates _coordinates;

        public string DataPath
        {
            get => _dataPath;
            set
            {
                _dataPath = value;
                OnPropertyChanged();
            }
        }

        public string CoordinatePath
        {
            get => _coordinatePath;
            set
            {
                _coordinatePath = value;
                OnPropertyChanged();
            }
        }

        public Cmd SetCoordinatePathCmd { get; } = new Cmd();
        public Cmd SetDataPathCmd { get; } = new Cmd();
        public Cmd CalculateCmd { get; } = new Cmd();
        public Cmd PasteDataPathCmd { get; } = new Cmd();
        public Cmd PasteCoordinateCmd { get; } = new Cmd();

        public MainWindowVM()
        {
            SetDataPathCmd.Executed += SetDataPathCmdOnExecuted;
            SetCoordinatePathCmd.Executed += SetLengthPathCmdOnExecuted;
            PasteDataPathCmd.Executed += PasteDataPathCmdOnExecuted;
            PasteCoordinateCmd.Executed += PasteCoordinatesCmdOnExecuted;
            CalculateCmd.Executed += CalculateCmdOnExecuted;
        }

        private void PasteDataPathCmdOnExecuted()
        {
            _data = UiOperationsHelper.TryGetFromClipboard(DataItems.Parse);
            if (_data != null)
                DataPath = $"Вставлено из буффера {_data.Items.Length} строк ";
        }

        private void SetDataPathCmdOnExecuted()
        {
            var (path, data) = UiOperationsHelper.TryReadDataFileWithOfd();
            if (data == null)
                return;
            _data = data;
            DataPath = path;
        }

        private void PasteCoordinatesCmdOnExecuted()
        {
            _coordinates = UiOperationsHelper.TryGetFromClipboard(Coordinates.Parse);
            if (_coordinates != null)
                CoordinatePath = $"Вставлено из буффера {_coordinates.Items.Length} строк ";
        }


        private void SetLengthPathCmdOnExecuted()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != true)
                return;
            var lengthPath = ofd.FileName;
            if (string.IsNullOrWhiteSpace(lengthPath))
            {
                MessageBox.Show("Введите путь к файлу координат");
                return;
            }

            if (!File.Exists(lengthPath))
            {
                MessageBox.Show("Файл координат не найден по указанному пути");
                return;
            }

            try
            {
                _coordinates = Coordinates.ReadFromFile(lengthPath);
                CoordinatePath = lengthPath;
            }
            catch (InvalidDataException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void CalculateCmdOnExecuted()
        {
            if (_coordinates == null)
            {
                MessageBox.Show("Введите путь к файлу координат или вставьте их из буффера обмена");
                return;
            }

            if (_data == null)
            {
                MessageBox.Show("Введите путь к файлу c данными или вставьте их из буффера обмена");
                return;
            }

            var resultString = new StringBuilder();
            foreach (var item in _data.Items)
            {
                var coordinateInfo = _coordinates.GetOrNull(item.Distance);
                if (coordinateInfo == null)
                    continue;

                var newItem = new DataItem(
                    distance: coordinateInfo.Distance,
                    data: new[]
                        {
                            coordinateInfo.LatitudeMajor.ToString(),
                            coordinateInfo.LatitudeMinor.ToString(CultureInfo.InvariantCulture),
                            coordinateInfo.LongitudeMajor.ToString(),
                            coordinateInfo.LongitudeMinor.ToString(CultureInfo.InvariantCulture)
                        }
                        .Concat(item.Data)
                        .ToArray());
                newItem.AppendTo(resultString);
            }

            if (resultString.Length == 0)
                MessageBox.Show("Нет сопоставлений из данных в координат");
            else
                UiOperationsHelper.SetDataToClipboard(resultString.ToString());
        }
    }
}