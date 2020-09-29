using CardsLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClients
{
    public interface IGameManagerClient
    {
        public Task<IEnumerable<Card>> GetInitialCards(string name, string mail);
        Task<Card> GetNewCard(string name, string mail);
    }
}
