using UnityEngine;
using FrameConversions;

public class FrameHelper : MonoBehaviour {
    public bool AlwaysDraw = false;
    public float Scale = 1;

    [Header("Coordinate Frame")]
    public string Axes = "XY-Z";
    public string RotationOrder = "-Z-X-Y";
    void Draw() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Utilities.DrawFrame(new AxisSet(Axes, false), new AxisSet(RotationOrder, true), Scale);
    }

    private void OnDrawGizmosSelected() { if (!AlwaysDraw) Draw(); }
    private void OnDrawGizmos() { if (AlwaysDraw) Draw(); }
}
