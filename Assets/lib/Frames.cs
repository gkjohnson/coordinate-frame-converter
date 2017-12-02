namespace FrameConversions {
    static class Frames {

        static CoordinateFrame _Unity = null;
        static CoordinateFrame Unity { get { return _Unity = _Unity ?? new CoordinateFrame("+X+Y-Z", "-Z-X-Y"); } }

        static CoordinateFrame _ROS = null;
        static CoordinateFrame ROS { get { return _ROS = _ROS ?? new CoordinateFrame("-Y+Z+X", "+Z+Y+X"); } }

        static CoordinateFrameConverter _Unity2ROS = null;
        static CoordinateFrameConverter Unity2ROS { get { return _Unity2ROS = _Unity2ROS ?? new CoordinateFrameConverter(Unity, ROS); } }
        static CoordinateFrameConverter ROS2Unity { get { return Unity2ROS.inverse; } }
    }
}