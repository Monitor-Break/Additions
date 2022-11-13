# Notice on use

This code is not written with other people in mind so usefulness may vary. Anyone is free to use it however, and it won't break anything when imported into your project.

Related to this, **no proper official documentation is provided**, only the text below. While documentation will likely be written at some point, currently the majority of our focus is on ***MartEMart***.

Email: monitorbreakgames@gmail.com

Twitter: [@Monitor_Break_](https://twitter.com/Monitor_Break_)

# Installation

Package should be installed/updated using the Unity Package Manager via Git URL. 

[Unity Documentation on Installation from Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

# Monitor Break's Additions

This is a collection of all our additions to the Unity Engine, currently it includes:

In "MonitorBreak" namespace

	• Attributes
  
		IntializeAtRuntime
    
	• TimeManagment
  
	• ComponentContainer
  
In "MonitorBreak.Bebug" namespace

	• Console
  
	• Graph
  
	• Convertor
	
## Monitor Break Namespace
	using MonitorBreak;
### Attributes
#### IntializeAtRuntime
Creates an instance of the script on an empty game object, a path can be specified which will load an object from resources. This is intended to be used to load a version of the script with specific values/references.

### Time Managment
Control of time scale but with added priority control. Essentially an object can be given priority and only they are allowed to alter the time scale until the priority is given up.

### ComponentContainer
Implementaion of nested classes. Essentially allows a bunch of small components to be bundled into one file, to implement inherit on main class like you would with a monobehaviour. Then within the nested class make sure all functions (that will be called) are static.

Currently supports two functions	

- OnGenerate 

	Used for setting up values of nested classes, called when code is complied and when script is loaded in game.

- Update 

	Same as Unity Update, called every frame.

## Monitor Break Bebug Namespace
	using MonitorBreak.Bebug;
"Bebug" = "Better Debug"

***(While this namespace is intended for debug it is currently not automatically disabled on build, there is a static bool you can change at the top of the DebugManagment class as a temporary measure)***

### Console
A more performant version of the unity console, within the game itself. Also returns all objects logged for easy insertion into code. Supports multiple consoles at once and multiple console modes. To switch between console modes press the F1-F3 keys. F4 hides all consoles.

Full list of supported commands:

        '[Text]' Print something to console

        '/[Class Name].[Function Name]([Arguments])' Execute an arbitrary static function
	
        '.' or 'clear' clear console
	
        'hide' hide all consoles (same as F4)
	
        'exit' closes current console
	
        'new' add a new console
	
        TAB switch between consoles
	
### Graph
UI graphing, allows points to be plotted on a graph that is displayed on screen. Graph will resize to fit all points. 

### Convertor
Converts a string to any type with a constructor (no enums), implemented using recursion so use in actual builds may prove to be non-performant. Utilized in the console '/' command.
