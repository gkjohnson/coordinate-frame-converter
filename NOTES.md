# Notes

## Coordinate Frames

### Conventions

#### Directionality
Axis conventions should be considered to be ordered in `right`, `up`, `forward`.

```cs
// Conventional Right-Handed Frame

      U
      |
      |
      .-------R
   ／ 
 F       
```

#### Left Handedness
Left handed frames are therefore created by negating the appropriate axis.

Unity's coordinate frame would then be considered to be `X`, `Y`, `-Z` in that order

#### Rotation Order
Rotation order is considered to be applied in the specified order about the axes of the given coordinate frame.

#### Rotation Direction
Rotations are considered to be applied using the right-hand rule, so it's necessary to negate the axis to apply the left hand rule, or counter-clockwise rotations.

In simple terms, `+` indicates right-hand-rule with thumb in the positive direction of the axis, while `-` indicates left-hand-rule in the direction of hte axis.

Unity's rotation order would then be considered to be `-Z`, `-Y`, `-X`.

### Transforms
#### Example Frames
##### Frame1
position: `+X, +Y, -Z`
```cs
Y 
|   Z
| ／
.-------X
```

rotation: `-Z, -X, -Y`
```cs
(-3)
 |   (-1)
 | ／
 .-------(-2)
```

##### Frame2
position: `+Y, -Z, +X`
```cs
      .-------Y
   ／ |
 X    |
      Z
```

rotation: `+X, +Z, +Y`
```cs
      .-------(+3)
   ／ |
(+1)  |
     (+2)
```

#### Position Transforms
```cs
// Convert a vector as specified in the original coordinate
// frame's conventions into one specified in the new coordinate
// frame's conventions
Frame1        =>  Frame2
(+X, +Y, +Z)      (-Z, +X, -Y)

Frame2        =>  Frame1
(+X, +Y, +Z)      (+Y, -Z, -X)
```

#### Rotation Transforms
`(sign num)` indicates counter-clock and order of rotation application 

```cs
Frame1OrderInFrame1Axes       =>    Frame1OrderInFrame2Axes
// Z to X (same notional axis) - to + (negation because the axes flip, accomodating the counter-clock rotation)
// X to Y (same notional axis) - to - (in the same direction)
// Y to Z (same notional axis) - to + (negation because the axes flip, accomodating the counter-clock rotation)
(-Z, -X, -Y)                        (+X, -Y, +Z)

(-3)                                      .-------(-2)
 |  (-1)                               ／ |
 | ／                               (+1)  |
 .-------(-2)                            (+3)
```

TODO...
