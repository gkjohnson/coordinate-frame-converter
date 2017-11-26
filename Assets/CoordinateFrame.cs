using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using Debug = System.Diagnostics.Debug;

public class FrameConversions {

    // Description of an axis, such as -X
    public struct Axis {
        string _axis;
        public string name { get { return _axis; } }

        bool _negative;
        public bool negative { get { return _negative; } }
        
        public int xyzIndex { get { return name == "X" ? 0 : name == "Y" ? 1 : 2; } }

        public Axis(string axis, bool negative = false) {
            _axis = axis;
            _negative = negative;
        }

        // ToString override
        public string ToString(bool includeSign = false) {
            string str = name;
            if (includeSign) str = (negative ? "-" : "+") + str;
            return str;
        }

        // Operator overloading
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
    }

    // Description of a full axis set, such ax -X +Y +Z
    public class AxisSet {

        // Three axes ordered to describe left, up, and forward
        Axis[] _axes;
        Dictionary<string, int> _axisToIndex;

        public Axis this[int i] { get { return _axes[i]; } }
        public int this[string a] { get { return _axisToIndex[a.ToUpper()]; } }

        public Axis left    { get { return _axes[0]; } }
        public Axis right   { get { return -_axes[0]; } }
        public Axis up      { get { return _axes[1]; } }
        public Axis down    { get { return -_axes[1]; } }
        public Axis forward { get { return _axes[2]; } }
        public Axis back    { get { return -_axes[2]; } }

        private AxisSet() { }

        internal AxisSet(Axis left, Axis up, Axis forward) {
            _axes = new Axis[] { left, up, forward };
            _axisToIndex = GetIndexMap(_axes);
        }

        public AxisSet(string axes, bool guaranteeUniqueness = true) {
            _axes = SanitizeAxisDescription(axes, guaranteeUniqueness);
            _axisToIndex = GetIndexMap(_axes);
        }

        Axis[] SanitizeAxisDescription(string s, bool guaranteeUniqueness = true) {
            s = s.ToUpper();

            // Ensure we're in the right format
            Debug.Assert(new Regex("([+-]?[XYZ])([+-]?[XYZ])([+-]?[XYZ])").IsMatch(s));

            // Make sure we have three axes
            var matches = new Regex("[+-]?[XYZ]").Matches(s);
            Debug.Assert(matches.Count == 3);

            // Create the axes
            Axis[] axes = new Axis[3];
            for (int i = 0; i < matches.Count; i++) {
                string str = matches[i].Value;

                bool negative = str.Contains("-");
                string axis = str.Replace("-", "").Replace("+", "");

                axes[i] = new Axis(axis, negative);
            }

            // Make sure we don't have too many redundant axis names
            int redundantCount = 0;
            foreach (Axis a in axes) {
                foreach (Axis b in axes) {
                    redundantCount += a.name == b.name ? 1 : 0;
                }
            }

            if (guaranteeUniqueness) Debug.Assert(redundantCount == 0);

            return axes;
        }

        Dictionary<string, int> GetIndexMap(Axis[] axes) {
            var dict = new Dictionary<string, int>();
            for (int i = 0; i < 3; i++) dict[axes[i].name.ToUpper()] = i;
            return dict;
        }

        // ToString override
        public string ToString(bool includeSign = false) {
            string res = "";
            foreach (Axis a in _axes) res += a.ToString(includeSign);
            return res;
        }

        // Operator overloads
        public static bool operator ==(AxisSet a, AxisSet b) {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(AxisSet a, AxisSet b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            return right.GetHashCode() ^ up.GetHashCode() ^ forward.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(obj, null) || GetType() != obj.GetType()) return false;
            AxisSet other = (AxisSet)obj;
            return right == other.right && up == other.up && forward == other.forward;
        }
    }

    public class CoordinateFrame {
        AxisSet _axes, _rotationOrder;

        public AxisSet Axes { get { return _axes; } }
        public AxisSet RotationOrder { get { return _rotationOrder; } }

        private CoordinateFrame() { }

        public CoordinateFrame(string lufAxes, string rotationOrder) {
            _axes = new AxisSet(lufAxes);
            _rotationOrder = new AxisSet(rotationOrder);
        }

        public Vector3 ToPosition(CoordinateFrame other, Vector3 v) {
            return FrameConversions.ToPosition(_axes, other._axes, v);
        }

        public Quaternion ToQuaternion(Vector3 euler) {
            return FrameConversions.ToQuaternion(_rotationOrder, euler);
        }

        public Vector3 ToEulerOrder(CoordinateFrame other, Vector3 euler) {
            throw new NotImplementedException();
        }

        // ToString override
        public string ToString(bool includeSign = false) {
            return Axes.ToString(includeSign) + ", " + RotationOrder.ToString(includeSign);
        }

        // Operator overloads
        public static bool operator ==(CoordinateFrame a, CoordinateFrame b) {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(CoordinateFrame a, CoordinateFrame b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            return Axes.GetHashCode() ^ RotationOrder.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(obj, null) || GetType() != obj.GetType()) return false;
            CoordinateFrame other = (CoordinateFrame)obj;
            return Axes == other.Axes && RotationOrder == other.RotationOrder;
        }
    }

    public class CoordinateFrameConverter {
        CoordinateFrame _fromFrame, _toFrame;
        CoordinateFrameConverter _inverse;

        public CoordinateFrame from { get { return _fromFrame; } }
        public CoordinateFrame to { get { return _toFrame; } }
        public CoordinateFrameConverter inverse {
            get {
                if (_inverse == null) _inverse = new CoordinateFrameConverter(_toFrame, _fromFrame);
                return _inverse;
            }
        }

