using UnityEngine;

namespace FrameConversions {
    static class Utilities {
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
        
        static void DrawAxis(Axis axis, Vector3 direction) {
            Gizmos.color = axis.name == "X" ? Color.red : axis.name == "Y" ? Color.green : Color.blue;
            Gizmos.DrawRay(Vector3.zero, (axis.negative ? -1 : 1) * direction);
        }

        public static void DrawFrame(AxisSet axes, float scale = 1) {
            DrawAxis(axes[0], Vector3.right * scale);
            DrawAxis(axes[1], Vector3.up * scale);
            DrawAxis(axes[2], -Vector3.forward * scale);
        }
    }