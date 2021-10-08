// Decompiled with JetBrains decompiler
// Type: Pereverter.txtReader
// Assembly: Pereverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CB127099-1D57-4175-847F-3E155E90579B
// Assembly location: T:\Code\Deda\Pereverter\Pereverter alpha 0.2\Pereverter alpha 0.2\Pereverter.exe

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pereverter
{
  public class txtReader
  {
    public List<Str> strs = new List<Str>();

    public void Read(string path)
    {
      this.strs.Clear();
      if (!File.Exists(path))
        return;
      var streamReader = new StreamReader(File.Open(path, FileMode.Open, FileAccess.Read), Encoding.Default);
      while (!streamReader.EndOfStream)
        this.strs.Add(new Str(streamReader.ReadLine().Split('\t')));
      streamReader.Close();
    }
  }
}
