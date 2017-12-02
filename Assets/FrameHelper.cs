using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameHelper : MonoBehaviour {
    public string Axes = "XY-Z";
    public string RotationOrder = "-Z-X-Y";

    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;
        FrameConversions.Utilities.DrawFrame(new FrameConversions.AxisSet(Axes), new FrameConversions.AxisSet(RotationOrder, false));
    }
}