        private CoordinateFrameConverter() { }

        public CoordinateFrameConverter(CoordinateFrame from, CoordinateFrame to) {
            _fromFrame = from;
            _toFrame = to;
        }

        public CoordinateFrameConverter(string fromAxis, string fromRotation, string toAxis, string toRotation)
            :this(new CoordinateFrame(fromAxis, fromRotation), new CoordinateFrame(toAxis, toRotation)) { }

        public Vector3 ConvertPosition(Vector3 p) { return _fromFrame.ToPosition(_toFrame, p); }
        public Vector3 ConvertEulerAngles(Vector3 euler) { return _fromFrame.ToEulerOrder(_toFrame, euler); }
        
        // ToString override
        public string ToString(bool includeSign = false) {
            return from.ToString(includeSign) + " to " + to.ToString(includeSign);
        }

        // Operator overloads
        public static bool operator ==(CoordinateFrameConverter a, CoordinateFrameConverter b) {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(CoordinateFrameConverter a, CoordinateFrameConverter b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            return from.GetHashCode() ^ to.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(obj, null) || GetType() != obj.GetType()) return false;

            CoordinateFrameConverter other = (CoordinateFrameConverter)obj;
            return from == other.from && to == other.to;
        }
    }

    delegate Vector3 QuatEulerExtractionDelegate(Quaternion quat, out AngleExtraction.EulerResult eu);
    delegate Vector3 RawEulerExtractionDelegate(Matrix4x4 mat, out AngleExtraction.EulerResult eu);
    static Dictionary<string, QuatEulerExtractionDelegate> _extractionFunctions;
    static Dictionary<string, QuatEulerExtractionDelegate> extractionFunctions {
        get {
            if(_extractionFunctions == null) {
                // - Wrap all the functions to return *= -1 the value of the extraction function
                // because Unity's rotation order is applied as counter clockwise by default
                // - Wrap the functions so they can have quaternions passed into them directly
                // - Convert to degrees
                Func<RawEulerExtractionDelegate, QuatEulerExtractionDelegate> _wrapFunc = f => {
                    QuatEulerExtractionDelegate res = delegate(Quaternion q, out AngleExtraction.EulerResult eu) {
                        return Mathf.Rad2Deg * f(Matrix4x4.TRS(Vector3.zero, q, Vector3.one), out eu);
                    };
                    return res;
                };

                _extractionFunctions = new Dictionary<string, QuatEulerExtractionDelegate>();
                _extractionFunctions["XYZ"] = _wrapFunc(AngleExtraction.ExtractEulerXYZ);
                _extractionFunctions["XZY"] = _wrapFunc(AngleExtraction.ExtractEulerXZY);
                _extractionFunctions["YXZ"] = _wrapFunc(AngleExtraction.ExtractEulerYXZ);
                _extractionFunctions["YZX"] = _wrapFunc(AngleExtraction.ExtractEulerYZX);
                _extractionFunctions["ZXY"] = _wrapFunc(AngleExtraction.ExtractEulerZXY);
                _extractionFunctions["ZYX"] = _wrapFunc(AngleExtraction.ExtractEulerZYX);
                _extractionFunctions["XYX"] = _wrapFunc(AngleExtraction.ExtractEulerXYX);
                _extractionFunctions["XZX"] = _wrapFunc(AngleExtraction.ExtractEulerXZX);
                _extractionFunctions["YXY"] = _wrapFunc(AngleExtraction.ExtractEulerYXY);
                _extractionFunctions["YZY"] = _wrapFunc(AngleExtraction.ExtractEulerYZY);
                _extractionFunctions["ZXZ"] = _wrapFunc(AngleExtraction.ExtractEulerZXZ);
                _extractionFunctions["ZYZ"] = _wrapFunc(AngleExtraction.ExtractEulerZYZ);
            }

            return _extractionFunctions;
        }
    }

    public static Vector3 ToPosition(AxisSet from, AxisSet to, Vector3 v) {
        Vector3 res = new Vector3();
        for(int i = 0; i < 3; i ++) {
            Axis fromAxis = from[i];
            int toIndex = to[fromAxis.name];
            Axis toAxis = to[toIndex];

            res[toIndex] = fromAxis.negative == toAxis.negative ? v[i] : -v[i];
        }
        return res;
    }

    // Takes euler angles in degrees
    public static Quaternion ToQuaternion(AxisSet order, Vector3 euler) {
        Quaternion res = Quaternion.identity;

        for(int i = 0; i < 3; i ++) {
            Vector3 angles = Vector3.zero;
            Axis axis = order[i];
            angles[axis.xyzIndex] = axis.negative ? -euler[i] : euler[i];

            // Unity's default rotation order is negative
            angles *= -1;

            res = Quaternion.Euler(angles) * res;
        }
        return res;
    }

    // outputs euler angles in degrees
    public static Vector3 ExtractEulerAngles(AxisSet order, Quaternion quat) {
        AngleExtraction.EulerResult eu;
        Vector3 res = extractionFunctions[order.ToString()](quat, out eu);
        for (int i = 0; i < 3; i ++) {
            if (order[i].negative) res[i] *= -1;
        }
        return res;
    }

    public static Vector3 ToEulerOrder(AxisSet from, AxisSet to, Vector3 euler) {
        Quaternion quat = ToQuaternion(from, euler);
        return ExtractEulerAngles(to, quat);
    }
}
