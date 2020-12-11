﻿using AutoMapper;
using GloboTicket.Services.ShoppingBasket.Models;
using GloboTicket.Services.ShoppingBasket.Repositories;
using GloboTicket.Services.ShoppingBasket.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GloboTicket.Services.ShoppingBasket.Controllers
{
    [Route("api/baskets/{basketId}/basketlines")]
    [ApiController]
    public class BasketLinesController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IBasketLinesRepository _basketLinesRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventCatalogServiceForShoppingBasket _eventCatalogService;
        private readonly IMapper _mapper;

        public BasketLinesController(IBasketRepository basketRepository,
            IBasketLinesRepository basketLinesRepository, IEventRepository eventRepository,
            IEventCatalogServiceForShoppingBasket eventCatalogService, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _basketLinesRepository = basketLinesRepository;
            _eventRepository = eventRepository;
            _eventCatalogService = eventCatalogService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BasketLine>>> Get(Guid basketId)
        {
            if (!await _basketRepository.BasketExists(basketId))
            {
                return NotFound();
            }

            var basketLines = await _basketLinesRepository.GetBasketLines(basketId);
            return Ok(_mapper.Map<IEnumerable<BasketLine>>(basketLines));
        }

        [HttpGet("{basketLineId}", Name = "GetBasketLine")]
        public async Task<ActionResult<BasketLine>> Get(Guid basketId,
            Guid basketLineId)
        {
            if (!await _basketRepository.BasketExists(basketId))
            {
                return NotFound();
            }

            var basketLine = await _basketLinesRepository.GetBasketLineById(basketLineId);
            if (basketLine == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BasketLine>(basketLine));
        }

        [HttpPost]
        public async Task<ActionResult<BasketLine>> Post(Guid basketId,
            [FromBody] BasketLineForCreation basketLineForCreation)
        {
            if (!await _basketRepository.BasketExists(basketId))
            {
                return NotFound();
            }

            var domain = $"{ HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            _eventCatalogService.SetBaseUri(domain);

            if (!await _eventRepository.EventExists(basketLineForCreation.EventId))
            {
                var eventFromCatalog = await _eventCatalogService.GetEvent(basketLineForCreation.EventId);
                _eventRepository.AddEvent(eventFromCatalog);
                await _eventRepository.SaveChanges();
            }

            var basketLineEntity = _mapper.Map<Entities.BasketLine>(basketLineForCreation);

            var processedBasketLine = await _basketLinesRepository.AddOrUpdateBasketLine(basketId, basketLineEntity);
            await _basketLinesRepository.SaveChanges();

            var basketLineToReturn = _mapper.Map<BasketLine>(processedBasketLine);

            return CreatedAtRoute(
                "GetBasketLine",
                new { basketId = basketLineEntity.BasketId, basketLineId = basketLineEntity.BasketLineId },
                basketLineToReturn);
        }

        [HttpPut("{basketLineId}")]
        public async Task<ActionResult<BasketLine>> Put(Guid basketId,
            Guid basketLineId,
            [FromBody] BasketLineForUpdate basketLineForUpdate)
        {
            if (!await _basketRepository.BasketExists(basketId))
            {
                return NotFound();
            }

            var basketLineEntity = await _basketLinesRepository.GetBasketLineById(basketLineId);

            if (basketLineEntity == null)
            {
                return NotFound();
            }

            // map the entity to a dto
            // apply the updated field values to that dto
            // map the dto back to an entity
            _mapper.Map(basketLineForUpdate, basketLineEntity);

            _basketLinesRepository.UpdateBasketLine(basketLineEntity);
            await _basketLinesRepository.SaveChanges();

            return Ok(_mapper.Map<BasketLine>(basketLineEntity));
        }

        [HttpDelete("{basketLineId}")]
        public async Task<IActionResult> Delete(Guid basketId,
            Guid basketLineId)
        {
            if (!await _basketRepository.BasketExists(basketId))
            {
                return NotFound();
            }

            var basketLineEntity = await _basketLinesRepository.GetBasketLineById(basketLineId);

            if (basketLineEntity == null)
            {
                return NotFound();
            }

            _basketLinesRepository.RemoveBasketLine(basketLineEntity);
            await _basketLinesRepository.SaveChanges();

            return NoContent();
        }
    }
}
