using System;
using UnityEngine;

namespace FrameConversions {
    public class CoordinateFrame {
        AxisSet _axes, _rotationOrder;

        public AxisSet Axes { get { return _axes; } }
        public AxisSet RotationOrder { get { return _rotationOrder; } }

        private CoordinateFrame() { }

        #region Constructors
        public CoordinateFrame(string rufAxes, string rotationOrder) {
            _axes = new AxisSet(rufAxes);
            _rotationOrder = new AxisSet(rotationOrder);
        }

        public Vector3 ToPosition(CoordinateFrame other, Vector3 v) {
            return Conversions.ToPosition(_axes, other._axes, v);
        }
        #endregion

        #region API
        public Quaternion ToQuaternion(Vector3 eulerAngles) {
            return Conversions.ToQuaternion(_rotationOrder, eulerAngles);
        }

        public Vector3 ToEulerAngles(CoordinateFrame other, Vector3 eulerAngles) {

            var frame1 = other;
            var frame2 = this;

            // Step 1
            var axis1 = frame1.RotationOrder[0];
            var axis2 = frame1.RotationOrder[1];
            var axis3 = frame1.RotationOrder[2];

            // Step 2
            axis1 = new Axis(
                axis1.name,
                axis1.negative != frame1.Axes[frame1.Axes[axis1.name]].negative
                );

            axis2 = new Axis(
                axis2.name,
                axis2.negative != frame1.Axes[frame1.Axes[axis2.name]].negative
                );

            axis3 = new Axis(
                axis3.name,
                axis3.negative != frame1.Axes[frame1.Axes[axis3.name]].negative
                );

            // Step 3
            axis1 = new Axis(
                frame2.Axes[frame1.Axes[axis1.name]].name,
                axis1.negative != frame2.Axes[frame1.Axes[axis1.name]].negative
                );

            axis2 = new Axis(
                frame2.Axes[frame1.Axes[axis2.name]].name,
                axis2.negative != frame2.Axes[frame1.Axes[axis2.name]].negative
                );

            axis3 = new Axis(
                frame2.Axes[frame1.Axes[axis3.name]].name,
                axis3.negative != frame2.Axes[frame1.Axes[axis3.name]].negative
                );

            // Step 4
            var newOrder = new AxisSet(axis1, axis2, axis3);

            UnityEngine.Debug.Log(newOrder.ToString(true));
            throw new NotImplementedException();

            // quat in current coordinate frame
            Quaternion thisquat = frame2.ToQuaternion(eulerAngles);

            // extract in the converted order
            Vector3 extracteuler = Conversions.ExtractEulerAngles(newOrder, thisquat);

            return Conversions.ToPosition(
                new AxisSet(frame2.Axes.ToString(false)),
                new AxisSet(frame1.Axes.ToString(false)),
                extracteuler
            );
        }
        #endregion

        #region To String
        public string ToString(bool includeSign = false) {
            return Axes.ToString(includeSign) + ", " + RotationOrder.ToString(includeSign);
        }
        #endregion

        #region Operator Overloads
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
        #endregion
    }
}