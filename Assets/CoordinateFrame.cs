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

        public string ToString(bool includeSign = false) {
            string res = "";
            foreach (Axis a in _axes) res += a.ToString(includeSign);
            return res;
        }
    }

    delegate Vector3 EulerExtractionDelegate(Matrix4x4 mat, out AngleExtraction.EulerResult eu);
    static Dictionary<string, EulerExtractionDelegate> _extractionFunctions;
    static Dictionary<string, EulerExtractionDelegate> extractionFunctions {
        get {
            if(_extractionFunctions == null) {
                _extractionFunctions = new Dictionary<string, EulerExtractionDelegate>();
                _extractionFunctions["XYZ"] = AngleExtraction.ExtractEulerXYZ;
                _extractionFunctions["XZY"] = AngleExtraction.ExtractEulerXZY;
                _extractionFunctions["YXZ"] = AngleExtraction.ExtractEulerYXZ;
                _extractionFunctions["YZX"] = AngleExtraction.ExtractEulerYZX;
                _extractionFunctions["ZXY"] = AngleExtraction.ExtractEulerZXY;
                _extractionFunctions["ZYX"] = AngleExtraction.ExtractEulerZYX;
                _extractionFunctions["XYX"] = AngleExtraction.ExtractEulerXYX;
                _extractionFunctions["XZX"] = AngleExtraction.ExtractEulerXZX;
                _extractionFunctions["YXY"] = AngleExtraction.ExtractEulerYXY;
                _extractionFunctions["YZY"] = AngleExtraction.ExtractEulerYZY;
                _extractionFunctions["ZXZ"] = AngleExtraction.ExtractEulerZXZ;
                _extractionFunctions["ZYZ"] = AngleExtraction.ExtractEulerZYZ;
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

    public static Quaternion ToQuaterion(AxisSet order, Vector3 euler) {
        Quaternion res = new Quaternion();

        for(int i = 0; i < 3; i ++) {
            Vector3 angles = Vector3.zero;
            Axis axis = order[i];
            angles[axis.xyzIndex] = axis.negative ? -euler[i] : euler[i];

            res *= Quaternion.Euler(angles);
        }
        return res;
    }

    public static Vector3 ExtractEulerAngles(AxisSet order, Quaternion quat) {
        Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero, quat, Vector3.one);
        AngleExtraction.EulerResult eu;

        Vector3 res = extractionFunctions[order.ToString()](mat, out eu);
        for(int i = 0; i < 3; i ++) {
            if (order[i].negative) res[i] *= -1;
        }
        return res;
    }

    public static Vector3 ToEulerOrder(AxisSet from, AxisSet to, Vector3 euler) {
        Quaternion quat = ToQuaterion(from, euler);
        return ExtractEulerAngles(to, quat);
    }
}
