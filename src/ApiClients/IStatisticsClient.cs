using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClients
{
    public interface IStatisticsClient
    {
        Task<PlayerStatistic> GetPlayerStats(string player);

        Task AddLose(string player);
        Task AddWin(string player);

    }
}
