using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib
{
    public static class CardsRepository
    {
        private static List<Card> _cards;

        static CardsRepository()
        {
            _cards = new List<Card>();
            AddAllCards();
        }

        private static void AddAllCards()
        {
            _cards.Add(new Card()
            {
                Id =1,
                Ibus = 1,
                Name ="Industrial Lager",
                Style = BeerStyle.Lager,
                Text="There are some good lagers out there, but they're all craft beers. Industrial lagers are just like soda.",
            });
            _cards.Add(new Card()
            {
                Id = 2,
                Ibus = 0,
                Name = "Adam",
                Style = BeerStyle.Effect,
                Text = "Having a Master Brewer is always a good thing. Even if is a crazy one like Adam. IBUS of all your beers get doubled",
                Effect = c => c.Ibus * 2
            });
            _cards.Add(new Card()
            {
                Id = 3,
                Ibus = 1000,
                Name = "Mikkeller 1000 IBUs",
                Style = BeerStyle.IPA,
                Text = "If IBUS are all that matter, this beer is the one above all."
            });
            _cards.Add(new Card()
            {
                Id = 4,
                Ibus = 250,
                Name = "Ibuprofano",
                Style = BeerStyle.DIPA,
                Text = "Headache? Time for an IbuprofAno!"
            });
            _cards.Add(new Card()
            {
                Id = 5,
                Ibus = 0,
                Name = "Your car!",
                Style = BeerStyle.Effect,
                Text = "You love your car, but not today. You must drive, so not drinks today. IBUs of all your beers go to 0",
                Effect = c => 0
            }); 
            _cards.Add(new Card()
            {
                Id = 6,
                Ibus = 20,
                Name = "Dead Monk",
                Style = BeerStyle.Dubbel,
                Text = "Crafted in the crypts below Castle Sant Angelo."
            });
            _cards.Add(new Card()
            {
                Id = 7,
                Ibus = 0,
                Name = "Monk Party!",
                Style = BeerStyle.Effect,
                Text = "Long live drunken monks! All Belgian beers double IBUs",
                Effect = c => c.Style.IsBelgian() ? c.Ibus * 2 : c.Ibus
            });
            _cards.Add(new Card()
            {
                Id = 8,
                Ibus = 0,
                Name = "Dry Hopping",
                Style = BeerStyle.Effect,
                Text = "Adding an extra hop to all your IPAs? Good! Let's double their IBUs!",
                Effect = c => c.Style.IsIpa() ? c.Ibus * 2 : c.Ibus
            });
            _cards.Add(new Card()
            {
                Id = 9,
                Ibus = 40,
                Name = "Punk IPA",
                Style = BeerStyle.IPA,
                Text = "The one who started it all. #Respect",
            });
            _cards.Add(new Card()
            {
                Id = 10,
                Ibus = 90,
                Name = "Impaled",
                Style = BeerStyle.DIPA,
                Text = "One of the best IPAs of the world. PERIOD.",
            });
            _cards.Add(new Card()
            {
                Id = 11,
                Ibus = 10,
                Name = "Voll Damm",
                Style = BeerStyle.Bock,
                Text = "Standard Industrial Bock. Nothing more, nothing else",
            });
            _cards.Add(new Card()
            {
                Id = 12,
                Ibus = 10,
                Name = "1906",
                Style = BeerStyle.Bock,
                Text = "Standard Industrial Bock. Nothing more, nothing else",
            });
            _cards.Add(new Card()
            {
                Id = 13,
                Ibus = 0,
                Name = "Paga Fantas",
                Style = BeerStyle.Effect,
                Text = "He buyed a lot of fantas, and now is making radlers with all your beers. All beers lose half IBUs",
                Effect = c => c.Ibus / 2
            });
            _cards.Add(new Card()
            {
                Id = 14,
                Ibus = 20,
                Name = "Guinness",
                Style = BeerStyle.Stout,
                Text = "The king of stouts makes all other stouts even better! Their IBUs are doubled!",
                Effect = c => c.Style.IsBlack() && c.Id != 14 ? c.Ibus * 2 : c.Ibus
            });
            _cards.Add(new Card()
            {
                Id = 15,
                Ibus = 80,
                Name = "Mala Vida Bourbon",
                Style = BeerStyle.Stout,
                Text = "It's more liqueur than beer..."
            });
            _cards.Add(new Card()
            {
                Id = 16,
                Ibus = 35,
                Name = "Optimo Bruno",
                Style = BeerStyle.Quadrupel,
                Text = "Simply the best Grimbergen out there."
            });
            _cards.Add(new Card()
            {
                Id = 17,
                Ibus = 0,
                Name = "Gushing",
                Style = BeerStyle.Effect,
                Text = "Bad luck! All your beers had overflown. No IBUs for you today.",
                Effect = c => 0
            });
            _cards.Add(new Card()
            {
                Id = 18,
                Ibus = 20,
                Name = "Murphy's",
                Style = BeerStyle.Stout,
                Text = "Looks and tastes like a Guinness. But is a Murphy's!"
            });
            _cards.Add(new Card()
            {
                Id = 19,
                Ibus = 0,
                Name = "Industrial Sales Representative",
                Style = BeerStyle.Effect,
                Text = "He sells you all their industrial lagers (1 IBU each beer)",
                Effect = c => c.Style != BeerStyle.Effect ? 1 : 0
            });
            _cards.Add(new Card()
            {
                Id = 20,
                Ibus = 40,
                Name = "Fuller's London Porter",
                Style = BeerStyle.Porter,
                Text = "Welcome to the reign of black beers!"
            });
        }

        public static Card EmptyCard()
        {
            return new Card()
            {
                Id = 0,
                Ibus = 0,
                Name = "Who Knows?",
                Style = BeerStyle.Effect,
                Text = "Is Good? Or Is Bad?"
            };
        }

        public static Card GetCardById (int id)
        {
            return _cards[id - 1];
        }

        public static int MaxCards { get => _cards.Count; }
    }
}
