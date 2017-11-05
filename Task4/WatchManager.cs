using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Task4.Configuration;
using Task4.Properties;

namespace Task4
{
    public class WatchManager : IWatchManager
    {
        private readonly ILogger _logger;
        private readonly FileSystemWatcher[] _watchers;
        private readonly Dictionary<Regex, string> _rules;

        public WatchManager(WatchManagerConfig config, ILogger logger)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;

            _watchers = config.Watchers.Select(w => new FileSystemWatcher(w.Path)).ToArray();
            _rules = config.Rules.ToDictionary(r => ConvertFileMaskToRegex(r.Filter), r => r.DestPath ?? config.Rules.DefaultDestPath);

            foreach (var watcher in _watchers)
            {
                watcher.Created += NewFileDetected;
            }
        }

        public void EnableWatching()
        {
            _logger.Log(Resources.StartWatching);

            foreach (var watcher in _watchers)
            {
                watcher.EnableRaisingEvents = true;
            }
        }

        public void DisableWatching()
        {
            foreach (var watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
            }
        }

        private void NewFileDetected(object sender, FileSystemEventArgs args)
        {
            _logger.Log(Resources.FileDetected, args.Name);

            var matchedRules = _rules.Where(r => r.Key.IsMatch(args.Name)).ToArray();

            _logger.Log(Resources.RulesForFileDetected, matchedRules.Length, args.Name);

            foreach (var rule in matchedRules)
            {
                _logger.Log(Resources.FileCopyToStart, args.Name, rule.Value);
                File.Copy(args.FullPath, Path.Combine(rule.Value, args.Name), true);
                _logger.Log(Resources.FileCopyToEnd, args.Name, rule.Value);
            }
        }

        private Regex ConvertFileMaskToRegex(string fileMask)
        {
            return new Regex(
                '^' +
                fileMask
                    .Replace(".", "[.]")
                    .Replace("*", ".*")
                    .Replace("?", ".")
                + '$',
                RegexOptions.IgnoreCase);
        }
    }
}