using CardsLib;
using GameManagerService;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApiClients
{
    public class GameManagerClient : IGameManagerClient
    {
        private string _url;
        private readonly ILogger<IGameManagerClient> _logger;
        private readonly bool _useHttp;
        public GameManagerClient(string url, ILogger<IGameManagerClient> logger)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _useHttp = new Uri(url).Scheme.ToLowerInvariant() == "http";
            _logger.LogInformation($"GameManager gRPC url: {_url}. Using http: {_useHttp}");
        }

        public async Task<IEnumerable<Card>> GetInitialCards(string name, string mail)
        {
            if (_useHttp)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new Manager.ManagerClient(channel);
            var response = await client.GetInitialHandAsync(new InitialHandRequest()
            {
                MaxCardId = CardsRepository.MaxCards,
                PlayerEmail = mail ?? "",
                PlayerName = name ?? ""
            });

            return response.Cards.Select(c => CardsRepository.GetCardById(c.CardId));
        }

        public async Task<Card> GetNewCard(string name, string mail)
        {
            if (_useHttp)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            using var channel = GrpcChannel.ForAddress(_url);
            var client = new Manager.ManagerClient(channel);
            var response = await client.GetNewCardAsync(new InitialHandRequest()
            {
                MaxCardId = CardsRepository.MaxCards,
                PlayerEmail = mail,
                PlayerName = name
            });

            return CardsRepository.GetCardById(response.CardId);

        }
    }
}
