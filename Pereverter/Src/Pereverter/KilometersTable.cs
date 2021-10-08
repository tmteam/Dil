// Decompiled with JetBrains decompiler
// Type: Pereverter.TT
// Assembly: Pereverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CB127099-1D57-4175-847F-3E155E90579B
// Assembly location: T:\Code\Deda\Pereverter\Pereverter alpha 0.2\Pereverter alpha 0.2\Pereverter.exe

using System.Collections.Generic;

namespace Pereverter
{
  public class KilometersTable
  {
    public List<StrL> Headers = new List<StrL>();
    public List<KilometerRow> Rows = new List<KilometerRow>();
    public bool EndsKmMExist;
    public bool KillometersAndMettersInSingleCell;
  }
}
