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

Unity's rotation order would then be considered to be `-Z`, `-Y`, `-X`.

### Transforms
#### Example Frames
##### Frame1
position: `+X, +Y, -Z`

rotation: `-Z, -X, -Y`

```
      Y 
      |   Z
      | ／
      .-------X
```

##### Frame2
position: `+Y, -Z, +X`

rotation: `+X, +Z, +Y`

```
      .-------Y
   ／ |
 X    |
      Z
```

#### Position Transforms
```js
// Convert a vector as specified in the original coordinate
// frame's conventions into one specified in the new coordinate
// frame's conventions
Frame1        =>  Frame2
(X, Y, Z)         (-Z, X, -Y)

Frame2        =>  Frame1
(X, Y, Z)         (Y, -Z, -X)
```

#### Rotation Transforms
TODO
