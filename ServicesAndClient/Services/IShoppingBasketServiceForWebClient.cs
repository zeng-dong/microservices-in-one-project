using GloboTicket.Web.Models.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GloboTicket.Web.Services
{
    public interface IShoppingBasketServiceForWebClient
    {
        Task<BasketLine> AddToBasket(Guid basketId, BasketLineForCreation basketLine);
        Task<IEnumerable<BasketLine>> GetLinesForBasket(Guid basketId);
        Task<Basket> GetBasket(Guid basketId);
        Task UpdateLine(Guid basketId, BasketLineForUpdate basketLineForUpdate);
        Task RemoveLine(Guid basketId, Guid lineId);

        void SetBaseUri(string baseUri);
    }
}
