# Notice on use

This code is not written with other people in mind so usefulness may vary. Anyone is free to use it however, and it won't break anything when imported into your project.

Related to this, **no proper official documentation is provided**, only the text below. While documentation will maybe be written at some point, currently the majority of our focus is on ***MartEMart***.

Email: monitorbreakgames@gmail.com

Twitter: [@Monitor_Break_](https://twitter.com/Monitor_Break_)

# License

Additions is licensed under a [MIT License](https://opensource.org/licenses/MIT) with an inserted [Commons Clause](https://commonsclause.com/). 

**The common clause was added to prevent people simply taking the things we have coded and selling them. Using Additions in commercial products is fully
in line with the license.**

A plain-text version of the license is included in the repository.

# Installation

Package should be installed/updated using the Unity Package Manager via Git URL. 

```
https://github.com/Monitor-Break/Additions.git
```

[Unity Documentation on Installation from Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

# Monitor Break's Additions

This is a collection of all our additions to the Unity Engine, currently it includes:

In "MonitorBreak" namespace

	• Attributes
  
		- IntializeAtRuntime
    
	• TimeManagement
  
	• ComponentContainer
  
	• CodeSuspender
  
In "MonitorBreak.Bebug" namespace

	• Console
  
	• Graph
  
	• Convertor
	
## Monitor Break Namespace
	using MonitorBreak;
### Attributes
#### IntializeAtRuntime
Creates an instance of the script on an empty game object, a path can be specified which will load an object from resources. This is intended to be used to load a version of the script with specific values/references.

### Time Management
Control of time scale but with added priority control. Essentially an object can be given priority and only they are allowed to alter the time scale until the priority is given up.

### ComponentContainer
Implementaion of nested classes. Essentially allows a bunch of small components to be bundled into one file. To implement, inherit on container class like you would with a monobehaviour.

Currently supports two functions	

- OnGenerate 

	Used for setting up values of nested classes, called when code is complied and when script is loaded in game.

- Update 

	Same as Unity Update, called every frame.

**Make sure they are static, otherwise the code won't pick them up!**

### CodeSuspender
Similar to an event but everything that is registered with the event also passes itself. When the suspender is triggered all objects registered are given 'priority', after they have finished executing they should run the 'Done()' function (passing themselves). Once all registered objects have said they are done (or on trigger if no objects are registered) then a method passed in the CodeSuspender constructor is called.

Essentially it allows you to run code only after other code has been executed.

## Monitor Break Bebug Namespace
	using MonitorBreak.Bebug;
"Bebug" = "Better Debug"

***(While this namespace is intended for debug it is currently not automatically disabled on build, there is a bool you can change at the top of the DebugManagement class as a temporary measure. However, this isn't a proper solution because all the log code will still run. We do also want the console avaliable in the builds of the game if needed so it might be the best solution right now.)***

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
Converts a string to any type with a constructor (no enums)/any fundamental type, implemented using recursion so use in actual builds may prove to be non-performant. Utilized in the console '/' command.
