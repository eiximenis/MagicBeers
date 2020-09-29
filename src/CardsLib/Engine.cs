using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib
{
    public static class Engine
    {
        public static int GetIbus(IEnumerable<Card> cards)
        {
            if (cards == null || !cards.Any()) return 0;

            var final = cards.ToList();
            foreach (var card in cards)
            {
                final = final.Select(c => c with { Ibus = card.Effect(c) }).ToList();
            }

            return final.Sum(c => c.Ibus);
        }
    }
}
