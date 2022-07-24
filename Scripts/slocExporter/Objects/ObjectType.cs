﻿namespace slocExporter.Objects {

    public enum ObjectType : byte {

        None = 0,
        Light = 1,
        Cube = 2,
        Sphere = 3,
        Capsule = 4,
        Cylinder = 5,
        Plane = 6,
        Quad = 7,
        Empty = 8 // TODO: Add support for this object type

    }

}
