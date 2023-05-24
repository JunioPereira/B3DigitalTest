using B3DigitalModel;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace B3DigitalService
{
    public interface IObservableQuotesInfo 
    {
        void OnNext(QuoteInfo data);
        void Subscrible(Action<QuoteInfo> action);
    }

    public class ObservableQuotesInfo : IObservableQuotesInfo
    {
        Subject<QuoteInfo> obs { get; }
        IDistributedCache iDistributedCache { get; }

        public ObservableQuotesInfo(IDistributedCache distributedCache) 
        {
            iDistributedCache = distributedCache;
            obs = new Subject<QuoteInfo>();
        }
        

        public void OnNext(QuoteInfo data)
        {
            var obj = JsonConvert.SerializeObject(data);

            var body = Encoding.UTF8.GetBytes(obj);

            iDistributedCache.Set(data.Symbol, body);

            obs.OnNext(data);
        }

        public void Subscrible(Action<QuoteInfo> action)
        {
            obs.Subscribe(x =>
            {
                action.Invoke(x);
            });
        }
    }
}
