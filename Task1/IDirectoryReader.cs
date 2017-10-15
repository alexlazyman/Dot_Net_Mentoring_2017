using System.Collections.Generic;

namespace Task1
{
    public interface IDirectoryReader
    {
        IEnumerable<string> GetDirectories(string path);

        IEnumerable<string> GetFiles(string path);

        bool IsFile(string path);

        bool IsDirectory(string path);
    }
}