using UnityEngine;
using FrameConversions;

public class FrameHelper : MonoBehaviour {
    public string Axes = "XY-Z";
    public string RotationOrder = "-Z-X-Y";
    public float speed = 1;

    private void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;

        float st = Time.frameCount * speed;
        float ed = st + 180;
        Utilities.DrawFrame(new AxisSet(Axes), new AxisSet(RotationOrder, false), new EulerAngles(st,st,st), new EulerAngles(ed,ed,ed), 0.5f);
    }
}
