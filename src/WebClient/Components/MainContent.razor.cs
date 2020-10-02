using ApiClients;
using CardsLib;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebClient.Components
{
    public partial class MainContent
    {

        private Card[] _cards;

        [Inject]
        public IGameManagerClient GameManagerClient { get; set; }
        [Inject]
        public IStatisticsClient StatsClient { get; set; }
        
        [Inject]
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        protected string PlayerName { get; private set; }

        protected IEnumerable<Card> Hand { get => _cards; }

        public IEnumerable<Card> RivalHand { get; private set; }

        protected int ChangesRemaining { get; private set; }

        public int YourIbus { get; private set; }
        public int RivalIbus { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            YourIbus = -1;
            RivalIbus = -1;
            PlayerName = HttpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "<none>";
            ChangesRemaining = 2;
            RivalHand = Enumerable.Range(1, 4).Select(x => CardsRepository.EmptyCard());
            _cards = (await GameManagerClient.GetInitialCards(PlayerName, "not@used.com")).ToArray();

            foreach (var card in _cards)
            {
                await StatsClient.AddCardUsed(PlayerName, card.Id);
            }
        }

        protected async Task ChangeCard(int idx)
        {
            var card = await GameManagerClient.GetNewCard(PlayerName, "not@used.com");
            _cards[idx] = card;
            await StatsClient.AddCardUsed(PlayerName, card.Id);
            ChangesRemaining--;
        }

        protected async Task Challenge()
        {
            ChangesRemaining = 0;
            RivalHand = (await GameManagerClient.GetInitialCards(null, null)).ToList();

            var rivalIbus = Engine.GetIbus(RivalHand);
            var yourIbus = Engine.GetIbus(Hand);

            if (rivalIbus >= yourIbus)
            {
                await StatsClient.AddLose(PlayerName);
            }
            else
            {
                await StatsClient.AddWin(PlayerName);
            }

            RivalIbus = rivalIbus;
            YourIbus = yourIbus;

        }

    }
}
