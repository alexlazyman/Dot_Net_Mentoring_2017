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
        private readonly CopyOptions _copyOptions;
        private readonly string _dateTimeFormat;

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

            _watchers = config.Watchers
                .Where(w => new DirectoryInfo(w.Path).Exists)
                .Select(w => new FileSystemWatcher(w.Path))
                .ToArray();

            _rules = config.Rules.ToDictionary(
                r => new Regex(r.Filter),
                r => string.IsNullOrEmpty(r.DestPath) ? config.Rules.DefaultDestPath : r.DestPath);

            _copyOptions = config.CopyOptions;
            _dateTimeFormat = config.DateTimeFormat;

            foreach (var watcher in _watchers)
            {
                watcher.Created += FileDetected;
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

        private void FileDetected(object sender, FileSystemEventArgs args)
        {
            _logger.Log(Resources.FileDetected, args.Name);

            var matchedRules = _rules.Where(r => r.Key.IsMatch(args.Name)).ToArray();

            _logger.Log(Resources.RulesForFileDetected, matchedRules.Length, args.Name);

            foreach (var rule in matchedRules)
            {
                var fileName = TransformFileName(args.Name, rule.Value, _copyOptions);
                var destPath = Path.Combine(rule.Value, fileName);

                _logger.Log(Resources.FileCopyToStart, args.FullPath, destPath);

                if (File.Exists(destPath))
                {
                    File.Delete(destPath);
                }

                File.Move(args.FullPath, destPath);

                _logger.Log(Resources.FileCopyToEnd, args.FullPath, destPath);
            }
        }

        private string TransformFileName(string fileName, string destPath, CopyOptions copyOptions)
        {
            switch (copyOptions)
            {
                case CopyOptions.DateTime:
                    return fileName += "-" + DateTime.Now.ToString(_dateTimeFormat);
                case CopyOptions.Index:
                    for (int i = 0; i < int.MaxValue; i++)
                    {
                        var path = Path.Combine(destPath, fileName + "-" + i);
                        if (!File.Exists(path))
                        {
                            return fileName + "-" + i;
                        }
                    }
                    return fileName + "-" + int.MaxValue;
                default:
                    return fileName;
            }
        }
    }
}