using System.IO;
using System.Text;
using System.Windows;
using Dil.Core;
using Dil.Core.Entities;

namespace Usrednator;

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
            var lines = await Task.Run(()=>DilHelper.SplitLines(data));
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
                UsrednatorLogic.SetZeroPivot(_entries, averageMeters);
            var useNextIntervalStart = useNextChbx.IsChecked == true;
            
            var resultText  = await Task.Run(()=> UsrednatorLogic.ConvertTableToFormatedText(_entries));
            _results        = await Task.Run(()=> UsrednatorLogic.ApproximationFilter(_entries, averageMeters, useZeroPivots, useNextIntervalStart));
            var filtredText = await Task.Run(()=> UsrednatorLogic.ConvertTableToFormatedText(_results));

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
            var output = _results.ToFormattedString();
            Clipboard.SetText(output);
            MessageBox.Show("Результаты скопированны в буффер. Вставьте их в таблицу Excel или текстовый файл");
        }
    }
}