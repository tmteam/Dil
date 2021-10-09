using System;
using System.IO;
using System.Text;
using System.Windows;
using Dil.Core.Entities;
using Microsoft.Win32;

namespace Dil.Core
{
    public static class UiOperationsHelper
    {
        public static (string, DataItems) TryReadDataFileWithOfd()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != true)
                return default;
            var dataPath = ofd.FileName;
            if (string.IsNullOrWhiteSpace(dataPath))
            {
                MessageBox.Show("Введите путь к файлу с данными");
                return default;
            }
            if (!File.Exists(dataPath))
            {
                MessageBox.Show("Файл с данными не найден по указанному пути");
                return default;
            }
            try {
                return (dataPath, DataItems.ReadFromFile(dataPath));
            }
            catch (InvalidDataException e) {
                MessageBox.Show(e.Message);
            }
            return default;
        }
        
        public static T TryGetFromClipboard<T>(Func<string,T> parser)
        {
            try
            {
                var clipboard = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboard))
                    throw new InvalidOperationException("Содержимое буфера обмена пусто");
                return parser(clipboard);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка вставки из буфера: {e.Message}");
            }

            return default;
        }

        public static void SetDataToClipboard(string data)
        {
            Clipboard.SetText(data);
            MessageBox.Show("Результат скопирован в буффер обмена");
        }
    }
}