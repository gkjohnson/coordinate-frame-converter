using System.Diagnostics;
using System.Text.RegularExpressions;

public class CoordinateFrame {
    struct Axis {
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

    abstract class AxisSet {
        Axis[] _axes;

        public AxisSet(string axes, bool guaranteeUniqueness = true) {
            _axes = SanitizeAxisDescription(axes, guaranteeUniqueness);
        }

        Axis[] SanitizeAxisDescription(string s, bool guaranteeUniqueness = true) {
            s = s.ToUpper();

            Regex rgx = new Regex("([+-]?[XYZ])([+-]?[XYZ])([+-]?[XYZ])");
            Debug.Assert(rgx.IsMatch(s));

            var matches = rgx.Matches(s);
            Debug.Assert(matches.Count == 3);

            Axis[] axes = new Axis[3];
            for (int i = 0; i < matches.Count; i++) {
                string str = matches[i].Value;
                bool negative = str.Contains("-");
                string axis = str.Replace("-", "").Replace("+", "");

                axes[i] = new Axis(axis, negative);
            }

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
    
    CoordinateFrame(string axes, string rotationOrder, bool clockwiseRotation = true) { }
}
