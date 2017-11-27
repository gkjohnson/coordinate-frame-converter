using UnityEngine;

namespace FrameConversions {
    public struct EulerAngles {
        float _e0, _e1, _e2;

        public float this[int i] {
            get { return i == 0 ? _e0 : i == 1 ? _e1 : _e2; }
            set {
                if (i == 0) _e0 = value;
                if (i == 1) _e1 = value;
                if (i == 2) _e2 = value;
            }
        }

        public EulerAngles(float e0, float e1, float e2) {
            _e0 = e0;
            _e1 = e1;
            _e2 = e2;
        }

        public string ToString(string precision = "0.0") {
            return _e0.ToString(precision) + ", " + _e1.ToString(precision) + ", " + _e2.ToString(precision);
        }

        public EulerAngles(Vector3 v) : this(v.x, v.y, v.z) { }
    }
}