# XC2SaveNETThingy
## A Windows PC Save Editor for Xenoblade Chronicles 2

**WARNING! Back up your save file before modifying it!**

This app can edit just about every byte of data in a Xenoblade Chronicles 2 Save File.
This includes cahnging data to values that could not normally be obtained via regular gameplay.

## Links:
Gitlab - 

## How to use:

### 0. Ensure you have Windows computer and Microsoft .NET Framework 4.6 installed.
This is a Windows app running on the .NET Framework, so you require a modern Windows system, and the [Microsoft .NET Framework 4.6](https://www.microsoft.com/net/download/dotnet-framework-runtime/net46) installed on your system.
Running under other environments under things like wine/mono have not been tested.

### 1. Extract/Obtain your save file.
First you must obtain your save file in some way, and get it from your Switch to your PC. A good way to do this is to use the [Checkpoint Save Manager Switch Homebrew App](https://github.com/BernardoGiordano/Checkpoint), or something similar.

### 2. BACK IT UP!
Beefore going any further, be sure to make a backup copy of your extracted save file, and store it in a safe place. This is just in case you change some data to values that the game itself does not like.

### 3. Load it into the app.
Start the app, then click the "Load Save File..." button to select and load your save file, and the file should be loaded into the program, provided there are no issues with it.

### 4. Edit stuff!
Edit whatever fields you want! Keep in mind that the game may react strangely after loading in save data with values that are not obtainable via normal gameplay.
(Not that it should stop you from trying and experimenting, let's see how far the game can be pushed! :P)

### 5. Save your edited file.
After you are done editing, click the "Save File As..." button to save your save data to a file. the file needs to be called "bf2savefile.sav" for the game to recognize it.

### 6. Restore the save file to the console.
After that, you must restore it back to your Switch console. Again, this can be done with the [Checkpoint Save Manager Switch Homebrew App](https://github.com/BernardoGiordano/Checkpoint), or something similar.

### 7. Load up the game and check thing out/see what happens.
Finally, load up the game and your save, and enjoy your changes/see your data altering experiments!

## NOTES:
1. The game can behave strangely in reaction to certain changes that are not normally possible in regular gameplay. The game could do a few different things in such a situation:
- The game may be perfectly happy with your changes and run fine, changes and all
- The game may refuse your changes and reset them back to what they were supposed to be, as if your edit was never done.
- The game may try to run with your changes, only to not be programmed to handle your alterations and crash instead.

2. There has been limited testing done on the results of save editing on the game's side of things. While some testing has been done to ensure that the editor does in fact modify the correct part of the save for each field in the app, since there are no "safety gloves" or "hand holding" regarding what data values are allowed or not, the alterations that this app can make have not necessarily been tested in-game, and thus, it is not yet known exactly how the game will react to modifying every single field that is editable, especially if modified to values that cannot be obtained via normal gameplay. If you wish to experiment with this sort of stuff, then by all means do so, and please report your findings if you like. But please do make sure to backup your save file beforehand if you care about it!

