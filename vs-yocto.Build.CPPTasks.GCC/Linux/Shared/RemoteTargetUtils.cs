// Decompiled with JetBrains decompiler
// Type: Microsoft.Build.Linux.Shared.RemoteTargetUtils
// Assembly: Microsoft.Build.Linux.Tasks, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 949216A7-AC49-4527-8BB4-2F13DD292BAC
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\VC\VCTargets\Application Type\Linux\1.0\Microsoft.Build.Linux.Tasks.dll

using liblinux;
using liblinux.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Build.Linux.Shared
{
    internal static class RemoteTargetUtils
    {
        public static string ConnectionToString(StoredConnectionInfo storedConnectionInfo)
        {
            ConnectionInfo connectionInfo = (ConnectionInfo)storedConnectionInfo;
            string str = connectionInfo is PasswordConnectionInfo ? "Password" : "PrivateKey";
            return string.Format("{0};{1} (username=, port={2}, authentication={3})", (object)storedConnectionInfo.Id, (object)connectionInfo.HostNameOrAddress, (object)connectionInfo.Port, (object)str);
        }

        public static string ConnectionToDisplayName(StoredConnectionInfo storedConnectionInfo)
        {
            ConnectionInfo connectionInfo = (ConnectionInfo)storedConnectionInfo;
            string str = connectionInfo is PasswordConnectionInfo ? "Password" : "PrivateKey";
            return string.Format("{0} (username={1}, port={2}, authentication={3})", (object)connectionInfo.HostNameOrAddress, (object)connectionInfo.UserName, (object)connectionInfo.Port, (object)str);
        }

        public static string ConnectionToUserName(StoredConnectionInfo storedConnectionInfo)
        {
            ConnectionInfo connectionInfo = (ConnectionInfo)storedConnectionInfo;
            return string.Format("{0}", (object)connectionInfo.UserName);
        }

        public static string ConnectionToDisplayNameShort(StoredConnectionInfo storedConnectionInfo)
        {
            string platform = RemoteTargetUtils.ProcessorArchToPlatform(storedConnectionInfo.Properties.Get("Platform"));
            ConnectionInfo connectionInfo = (ConnectionInfo)storedConnectionInfo;
            string hostNameOrAddress = connectionInfo.HostNameOrAddress;
            if (!string.IsNullOrEmpty(platform))
                return string.Format("{0} ({1})", (object)hostNameOrAddress, (object)platform);
            return hostNameOrAddress;
        }

        internal static bool TryParseRemoteTarget(string remoteTarget, out RemoteTarget result)
        {
            result = (RemoteTarget)null;
            if (string.IsNullOrWhiteSpace(remoteTarget))
                return false;
            int length1 = remoteTarget.IndexOf(';');
            if (length1 == -1 || length1 + 1 >= remoteTarget.Length)
                return false;
            string s = remoteTarget.Substring(0, length1);
            int result1 = 0;
            if (!int.TryParse(s, out result1))
                return false;
            string displayName = remoteTarget.Substring(length1 + 1);
            string name = "";
            int length2 = displayName.IndexOf('(');
            if (length2 > 0)
                name = displayName.Substring(0, length2).Trim();
            result = new RemoteTarget(result1, name, displayName);
            return true;
        }

        public static bool TryGetConnectionInfoOrDefault(
          string remoteTargetStringOrId,
          out StoredConnectionInfo connectionInfo)
        {
            return RemoteTargetUtils.TryGetConnectionInfoOrDefault(new ConnectionInfoStore(), remoteTargetStringOrId, RetrieveMode.All, out connectionInfo);
        }

        public static bool TryGetConnectionInfoOrDefault(
          ConnectionInfoStore store,
          string remoteTargetStringOrId,
          out StoredConnectionInfo connectionInfo)
        {
            return RemoteTargetUtils.TryGetConnectionInfoOrDefault(store, remoteTargetStringOrId, RetrieveMode.All, out connectionInfo);
        }

        public static bool TryGetConnectionInfoOrDefault(
          string remoteTargetStringOrId,
          RetrieveMode mode,
          out StoredConnectionInfo connectionInfo)
        {
            return RemoteTargetUtils.TryGetConnectionInfoOrDefault(new ConnectionInfoStore(), remoteTargetStringOrId, mode, out connectionInfo);
        }

        public static bool TryGetConnectionInfoOrDefault(
          ConnectionInfoStore store,
          string remoteTargetStringOrId,
          RetrieveMode mode,
          out StoredConnectionInfo connectionInfo)
        {
            connectionInfo = (StoredConnectionInfo)null;
            if (string.IsNullOrWhiteSpace(remoteTargetStringOrId))
                return RemoteTargetUtils.TryGetDefaultConnectionInfo(store, mode, new StoredConnectionInfo[0], out connectionInfo);
            RemoteTarget result1 = (RemoteTarget)null;
            int result2 = -1;
            if (RemoteTargetUtils.TryParseRemoteTarget(remoteTargetStringOrId, out result1))
            {
                if (!store.TryGetById(result1.Id, mode, out connectionInfo))
                    return false;
            }
            else
            {
                if (!int.TryParse(remoteTargetStringOrId, out result2))
                    return store.TryGetByName(remoteTargetStringOrId, out connectionInfo);
                if (!store.TryGetById(result2, out connectionInfo))
                    return false;
            }
            return true;
        }

        public static bool TryGetDefaultConnectionInfo(out StoredConnectionInfo connectionInfo)
        {
            return RemoteTargetUtils.TryGetDefaultConnectionInfo(new ConnectionInfoStore(), RetrieveMode.All, new StoredConnectionInfo[0], out connectionInfo);
        }

        public static bool TryGetDefaultConnectionInfo(
          ConnectionInfoStore store,
          out StoredConnectionInfo connectionInfo)
        {
            return RemoteTargetUtils.TryGetDefaultConnectionInfo(store, RetrieveMode.All, new StoredConnectionInfo[0], out connectionInfo);
        }

        public static bool TryGetDefaultConnectionInfo(
          ConnectionInfoStore store,
          StoredConnectionInfo[] excludedConnections,
          out StoredConnectionInfo connectionInfo)
        {
            return RemoteTargetUtils.TryGetDefaultConnectionInfo(store, RetrieveMode.All, excludedConnections, out connectionInfo);
        }

        public static bool TryGetDefaultConnectionInfo(
          ConnectionInfoStore store,
          RetrieveMode mode,
          StoredConnectionInfo[] excludedConnections,
          out StoredConnectionInfo connectionInfo)
        {
            connectionInfo = (StoredConnectionInfo)null;
            if (store.Connections.Count == 0)
                return false;
            StoredConnectionInfo[] connectionsByAddedDate = RemoteTargetUtils.GetAvailableConnectionsByAddedDate(store);
            connectionInfo = store.Connections.Where<StoredConnectionInfo>((Func<StoredConnectionInfo, bool>)(c => !((IEnumerable<StoredConnectionInfo>)excludedConnections).Any<StoredConnectionInfo>((Func<StoredConnectionInfo, bool>)(e => e.Id == c.Id)))).Where<StoredConnectionInfo>((Func<StoredConnectionInfo, bool>)(c => c.LastSuccessful != DateTime.MinValue)).DefaultIfEmpty<StoredConnectionInfo>(((IEnumerable<StoredConnectionInfo>)connectionsByAddedDate).FirstOrDefault<StoredConnectionInfo>()).FirstOrDefault<StoredConnectionInfo>();
            return connectionInfo != null;
        }

        public static StoredConnectionInfo[] GetAvailableConnectionsByAddedDate()
        {
            return RemoteTargetUtils.GetAvailableConnectionsByAddedDate(new ConnectionInfoStore());
        }

        public static StoredConnectionInfo[] GetAvailableConnectionsByAddedDate(
          ConnectionInfoStore store)
        {
            return store.Connections.OrderBy<StoredConnectionInfo, DateTime>((Func<StoredConnectionInfo, DateTime>)(c => c.DateAdded)).ToArray<StoredConnectionInfo>();
        }

        public static string ResolveRemoteTargetId(string remoteTargetStringOrId)
        {
            RemoteTarget result1 = (RemoteTarget)null;
            int result2 = -1;
            if (RemoteTargetUtils.TryParseRemoteTarget(remoteTargetStringOrId, out result1))
                return result1.Id.ToString();
            if (int.TryParse(remoteTargetStringOrId, out result2))
                return result2.ToString();
            StoredConnectionInfo connectionInfo = (StoredConnectionInfo)null;
            if (RemoteTargetUtils.TryGetConnectionInfoOrDefault(remoteTargetStringOrId, out connectionInfo))
                return connectionInfo.Id.ToString();
            return remoteTargetStringOrId;
        }

        public static string GetLastRemoteTargetIdFromFile(string lastRemoteTargetFile)
        {
            string str = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(lastRemoteTargetFile) && File.Exists(lastRemoteTargetFile))
                    str = File.ReadAllText(lastRemoteTargetFile);
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return str;
        }

        public static string ProcessorArchToPlatform(string remoteArchitecture)
        {
            string str = string.Empty;
            if (!string.IsNullOrEmpty(remoteArchitecture))
            {
                if (remoteArchitecture.StartsWith("aarch64", StringComparison.OrdinalIgnoreCase) || remoteArchitecture.StartsWith("arm64", StringComparison.OrdinalIgnoreCase))
                    str = "ARM64";
                else if (remoteArchitecture.StartsWith("arm", StringComparison.OrdinalIgnoreCase))
                    str = "ARM";
                else if (remoteArchitecture.Equals("i686", StringComparison.OrdinalIgnoreCase) || remoteArchitecture.Equals("x86", StringComparison.OrdinalIgnoreCase) || remoteArchitecture.Equals("i386", StringComparison.OrdinalIgnoreCase) || remoteArchitecture.Equals("i586", StringComparison.OrdinalIgnoreCase))
                    str = "x86";
                else if (remoteArchitecture.Equals("x86_64", StringComparison.OrdinalIgnoreCase) || remoteArchitecture.Equals("x64", StringComparison.OrdinalIgnoreCase) || remoteArchitecture.Equals("amd64", StringComparison.OrdinalIgnoreCase))
                    str = "x64";
            }
            return str;
        }

        public static bool TryGetConnectionIdByName(string name, out int id)
        {
            ConnectionInfoStore connectionInfoStore = new ConnectionInfoStore();
            StoredConnectionInfo storedConnectionInfo = (StoredConnectionInfo)null;
            if (connectionInfoStore.TryGetByName(name, RetrieveMode.All, out storedConnectionInfo))
            {
                id = storedConnectionInfo.Id;
                return true;
            }
            id = 0;
            return false;
        }
    }
}
