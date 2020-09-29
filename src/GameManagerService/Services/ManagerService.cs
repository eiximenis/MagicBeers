using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GameManagerService
{
    public class ManagerService : Manager.ManagerBase
    {
        private readonly ILogger<ManagerService> _logger;
        public ManagerService(ILogger<ManagerService> logger)
        {
            _logger = logger;
        }

        public override Task<InitialHandResponse> GetInitialHand(InitialHandRequest request, ServerCallContext context)
        {
            var rnd = new Random();
            var ids = Enumerable.Range(1, 4)
                .Select(x => rnd.Next(1, request.MaxCardId + 1))
                .Select(i => new CardInfo() { CardId = i });


            _logger.LogDebug($"Drawn cards for {request.PlayerEmail}: ${string.Join(',', ids.Select(c => c.CardId))}");

            var response = new InitialHandResponse();
            response.Cards.AddRange(ids);
            return Task.FromResult(response);
        }

        public override Task<CardInfo> GetNewCard(InitialHandRequest request, ServerCallContext context)
        {
            var rnd = new Random();
            var id = rnd.Next(1, request.MaxCardId + 1);
            return Task.FromResult(new CardInfo() { CardId = id });
        }
    }
}
