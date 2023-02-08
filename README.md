# Unity Camera Image Capture

![version](https://badgen.net/badge/version/0.6.5/orange) ![license](https://badgen.net/github/license/blightue/unitycameraImagecapture)

Capture camera image and save to a specified path.

![Demo Gif](https://imgur.com/a/hLTTZzF)

[Full Documentation](https://blightue.github.io/CameraImageCapture)

## Installation instructions

### Unity package manager(Git)

1. Go to **Package Manager** window via `Window/Package Manager`
2. Click the **add** ![img](https://docs.unity3d.com/uploads/Main/iconAdd.png) button in the status bar
3. Select **Add package from git URL** from the add menu
4. Fill the git URL with https://github.com/blightue/UnityCameraImageCapture.git and click **Add**

### Unity package manager(Local)

1. Download and unzip the source code to your disk
2. Go to **Package Manager** window via `Window/Package Manager`
3. Click the **add** ![img](https://docs.unity3d.com/uploads/Main/iconAdd.png) button in the status bar
4. Select **Add package from disk** from the add menu
5. Select the **package.json** file in the unzipped folder

## Quick Start

### Version 0.5.0 Update: 

Recommend use component **Capture with config**

Create a config asset by click the plus button in the Project window and follow `Camera Image Capture/Capturer config`

![Config Gif](https://imgur.com/a/0yBhIZt)

### Editor

#### Component

1. Add component `CameraImageCapture` to a `GameObject` via `Camera Image Capture/CameraImageCapture`

2. Config `CameraImageCapture` value in the Inspector.

   - **Target Camera** : Target camera for capture
   - **File Name** : The name of captured image file
   - **Export folder** : The folder for the captured image file.
   - **Image serialized** : Is the image file name serialized. If set to true, the file name will be **[fileName]-0.jpg** **[fileName]-1.jpg** ...
   - **Is Log Capture** : Is log capture information when image captured
   - **Write type** :
     - Main Thread: Write the file in main thread. It will block the main thread
     - Async: Write the file asynchronous. **Caution use this type in MonoBehavior.Update() function**

   - **Image format** : PNG JPG TGA
   - **Is Override Camera Resolution** : False to set your own image resolution. Image will follow target camera **FOV Axis**

3. Click **Capture and save** button for capturing

#### EditorWindow

1. Open `Camera Image Capture` window via `Tools/Camera Image Capture/Capturer`

2. Add and Config `CaptureConfig` value in the Inspector.

3. Click **Capture and save** button for capturing

### Runtime

#### Static Singleton

```c#
using SuiSuiShou.CIC.Core;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    void Start()
    {
        Photographer.IsImageSerial = false;
        Photographer.ImageResolution = new Vector2Int(1920, 1080);
        Photographer.CaptureAndSaveImage();
    }
}
```

#### Component

1. Assign the `CameraImageCapture` component  in your code

2. Call `CameraImageCapture.CaptureAndSaveImage()` 

3. `CameraImageCapture` fields can changed during runtime

   **Code Sample**

```c#
using SuiSuiShou.CIC.Core;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    public CameraImageCapture cic;

    private void Start()
    {
        cic.FileName = "Sample";
        cic.ImageResolution = new Vector2Int(1920, 1080);
        cic.CaptureAndSaveImage();
    }
}
```
