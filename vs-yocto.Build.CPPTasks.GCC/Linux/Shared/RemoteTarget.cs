// Decompiled with JetBrains decompiler
// Type: Microsoft.Build.Linux.Shared.RemoteTarget
// Assembly: Microsoft.Build.Linux.Tasks, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 949216A7-AC49-4527-8BB4-2F13DD292BAC
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\VC\VCTargets\Application Type\Linux\1.0\Microsoft.Build.Linux.Tasks.dll

using System.Diagnostics;

namespace Microsoft.Build.Linux.Shared
{
  [DebuggerDisplay("{Id};{DisplayName}")]
  internal class RemoteTarget
  {
    private int _id = 0;
    private string _name = "";
    private string _displayName = "";

    public int Id
    {
      get
      {
        return this._id;
      }
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public string DisplayName
    {
      get
      {
        return this._displayName;
      }
    }

    internal RemoteTarget(int id)
    {
      this._id = id;
    }

    internal RemoteTarget(int id, string displayName)
    {
      this._id = id;
      this._displayName = displayName;
    }

    internal RemoteTarget(int id, string name, string displayName)
    {
      this._id = id;
      this._name = name;
      this._displayName = displayName;
    }
  }
}
