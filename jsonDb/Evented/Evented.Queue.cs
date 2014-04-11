using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evented
{
    public class EQueue<T> : jsonDb.JDbObject where T : MessageBaseClass
    {
        /// <summary>
        /// Messages that failed execution are stored in this queue
        /// </summary>
        public Queue<T> FailedMessages { get; set; }
        /// <summary>
        /// Messages that succeeded execution are stored in this queue
        /// </summary>
        public Queue<T> SucceededMessages  { get; set; }

        private Queue<T> _queue  { get; set; }

        /// <summary>
        /// If defined the queue is persisted in the store
        /// </summary>
        private jsonDb.IStore _persistenceStore;

        public EQueue(jsonDb.IStore persistenceStore = null) {

            this.FailedMessages    = new Queue<T>();
            this.SucceededMessages = new Queue<T>();
            this._queue            = new Queue<T>();
            this._persistenceStore = persistenceStore;
        }
        public bool IsPersisted {
            get { return _persistenceStore != null; }
        }
        private void Persist() {

            if(this.IsPersisted) {
                this.__metadata.Store = _persistenceStore;
                this.Save();
            }
        }
        public void Reset() {

            this.FailedMessages.Clear();
            this.SucceededMessages.Clear();
            this._queue.Clear();
        }
        public void Enqueue(T t) {
            
            t.Queued = DateTime.Now;
            t.State  = MessageState.Pending;
            this._queue.Enqueue(t);
            this.Persist();
        }
        public T Dequeue() {

            var t      = this._queue.Dequeue();
            t.Dequeued = DateTime.Now;
            return t;
        }
        public int Count {

            get { return this._queue.Count; }
        }
        public List<MessageExecution> ExecuteAllMessages() {

            var l = new List<MessageExecution>();

            while(this.Count > 0) {
                l.Add(this.ExecuteMessage());
            }
            return l;
        }
        public MessageExecution ExecuteMessage() {

            var r = MessageExecution.NoMessage;

            if(this.Count == 0)
                return r;

            var m = this.Dequeue();
            m.ExecuteBegin();
            try {
                if(m.Execute()) {
                    this.SucceededMessages.Enqueue(m);
                    r = MessageExecution.Succeeded;
                }
                else {
                    this.FailedMessages.Enqueue(m);
                    r = MessageExecution.Failed;
                }
            }
            catch(System.Exception ex) {
                m.OnError(ex);
                this.FailedMessages.Enqueue(m);
                r = MessageExecution.Failed;
            }
            finally {
                m.ExecuteEnd();
            }
            return r;
        }
    }
}
