using UnityEngine;
using FrameConversions;

public class FrameHelper : MonoBehaviour {
    public string Axes = "XY-Z";
    public string RotationOrder = "-Z-X-Y";

    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Utilities.DrawFrame(new AxisSet(Axes), new AxisSet(RotationOrder, false));
    }
}
