using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameConversions {
    static class Conversions {
        delegate EulerAngles QuatEulerExtractionDelegate(Quaternion quat, out AngleExtraction.EulerResult eu);
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
                            return new EulerAngles(Mathf.Rad2Deg * f(Matrix4x4.TRS(Vector3.zero, q, Vector3.one), out eu));
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

        // Convert a vector position between the two coordinate frames
        public static Vector3 ConvertPosition(string from, string to, Vector3 v) {
            return ConvertPosition(new AxisSet(from, false), new AxisSet(to, false), v);
        }
        public static Vector3 ConvertPosition(AxisSet from, AxisSet to, Vector3 v) {
            Vector3 res = new Vector3();

            // Iterate over right, then up, then forward
            for (int i = 0; i < 3; i++) {
                // Get the axis name associated with the 
                // given direction
                int directionIndex = i; // the direction

                // Associated axes
                Axis fromAxis = from[directionIndex];
                Axis toAxis = to[directionIndex];

                // The vector index to take the value out of
                // and put it in to
                int fromIndex = fromAxis.xyzIndex;
                int toIndex = toAxis.xyzIndex;
                
                res[toIndex] = fromAxis.negative == toAxis.negative ? v[fromIndex] : -v[fromIndex];
            }
            return res;
        }

        // Convert between the provided euler orders
        public static EulerAngles ConvertEulerOrder(string from, string to, EulerAngles eulerAngles) {
            return ConvertEulerOrder(new AxisSet(from, false), new AxisSet(to, false), eulerAngles);
        }
        public static EulerAngles ConvertEulerOrder(AxisSet from, AxisSet to, EulerAngles eulerAngles) {
            Quaternion quat = ToQuaternion(from, eulerAngles);
            return ExtractEulerAngles(to, quat);
        }

        // Convert the provided euler angles from an euler rotation in frame 1 to
        // frame 2 given the direction and rotation conventions
        public static EulerAngles ConvertEulerAngles(string fromAxes, string fromRotorder, string toAxes, string toRotOrder, EulerAngles eulerAngles) {
            return ConvertEulerAngles(new AxisSet(fromAxes, true), new AxisSet(fromRotorder, true), new AxisSet(toAxes, true), new AxisSet(toRotOrder, true), eulerAngles);
        }
        public static EulerAngles ConvertEulerAngles(CoordinateFrame cf1, CoordinateFrame cf2, EulerAngles eulerAngles) {
            return ConvertEulerAngles(cf1.Axes, cf1.RotationOrder, cf2.Axes, cf2.RotationOrder, eulerAngles);
        }
        public static EulerAngles ConvertEulerAngles(AxisSet axes1, AxisSet rot1, AxisSet axes2, AxisSet rot2, EulerAngles eulerAngles) {
        
            // ToQuaternion doesn't know anything about the axis conventions
            // about which the the rotations order apply, so it assumes
            // XYZ are up right left. We use a consistent frame when converting
            // so the conventions when going back and forth are consistent

            // TODO: Should ToQuaternion be changed to account for this? At the 
            // moment it assumes the Unity direction convention.
            AxisSet intermediateAxes = new AxisSet("XYZ", false);
            
            AxisSet order1InInter = ToEquivelentRotationOrder(axes1, intermediateAxes, rot1);
            AxisSet order2InInter = ToEquivelentRotationOrder(axes2, intermediateAxes, rot2);

            Quaternion intermediateQuat = ToQuaternion(order1InInter, eulerAngles);
            EulerAngles resultantAngles = ExtractEulerAngles(order2InInter, intermediateQuat);

            return resultantAngles;
        }

        #region Helpers
        // Convert the provided rotation order in frame 1 to the rotation order
        // that would yield the same rotation in frame 2, given the direction
        // conventions
        internal static AxisSet ToEquivelentRotationOrder(AxisSet axes1, AxisSet axes2, AxisSet rotOrder) {
            // State the axes to rotate about
            var r0 = rotOrder[0];
            var r1 = rotOrder[1];
            var r2 = rotOrder[2];

            // 1. Swap the rotation direction on each axis
            // based on each axis in the original frame

            // 2. Then convert the axis names into the new
            // frame and change rotation order based on that

            // Originally two operations
            // r0 = new Axis(r0.name, r0.negative != axes1[axes1[r0.name]].negative);
            // r0 = new Axis(axes2[axes1[r0.name]].name, r0.negative != axes2[axes1[r0.name]].negative);

            // Change the rotation direction based on the axis direction
            r0 = new Axis(
                axes2[axes1[r0.name]].name,
                (r0.negative != axes1[axes1[r0.name]].negative) != axes2[axes1[r0.name]].negative
            );

            r1 = new Axis(
                axes2[axes1[r1.name]].name,
                (r1.negative != axes1[axes1[r1.name]].negative) != axes2[axes1[r1.name]].negative
            );

            r2 = new Axis(
                axes2[axes1[r2.name]].name,
                (r2.negative != axes1[axes1[r2.name]].negative) != axes2[axes1[r2.name]].negative
            );

            return new AxisSet(r0, r1, r2, true);
        }

        // Takes euler angles in degrees
        // Returns the rotation as a quaternion that results from
        // applying the rotations in the provided order
        internal static Quaternion ToQuaternion(AxisSet order, EulerAngles euler) {
            Quaternion res = Quaternion.identity;

            for (int i = 0; i < 3; i++) {
                Axis axis = order[i];
                Vector3 angles = Vector3.zero;
                angles[axis.xyzIndex] = axis.negative ? -euler[i] : euler[i];
                
                // Unity's default rotation direction is negative
                angles *= -1;

                res = Quaternion.Euler(angles) * res;
            }
            return res;
        }

        // Outputs euler angles in degrees
        // Extracts the rotation in Euler degrees in the 
        // given order
        internal static EulerAngles ExtractEulerAngles(AxisSet order, Quaternion quat) {
            AngleExtraction.EulerResult eu;
            EulerAngles res = extractionFunctions[order.ToString()](quat, out eu);

            for (int i = 0; i < 3; i++) {
                if (order[i].negative) res[i] *= -1;
            }
            return res;
        }
        #endregion
    }
}
