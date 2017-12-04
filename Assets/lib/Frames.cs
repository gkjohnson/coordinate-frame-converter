namespace FrameConversions {
    static class Frames {

        public static CoordinateFrame _Unity = null;
        public static CoordinateFrame Unity { get { return _Unity = _Unity ?? new CoordinateFrame("+X+Y-Z", "-Z-X-Y"); } }

        public static CoordinateFrame _ROS = null;
        public static CoordinateFrame ROS { get { return _ROS = _ROS ?? new CoordinateFrame("-Y+Z+X", "+Z+Y+X"); } }

        public static CoordinateFrameConverter _Unity2ROS = null;
        public static CoordinateFrameConverter Unity2ROS { get { return _Unity2ROS = _Unity2ROS ?? new CoordinateFrameConverter(Unity, ROS); } }
        public static CoordinateFrameConverter ROS2Unity { get { return Unity2ROS.inverse; } }
    }
}