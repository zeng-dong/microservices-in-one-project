using GloboTicket.Web.Extensions;
using GloboTicket.Web.Models;
using GloboTicket.Web.Models.Api;
using GloboTicket.Web.Models.View;
using GloboTicket.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GloboTicket.Web.Controllers
{
    public class ShoppingBasketController : Controller
    {
        private readonly IShoppingBasketServiceForWebClient _basketService;
        private readonly Settings settings;

        public ShoppingBasketController(IShoppingBasketServiceForWebClient basketService, Settings settings)
        {
            this._basketService = basketService;
            this.settings = settings;
        }

        public async Task<IActionResult> Index()
        {
            var domain = $"{ HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            _basketService.SetBaseUri(domain);
            ViewBag.Domain = domain;

            var basketLines = await _basketService.GetLinesForBasket(Request.Cookies.GetCurrentBasketId(settings));
            var lineViewModels = basketLines.Select(bl => new BasketLineViewModel
            {
                LineId = bl.BasketLineId,
                EventId = bl.EventId,
                EventName = bl.Event.Name,
                Date = bl.Event.Date,
                Price = bl.Price,
                Quantity = bl.TicketAmount
            }
            );
            return View(lineViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLine(BasketLineForCreation basketLine)
        {
            var domain = $"{ HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            _basketService.SetBaseUri(domain);
            ViewBag.Domain = domain;

            var basketId = Request.Cookies.GetCurrentBasketId(settings);
            var newLine = await _basketService.AddToBasket(basketId, basketLine);
            Response.Cookies.Append(settings.BasketIdCookieName, newLine.BasketId.ToString());

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLine(BasketLineForUpdate basketLineUpdate)
        {
            var domain = $"{ HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            _basketService.SetBaseUri(domain);
            ViewBag.Domain = domain;

            var basketId = Request.Cookies.GetCurrentBasketId(settings);
            await _basketService.UpdateLine(basketId, basketLineUpdate);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveLine(Guid lineId)
        {
            var domain = $"{ HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            _basketService.SetBaseUri(domain);
            ViewBag.Domain = domain;

            var basketId = Request.Cookies.GetCurrentBasketId(settings);
            await _basketService.RemoveLine(basketId, lineId);
            return RedirectToAction("Index");
        }
    }
}
