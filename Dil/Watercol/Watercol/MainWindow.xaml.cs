using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Dil.Core;
using Dil.Core.Entities;

namespace Watercol
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private LinkedList<NumberDataItem> _entries = null;
        private IList<NumberDataItem> _results = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void PasteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Clipboard.ContainsText())
            {
                MessageBox.Show("Буфер обмена пуст");
                return;
            }

            try
            {
                StubLbl.Content = "Идет обработка...";
                StubLbl.Visibility = Visibility.Visible;
                var data = Clipboard.GetText();
                var lines = await Task.Run(() => DilHelper.SplitLines(data));

                _entries = new LinkedList<NumberDataItem>();
                _results = new List<NumberDataItem>();
                for (int i = 0; i < lines.Length; i++)
                {
                    try
                    {
                        var result = NumberDataItem.ParseOrThrow(lines[i]);
                        _entries.AddLast(result);
                    }
                    catch (InvalidDataException ex)
                    {
                        MessageBox.Show($"Ошибка в строке {i}:\r\n{ex.Message} ");
                        return;
                    }
                }

                if (!_entries.Any())
                {
                    MessageBox.Show("Буфер обмена пуст");
                    return;
                }

                _results = _entries.Select(e => WaterColRow.FromRow(e).ToNumberDataItem()).ToList();
                
                var resultText  = await Task.Run(()=> DilHelper.ConvertTableToFormatedText(_entries));
                var filtredText = await Task.Run(()=> DilHelper.ConvertTableToFormatedText(_results));

                TableTxt.Text = resultText;
                ResultTxt.Text = filtredText;
            }
            catch (Exception)
            {
                MessageBox.Show("Неопознанная ошибка обработки данных. Проверьте исходные данные и повторите попытку");
                _entries.Clear();
                _results.Clear();
            }
            finally
            {
                if (_entries == null)
                    originLinesLbl.Content = "Строки не загружены";
                else
                    originLinesLbl.Content = "Строк: " + _entries.Count;

                if (_results == null)
                    originLinesLbl.Content = "";
                else
                    resultLinesLbl.Content = "Строк: " + _results.Count;
                StubLbl.Visibility = Visibility.Collapsed;
            }
        }

        private void CopyBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_results == null || !_results.Any())
                MessageBox.Show("Нету данных для копирования");
            else
            {
                var output = _results.ToFormattedString();
                Clipboard.SetText(output);
                MessageBox.Show("Результаты скопированны в буффер. Вставьте их в таблицу Excel или текстовый файл");
            }
        }
    }
}