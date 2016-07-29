namespace SexyFishHorse.CitiesSkylines.Infrastructure.Configuration
{
    using System;
    using System.Collections.Generic;

    public class ModConfiguration
    {
        public ModConfiguration()
        {
            Settings = new List<KeyValuePair<string, object>>();
        }

        public List<KeyValuePair<string, object>> Settings { get; set; }
    }
}
