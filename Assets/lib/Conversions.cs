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

        public static Vector3 ConvertPosition(string from, string to, Vector3 v) {
            return ConvertPosition(new AxisSet(from), new AxisSet(to), v);
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
        public static Vector3 ConvertEulerOrder(string from, string to, Vector3 eulerAngles) {
            return ConvertEulerOrder(new AxisSet(from, false), new AxisSet(to, false), eulerAngles);
        }
        public static Vector3 ConvertEulerOrder(AxisSet from, AxisSet to, Vector3 eulerAngles) {
            Quaternion quat = ToQuaternion(from, eulerAngles);
            return ExtractEulerAngles(to, quat);
        }

        public static Vector3 ConvertEulerAngles(string fromAxes, string fromRotorder, string toAxes, string toRotOrder, Vector3 eulerAngles) {
            return ConvertEulerAngles(new CoordinateFrame(fromAxes, fromRotorder), new CoordinateFrame(toAxes, toRotOrder), eulerAngles);
        }
        public static Vector3 ConvertEulerAngles(CoordinateFrame frame1, CoordinateFrame frame2, Vector3 eulerAngles) {

            // Step 1
            // State the axes to rotate about
            var axis1 = frame2.RotationOrder[0];
            var axis2 = frame2.RotationOrder[1];
            var axis3 = frame2.RotationOrder[2];

            // Step 2
            // Change the rotation direction based on the axis direction
            axis1 = new Axis(
                axis1.name,
                axis1.negative != frame2.Axes[frame2.Axes[axis1.name]].negative
                );

            axis2 = new Axis(
                axis2.name,
                axis2.negative != frame2.Axes[frame2.Axes[axis2.name]].negative
                );

            axis3 = new Axis(
                axis3.name,
                axis3.negative != frame2.Axes[frame2.Axes[axis3.name]].negative
                );

            // Step 3
            // Substitute in the target axes
            axis1 = new Axis(
                frame1.Axes[frame2.Axes[axis1.name]].name,
                axis1.negative != frame1.Axes[frame2.Axes[axis1.name]].negative
                );

            axis2 = new Axis(
                frame1.Axes[frame2.Axes[axis2.name]].name,
                axis2.negative != frame1.Axes[frame2.Axes[axis2.name]].negative
                );

            axis3 = new Axis(
                frame1.Axes[frame2.Axes[axis3.name]].name,
                axis3.negative != frame1.Axes[frame2.Axes[axis3.name]].negative
                );

            // Step 4
            // Create the new extraction order
            var newOrder = new AxisSet(axis1, axis2, axis3);

            UnityEngine.Debug.Log(newOrder.ToString(true));
            throw new NotImplementedException();

            // quat in current coordinate frame
            Quaternion thisquat = ToQuaternion(frame1.RotationOrder, eulerAngles);

            // extract in the converted order
            Vector3 extracteuler = ExtractEulerAngles(newOrder, thisquat);

            return ConvertPosition(
                new AxisSet(frame1.Axes.ToString(false)),
                new AxisSet(frame2.Axes.ToString(false)),
                extracteuler
            );
        }

        #region Helpers
        // Takes euler angles in degrees
        // Returns the rotation as a quaternion that results from
        // applying the rotations in the provided order
        public static Quaternion ToQuaternion(AxisSet order, Vector3 euler) {
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
        public static Vector3 ExtractEulerAngles(AxisSet order, Quaternion quat) {
            AngleExtraction.EulerResult eu;
            Vector3 res = extractionFunctions[order.ToString()](quat, out eu);
            for (int i = 0; i < 3; i++) {
                if (order[i].negative) res[i] *= -1;
            }
            return res;
        }
        #endregion
    }
}
