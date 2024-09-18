using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class LogItem
    {
        public string TaskName { get; set; }
        public string Detail { get; set; }
        public bool isError { get; set; }
        public string Exception { get; set; }
        public bool IsDisplayed { get; set; }

        public const string INFOR = "\n{0}  [{1}]:{2}";
        public const string ERROR = "\n{0}  [{1}]:{2}, Detail:{3}";

        public string GetInformationLog()
        {
            return string.Format(LogItem.INFOR, DateTime.Now.ToString(), this.TaskName, this.Detail);
        }
        public string GetErrorLog()
        {
            return string.Format(LogItem.ERROR, DateTime.Now.ToString(), this.TaskName, this.Detail, this.Exception);
        }
    }
}
