# coordinate-frame-converter

A Unity utility for simply converting between different coordinate frames.

![Example Rotation](docs/example.gif)

Showing difference in euler angle application for frame `+X+Y-Z, -Z-X-Y` (left) and `+Z+X+Y, +X+Y+X` (right).

The angles `40, -120, -105` in the first frame are equivelant to `-40, -100, -150` in the second.



## Considerations
### Axis Direction

Which way is notionally considered "up", "right", and "forward":

```
 U    
 |   F
 | ／  
 .-----R
```

### Rotation Order

Which order Euler angles are applied.

### Rotation Direction

Whether positive rotation is clockwise or counterclockwise about the "right hand rule". Counterclockwise rotations are considered to be negative.

### Extrinsic and Intrinsic Rotations


## Use
```cs
using FrameConversions;

//...

// Unity forward is _into_ the screen, and uses left handed rotations
CoordinateFrame UnityFrame = new CoordinateFrame("+X+Y-Z", "-Z-X-Y");
CoordinateFrame OtherFrame = new CoordinateFrame("XYZ", "XYZ");

// Convert a position from the other coordinate frame into Unity's
OtherFrame.ConvertPosition(UnityFrame, new Vector3(1, 3, 2));

// Convert a set of euler angles from Unity Frame into the other frame
// Angles are specified in 20 about Z, 30 about X, 40 about Y
UnityFrame.ConvertEulerAngles(OtherFrame, new EulerAngles(20, 30, 40));

// Wrapper for converting between frames
CoordinateFrameConverter Unity2Other = new CoordinateFrameConverter(UnityFrame, OtherFrame);
Unity2Other.ConvertPosition(new Vector3(1,2,3));

// Converting from the other frame to Unity's
Unity2Other.inverse.ConvertPosition(new Vector3(1,2,3));
```

## Structs
### Unity.Vector3
Represents the position in a coordinate frame. Specifys a point `x` units on the X axis, `y` units on the Y axis, and `z` units on the Z axis in the particular frame the point is in.

### EulerAngles
Represents the ordered rotation in a coordinate frame. Specifies the order in degrees a rotation should occur about the first, second, then third rotation axis as specified in a rotation order axis set.

## Gotchas
- Unity takes euler orders as a vector and applies them in Z, X, Y order (taking them from place 2, then 0, then 1 from the vector). This library assumes the angles in EulerAngles are specified in the order they are applied, so Unity's would have to be specified as [Z, X, Y] in the struct. So Euler Angles specified as `Vector3(10, 20, 30)` in Unity would have to be specified as `EulerAngles(30, 10, 20)`

## TODO
- [ ] Get some real world test cases
- [ ] Look at options for reducing memory allocation and making AxisSets into structs
- [ ] Investigate using doubles to reduce floating point error in conversions
- [ ] Support extrinsic vs intrinsic rotation application
