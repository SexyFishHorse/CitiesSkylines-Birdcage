namespace SexyFishHorse.CitiesSkylines.Infrastructure.IO
{
    using System;

    public class ModFolderPathAttribute : Attribute
    {
        public ModFolderPathAttribute(string path)
            : this(new[] { path })
        {
        }

        public ModFolderPathAttribute(string[] paths)
        {
            Paths = paths;
        }

        public string[] Paths { get; private set; }
    }
}
