using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Dil.Core;
using Dil.Core.Models;

namespace Dil.Kolan.App;

public class MainViewModel : Core.ViewModelBase
{
    private string _statusText = "Нажмите «Вставить из буфера» — 4 столбца: км, м, левая колея, правая колея";
    private double _threshold = 20.0;
    private int _windowSize = 10;
    private int _minCardLength = 100;
    private int _mergeDistance = 100;
    private bool _hasInput;
    private bool _hasResults;

    private List<RutMeasurement>? _measurements;

    public string StatusText
    {
        get => _statusText;
        set => SetField(ref _statusText, value);
    }

    public bool HasInput
    {
        get => _hasInput;
        set => SetField(ref _hasInput, value);
    }

    public bool HasResults
    {
        get => _hasResults;
        set => SetField(ref _hasResults, value);
    }

    public double Threshold
    {
        get => _threshold;
        set { if (SetField(ref _threshold, value)) AutoRecalculate(); }
    }

    public int WindowSize
    {
        get => _windowSize;
        set { if (SetField(ref _windowSize, value)) AutoRecalculate(); }
    }

    public int MinCardLength
    {
        get => _minCardLength;
        set { if (SetField(ref _minCardLength, value)) AutoRecalculate(); }
    }

    public int MergeDistance
    {
        get => _mergeDistance;
        set { if (SetField(ref _mergeDistance, value)) AutoRecalculate(); }
    }

    private void AutoRecalculate()
    {
        if (HasInput)
            Calculate();
    }

    public ObservableCollection<InputRow> InputRows { get; } = new();
    public ObservableCollection<CardRow> Cards { get; } = new();

    // Данные для графика (заполняются при расчёте)
    public double[]? PlotXPositions { get; private set; }
    public double[]? PlotAvgLeft { get; private set; }
    public double[]? PlotAvgRight { get; private set; }
    public List<RepairCard>? PlotCards { get; private set; }
    public List<RutMeasurement>? Measurements => _measurements;

    public event Action? PlotDataReady;

    public Core.RelayCommand CalculateCommand { get; }

    public MainViewModel()
    {
        CalculateCommand = new Core.RelayCommand(Calculate);
    }

    public void PasteData(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            StatusText = "Буфер обмена пуст.";
            return;
        }

        try
        {
            _measurements = ParseInput(text);

            InputRows.Clear();
            foreach (var m in _measurements)
                InputRows.Add(new InputRow(m.Km, m.Meter, m.LeftRut, m.RightRut));

            HasInput = _measurements.Count > 0;
            Cards.Clear();
            HasResults = false;

            // Автоматический расчёт при вставке
            if (HasInput)
                Calculate();
        }
        catch (FormatException ex)
        {
            _measurements = null;
            InputRows.Clear();
            HasInput = false;
            StatusText = $"Ошибка вставки: {ex.Message}";
        }
    }

    private void Calculate()
    {
        if (_measurements == null || _measurements.Count == 0)
        {
            StatusText = "Нет данных. Нажмите «Вставить из буфера».";
            return;
        }

        try
        {
            var parameters = new AnalysisParameters(
                Threshold: _threshold,
                WindowSize: _windowSize,
                MinCardLength: _minCardLength,
                MergeDistance: _mergeDistance);

            var analyzer = new RutAnalyzer();
            var cards = analyzer.Analyze(_measurements, parameters);

            Cards.Clear();
            int num = 1;
            foreach (var c in cards)
            {
                Cards.Add(new CardRow
                {
                    Number = num++,
                    Start = c.Start.ToString(),
                    End = c.End.ToString(),
                    Length = c.LengthMeters,
                    AvgLeft = Math.Round(c.AvgLeftRut, 1),
                    AvgRight = Math.Round(c.AvgRightRut, 1),
                    MaxLeft = Math.Round(c.MaxLeftRut, 1),
                    MaxRight = Math.Round(c.MaxRightRut, 1),
                    Extended = c.WasExtended ? "да" : "",
                });
            }

            // Подготовить данные для графика
            var analyzer2 = new RutAnalyzer();
            var (avgL, avgR) = analyzer2.ComputeSlidingAverages(_measurements, parameters.WindowSize);
            PlotXPositions = new double[_measurements.Count];
            for (int i = 0; i < _measurements.Count; i++)
                PlotXPositions[i] = i;
            PlotAvgLeft = avgL;
            PlotAvgRight = avgR;
            PlotCards = cards;

            HasResults = Cards.Count > 0;
            StatusText = $"Загружено {_measurements.Count} м. Найдено карт ремонта: {cards.Count}";
            PlotDataReady?.Invoke();
        }
        catch (Exception ex)
        {
            StatusText = $"Ошибка расчёта: {ex.Message}";
        }
    }

    public string GetResultAsText()
    {
        if (Cards.Count == 0)
            return "";

        var sb = new StringBuilder();
        sb.AppendLine("#\tНачало\tКонец\tДлина, м\tСр.лев\tСр.прав\tМакс.лев\tМакс.прав\tУдлинена");
        foreach (var c in Cards)
        {
            sb.AppendLine($"{c.Number}\t{c.Start}\t{c.End}\t{c.Length}\t" +
                $"{c.AvgLeft}\t{c.AvgRight}\t{c.MaxLeft}\t{c.MaxRight}\t{c.Extended}");
        }
        return sb.ToString();
    }

    private static List<RutMeasurement> ParseInput(string text)
    {
        var result = new List<RutMeasurement>();
        // Разделители строк: \r и \n, как в старых dil-приложениях
        var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int lineNum = 0;
        foreach (var line in lines)
        {
            lineNum++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Разделители столбцов: таб и пробел, как в старых dil-приложениях
            var parts = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Минимум 4 столбца (км, м, левая, правая), но допускаем больше
            if (parts.Length < 4)
                throw new FormatException($"Строка {lineNum}: ожидается минимум 4 столбца (км, м, левая, правая), получено {parts.Length}");

            if (!int.TryParse(parts[0], out int km))
                throw new FormatException($"Строка {lineNum}: первый столбец должен содержать километры, а тут '{parts[0]}'");
            if (!int.TryParse(parts[1], out int m))
                throw new FormatException($"Строка {lineNum}: второй столбец должен содержать метры, а тут '{parts[1]}'");
            if (!TryParseDouble(parts[2], out double left))
                throw new FormatException($"Строка {lineNum}: в столбце 3 должно быть число, а тут '{parts[2]}'");
            if (!TryParseDouble(parts[3], out double right))
                throw new FormatException($"Строка {lineNum}: в столбце 4 должно быть число, а тут '{parts[3]}'");

            result.Add(new RutMeasurement(km, m, left, right));
        }
        return result;
    }

    /// <summary>
    /// Парсинг double: заменяем запятую на точку (как в старых dil-приложениях),
    /// затем парсим с InvariantCulture.
    /// </summary>
    private static bool TryParseDouble(string s, out double value)
    {
        return double.TryParse(
            s.Replace(",", "."),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out value);
    }
}

public record InputRow(int Km, int Meter, double Left, double Right);

public class CardRow
{
    public int Number { get; init; }
    public string Start { get; init; } = "";
    public string End { get; init; } = "";
    public int Length { get; init; }
    public double AvgLeft { get; init; }
    public double AvgRight { get; init; }
    public double MaxLeft { get; init; }
    public double MaxRight { get; init; }
    public string Extended { get; init; } = "";
}
