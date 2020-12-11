using GloboTicket.Web.Extensions;
using GloboTicket.Web.Models;
using GloboTicket.Web.Models.Api;
using GloboTicket.Web.Models.View;
using GloboTicket.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GloboTicket.Web.Controllers
{
    public class EventCatalogController : Controller
    {
        private readonly IEventCatalogServiceForWebClient _eventCatalogService;
        private readonly IShoppingBasketServiceForWebClient _shoppingBasketService;
        private readonly Settings settings;

        public EventCatalogController(IEventCatalogServiceForWebClient eventCatalogService, IShoppingBasketServiceForWebClient shoppingBasketService, Settings settings)
        {
            this._eventCatalogService = eventCatalogService;
            this._shoppingBasketService = shoppingBasketService;
            this.settings = settings;
        }

        public async Task<IActionResult> Index(Guid categoryId)
        {
            var currentBasketId = Request.Cookies.GetCurrentBasketId(settings);

            var domain = $"{ HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            _eventCatalogService.SetBaseUri(domain);
            _shoppingBasketService.SetBaseUri(domain);

            var getBasket = currentBasketId == Guid.Empty ? Task.FromResult<Basket>(null) :
                _shoppingBasketService.GetBasket(currentBasketId);
            var getCategories = _eventCatalogService.GetCategories();
            var getEvents = categoryId == Guid.Empty ? _eventCatalogService.GetAll() :
                _eventCatalogService.GetByCategoryId(categoryId);
            await Task.WhenAll(new Task[] { getBasket, getCategories, getEvents });

            var numberOfItems = getBasket.Result == null ? 0 : getBasket.Result.NumberOfItems;

            return View(
                new EventListModel
                {
                    Events = getEvents.Result,
                    Categories = getCategories.Result,
                    NumberOfItems = numberOfItems,
                    SelectedCategory = categoryId
                }
            );
        }

        [HttpPost]
        public IActionResult SelectCategory([FromForm] Guid selectedCategory)
        {
            return RedirectToAction("Index", new { categoryId = selectedCategory });
        }

        public async Task<IActionResult> Detail(Guid eventId)
        {
            var domain = $"{ HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            _eventCatalogService.SetBaseUri(domain);

            var ev = await _eventCatalogService.GetEvent(eventId);
            return View(ev);
        }
    }
}
