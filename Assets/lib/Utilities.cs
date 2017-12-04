using UnityEngine;

namespace FrameConversions {
    static class Utilities {
        #region To String
        // Output a string that visually displays the coordinate axes
        public static string ToCoordinateFrameString(CoordinateFrame cf) {
            return ToCoordinateFrameString(cf.Axes);
        }

        public static string ToCoordinateFrameString(AxisSet axes) {
            string r = axes[0].negative ? " " : "-";
            string l = axes[0].negative ? "-" : " ";

            string u = axes[1].negative ? " " : "|";
            string d = axes[1].negative ? "|" : " ";

            string f = axes[2].negative ? " " : "／";
            string b = axes[2].negative ? "／" : " ";

            string template =
                "       U    \n" +
                "       u   B\n" +
                "       u b  \n" +
                " Llllll.rrrrr R\n" +
                "     f d    \n" +
                "   F   d    \n" +
                "       D    \n";

            return template
                .Replace("R", axes[0].negative ? " " : axes[0].ToString())
                .Replace("r", r)
                .Replace("L", axes[0].negative ? axes[0].ToString() : " ")
                .Replace("l", l)

                .Replace("U", axes[1].negative ? " " : axes[1].ToString())
                .Replace("u", u)
                .Replace("D", axes[1].negative ? axes[1].ToString() : " ")
                .Replace("d", d)

                .Replace("F", axes[2].negative ? " " : axes[2].ToString())
                .Replace("f", f)
                .Replace("B", axes[2].negative ? axes[2].ToString() : " ")
                .Replace("b", b);
        }

        // Output a string that visually displays the coordinate axes
        // with the rotation order
        public static string ToRotationOrderFrameString(CoordinateFrame cf) {
            return ToRotationOrderFrameString(cf.Axes, cf.RotationOrder);
        }

        public static string ToRotationOrderFrameString(AxisSet axes, AxisSet rotOrder) {
            string str = ToCoordinateFrameString(axes);

            return str
                .Replace(" " + rotOrder[0].name, rotOrder[0].negative ? "-1" : "+1")
                .Replace(" " + rotOrder[1].name, rotOrder[1].negative ? " -2" : " +2")
                .Replace(" " + rotOrder[2].name, rotOrder[2].negative ? "-3" : "+3");
        }
        #endregion

        #region Gizmos
        // Draw a colored axis in the given direction
        static void DrawAxis(Axis axis, Vector3 direction) {
            Gizmos.color = axis.name == "X" ? Color.red : axis.name == "Y" ? Color.green : Color.blue;
            Gizmos.DrawRay(Vector3.zero, (axis.negative ? -1 : 1) * direction);
        }

        // Draws the coordinate frame representing the provided axis set
        public static void DrawFrame(AxisSet axes, float scale = 1) {
            DrawAxis(axes[0], Vector3.right * scale);
            DrawAxis(axes[1], Vector3.up * scale);
            DrawAxis(axes[2], -Vector3.forward * scale);
        }

        // Draws an arc with an arrow representing the rotation direction of an axis
        // Center represents the axis to rotate about and the position to center the arc at
        // Up is the direction to start drawing the axis is
        // StAngle and endAngle indicate how far of an arc to draw
        static void DrawRotationDirection(Vector3 center, Vector3 up, float stAngle, float endAngle, float radius, bool leftHanded) {
            // Unity is left handed by default
            Vector3 rotAxis = center.normalized;
            if (!leftHanded) rotAxis *= -1;

            // Draw the arc
            int steps = 15;
            float angleDelta = stAngle - endAngle;
            float angleStep = angleDelta / steps;
            for (int i = 0; i < steps; i++) {
                Vector3 p0 = center + Quaternion.AngleAxis(stAngle + angleStep * (i + 0), rotAxis) * up * radius;
                Vector3 p1 = center + Quaternion.AngleAxis(stAngle + angleStep * (i + 1), rotAxis) * up * radius;

                Gizmos.DrawLine(p0, p1);

                // Draw the arrowhead if this is the first point
                if (i == 0) {
                    Matrix4x4 origMat = Gizmos.matrix;
                    
                    Gizmos.matrix = origMat * Matrix4x4.TRS(p0, Quaternion.LookRotation(p0 - p1, center), Vector3.one);

                    Gizmos.DrawFrustum(Vector3.zero, 45, radius * -0.2f, 0, 1);
                    Gizmos.matrix = origMat;
                }
            }

        }

        // Draw the frame with the position and rotation order axes
        public static void DrawFrame(AxisSet axes, AxisSet rotationOrder, EulerAngles stAngles, EulerAngles endAngles, float scale = 1) {
            DrawFrame(axes, scale);

            // Associate axes to the xyz index
            Vector3[] dir = new Vector3[] { Vector3.right, Vector3.up, -Vector3.forward };
            for (int i = 0; i < 3; i++) {
                Axis rotAxis = rotationOrder[i];
                int index = axes[rotAxis.name];
                Axis posAxis = axes[index];

                Gizmos.color = posAxis.name == "X" ? Color.red : posAxis.name == "Y" ? Color.green : Color.blue;

                Vector3 center = dir[index];
                if (posAxis.negative) center *= -1;

                DrawRotationDirection(center * (0.45f + i * 0.05f) * scale, dir[(index + 1) % 3], stAngles[i], endAngles[i], scale * 0.25f, rotAxis.negative);
            }
        }

        public static void DrawFrame(AxisSet axes, AxisSet rotationOrder, float scale = 1) {
            DrawFrame(axes, rotationOrder, new EulerAngles(0,0,0), new EulerAngles(120, 120, 120), scale);
        }
        #endregion
    }
}