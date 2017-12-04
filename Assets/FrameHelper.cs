using UnityEngine;
using FrameConversions;

public class FrameHelper : MonoBehaviour {
    public string Axes = "XY-Z";
    public string RotationOrder = "-Z-X-Y";

    private void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Utilities.DrawFrame(new AxisSet(Axes), new AxisSet(RotationOrder, false), 0.5f);
    }
}
