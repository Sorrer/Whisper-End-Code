using System;

namespace Game.SaveSystem {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SavableBundleAttribute : Attribute {

        public string FileName { get; }
        
        public SavableBundleAttribute(string fileName) {
            this.FileName = fileName;
        }
        
    }
}
