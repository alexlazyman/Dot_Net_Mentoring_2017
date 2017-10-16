using System;
using System.Collections.Generic;

namespace Task1
{
    public interface IFileSystemVisitor : IEnumerable<string>
    {
        event EventHandler Start;

        event EventHandler Finish;

        event EventHandler<VisitArgs> FileFound;

        event EventHandler<VisitArgs> FilteredFileFound;

        event EventHandler<VisitArgs> DirectoryFound;

        event EventHandler<VisitArgs> FilteredDirectoryFound;
    }
}
