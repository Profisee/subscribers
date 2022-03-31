using Profisee.Common.EventProcessing.Interfaces;
using Profisee.Services.Sdk.AcceleratorFramework;

namespace Profisee.EventSubscribers
{
    public class SubscriberContext
    {
        public MdmSource MdmSource { get; set; }
        public MdmModel MdmModel { get; set; }
        public MdmEntity MdmEntity { get; set; }
        public IExecuteContext ExecuteContext { get; set; }
    }
}
