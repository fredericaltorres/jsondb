using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicSugar;

namespace Evented
{
   
    public class MessageBaseClass
    {
        public string ID { get; set; }
        public Exception Exception     { get; set; }
        public DateTime Queued         { get; set; }
        public DateTime Dequeued       { get; set; }
        public DateTime StartProcessed { get; set; }
        public DateTime EndProcessed   { get; set; }
        public MessageState State      { get; set; }

        public MessageBaseClass() {

            var d               = DateTime.Now;
            this.ID             = "{0:0000}.{1:00}.{2:00}-{3:00}.{4:00}.{5:00}.{6:000}-{7}".format(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Millisecond, Environment.TickCount);
            this.State          = MessageState.New;
            this.Queued         = DateTime.MinValue;
            this.Dequeued       = DateTime.MinValue;
            this.StartProcessed = DateTime.MinValue;
            this.EndProcessed   = DateTime.MinValue;            
        }
        public virtual void OnError(Exception exception) {

            this.Exception = exception;
            this.State     = MessageState.ProcessingFailed;
        }
        public void ExecuteBegin() {            

            this.StartProcessed = DateTime.Now;
            this.State = MessageState.Processing;
        }
        public void ExecuteEnd() {            

            this.EndProcessed = DateTime.Now;
            this.State = MessageState.Processed;
        }
        public virtual bool Execute() {                        

            return true;
        }
    }
}
