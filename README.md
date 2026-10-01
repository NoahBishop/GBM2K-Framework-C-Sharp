# GBM2K-Framework-C-Sharp

The GBM2K (Grid Based Movement 2K) Framework is a Godot 4 C# project that facilitates the creation of 2D grid-based games like *YumeNikki*.

> **Note:** The original GDScript-based logic has been modified and rewritten in **C#**. This version is now implemented in C#.

The framework replicates the grid movement seen in *RPG Maker 2003* while keeping things simple and easily modifiable. But if you want grid movement more akin to *RPG Maker 2000*, you can use [this](https://gist.github.com/Oplexitie/fd25d94caa8970f743bd86ef5e33e0ee).

## Features

- C# implementation (migrated from the original GDScript version)
- Grid movement that doesn't utilize Godot's collision system
- An input priority system that prioritizes the last directional key pressed
- NPCs that can be interacted with (triggers a function)
- An event system (cells that can trigger a function if stepped on)
- A* Pathfinding system (with NPC example included)

## C# Version

- The framework logic was previously implemented in GDScript.
- It has now been modified and rewritten in C#.
- Use a Godot 4 build with .NET/C# support and a compatible .NET SDK.

## Other info

Credits to GDQuest and their Grid Based Movement system. Without it, this wouldn't be possible.  
Also, credits to Jason Perry for the sprites. These sprites are from the OpenRTP.

If you encounter any bugs with an **unmodified version of the framework**, please post the issue on GitHub with a video attached.  
If you want to contribute to the project, do a pull request on GitHub and I'll take a look at it.

The GBM2K Framework is licensed under the MIT license.  
Meaning that you can do whatever you want as long as you credit me, and include the MIT license contained within this repository with your software/source.  
Reminder, this is my understanding of the MIT license. I'm not a lawyer, do your own research.
