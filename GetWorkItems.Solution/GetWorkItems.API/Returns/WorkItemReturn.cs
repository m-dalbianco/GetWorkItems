using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetWorkItems.API.Returns {
    public class WorkItemReturn {
        public int Id { get; set; }
        public string Title { get; set; }
        public string WorkItemType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
