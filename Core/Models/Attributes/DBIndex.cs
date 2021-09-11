using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Attributes {
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class DBIndex : Attribute {
        public bool isUnique { get; private set; }

        public DBIndex(bool _isUnique = false) {
            this.isUnique = _isUnique;
        }
    }
}
