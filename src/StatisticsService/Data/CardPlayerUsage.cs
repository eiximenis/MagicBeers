using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticsService.Data
{
    public class CardPlayerUsage
    {
        public int Id { get; set; }
        public string Player { get; set; }
        public int CardId { get; set; }
        public int Count { get; set; }

        public void IncrementUsage()
        {
            Count++;
        }
    }
}
