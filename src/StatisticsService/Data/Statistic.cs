namespace StatisticsService.Data
{
    public class Statistic
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int Wins { get; private set; }
        public int Loses { get; private set; }
        public double Ratio { get; private set; }

        public void AddVictory()
        {
            Wins++;
            Ratio = (Wins / (Wins + Loses)) * 100;
        }

        public void AddDefeat()
        {
            Loses++;
            Ratio = (Wins / (Wins + Loses)) * 100;
        }
    }
}