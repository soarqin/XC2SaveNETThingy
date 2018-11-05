# CHANGELOG

## Release 6 (Release versioning changed due to conversion to multi-component package)
### XC2SaveNETThingy Core v1.0
Separated all the core backend stuff into a separate .dll file
- UPDATE: Added proper support for handling v1.5.0 data
- NEW: Added support for handling Torna Golden Country save data
- UPDATE: Fixed typos in GUI Text data and changed a few field names to reduce confusion (Select Weapon text for Driver Arts, Blade Special Recast Rev to Damage Multiplier)

### XC2Bf2SaveNETThingy v1.5
Properly handles Driver v1.5.0+ data + other fixes/updates
- UPDATE: Now properly handles Driver v1.5.0+ specific data
- BUGFIX: Driver ListBox could become blank if loading a save file after having already loaded one
- UPDATE: Moved Driver Skill Points GroupBox to Driver Affinity Chart tab

### XC2IraSaveNETThingy v1.0
Initial Release
- NEW: Created Save Editor for Torna Golden Country Save Files

## v1.4
Quick Bugfix Release + DB update to handle 2.0.0 content
- UPDATE: Identified BladeArtsExId and BladeArtsEx2Id as Blade Special IV IDs
- BUGFIX: BladeArtsExH and BladeArtsEx2H were being replaced by Blade Special IV IDs
- UPDATE: Added controls to more easily edit Blade Special IV
- UPDATE: Moved Blade Specials to new tab, separate from Blade Arts
- UPDATE: Added 2.0.0/Torna DLC Content to db (Names, Arts, Items etc.)
- NEW: Added setting to enable/disable the ability to edit Read-Only fields

## v1.3
New Features from User Requests
- NEW: Added Item sort function (Can sort by ID, Name, Qty, Equipped, Time Obtained, or Serial by clicking respective column header in Item grid
- NEW: Added button to Max out Qty of all Items in currently viewed Item Box

## v1.2
New Features + Ver 1.5.2 Support
- NEW: Added Blade Import/Export to/from File
- NEW: Added Existing Item search function
- NEW: Added Easy-Add New Item to ItemBox function
- NEW: Added Right-Click menu for Items to set Acquired Time to Current Time, and to Auto-Generate New Serial for an Item
- UPDATE: Changed how ComboBoxes show their data so name comes first, to allow for easy searching by typing into ComboBox
- UPDATE: Completely overhauled Game Flag Data section, now are several lists instead of hex box, also known Flags have ben named, such as Blade Awakening Videos Watched, or various other game stats (ex. some are used for Affinity Charts)
- UPDATE: Made Driver Stats, and some Rare Blade stats Read Only to avoid confusion since the game ignores those values anyway
- BUGFIX: Driver Arts Levels Combo Box could attempt to load an Art Level beyond what is known/implemented
- UPDATE: Restructured backend Database to easily allow future support for App translations to other languages
- UPDATE: Added support for 1.5.2 DLC data

## v1.1
Update to support Ver 1.5.1 DLC + Affinity Chart Import/Export
- NEW: Added ability to Import/Export Blade Affinity Chart Data to/from file
- UPDATE: Updated data files to support Ver 1.5.1 DLC
- UPDATE: Updated data files to recognise Driver: Jin (Challenge Mode)

## v1.0.1
- BUGFIX: Main Form refused to load save if Merc Group Blade Data was invalid
- BUGFIX: About form hyperlinks did not load URL when clicked

## v1.0
- Initial Release