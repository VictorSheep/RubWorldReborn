using UnityEngine;

namespace DependencyInjection
{
    public class InjectAttribute : PropertyAttribute
    {
        private string dependencyName = "";

        private string[] otherFieldsToFill = null;

        public string DependencyName => dependencyName;

        public string[] OtherFieldsToFill => otherFieldsToFill;

        public InjectAttribute(string dependencyName = "", string [] otherFieldsToFill = null)
        {
            this.dependencyName    = dependencyName;
            this.otherFieldsToFill = otherFieldsToFill;
        }
    }
}