# coordinate-frame-converter

A Unity utility for simply converting between different coordinate frames.

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

## Classes
### Unity.Vector3
TODO ...

### EulerAngles
TODO ...

### AxisSet
TODO ...

### CoordinateFrame
TODO ...

### CoordinateFrameConverter
TODO ...

## Gotchas
- Unity takes euler orders as a vector and applies them in Z, X, Y order (taking them from place 2, then 0, then 1 from the vector). This library assumes the angles in EulerAngles are specified in the order they are applied, so Unity's would have to be specified as [Z, X, Y] in the struct.

TODO ...

## TODO
- [ ] Add a custom struct for a vector
- [ ] Get some real world test cases
- [ ] Document how Vector3 and EulerAngle structs work relative to the coordinate frames
- [ ] Add an example for converting between frames
- [ ] Pictures
- [ ] Gizmo Helper
