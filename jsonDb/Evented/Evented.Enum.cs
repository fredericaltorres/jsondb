using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evented
{
    public enum MessageState {
        New,
        Pending,
        Processing,
        Processed,
        Disabled,
        ProcessingFailed
    }
    public enum MessageExecution {
        NoMessage,
        Succeeded,
        Failed,
    }
}
