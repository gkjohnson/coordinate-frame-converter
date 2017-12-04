using UnityEngine;
using FrameConversions;

public class FrameHelper : MonoBehaviour {
    public bool AlwaysDraw = false;

    [Header("Coordinate Frame")]
    public string Axes = "XY-Z";
    public string RotationOrder = "-Z-X-Y";
    void Draw() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Utilities.DrawFrame(new AxisSet(Axes), new AxisSet(RotationOrder, false), 0.5f);
    }

    private void OnDrawGizmosSelected() { if (!AlwaysDraw) Draw(); }
    private void OnDrawGizmos() { if (AlwaysDraw) Draw(); }
}
