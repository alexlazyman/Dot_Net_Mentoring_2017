using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public class FileSystemVisitor : IEnumerable<string>
    {
        private string _path;
        private bool _isVisitFinished;

        public event EventHandler<FilterArgs> Filter;

        public event EventHandler Start;

        public event EventHandler Finish;

        public event EventHandler<VisitArgs> FileFound;

        public event EventHandler<VisitArgs> FilteredFileFound;

        public event EventHandler<VisitArgs> DirectoryFound;

        public event EventHandler<VisitArgs> FilteredDirectoryFound;

        public FileSystemVisitor(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            this._path = path;
        }

        public FileSystemVisitor(string path, EventHandler<FilterArgs> filter)
            : this(path)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            Filter += filter;
        }

        public IEnumerator<string> GetEnumerator()
        {
            _isVisitFinished = false;
            OnStart();

            if (IsFile(_path))
            {
                if (VisitFile(_path) == _path)
                {
                    yield return _path;
                }
            }
            else
            {
                foreach (var path in VisitDirectory(_path))
                {
                    yield return path;

                    if (_isVisitFinished)
                    {
                        break;
                    }
                }
            }

            OnFinish();
            _isVisitFinished = true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<string> VisitDirectory(string path)
        {
            var visitArgs = new VisitArgs(path);

            OnDirectoryFound(visitArgs);
            if (visitArgs.StopSearch)
            {
                _isVisitFinished = true;
            }

            if (!IsFiltered(path))
            {
                yield break;
            }

            OnFilteredDirectoryFound(visitArgs);
            if (visitArgs.StopSearch)
            {
                _isVisitFinished = true;
            }

            yield return path;

            foreach (var visitPath in Directory.EnumerateDirectories(path).SelectMany(VisitDirectory))
            {
                yield return visitPath;
            }

            foreach (var filePath in Directory.EnumerateFiles(path))
            {
                if (VisitFile(path) == filePath)
                {
                    yield return filePath;
                }
            }
        }

        private string VisitFile(string path)
        {
            var visitArgs = new VisitArgs(path);

            OnFileFound(visitArgs);
            if (visitArgs.StopSearch)
            {
                _isVisitFinished = true;
            }

            if (!IsFiltered(path))
            {
                return null;
            }

            OnFilteredFileFound(visitArgs);
            if (visitArgs.StopSearch)
            {
                _isVisitFinished = true;
            }

            return path;
        }

        private bool IsFiltered(string path)
        {
            var filterArgs = new FilterArgs(path);

            OnFilter(filterArgs);

            return !filterArgs.Exclude;
        }

        private bool IsFile(string path)
        {
            return new FileInfo(path).Exists;
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
