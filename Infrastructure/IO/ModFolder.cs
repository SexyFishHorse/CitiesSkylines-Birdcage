namespace SexyFishHorse.CitiesSkylines.Infrastructure.IO
{
    public enum ModFolder
    {
        [ModFolderPath("")]
        Root = 0,

        [ModFolderPath("Addons")]
        Addons = 1,

        [ModFolderPath(new[] { "Addons", "Assets" })]
        Assets = 100,

        [ModFolderPath(new[] { "Addons", "ColorCorrections" })]
        ColorCorrections = 101,

        [ModFolderPath(new[] { "Addons", "Import" })]
        Import = 102,

        [ModFolderPath(new[] { "Addons", "MapEditor" })]
        MapEditor = 103,

        [ModFolderPath(new[] { "Addons", "MapEditor", "Brushes" })]
        Brushes = 103001,

        [ModFolderPath(new[] { "Addons", "MapEditor", "Heightmaps" })]
        Heightmaps = 103002,

        [ModFolderPath("Maps")]
        Maps = 2,

        [ModFolderPath("Saves")]
        Saves = 3,

        [ModFolderPath("Snapshots")]
        Snapshots = 4,

        [ModFolderPath("WorkshopStagingArea")]
        WorkshopStagingArea = 5,
    }
}
