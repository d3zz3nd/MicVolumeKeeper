Keep mic volume on 100%(or any other value, if you change it)

Here you can change the value (Program.cs):
```
                if (device.AudioEndpointVolume.MasterVolumeLevelScalar < 1.0f)
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = 1.0f;
                }
```
then just add shortcut on it in your startup
