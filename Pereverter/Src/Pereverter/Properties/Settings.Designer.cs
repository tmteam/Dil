// Decompiled with JetBrains decompiler
// Type: Pereverter.Properties.Settings
// Assembly: Pereverter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CB127099-1D57-4175-847F-3E155E90579B
// Assembly location: T:\Code\Deda\Pereverter\Pereverter alpha 0.2\Pereverter alpha 0.2\Pereverter.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Pereverter.Properties
{
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
  [CompilerGenerated]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        return Settings.defaultInstance;
      }
    }
  }
}
