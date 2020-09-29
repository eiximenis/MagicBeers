using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatisticsService;

namespace ApiClients
{
    public class StatisticsClient : IStatisticsClient
    {
        private string _url;
        public StatisticsClient(string url)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public async Task AddLose(string player)
        {
            var request = new PlayerRequest() { PlayerName = player };
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new Statistics.StatisticsClient(channel);
            await client.AddLoseAsync(request);
        }

        public async Task AddWin(string player)
        {
            var request = new PlayerRequest() { PlayerName = player };
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new Statistics.StatisticsClient(channel);
            await client.AddWinAsync(request);
        }

        public async Task<PlayerStatistic> GetPlayerStats(string player)
        {
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
    }
}
