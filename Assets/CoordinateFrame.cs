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
        public string axis { get { return _axis; } }

        bool _negative;
        public bool negative { get { return _negative; } }
        
        public int xyzIndex { get { return axis == "X" ? 0 : axis == "Y" ? 1 : 2; } }

        public Axis(string axis, bool negative = false) {
            _axis = axis;
            _negative = negative;
        }

        public string ToString(bool includeSign = false) {
            string str = axis;
            if (includeSign) str = (negative ? "-" : "+") + str;
            return str;
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
        public Axis up      { get { return _axes[1]; } }
        public Axis forward { get { return _axes[2]; } }

        private AxisSet() { }

        public AxisSet(string axes, bool guaranteeUniqueness = true) {
            _axes = SanitizeAxisDescription(axes, guaranteeUniqueness);
            _axisToIndex = GetIndexMap(_axes);
        }

        public string ToString(bool includeSign = false) {
            string res = "";
            foreach (Axis a in _axes) res += a.ToString(includeSign);
            return res;
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
                    redundantCount += a.axis == b.axis ? 1 : 0;
                }
            }

            if (guaranteeUniqueness) Debug.Assert(redundantCount == 0);

            return axes;
        }

        Dictionary<string, int> GetIndexMap(Axis[] axes) {
            var dict = new Dictionary<string, int>();
            for (int i = 0; i < 3; i++) dict[axes[i].axis.ToUpper()] = i;
            return dict;
        }
    }

    public class CoordinateFrame {
        AxisSet _axes, _rotationOrder;

        public string Axes { get { return _axes.ToString(true); } }
        public string RotationOrder { get { return _rotationOrder.ToString(true); } }

        private CoordinateFrame() { }

        public CoordinateFrame(string lufAxes, string rotationOrder) {
            _axes = new AxisSet(lufAxes);
            _rotationOrder = new AxisSet(rotationOrder);
        }

        public Vector3 ToPosition(CoordinateFrame other, Vector3 v) {
            return FrameConversions.ToPosition(_axes, other._axes, v);
        }

        public Quaternion ToQuaternion(Vector3 euler) {
            return FrameConversions.ToQuaterion(_rotationOrder, euler);
        }

        public Vector3 ToEulerOrder(CoordinateFrame other, Vector3 euler) {
            throw new NotImplementedException();
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
    }

    delegate Vector3 EulerExtractionDelegate(Matrix4x4 mat, out AngleExtraction.EulerResult eu);
    static Dictionary<string, EulerExtractionDelegate> _extractionFunctions;
    static Dictionary<string, EulerExtractionDelegate> extractionFunctions {
        get {
            if(_extractionFunctions == null) {
                // Wrap all the functions to return *= -1 the value of the extraction function
                // The results seem to be negative. Also convert to degrees because we're considering
                // euler angles as degrees everywhere else
                // TODO: Is this because Unity's arrays aren't what we expect?
                Func<EulerExtractionDelegate, EulerExtractionDelegate> _wrapFunc = f => {
                    EulerExtractionDelegate res = delegate(Matrix4x4 m, out AngleExtraction.EulerResult eu) { return -Mathf.Rad2Deg * f(m, out eu); };
                    return res;
                };

                _extractionFunctions = new Dictionary<string, EulerExtractionDelegate>();
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
            int toIndex = to[fromAxis.axis];
            Axis toAxis = to[toIndex];

            res[toIndex] = fromAxis.negative == toAxis.negative ? v[i] : -v[i];
        }
        return res;
    }

    // Takes euler angles in degrees
    public static Quaternion ToQuaterion(AxisSet order, Vector3 euler) {
        Quaternion res = Quaternion.identity;

        for(int i = 0; i < 3; i ++) {
            Vector3 angles = Vector3.zero;
            Axis axis = order[i];
            angles[axis.xyzIndex] = axis.negative ? -euler[i] : euler[i];
           
            res = Quaternion.Euler(angles) * res;
        }
        return res;
    }

    // outputs euler angles in degrees
    public static Vector3 ExtractEulerAngles(AxisSet order, Quaternion quat) {
        Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero, quat, Vector3.one);
        AngleExtraction.EulerResult eu;

        Vector3 res = extractionFunctions[order.ToString()](mat, out eu);
        for (int i = 0; i < 3; i ++) {
            if (order[i].negative) res[i] *= -1;
        }
        return res;
    }

    public static Vector3 ToEulerOrder(AxisSet from, AxisSet to, Vector3 euler) {
        Quaternion quat = ToQuaterion(from, euler);
        return ExtractEulerAngles(to, quat);
    }
}
