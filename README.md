# Unity Camera Image Capture

Capture camera image and save to a specified path.

![Demo Gif](https://github.com/blightue/UnityCameraImageCapture/blob/main/Resource/Demo.gif)

## Installation instructions

### Unity package manager(Git)

Follow the [official instruction](https://docs.unity3d.com/Manual/upm-ui-giturl.html) and fill the git URL with https://github.com/blightue/UnityCameraImageCapture.git



## Tutorials

1. Add component `CameraImageCapture` to a GameObject and assign the component in your script.
2. Config `CameraImageCapture` value in the Inspector.
3. Call `CameraImageCapture.CaptureAndSaveImage()` .
4. `CameraImageCapture` fields can changed during runtime.

```c#
using CIC.Core;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    public CameraImageCapture cic;

    private void Start()
    {
        cic.fileName = "Sample";
        cic.imageResolution = new Vector2Int(1920, 1080);
        cic.CaptureAndSaveImage();
    }
}
```

