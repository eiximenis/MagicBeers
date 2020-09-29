using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StatisticsService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticsService.Controllers
{
    [Route("cardsused")]
    [ApiController]
    public class CardsStatController : ControllerBase
    {
        private readonly ILogger<CardsStatController> _logger;
        private readonly StatisticsDbContext _db;
        public CardsStatController(ILogger<CardsStatController> logger, StatisticsDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        [HttpPost]
        [Topic("pubsub", "cardsused")]
        public async Task<IActionResult> Index(CardUsage usage)
        {
            _logger.LogInformation($"CardUsage received: User {usage.PlayerName} used {usage.CardId}");
            var cardUsage = _db.CardUsages.SingleOrDefault(cu => cu.CardId == usage.CardId && cu.Player == usage.PlayerName);
            if (cardUsage == null)
            {
                cardUsage = new CardPlayerUsage() { CardId = usage.CardId, Player = usage.PlayerName };
                await _db.CardUsages.AddAsync(cardUsage);
            }

            cardUsage.IncrementUsage();
            await _db.SaveChangesAsync();

            return Ok(new { status = "SUCCESS" });
        }
    }
}
