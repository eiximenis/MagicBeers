using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatisticsService;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using ApiClients.Requests;
using System.Text.Json;

namespace ApiClients
{
    public class StatisticsClient : IStatisticsClient
    {
        private string _url;
        private readonly HttpClient _daprClient;
        private readonly ILogger<IStatisticsClient> _logger;
        private readonly bool _useHttpForGrpc;
        public StatisticsClient(string url, HttpClient daprClient, ILogger<IStatisticsClient> logger)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _useHttpForGrpc = new Uri(url).Scheme.ToLowerInvariant() == "http";
            _daprClient = daprClient;
            _logger = logger;
            _logger.LogInformation($"GameManager gRPC url: {_url}. Using http: {_useHttpForGrpc}. Dapr url {_daprClient?.BaseAddress}");
        }

        public async Task AddLose(string player)
        {
            if (_useHttpForGrpc)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            var request = new PlayerRequest() { PlayerName = player };
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new Statistics.StatisticsClient(channel);
            await client.AddLoseAsync(request);
        }

        public async Task AddWin(string player)
        {
            if (_useHttpForGrpc)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            var request = new PlayerRequest() { PlayerName = player };
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new Statistics.StatisticsClient(channel);
            await client.AddWinAsync(request);
        }

        public async Task<PlayerStatistic> GetPlayerStats(string player)
        {
            if (_useHttpForGrpc)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new Statistics.StatisticsClient(channel);
            var response = await client.GetPlayerStatisticsAsync(new PlayerRequest() { PlayerName = player });

            return new PlayerStatistic()
            {
                Loses = response.Loses,
                Wins = response.Wins,
                Ratio = response.Ratio
            };
        }

        public async Task AddCardUsed(string player, int cardId)
        { 
            if (_daprClient == null)
            {
                _logger.LogInformation("Dapr not enabled. Called ignored");
                return;
            }
            var request = new CardUsedRequest() { PlayerName = player, CardId = cardId };
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var result = await _daprClient.PostAsync("v1.0/publish/pubsub/cardsused",
                new StringContent(json, Encoding.UTF8, "application/json"));
            _logger.LogInformation($"Card {cardId} usage from {player} published with Dapr with status: {result.StatusCode}!");
        }

        public async Task<IEnumerable<CardUsage>> GetCardUsages(string player)
        {
            if (_useHttpForGrpc)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new Statistics.StatisticsClient(channel);
            var result = await client.GetCardsUsagesAsync(new PlayerRequest() { PlayerName = player });
            return result.Items.Select(i => new CardUsage() { CardId = i.CardId, PlayerName = player, Usages = i.Counts }).ToList();
        }
    }
}
