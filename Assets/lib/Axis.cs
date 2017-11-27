namespace FrameConversions {
    // Description of an axis, such as -X
    public struct Axis {
        enum Direction { RIGHT = 0, UP = 1, FORWARD = 2 }

        // The name of the axis
        string _name;
        public string name { get { return _name; } }

        // Whether or not the axis is considered negative
        // for the given direction
        bool _negative;
        public bool negative { get { return _negative; } }
        
        // TODO: Remove this
        public int xyzIndex { get { return name == "X" ? 0 : name == "Y" ? 1 : 2; } }

        #region Constructors
        public Axis(string axis, bool negative = false) {
            _name = axis;
            _negative = negative;
        }
        #endregion

        #region To String
        public string ToString(bool includeSign = false) {
            string str = name;
            if (includeSign) str = (negative ? "-" : "+") + str;
            return str;
        }
        #endregion

        #region Operator Overloading
        public static Axis operator -(Axis axis) {
            Axis a = axis;
            a._negative = !a._negative;
            return a;
        }

        public static bool operator ==(Axis a, Axis b) {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Axis a, Axis b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(obj, null) || GetType() != obj.GetType()) return false;

            Axis other = (Axis)obj;
            return name == other.name && negative == other.negative;
        }
        #endregion
    }
}