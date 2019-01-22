// Decompiled with JetBrains decompiler
// Type: Microsoft.Build.Linux.Tasks.ResolveRemoteTarget
// Assembly: Microsoft.Build.Linux.Tasks, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 949216A7-AC49-4527-8BB4-2F13DD292BAC
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\VC\VCTargets\Application Type\Linux\1.0\Microsoft.Build.Linux.Tasks.dll

using liblinux.Persistence;
using Microsoft.Build.Framework;
using Microsoft.Build.Linux.Shared;
using Microsoft.Build.Utilities;
using System.Reflection;
using System.Resources;

namespace Microsoft.Build.Linux.Tasks
{
    public class ResolveRemoteTarget : Task
    {
        public string RemoteTarget { get; set; }

        public bool DesignTimeBuild { get; set; }

        [Output]
        public string ResolvedRemoteTarget { get; set; }

        [Output]
        public string ResolvedRemoteTargetId { get; set; }

        [Output]
        public string ResolvedRemoteUserName { get; set; }

        [Output]
        public string RemoteTargetHash { get; set; }

        [Output]
        public string RemoteTargetArchitecture { get; set; }

        public ResolveRemoteTarget()
          : base(new ResourceManager("Microsoft.Build.Linux.Strings", Assembly.GetExecutingAssembly()))
        {
        }

        public override bool Execute()
        {
            ConnectionInfoStore store = new ConnectionInfoStore();
            StoredConnectionInfo connectionInfo = (StoredConnectionInfo)null;
            if (RemoteTargetUtils.TryGetConnectionInfoOrDefault(store, this.RemoteTarget, RetrieveMode.PropertiesOnly, out connectionInfo))
            {
                this.ResolvedRemoteTarget = RemoteTargetUtils.ConnectionToString(connectionInfo);
                this.ResolvedRemoteTargetId = connectionInfo.Id.ToString();
                this.RemoteTargetArchitecture = connectionInfo.Properties["Platform"] ?? "";
                this.ResolvedRemoteUserName = RemoteTargetUtils.ConnectionToUserName(connectionInfo);
            }
            else
            {
                if (store.Connections.Count == 0 && !this.DesignTimeBuild)
                {
                    this.Log.LogErrorFromResources("Error.NoRemoteTargets");
                    return false;
                }
                this.ResolvedRemoteTarget = this.RemoteTarget;
            }
            this.RemoteTargetHash = this.RemoteTarget?.GetHashCode().ToString();
            return true;
        }
    }
}
