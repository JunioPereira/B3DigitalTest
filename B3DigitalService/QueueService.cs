using B3DigitalModel;
using System.Collections.Concurrent;

namespace B3DigitalService
{
    public interface IQueueService 
    {
        void AddItem(KeyValuePair<CriptoType, Data> data);
        bool IsCompleted();
        KeyValuePair<CriptoType, Data> Take();
        void Stop();
    }

    public class QueueService : IQueueService
    {
        BlockingCollection<KeyValuePair<CriptoType, Data>> QueueItems { get; }

        public QueueService() 
        {
            QueueItems = new BlockingCollection<KeyValuePair<CriptoType, Data>>();
        }

        public void AddItem(KeyValuePair<CriptoType, Data> data)
        {
            QueueItems.Add(data);
        }

        public bool IsCompleted()
        {
            return QueueItems.IsCompleted;
        }

        public KeyValuePair<CriptoType, Data> Take()
        {
            return QueueItems.Take();
        }

        public void Stop() 
        {
            QueueItems.CompleteAdding();
            QueueItems.Dispose();
        }
    }
}
