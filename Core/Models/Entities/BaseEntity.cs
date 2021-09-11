using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Models.Entities {
    public class BaseEntity {
        public bool isNullOrEmpty(params string[] str) {
            return str.All(s => string.IsNullOrEmpty(s));
        }

        public bool isNull(params object[] objs) {
            return objs.All(s => s == null);
        }
    }
}
