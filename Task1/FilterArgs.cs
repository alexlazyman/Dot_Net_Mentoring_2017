using System;

namespace Task1
{
    public class FilterArgs : EventArgs
    {
        public string Path { get; private set; }

        public bool Exclude { get; set; }

        public FilterArgs(string path)
        {
            Path = path;
        }
    }
}