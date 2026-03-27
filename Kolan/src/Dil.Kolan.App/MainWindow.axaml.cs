using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using ScottPlot;

namespace Dil.Kolan.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Подписка на событие VM только после открытия окна (lifecycle-safe)
        Opened += (_, _) =>
        {
            if (DataContext is MainViewModel vm)
            {
                vm.PlotDataReady += () =>
                    Dispatcher.UIThread.Post(UpdatePlot, DispatcherPriority.Loaded);
            }

        };
    }

    private MainViewModel Vm => (MainViewModel)DataContext!;

    private async void OnPasteClick(object? sender, RoutedEventArgs e)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard == null) return;

        var text = await clipboard.GetTextAsync();
        Vm.PasteData(text ?? "");
    }

    private async void OnCopyClick(object? sender, RoutedEventArgs e)
    {
        var result = Vm.GetResultAsText();
        if (string.IsNullOrEmpty(result)) return;

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard == null) return;

        await clipboard.SetTextAsync(result);

        var dialog = new Window
        {
            Title = "Готово",
            Width = 340, Height = 130,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(20), Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Результат скопирован в буфер обмена.\nМожно вставить в Excel (Ctrl+V).",
                        TextWrapping = TextWrapping.Wrap,
                    },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Padding = new Thickness(24, 6),
                    }
                }
            }
        };
        ((Button)((StackPanel)dialog.Content).Children[1]).Click += (_, _) => dialog.Close();
        await dialog.ShowDialog(this);
    }

    private void UpdatePlot()
    {
        var vm = Vm;
        if (vm.PlotAvgLeft == null || vm.PlotAvgRight == null || vm.PlotAvgLeft.Length == 0)
            return;

        // Проверка на NaN/Infinity
        if (vm.PlotAvgLeft.Any(x => double.IsNaN(x) || double.IsInfinity(x)) ||
            vm.PlotAvgRight.Any(x => double.IsNaN(x) || double.IsInfinity(x)))
        {
            vm.StatusText = "Ошибка: данные содержат NaN/Infinity";
            return;
        }

        lock (MainPlot.Plot.Sync)
        {
            var plt = MainPlot.Plot;
            plt.Clear();

            // Подсветка карт ремонта
            if (vm.PlotCards != null)
            {
                foreach (var card in vm.PlotCards)
                {
                    var color = card.WasExtended
                        ? new ScottPlot.Color(0, 170, 255, 50)
                        : new ScottPlot.Color(255, 102, 0, 50);
                    var span = plt.Add.HorizontalSpan(card.StartIndex, card.EndIndex);
                    span.FillStyle.Color = color;
                }
            }

            // Линия порога
            var hl = plt.Add.HorizontalLine(vm.Threshold);
            hl.Color = ScottPlot.Colors.Red;
            hl.LineWidth = 1.5f;
            hl.LinePattern = LinePattern.Dashed;
            hl.LegendText = $"Порог {vm.Threshold} мм";

            // Левая колея
            var sigL = plt.Add.Signal(vm.PlotAvgLeft);
            sigL.Color = ScottPlot.Colors.Blue;
            sigL.LineWidth = 1f;
            sigL.LegendText = "Левая колея (ср.)";

            // Правая колея
            var sigR = plt.Add.Signal(vm.PlotAvgRight);
            sigR.Color = ScottPlot.Colors.Orange;
            sigR.LineWidth = 1f;
            sigR.LegendText = "Правая колея (ср.)";

            plt.ShowLegend(Alignment.UpperRight);
            plt.Axes.AutoScale();

            // Ограничить зум: нельзя уменьшить больше чем в 1.5x от полного вида
            var limX = plt.Axes.GetLimits().Rect;
            double padX = (limX.Right - limX.Left) * 0.25;
            double padY = (limX.Top - limX.Bottom) * 0.25;
            plt.Axes.SetLimits(
                limX.Left - padX, limX.Right + padX,
                limX.Bottom - padY, limX.Top + padY);
            var outerLimits = plt.Axes.GetLimits();
            plt.Axes.Rules.Clear();
            plt.Axes.Rules.Add(new ScottPlot.AxisRules.MaximumBoundary(
                plt.Axes.Bottom, plt.Axes.Left, outerLimits));
            plt.Axes.AutoScale();
        }

        MainPlot.Refresh();

        // Тепловая полоса
        UpdateHeatStrip(vm);
    }

    private void UpdateHeatStrip(MainViewModel vm)
    {
        HeatStrip.Children.Clear();
        if (vm.PlotAvgLeft == null || vm.PlotAvgRight == null) return;

        double stripWidth = HeatStrip.Bounds.Width;
        if (stripWidth <= 0) stripWidth = 900;

        int n = vm.PlotAvgLeft.Length;

        for (int px = 0; px < (int)stripWidth; px++)
        {
            int startIdx = (int)((long)px * n / stripWidth);
            int endIdx = Math.Min((int)((long)(px + 1) * n / stripWidth), n);
            if (startIdx >= endIdx) continue;

            double maxRut = 0;
            for (int i = startIdx; i < endIdx; i++)
            {
                double worst = Math.Max(vm.PlotAvgLeft[i], vm.PlotAvgRight[i]);
                if (worst > maxRut) maxRut = worst;
            }

            var rect = new Rectangle
            {
                Width = 1.5,
                Height = 12,
                Fill = new SolidColorBrush(RutToColor(maxRut, vm.Threshold)),
            };
            Canvas.SetLeft(rect, px);
            HeatStrip.Children.Add(rect);
        }
    }

    private static Avalonia.Media.Color RutToColor(double rut, double threshold)
    {
        double ratio = rut / threshold;
        if (ratio <= 0.5)
            return Avalonia.Media.Color.FromRgb(60, 180, 75);
        if (ratio <= 1.0)
        {
            byte r = (byte)(60 + 195 * (ratio - 0.5) * 2);
            byte g = (byte)(180 + 20 * (ratio - 0.5) * 2);
            return Avalonia.Media.Color.FromRgb(r, g, 0);
        }
        if (ratio <= 1.5)
        {
            byte g = (byte)(200 * (1 - (ratio - 1.0) * 2));
            return Avalonia.Media.Color.FromRgb(255, g, 0);
        }
        return Avalonia.Media.Color.FromRgb(200, 0, 0);
    }
}
