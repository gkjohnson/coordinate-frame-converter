using UnityEngine;
using System.Collections;
using FrameConversions;
public class Rotater : MonoBehaviour {
    public Transform target1, target2;

    public string targetAxes = "XY-Z";
    public string targetRotOrder = "-X-Y-X";

    void Start() {
        StartCoroutine(Rotate(Frames.Unity, new CoordinateFrame(targetAxes, targetRotOrder), new Vector3(40, -220, -105)));
    }

    IEnumerator Rotate(CoordinateFrame unityFrame, CoordinateFrame frame2, Vector3 unityAngles) {
        yield return new WaitForSeconds(1);

        CoordinateFrameConverter conv = new CoordinateFrameConverter(unityFrame, frame2);
        EulerAngles eu1 = new EulerAngles(unityAngles.z, unityAngles.x, unityAngles.y);
        EulerAngles eu2 = conv.ConvertEulerAngles(eu1);

        int steps = 40;

        EulerAngles teu1 = new EulerAngles(0, 0, 0);
        EulerAngles teu2 = new EulerAngles(0, 0, 0);
        for (int e = 0; e < 3; e++) {
            for (int i = 0; i < steps; i++) {
                teu1[e] = teu1[e] + eu1[e] * 1.0f / steps;
                teu2[e] = teu2[e] + eu2[e] * 1.0f / steps;

                target1.localRotation = Conversions.ToQuaternion(unityFrame.RotationOrder, teu1);
                target2.localRotation = Conversions.ToQuaternion(unityFrame.RotationOrder, conv.inverse.ConvertEulerAngles(teu2));

                yield return null;
            }
        }
    }
}
