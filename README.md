# **Littlewargame client made for Windows 7+**

It contains additional improvements to the web-based client such as borderless windowed mode, additional hotkeys, not being tied to your web browser to play and cursor lock to window not lagging.

The client also supports the Steam overlay, as well as multiple profiles.

This project is completely open source. Feel free to fork and make pull requests, additionally please report any issues or bugs [here](https://github.com/ivanpmartell/LittleWarGameClient/issues).

## Important

If updating from versions below 0.8.0:

Versions 0.8.0+ are not compatible with previous versions. The libraries required to run the program have been updated and a full update is needed.
Download the complete package from below, and replace your previous files with the updated files. Do not use update_64/86.zip file to update.
The data and settings folder can be kept so as to not lose any of your previous configurations, e.g. downloaded replays or login cookies.

## Download

Grab the `lwg_clientx64.zip` file from the latest release. Click [here](https://github.com/ivanpmartell/LittleWarGameClient/releases/latest) for easier access to the download.

## Settings

Settings for the client can be set and altered through the ```settings/{profile_name}.ini``` file. If the settings are changed in-game, these changes will be reflected on the ini file and viceversa.

## Profiles

To run with a certain profile add the command line argument `-profile` and the name you want the profile to have:

`LittleWarGameClient.exe -profile name`

This will also let you run multiple instance of the game at the same time. The default profile name is ```main```.

## Overlays

The OpenGL overlay is enabled by default. You can change the overlay to use Direct2D, or turn the overlay completely.
The overlay allows steam overlay integration, as well as notifications, e.g. Download progress of replays.

## Plugin Support

Plugin support has been added to enable custom Javascript functionality for the game. Please note that if the game server is updated, your custom functionality might stop working.
If you are a developer of a plugin, please update the plugin and send a pull request to the [LittleWarGame Plugins Repository](https://github.com/ivanpmartell/LittleWarGamePlugins/).