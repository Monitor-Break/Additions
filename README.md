# Additions

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
### Attributes
#### IntializeAtRuntime
Creates an instance of the script on an empty game object, a path can be specified which will load an object from resources. This is intended to be used to load a version of the script with specific values/references.

### Time Managment
Control of time scale but with added priority control. Essentially an object can be given priority and only they are allowed to alter the time scale until the priority is given up.

### ComponentContainer
Implementaion of nested classes. Essentially allows a bunch of small components to be bundled into one file, to implement inherit like you would with a monobehaviour.

## Monitor Break Bebug Namespace
"Bebug" = "Better Debug"

### Console
A more performant version of the unity console, also returns all objects logged. Supports multiple consoles at once and multiple console modes. To switch between console modes press the F1-F3 keys. F4 hides all consoles.

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

# Installation

Package should be installed using the Unity Package Manager via git url. 

[Unity Documentation on Installation from Git Url](https://docs.unity3d.com/Manual/upm-ui-giturl.html)
