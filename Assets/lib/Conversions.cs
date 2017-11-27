using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameConversions {
    static class Conversions {
        delegate Vector3 QuatEulerExtractionDelegate(Quaternion quat, out AngleExtraction.EulerResult eu);
        delegate Vector3 RawEulerExtractionDelegate(Matrix4x4 mat, out AngleExtraction.EulerResult eu);
        static Dictionary<string, QuatEulerExtractionDelegate> _extractionFunctions;
        static Dictionary<string, QuatEulerExtractionDelegate> extractionFunctions {
            get {
                if (_extractionFunctions == null) {
                    // - Wrap all the functions to return *= -1 the value of the extraction function
                    // because Unity's rotation order is applied as counter clockwise by default
                    // - Wrap the functions so they can have quaternions passed into them directly
                    // - Convert to degrees
                    Func<RawEulerExtractionDelegate, QuatEulerExtractionDelegate> _wrapFunc = f => {
                        QuatEulerExtractionDelegate res = delegate (Quaternion q, out AngleExtraction.EulerResult eu) {
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

            for (int i = 0; i < 3; i++) {
                float value = v[i];
                string str = i == 0 ? "X" : i == 1 ? "Y" : "Z";

                int fromIndex = from[str];
                Axis fromAxis = from[fromIndex];

                Axis toAxis = to[fromIndex];
                int toIndex = toAxis.name == "X" ? 0 : toAxis.name == "Y" ? 1 : 2;

                res[toIndex] = fromAxis.negative == toAxis.negative ? value : -value;
            }
            return res;
        }

        // Takes euler angles in degrees
        public static Quaternion ToQuaternion(AxisSet order, Vector3 euler) {
            Quaternion res = Quaternion.identity;

            for (int i = 0; i < 3; i++) {
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
            for (int i = 0; i < 3; i++) {
                if (order[i].negative) res[i] *= -1;
            }
            return res;
        }

        public static Vector3 ToEulerOrder(AxisSet from, AxisSet to, Vector3 euler) {
            Quaternion quat = ToQuaternion(from, euler);
            return ExtractEulerAngles(to, quat);
        }
    }
}
