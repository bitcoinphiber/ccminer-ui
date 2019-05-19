namespace ccminer_gui
{
    class DefaultConfig : IConfig
    {
        public string Intensity => "0";

        public string Username => "pHQbs2WC4jwqEAq3s6yJp4MZf3pBLWwrPPrZrHPDZCeTR47PXK5hFsd1cfBHoccdBMNYSqZ5q6B2otZL";
        public string Password => "x";
        public string PoolUrl => "stratum+tcp://testnetpool.bitcoinphiber.com:3333";

        public int StatsAvg => 60;

        public string Algorithm => "Bitcoin Phiber";
    }
}
