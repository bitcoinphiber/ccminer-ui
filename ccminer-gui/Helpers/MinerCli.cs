using ccminer_gui.Entities;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ccminer_gui
{
    class MinerCli : CLIHelper
    {
        public MinerCli()
        {
            _stratumDifficulty = new Regex(@"Stratum difficulty set to (.+?)$");
            _blockNumber = new Regex(@".+?:[0-9]+? .+? block (.+?)$");
            _gpuMatch = new Regex(@"GPU #[0-9]:.+?,(.+?) (kH/s|MH/s)");

            _sharesMatch = new Regex(@"accepted: (.+?)/(.+?) \(diff (.+?), ping.+?, (.+?) (kH/s|MH/s)");
        }

        public MinerReport MinerReport
        {
            get
            {
                return _minerReport;
            }
            set
            {
                _minerReport = value;
            }
        }

        public void Run(IConfig config, string algorithm)
        {
            Open(Path.Combine(Environment.CurrentDirectory, "ccminer.exe"), new string[] {
                "-N",
                config.StatsAvg.ToString(),
                "-i",
                config.Intensity,
                "-a",
                algorithm,
                "-o",
                config.PoolUrl,
                "-u",
                config.Username,
                "-p",
                config.Password,
                "--no-color --no-longpoll --no-extranonce"
            });
            OutputDataReceived += MinerCli_OutputDataReceived;
        }

        private void MinerCli_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            var data = e.Data;
            // Check for matches.
            if (_stratumDifficulty.IsMatch(data))
            {
                var match = _stratumDifficulty.Match(data);
                _minerReport.StratumDifficulty = Convert.ToDecimal(match.Groups[1].Value);
            }

            if (_blockNumber.IsMatch(data))
            {
                var match = _blockNumber.Match(data);
                _minerReport.Block = Convert.ToInt32(match.Groups[1].Value);
            }

            if (_gpuMatch.IsMatch(data))
            {
                var match = _gpuMatch.Match(data);
                if (match.Groups[2].Value == "MH/s")
                {
                    _minerReport.TotalHashrate = Convert.ToDecimal(match.Groups[1].Value);
                }
                else
                {
                    _minerReport.TotalHashrate = (Convert.ToDecimal(match.Groups[1].Value) / 1000);
                }
            }

            if (_sharesMatch.IsMatch(data))
            {
                var match = _sharesMatch.Match(data);
                _minerReport.AcceptedShares = Convert.ToInt32(match.Groups[1].Value);
                _minerReport.TotalShares = Convert.ToInt32(match.Groups[2].Value);
                _minerReport.BlockDifficulty = Convert.ToDecimal(match.Groups[3].Value);
                _minerReport.StaleShares = _minerReport.TotalShares - _minerReport.AcceptedShares;

                if (match.Groups[5].Value == "MH/s")
                {
                    _minerReport.TotalHashrate = Convert.ToDecimal(match.Groups[4].Value);
                }
                else
                {
                    _minerReport.TotalHashrate = (Convert.ToDecimal(match.Groups[4].Value) / 1000);
                }
            }
        }

        private MinerReport _minerReport = new MinerReport();

        private Regex _stratumDifficulty;
        private Regex _blockNumber;

        private Regex _gpuMatch;
        private Regex _sharesMatch;
    }
}
