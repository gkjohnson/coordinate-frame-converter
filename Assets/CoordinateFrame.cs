using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class CoordinateFrame {

    public struct Vector3 {
        public float x, y, z;
        public float this[int i] {
            get {
                switch (i) {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default:  throw new IndexOutOfRangeException();
                }
            }
            set {
                switch (i) {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default:  throw new IndexOutOfRangeException();
                }
            }
        }

    }
    

    // Description of an axis, such as -X
    public struct Axis {
        string _axis;
        public string axis { get { return _axis; } }

        bool _negative;
        public bool negative { get { return _negative; } }
        
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

        public Axis this[int i] { get { return _axes[i]; } }

        public AxisSet(string axes, bool guaranteeUniqueness = true) {
            _axes = SanitizeAxisDescription(axes, guaranteeUniqueness);
        }

        Axis[] SanitizeAxisDescription(string s, bool guaranteeUniqueness = true) {
            s = s.ToUpper();

            // Ensure we're in the right format
            Regex rgx = new Regex("([+-]?[XYZ])([+-]?[XYZ])([+-]?[XYZ])");
            Debug.Assert(rgx.IsMatch(s));

            // Make sure we have three axes
            var matches = rgx.Matches(s);
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
    }

    public Vector3 toPosition(AxisSet from, AxisSet to, Vector3 v) {
        throw new NotImplementedException();

        Vector3 res = new Vector3();

        // TODO: add a map in the axis set ot map type => index
        // iterate over each in from
        // find the index in which the value belongs
        // negate it if needed
        // set the value
    }


}
