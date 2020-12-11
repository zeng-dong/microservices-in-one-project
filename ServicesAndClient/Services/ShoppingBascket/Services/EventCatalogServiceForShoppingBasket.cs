using GloboTicket.Services.ShoppingBasket.Entities;
using GloboTicket.Services.ShoppingBasket.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GloboTicket.Services.ShoppingBasket.Services
{
    public class EventCatalogServiceForShoppingBasket : IEventCatalogServiceForShoppingBasket
    {
        private readonly HttpClient client;

        public EventCatalogServiceForShoppingBasket(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Event> GetEvent(Guid id)
        {
            var response = await client.GetAsync($"/api/events/{id}");
            return await response.ReadContentAs<Event>();
        }

        public void SetBaseUri(string baseUri)
        {
            this.client.BaseAddress = new Uri(baseUri);
        }
    }
}
