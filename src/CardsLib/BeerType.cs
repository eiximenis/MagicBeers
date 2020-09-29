using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CardsLib
{

    public enum BeerStyle
    {
        Lager,
        Bock,
        IPA,
        DIPA,
        Neipa,
        Dubbel,
        Tripel,
        Quadrupel,
        Porter,
        Stout,
        Effect
    }

    public static class BeerStyleExtensions
    {
        public static bool IsBlack(this BeerStyle style) => style == BeerStyle.Porter || style == BeerStyle.Stout;
        public static bool IsIpa(this BeerStyle style) => style == BeerStyle.IPA || style == BeerStyle.DIPA || style == BeerStyle.Neipa;
        public static bool IsAle(this BeerStyle style) => style != BeerStyle.Lager && style != BeerStyle.Bock;

        public static bool IsBelgian(this BeerStyle style) => style == BeerStyle.Dubbel || style == BeerStyle.Tripel || style == BeerStyle.Quadrupel;



    }
}
