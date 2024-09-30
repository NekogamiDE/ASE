using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace LogReader
{
    internal class Session
    {
        public string sessionId;
        private DateTime datetime;

        public Session()
        {
            this.sessionId = Guid.NewGuid().ToString();
            this.datetime = DateTime.Now;
        }
        private string GetSessionId()
        {
            return this.sessionId;
        }
        private string GetDatetime()
        {
            return this.datetime.ToString();
        }
    }
}
