using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FrameConversions {
    // Description of a full axis set, such ax -X +Y +Z
    public class AxisSet {
        // Three axes ordered to describe right, up, and forward
        Axis[] _axes;
        Dictionary<string, int> _axisToIndex;

        public Axis this[int i] { get { return _axes[i]; } }
        public int this[string a] { get { return _axisToIndex[a.ToUpper()]; } }

        #region Constructors
        private AxisSet() { }

        internal AxisSet(Axis right, Axis up, Axis forward) {
            _axes = new Axis[] { right, up, forward };
            _axisToIndex = GetIndexMap(_axes);
        }

        public AxisSet(string axes, bool guaranteeUniqueness = true) {
            _axes = SanitizeAxisDescription(axes, guaranteeUniqueness);
            _axisToIndex = GetIndexMap(_axes);
        }
        #endregion

        #region Helpers
        static Axis[] SanitizeAxisDescription(string s, bool guaranteeUniqueness = true) {
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
            for (int i = 0; i < axes.Length; i ++) {
                for (int j = 0; j < axes.Length; j ++) {
                    if (i == j) continue;

                    Axis a = axes[i];
                    Axis b = axes[j];
                    redundantCount += a.name == b.name ? 1 : 0;
                }
            }

            if (guaranteeUniqueness) Debug.Assert(redundantCount == 0);

            return axes;
        }

        static Dictionary<string, int> GetIndexMap(Axis[] axes) {
            var dict = new Dictionary<string, int>();
            for (int i = 0; i < 3; i++) dict[axes[i].name.ToUpper()] = i;
            return dict;
        }
        #endregion

        #region To String
        public string ToString(bool includeSign = false) {
            string res = "";
            foreach (Axis a in _axes) res += a.ToString(includeSign);
            return res;
        }
        #endregion

        #region Operator Overloading
        public static bool operator ==(AxisSet a, AxisSet b) {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(AxisSet a, AxisSet b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            return _axes[0].GetHashCode() ^ _axes[1].GetHashCode() ^ _axes[2].GetHashCode();
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(obj, null) || GetType() != obj.GetType()) return false;
            AxisSet other = (AxisSet)obj;
            return _axes[0] == other._axes[0] && _axes[1] == other._axes[1] && _axes[2] == other._axes[2];
        }
        #endregion
    }
}