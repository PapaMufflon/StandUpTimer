StandUpTimer
============
This program came to life because I read two articles: one about sit-stand-dynamcis and the other one about Squirrel.Windows. So, the Stand-Up Timer is a Windows program that reminds you when it is best to stand up from your seat and continue working in a standing position. Equally it reminds you to sit down again when you stood long enough.

http://papamufflon.github.io/StandUpTimer/ is the place to read the documentation for the program, and here is the place to read about the implementation details.

##Squirrel.Windows
For me, ClickOnce is like TFS (Version 2008). It works, but if you really want to work with it, digging rather deep, you are screwed. [Squirrel.Windows](https://github.com/Squirrel/Squirrel.Windows) fixes that (at least it works with this small program really well). The setup installs silently and immediately starts the program. Updates are downloaded and installed while the program runs and on the next run, the updated version starts.

###Integration steps
For the Stand-Up Timer, it is enough to use the standard way of Squirrel.Windows. I just included

	Task.Run(async () =>
		{
			using (var mgr = new UpdateManager(@"http://mufflonosoft.blob.core.windows.net/standuptimer", "StandUpTimer", FrameworkVersion.Net45))
				await mgr.UpdateApp();
		});

in the OnStartup method. The updates will be installed silently and on the next start, the newer version - undetected by the user - is started. That's it for integration.

###Deployment Steps
In order to use Squirrel.Windows, you have to create a NuGet package as a container for all your files. That container uses a command line utility of Squirrel.Windows to create the final files to distribute (a setup.exe, the NuGet package and a file with the current version of the program). With the tool azcopy, these files will be uploaded to Azure Blob Storage where users can download the setup and Squirrel can find the updates.

##Concordion.Net
One more article I read was about [Concordion](http://concordion.org/dotnet/index.html). It is an acceptance test tool like FitNesse, Cucumber or RSpec. I already tried the others, so let's build some living documentation with this one.

The interesting thing with Concordion is, that it parses html files interweaved with your specifications and writes the results back to it. With the help of [TestStack.White](https://github.com/TestStack/White) and [TestStack.Seleno](https://github.com/TestStack/TestStack.Seleno), I create a nice little automated user documentation which gets uploaded to http://papamufflon.github.io/StandUpTimer/ every time I deploy a new version.
