﻿using UnityEngine;

// Class for converting between to fixed coordinate frames
namespace FrameConversions {
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

        #region Constructors
        private CoordinateFrameConverter() { }

        public CoordinateFrameConverter(CoordinateFrame from, CoordinateFrame to) {
            _fromFrame = from;
            _toFrame = to;
        }

        public CoordinateFrameConverter(string fromAxis, string fromRotation, string toAxis, string toRotation)
            : this(new CoordinateFrame(fromAxis, fromRotation), new CoordinateFrame(toAxis, toRotation)) { }
        #endregion

        #region API
        public Vector3 ConvertPosition(Vector3 p) { return _fromFrame.ConvertPosition(_toFrame, p); }
        public EulerAngles ConvertEulerAngles(EulerAngles euler) { return _fromFrame.ConvertEulerAngles(_toFrame, euler); }
        #endregion

        #region To String
        public string ToString(bool includeSign = false) {
            return from.ToString(includeSign) + " to " + to.ToString(includeSign);
        }
        #endregion

        #region Operator Overloads
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
        #endregion
    }
}