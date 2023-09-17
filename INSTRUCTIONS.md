# START THE GAME:


## MacOS

1. Locate the WebGL Build: First, make sure you have built your Unity project for WebGL. This should generate a folder containing the necessary files for the WebGL version of your game.

2. Open Terminal: Open the Terminal app on your macOS. You can find it in the Utilities folder within the Applications folder, or you can use Spotlight Search to locate it quickly.

3. Navigate to the WebGL Build Folder: Use the cd (change directory) command to navigate to the folder containing your WebGL build. For example, if your build is on your desktop, you can navigate to it with the following command:

bash

```
cd ~/Desktop/YourWebGLBuildFolder
```
(Replace `YourWebGLBuildFolder` with the actual name of your build folder.)

4. Start a Simple Web Server: macOS comes with a built-in Python web server that you can use to serve your WebGL build. In the terminal, enter the following command:

```
   python -m SimpleHTTPServer 8000
```

If you're using Python 3, you can use:

```
python3 -m http.server 8000
```

5. Access Your Game: After running the above command, your game should now be hosted locally. Open a web browser and go to the following address:

```
   http://localhost:8000
```


## Windows


If you don't know how to start a web server to host the game (and have Unity installed, else please google):

1. Open your terminal and navigate to the folder where you installed unity
2. Continue to open your editors build tools, you should now be in a path that looks similar to this:
   
```
   D:\Programs\Unity\Hub\Editor\2021.3.18f1\Editor\Data\PlaybackEngines\WebGLSupport\BuildTools
```

3. Execute: `.\SimpleWebServer.exe "Path to the unpacked Build_WebGL.zip" 8000`
4. Open a browser and go to the address "localhost:8000"
