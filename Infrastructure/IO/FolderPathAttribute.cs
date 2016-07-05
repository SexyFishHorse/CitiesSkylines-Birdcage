namespace SexyFishHorse.CitiesSkylines.Infrastructure.IO
{
    using System;

    public class FolderPathAttribute : Attribute
    {
        public FolderPathAttribute(string path)
            : this(new[] { path })
        {
        }

        public FolderPathAttribute(string[] paths)
        {
            Paths = paths;
        }

        public string[] Paths { get; private set; }
    }
}
