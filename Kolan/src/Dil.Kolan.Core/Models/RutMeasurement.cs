namespace Dil.Core.Models;

/// <summary>
/// Одна строка исходных данных — измерение глубины колеи на одном метре дороги.
/// Км и метр — адрес (километровый столб + смещение), НЕ абсолютная координата.
/// Между километровыми столбами может быть любое количество метров.
/// </summary>
public record RutMeasurement(int Km, int Meter, double LeftRut, double RightRut);
