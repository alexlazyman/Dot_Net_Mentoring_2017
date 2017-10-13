using System;

namespace Task1
{
    public class VisitArgs : EventArgs
    {
        public string Path { get; private set; }

        public bool StopSearch { get; set; }

        public VisitArgs(string path)
        {
            Path = path;
        }
    }
}