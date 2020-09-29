using System;

namespace CardsLib
{
    public record Card
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public string Text { get; init; }

        public BeerStyle Style { get; init; }

        public int Ibus { get; init; }

        public Func<Card, int> Effect { get; init; }

        public Card()
        {
            Effect = x => x.Ibus;
        }
    }
}
