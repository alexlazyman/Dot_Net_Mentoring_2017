using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Task1
{
    public class FileSystemVisitor : IEnumerable<string>
    {
        private readonly string _path;
        private readonly IDirectoryReader _directoryReader;

        public event EventHandler<FilterArgs> Filter;

        public event EventHandler Start;

        public event EventHandler Finish;

        public event EventHandler<VisitArgs> FileFound;

        public event EventHandler<VisitArgs> FilteredFileFound;

        public event EventHandler<VisitArgs> DirectoryFound;

        public event EventHandler<VisitArgs> FilteredDirectoryFound;

        public FileSystemVisitor(string path, IDirectoryReader directoryReader = null)
        {
            _path = path;
            _directoryReader = directoryReader ?? new DirectoryReader();
        }

        public FileSystemVisitor(string path, EventHandler<FilterArgs> filter, IDirectoryReader directoryReader = null)
            : this(path, directoryReader)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            Filter += filter;
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
            var filterArgs = new FilterArgs(path);

            OnFilter(filterArgs);

            return !filterArgs.Exclude;
        }

        #region Event calls

        private void OnFilter(FilterArgs args)
        {
            if (Filter == null)
            {
                return;
            }

            var handlers = Filter.GetInvocationList();
            foreach (EventHandler<FilterArgs> handler in handlers)
            {
                handler.Invoke(this, args);
                if (args.Exclude)
                {
                    return;
                }
            }
        }

        private void OnStart()
        {
            if (Start == null)
            {
                return;
            }

            var handlers = Start.GetInvocationList();
            foreach (EventHandler handler in handlers)
            {
                handler.Invoke(this, null);
            }
        }

        private void OnFinish()
        {
            if (Finish == null)
            {
                return;
            }

            var handlers = Finish.GetInvocationList();
            foreach (EventHandler handler in handlers)
            {
                handler.Invoke(this, null);
            }
        }

        private void OnFileFound(VisitArgs args)
        {
            if (FileFound == null)
            {
                return;
            }

            var handlers = FileFound.GetInvocationList();
            foreach (EventHandler<VisitArgs> handler in handlers)
            {
                handler.Invoke(this, args);
            }
        }

        private void OnFilteredFileFound(VisitArgs args)
        {
            if (FilteredFileFound == null)
            {
                return;
            }

            var handlers = FilteredFileFound.GetInvocationList();
            foreach (EventHandler<VisitArgs> handler in handlers)
            {
                handler.Invoke(this, args);
            }
        }

        private void OnDirectoryFound(VisitArgs args)
        {
            if (DirectoryFound == null)
            {
                return;
            }

            var handlers = DirectoryFound.GetInvocationList();
            foreach (EventHandler<VisitArgs> handler in handlers)
            {
                handler.Invoke(this, args);
            }
        }

        private void OnFilteredDirectoryFound(VisitArgs args)
        {
            if (FilteredDirectoryFound == null)
            {
                return;
            }

            var handlers = FilteredDirectoryFound.GetInvocationList();
            foreach (EventHandler<VisitArgs> handler in handlers)
            {
                handler.Invoke(this, args);
            }
        }

        #endregion
    }
}
