using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Task1
{
    public class FileSystemVisitor : IFileSystemVisitor
    {
        private readonly string _path;
        private readonly IDirectoryReader _directoryReader;
        private readonly Predicate<string> _filter;

        public event EventHandler Start;

        public event EventHandler Finish;

        public event EventHandler<VisitArgs> FileFound;

        public event EventHandler<VisitArgs> FilteredFileFound;

        public event EventHandler<VisitArgs> DirectoryFound;

        public event EventHandler<VisitArgs> FilteredDirectoryFound;

        public FileSystemVisitor(string path, IDirectoryReader directoryReader, Predicate<string> filter = null)
        {
            _path = path;
            _directoryReader = directoryReader;
            _filter = filter;
        }

        public IEnumerator<string> GetEnumerator()
        {
            var visitState = new VisitState();

            OnStart();

            if (_directoryReader.IsFile(_path))
            {
                if (VisitFile(_path, visitState) == _path)
                {
                    yield return _path;
                }
            }
            else if (_directoryReader.IsDirectory(_path))
            {
                foreach (var path in VisitDirectory(_path, visitState))
                {
                    yield return path;

                    if (visitState.SearchFinished)
                    {
                        break;
                    }
                }
            }

            visitState.SearchFinished = true;

            OnFinish();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<string> VisitDirectory(string path, VisitState visitState)
        {
            var visitArgs = new VisitArgs(path);

            OnDirectoryFound(visitArgs);
            if (visitArgs.StopSearch)
            {
                visitState.SearchFinished = true;
            }

            if (!IsFiltered(path))
            {
                yield break;
            }

            OnFilteredDirectoryFound(visitArgs);
            if (visitArgs.StopSearch)
            {
                visitState.SearchFinished = true;
            }

            yield return path;

            foreach (var visitPath in _directoryReader.GetDirectories(path).SelectMany(p => VisitDirectory(p, visitState)))
            {
                yield return visitPath;
            }

            foreach (var filePath in _directoryReader.GetFiles(path))
            {
                if (VisitFile(filePath, visitState) == filePath)
                {
                    yield return filePath;
                }
            }
        }

        private string VisitFile(string path, VisitState visitState)
        {
            var visitArgs = new VisitArgs(path);

            OnFileFound(visitArgs);
            if (visitArgs.StopSearch)
            {
                visitState.SearchFinished = true;
            }

            if (!IsFiltered(path))
            {
                return null;
            }

            OnFilteredFileFound(visitArgs);
            if (visitArgs.StopSearch)
            {
                visitState.SearchFinished = true;
            }

            return path;
        }

        private bool IsFiltered(string path)
        {
            return _filter == null || _filter(path);
        }

        #region Event calls

        private void OnStart()
        {
            Start.Run(this, null);
        }

        private void OnFinish()
        {
            Finish.Run(this, null);
        }

        private void OnFileFound(VisitArgs args)
        {
            FileFound.Run(this, args);
        }

        private void OnFilteredFileFound(VisitArgs args)
        {
            FilteredFileFound.Run(this, args);
        }

        private void OnDirectoryFound(VisitArgs args)
        {
            DirectoryFound.Run(this, args);
        }

        private void OnFilteredDirectoryFound(VisitArgs args)
        {
            FilteredDirectoryFound.Run(this, args);
        }

        #endregion
    }
}
