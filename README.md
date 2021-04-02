# CitiesSkylines-Birdcage
Source code for the Birdcage mod.

Visit the [Steam Workshop page](http://steamcommunity.com/sharedfiles/filedetails/?id=649147853) to learn more about the mod.

[![AppVeyor](https://ci.appveyor.com/api/projects/status/github/sexyfishhorse/citiesskylines-birdcage?svg=true)](https://ci.appveyor.com/project/asser-dk/citiesskylines-birdcage) [![Chat on discord](https://img.shields.io/badge/chat-on%20discord-738bd7.svg)](https://discord.gg/AKvKQWr) [![License](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)](https://sexyfishhorse.mit-license.org/)

![Preview Image(PreviewImage.png)]

# How to build

You need to first add an environmental variable called `CS_GAME_DIRECTORY` which points to where Cities Skylines is installed.
E.g. `"c:\steam\steamapps\common\Cities_Skylines"`

# How to debug

When you build the project it will automatically copy the files to the mods folder. In order for that procedure to work you need to add an environmental variable called `CS_ADDONS_DIRECTORY` which points to the "Addons" folder. E.g. `"%LOCALAPPDATA%\Colossal Order\Cities_Skylines\Addons"`
