using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StatisticsService.Data;

namespace StatisticsService
{
    public class Service : Statistics.StatisticsBase
    {

        private readonly ILogger<Service> _logger;
        private readonly StatisticsDbContext _db;
        public Service(ILogger<Service> logger, StatisticsDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public override async Task<StatisticsResponse> GetLeaderboard(StatisticsRequest request, ServerCallContext context)
        {
            var from = request.From;
            var to = request.To;
            IQueryable<Statistic> query = _db.Statistics;
            switch (request.Order)
            {
                case StatisticsRequest.Types.OrderBy.Loses:
                    query = query.OrderBy(x => x.Loses);
                    break;
                case StatisticsRequest.Types.OrderBy.Wins:
                    query = query.OrderBy(x => x.Wins);
                    break;
                default:
                    query = query.OrderBy(x => x.Ratio);
                    break;
            }

            var result = await query.Skip(from).Take(to - from)
                .Select(s => new StatisticsResponseItem()
                {
                    Loses = s.Loses,
                    PlayerName = s.PlayerName,
                    Ratio = s.Ratio,
                    Wins = s.Wins
                })
                .ToListAsync();

            var response = new StatisticsResponse() { Count = result.Count };
            response.Items.AddRange(result);
            return response;            
        }

        public override async Task<StatisticsResponseItem> GetPlayerStatistics(PlayerRequest request, ServerCallContext context)
        {
            var player = request.PlayerName;
            var stats = (await _db.Statistics.SingleOrDefaultAsync(s => s.PlayerName == player)) ?? new Statistic() { PlayerName = player };
       
            return new StatisticsResponseItem()
            {
                Loses = stats.Loses,
                PlayerName = stats.PlayerName,
                Ratio = stats.Ratio,
                Wins = stats.Wins
            };
        }

        public override async Task<CardsUsageResponse> GetCardsUsages(PlayerRequest request, ServerCallContext context)
        {
            var player = request.PlayerName;
            var data = await _db.CardUsages.Where(cu => cu.Player == player)
                .OrderByDescending(c => c.Count).Take(10).ToListAsync();

            var response = new CardsUsageResponse() { Count = data.Count };
            response.Items.AddRange(data.Select(i => new CardsUsageReponseItem()
            {
                CardId = i.CardId,
                Counts = i.Count,
                PlayerName = i.Player
            }));

            return response;
        }

        public override async Task<EmptyResponse> AddLose(PlayerRequest request, ServerCallContext context)
        {
            var player = request.PlayerName;
            _logger.LogInformation($"Adding Defeat for player: {player}");
            await AddWinOrLose(player, isWin: false);

            return new EmptyResponse();
        }

        public override async Task<EmptyResponse> AddWin(PlayerRequest request, ServerCallContext context)
        {
            var player = request.PlayerName;
            _logger.LogInformation($"Adding Victory for player: {player}");
            await AddWinOrLose(player, isWin: true);
            return new EmptyResponse();
        }

        private async Task AddWinOrLose(string player, bool isWin)
        {
            var games = _db.Statistics.SingleOrDefault(s => s.PlayerName == player);
            if (games == null)
            {
                games = new Statistic()
                {
                    PlayerName = player
                };
                _db.Statistics.Add(games);
            }

            if (isWin)
            {
                games.AddVictory();
            }
            else
            {
                games.AddDefeat();
            }

            await _db.SaveChangesAsync();
        }
    }
}
