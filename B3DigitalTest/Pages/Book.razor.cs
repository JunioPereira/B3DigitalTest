using B3DigitalModel;
using B3DigitalService;
using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;

namespace B3DigitalTest.Pages
{
    public partial class Book : ComponentBase, IDisposable
    {
        ConcurrentDictionary<string, QuoteInfo> Quotes { get; set; } = new ConcurrentDictionary<string, QuoteInfo>();
        
        [Inject]
        private IObservableQuotesInfo iObservableQuotesInfo { get; set; } = default!;

        protected override void OnInitialized()
        {
            iObservableQuotesInfo.Subscrible(x =>
            {
                Quotes[x.Symbol] = x;
                InvokeAsync(() => StateHasChanged());
            });
        }

        public void Dispose()
        {
            
        }

        public void OnGet()
        {
        }
    }
}
