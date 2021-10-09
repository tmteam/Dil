using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dil.Core.Entities;

namespace Usrednator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            midIntervalTxt.Text = "10";
        }

        private LinkedList<NumberDataItem> _entries = null;
        private LinkedList<NumberDataItem> _results = null;

        private async void PasteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(midIntervalTxt.Text.Trim(), out var averageMeters))
            {
                MessageBox.Show("Неверный интервал осреднения");
                return;
            }

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
                var lines = await Task.Run(()=>Helper.SplitLines(data));
                _entries = new LinkedList<NumberDataItem>();
                _results = new LinkedList<NumberDataItem>();
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

                var useZeroPivots = zeroPivotChbx.IsChecked ==true;

                if (useZeroPivots)
                    Helper.SetZeroPivot(_entries, averageMeters);

                var resultText  = await Task.Run(()=> Helper.ConvertTableToFormatedText(_entries));
                _results        = await Task.Run(()=> Helper.AverageFilterSync(_entries, averageMeters, useZeroPivots));
                var filtredText = await Task.Run(()=> Helper.ConvertTableToFormatedText(_results));

                TableTxt.Text = resultText;
                ResultTxt.Text = filtredText;

            }
            catch(Exception)
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


        private void CopyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_results == null || !_results.Any())
                MessageBox.Show("Нету данных для копирования");
            else
            {
                var filtredText = new StringBuilder();
                foreach (var entry in _results)
                {
                    filtredText.AppendLine($"{entry.Distance.Km}\t{entry.Distance.M}\t{string.Join("\t", entry.Data.Select(d => d.ToString("F2")))}");
                }

                Clipboard.SetText(filtredText.ToString());
                MessageBox.Show("Результаты скопированны в буффер. Вставьте их в таблицу Excel или текстовый файл");
            }

        }
    }
}
