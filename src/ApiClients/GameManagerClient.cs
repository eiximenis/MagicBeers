using CardsLib;
using GameManagerService;
using Grpc.Net.Client;
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
        public GameManagerClient(string url)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public async Task<IEnumerable<Card>> GetInitialCards(string name, string mail)
        {
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
