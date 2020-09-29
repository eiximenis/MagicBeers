using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClients.Requests
{
    class CardUsedRequest
    {
        public string PlayerName { get; init; }
        public int CardId { get; init; }
    }
}
