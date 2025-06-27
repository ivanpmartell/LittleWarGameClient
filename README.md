# **Littlewargame client made for Windows 7+**

It contains additional improvements to the web-based client such as borderless windowed mode, additional hotkeys, not being tied to your web browser to play and cursor lock to window not lagging.

The client also supports the Steam overlay, as well as multiple profiles.

This project is completely open source. Feel free to fork and make pull requests, additionally please report any issues [here](https://github.com/ivanpmartell/LittleWarGameClient/issues).

## Important

If updating from versions below 0.6.3:

Versions 0.6.3+ are not compatible with previous versions. The libraries required to run the program have been updated and a full update is needed.
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

Overlays are turned off by default. You can select an OpenGL or Direct2D overlay to allow steam overlay integration.

## Code Injection

Javascript can be replaced to allow for custom functionality. Please note that if the game server is updated, your custom functionality might not work.
In that case please update the ```js/lwg.js``` file to handle the server update.