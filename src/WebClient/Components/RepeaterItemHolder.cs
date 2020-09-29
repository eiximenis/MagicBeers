using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Components
{
    public class RepeaterItemHolder<T>
    {
        public T Item { get; }
        public int Index { get; }
        public RepeaterItemHolder(int index, T item)
        {
            Index = index;
            Item = item;
        }
    }
}
