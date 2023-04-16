# Notice on use

### This code is not written with other people in mind so usefulness may vary. Anyone is free to use it however, and it won't break anything when imported into your project.

Related to this, **no proper official documentation is provided**, only the text below. While documentation will maybe be written at some point, currently the majority of our focus is on making our own games.

Email: monitorbreakgames@gmail.com

Twitter: [@Monitor_Break_](https://twitter.com/Monitor_Break_)

# License

Additions is licensed under a [MIT License](https://opensource.org/licenses/MIT) with an inserted [Commons Clause](https://commonsclause.com/). 

**The common clause was added to prevent people simply taking the things we have coded and selling them. Using Additions in commercial products is fully
in line with the license and Additions's intended use.**

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
Control of time-scale but with added priority control. Essentially a time-scale is passed with a given priority, alongside an object for identification so it can be removed later. The time-scale is automatically reset to the default when a scene loads.

*(When the full-screen console is opened it will set the time-scale to zero automatically. This uses this system and has a priority of 100, currently if you load a scene using a console command the time-scale will reset and not be zero even if the full-screen console is open.)*

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

### SavingAndLoadingUtility
Allows for the saving of arbitrary fields (as long as they are both static and public) by utilizing the **[SaveThis]** attribute. 'Save()' is called to actually save the data (into a .data file), with 'Load()' used to load the data.

The location to save the file is determined by 'Application.persistentDataPath' and the file is called 'generalData'.

## Monitor Break Bebug Namespace
	using MonitorBreak.Bebug;
"Bebug" = "Better Debug"

***(While this namespace is intended for debug it is currently not automatically disabled on build, there is a bool you can change at the top of the BebugManagement class as a temporary measure. However, this isn't a proper solution because all the log code will still run. We do also want the console avaliable in the builds of the game if needed so it might be the best solution right now.)***

### Console
A more performant version of the unity console, within the game itself. Also returns all objects logged for easy insertion into code. Supports multiple consoles at once and multiple console modes. To switch between console modes press the F1-F3 keys. F4 hides all consoles.

List of basic commands:

       '[Text]' Print something to console
	
       '.' or 'clear' clear console
	
       'hide' hide all consoles (same as F4)
	
       'exit' closes current console
	
       'new' add a new console
	
       TAB switch between consoles
	
WARNING: Currently, there is an issue with logging every frame, causing performance issues. Will be fixed at some point promise!
	
### ConsoleCMD
An attribute that allows for custom commands that will run a public, static function. The function cannot take any arguments but typically we will use this to run an in-between method that runs another method passing arguments.

Extra command included alongside:

	'commands' log all custom commands and their descriptors

### Graph
UI graphing, allows points to be plotted on a graph that is displayed on screen. Graph will resize to fit all points. 
