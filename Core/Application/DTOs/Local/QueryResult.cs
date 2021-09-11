using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.DTOs.Local {
    public class QueryResult<T> {
        public IEnumerable<T> resultAsObject { get; set; }
        public string resultAsString { get; set; }
    }
}
