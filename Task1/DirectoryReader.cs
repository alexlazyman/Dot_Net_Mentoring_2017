using System.Collections.Generic;
using System.IO;

namespace Task1
{
    public class DirectoryReader : IDirectoryReader
    {
        public IEnumerable<string> GetDirectories(string path)
        {
            return Directory.EnumerateDirectories(path);
        }

        public IEnumerable<string> GetFiles(string path)
        {
            return Directory.EnumerateFiles(path);
        }

        public bool IsFile(string path)
        {
            return new FileInfo(path).Exists;
        }

        public bool IsDirectory(string path)
        {
            return new DirectoryInfo(path).Exists;
        }
    }
}