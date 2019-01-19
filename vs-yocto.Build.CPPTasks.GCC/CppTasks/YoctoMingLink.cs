using System.Collections;

namespace Microsoft.Build.CPPTasks
{
    public class YoctoMingLink : MingLink
    {
        private ArrayList switchOrderList;

        protected override string ToolName
        {
            get
            {
                return "g++.exe";
            }
        }

        protected override ArrayList SwitchOrderList
        {
            get
            {
                return this.switchOrderList;
            }
        }


        public override string Subsystem
        {
            get
            {
                if (this.IsPropertySet("Subsystem"))
                    return this.ActiveToolSwitches["Subsystem"].Value;
                return (string)null;
            }
            set
            {
                this.ActiveToolSwitches.Remove("Subsystem");
                ToolSwitch switchToAdd = new ToolSwitch(ToolSwitchType.String);
                switchToAdd.DisplayName = "Subsystem";
                switchToAdd.Description = "The type of application to be generated";
                switchToAdd.ArgumentRelationList = new ArrayList();
                string[][] switchMap = new string[2][]
                {
          new string[2]
          {
            "Application",
            ""
          },
          new string[2]
          {
            "SharedLibrary",
            "-shared"
          }
                };
                switchToAdd.SwitchValue = this.ReadSwitchMap("Subsystem", switchMap, value);
                switchToAdd.Name = "Subsystem";
                switchToAdd.Value = value;
                switchToAdd.MultipleValues = true;
                this.ActiveToolSwitches.Add("Subsystem", switchToAdd);
                this.AddActiveSwitchToolValue(switchToAdd);
            }
        }

        public override bool ThreadSupport
        {
            get
            {
                if (this.IsPropertySet("ThreadSupport"))
                    return this.ActiveToolSwitches["ThreadSupport"].BooleanValue;
                return false;
            }
            set
            {
                this.ActiveToolSwitches.Remove("ThreadSupport");
                ToolSwitch switchToAdd = new ToolSwitch(ToolSwitchType.Boolean);
                switchToAdd.DisplayName = "ThreadSupport";
                switchToAdd.Description = "Specifies that pthread support is to be used.";
                switchToAdd.ArgumentRelationList = new ArrayList();
                switchToAdd.SwitchValue = "-pthread";
                switchToAdd.Name = "ThreadSupport";
                switchToAdd.BooleanValue = value;
                this.ActiveToolSwitches.Add("ThreadSupport", switchToAdd);
                this.AddActiveSwitchToolValue(switchToAdd);
            }
        }

        public override string Sysroot
        {
            get
            {
                if (this.IsPropertySet("Sysroot"))
                    return this.ActiveToolSwitches["Sysroot"].Value;
                return (string)null;
            }
            set
            {
                this.ActiveToolSwitches.Remove("Sysroot");
                ToolSwitch switchToAdd = new ToolSwitch(ToolSwitchType.String);
                switchToAdd.DisplayName = "Sysroot";
                switchToAdd.Description = "Folder path to the root directory for headers and libraries.";
                switchToAdd.ArgumentRelationList = new ArrayList();
                switchToAdd.Name = "Sysroot";
                switchToAdd.Value = value;
                switchToAdd.Separator = "=";
                switchToAdd.SwitchValue = "--sysroot";
                this.ActiveToolSwitches.Add("Sysroot", switchToAdd);
                this.AddActiveSwitchToolValue(switchToAdd);
            }
        }

        public YoctoMingLink()
          : base()
        {
            this.switchOrderList = new ArrayList();
            this.switchOrderList.Add((object)"MSVCErrorReport");
            this.switchOrderList.Add((object)"OutputFile");
            this.switchOrderList.Add((object)"ShowProgress");
            this.switchOrderList.Add((object)"Version");
            this.switchOrderList.Add((object)"VerboseOutput");
            this.switchOrderList.Add((object)"IncrementalLink");
            this.switchOrderList.Add((object)"SharedLibrarySearchPath");
            this.switchOrderList.Add((object)"AdditionalLibraryDirectories");
            this.switchOrderList.Add((object)"UnresolvedSymbolReferences");
            this.switchOrderList.Add((object)"OptimizeforMemory");
            this.switchOrderList.Add((object)"GccToolChain");
            this.switchOrderList.Add((object)"Target");
            this.switchOrderList.Add((object)"TrackerLogDirectory");
            this.switchOrderList.Add((object)"IgnoreSpecificDefaultLibraries");
            this.switchOrderList.Add((object)"ForceSymbolReferences");
            this.switchOrderList.Add((object)"DebuggerSymbolInformation");
            this.switchOrderList.Add((object)"PackageDebugSymbols");
            this.switchOrderList.Add((object)"GenerateMapFile");
            this.switchOrderList.Add((object)"Subsystem");
            this.switchOrderList.Add((object)"ThreadSupport");
            this.switchOrderList.Add((object)"UnicodeBuild");
            this.switchOrderList.Add((object)"Sysroot");
            this.switchOrderList.Add((object)"AdditionalOptions");
            this.switchOrderList.Add((object)"Sources");
            this.switchOrderList.Add((object)"AdditionalDependencies");
            this.switchOrderList.Add((object)"LibraryDependencies");
            this.switchOrderList.Add((object)"BuildingInIde");

        }
    }
}
