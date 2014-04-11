using System;
using Evented;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;

namespace EventedUnitTests
{
    public class MyMessage : MessageBaseClass {

        private int _counter = 0;
        private int _seed    = 0;

        public MyMessage(int seed) : base() {

            this._seed = seed;
        }
        public override bool Execute() {
        
            this._counter += this._seed;
            return base.Execute();
        }
        public override void OnError(Exception exception)
        {
            base.OnError(exception);
        }
    }

    [TestClass]
    public class EQueueUnitTests
    {
        [TestMethod]
        public void ExecuteAllMessage_Basic()
        {
            var q = new EQueue<MyMessage>();
            for(var i = 0; i < 10; i++) {
                 q.Enqueue(new MyMessage(i+1));
            }
            var stateResults = q.ExecuteAllMessages();
            DS.Assert.AreEqual(DS.List(MessageExecution.Succeeded,MessageExecution.Succeeded,MessageExecution.Succeeded,MessageExecution.Succeeded,MessageExecution.Succeeded,MessageExecution.Succeeded,MessageExecution.Succeeded,MessageExecution.Succeeded,MessageExecution.Succeeded,MessageExecution.Succeeded), stateResults);

            Assert.AreEqual(00, q.Count);
            Assert.AreEqual(10, stateResults.Count);            
            Assert.AreEqual(10, q.SucceededMessages.Count);
            Assert.AreEqual(00, q.FailedMessages.Count);
            Assert.AreEqual(00, q.FailedMessages.Count);

            for(var i = 0; i < 10; i++) {
                var m = q.SucceededMessages.Dequeue();
                Assert.AreEqual(MessageState.Processed, m.State);
                Assert.IsTrue(m.EndProcessed >= m.StartProcessed);
                Assert.IsTrue(m.Dequeued >= m.Queued);
            }
        }

        [TestMethod]
        public void PersisteOneMessage()
        {
            var q = new EQueue<MyMessage>(new jsonDb.FileSystemStore());            
            q.Enqueue(new MyMessage(0));            
        }
    }
}
