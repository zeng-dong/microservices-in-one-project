using GloboTicket.Services.ShoppingBasket.Entities;
using System;
using System.Threading.Tasks;

namespace GloboTicket.Services.ShoppingBasket.Services
{
    public interface IEventCatalogServiceForShoppingBasket
    {
        Task<Event> GetEvent(Guid id);

        void SetBaseUri(string baseUri);
    }
}
