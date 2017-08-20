## Stellarator
Creates procedural, seed based solar systems for Kopernicus and Kerbal Space Program

### How to use it
At the moment there is no release candidate. You have to download the code and compile it using VisualStudio, MonoDevelop or whatever.
After doing that you need to run the .exe file in the Distribution/ folder. Enter your prefered seed, and the folder name you want to use.

Stellarator will create a systems/ folder with your system folder inside. Copy the system folder from systems/ to your GameData/. **Do NOT rename it.**

Be warned that Stellarator deletes the whole system and rebuilds it from scratch. Don't use this on existing savegames.

### Credit
Stellarator is based on the following libraries:
* [ConfigNode.dll](https://github.com/godarklight/ConfigNodeParser) - Standalone Implementation of KSPs config nodes by godarklight
* [libpqsmods.dll](https://github.com/Kopernicus/pqsmods-standalone) - Standalone Implementation of KSPs PQSMod types by ThomasKerman / The Kopernicus Team
* [libaccrete.dll](https://github.com/ThomasKerman/Accrete) - A library that simulates planet formation and accretation. Multiple authors, ported from C to C# by ThomasKerman

Other credit goes to:
* KillAshley and Sigma88: They are working on the prefab database used by Stellarator to create working but different planet setups
* regex / jbengston (Github): For giving me the idea to work on this and pointing me to the Accrete software
* GregroxMun: Made textures, names and pqs configs, and has awesome ideas
* Teknoman117: For writing the Kopernicus parser library and parts of the ModLoader classes, which are (ab)used in this program
* eggrobin: Name idea. Thanks :D

### License
Stellarator is licensed under the terms of the MIT license. It is based on Newtonsoft.Json and libpqsmods which are MIT licensed too.
ConfigNodeParser and libaccrete are released into the public domain.

Stellarator includes Code from the Kopernicus Library for KSP (ModLoader and Parser namespace), which is released under LGPL.

### Code Conventions

* Usings need to be outside of the namespace declaration.
* Use CLR-Type-Names instead of type aliases (e.g. Int32 instead of int).
* Don't use 'var', use CLR-Type-Names (e.g. Single instead of var).
* Watch out, some 'unused' methods mybe used by reflection, because it is called in a textfile.