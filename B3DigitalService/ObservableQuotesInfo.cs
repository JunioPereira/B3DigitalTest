using B3DigitalModel;
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
        Subject<QuoteInfo> obs = new Subject<QuoteInfo>();

        public void OnNext(QuoteInfo data)
        {
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
