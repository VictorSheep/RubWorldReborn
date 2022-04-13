using System;

namespace DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class Dependency : Attribute
    {
        private bool   autoCreateScriptable;
        private string dataFolder = "";

        public bool AutoCreateScriptable => autoCreateScriptable;

        public string DataFolder => dataFolder;

        public Dependency(bool autoCreateScriptable = true, string dataFolder = "")
        {
            this.autoCreateScriptable = autoCreateScriptable;
            this.dataFolder           = dataFolder;
        }
    }
}