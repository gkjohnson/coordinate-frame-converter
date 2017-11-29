namespace FrameConversions {
    static class Frames {
        static CoordinateFrame _Unity = null;
        static CoordinateFrame Unity { get { return _Unity = _Unity ?? new CoordinateFrame("+X+Y-Z", "-Z-X-Y"); } }
    }
}