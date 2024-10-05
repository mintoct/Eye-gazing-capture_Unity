# Eye-gazing-capture_Unity
The code is for the HTC Vive Pro Eye

Device: HTC Vive Eye Pro Environment: Unity Official Instructions: https://developer.tobii.com/xr/develop/xr-sdk/getting-started/vive-pro-eye/

Eye tracking data:

Add component to HMD Object
Chose S Ranipal_Eye_Framework
Chose Enable Eye, Enable Eye Data Callback, Enable Eye Version: Version 2
Add EyeTracking_v2 to the new component. Ref Details: https://forum.htc.com/topic/12586-sranipal-data-analysis/
Eye gazing data: Add collide in Unity to each Object (Box collider, Mesh collider, Capsule collider) Tips: when creating the 3D models in Blender, please make sure the object is a unit or parts (that you planned for). Thus, you can input the FBX file into Unity and use mesh collider directly.

Add component.
Attach objectData script.
