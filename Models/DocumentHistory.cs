using System;
using System.Collections.Generic; 

namespace Models
{
    public class DocumentHistory
    {
        public int unprocessedItemId { get; set; }
        public string parentSiteUrl { get; set; }
        public string unprocessedFileName { get; set; }
        public object appendices { get; set; }
        public bool isConvertedPDF { get; set; }
        public int edition { get; set; }
        public int revision { get; set; }
        public string comment { get; set; }
        public string published { get; set; }
        public string approvedBy { get; set; }
        public object workflowHistory { get; set; }
    }
    public class DocumentHistoryUpdate
    {
        public int edition { get; set; }
        public int revision { get; set; }
        public string comment { get; set; }
        public DateTime? published { get; set; }
        public string approvedBy { get; set; }
        public List<WorkflowHistory> workflowHistory { get; set; }
    }

    public class WorkflowHistory
    {
        public string workflowId { get; set; }
        public int workflowType { get; set; }
        public bool isCompleted { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? completedDate { get; set; }
        public string completedBy { get; set; }
        public int completedType { get; set; }
        public string initiatorName { get; set; }
        public object dueDate { get; set; }
        public string initiatorComment { get; set; }
        public List<object> workflowTasks { get; set; }
    }

}
