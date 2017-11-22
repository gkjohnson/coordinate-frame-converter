using UnityEngine;
using System.Collections.Generic;

public class Tests : MonoBehaviour {

    // Run the tests
    void Start () {
        var conventions = GetAxisConventions();
        var rotOrders = GetAxisConventions(true);

        Debug.Log("Ran into " + RunPositionTests(conventions) + " issues when doing position conversions");
        Debug.Log("Ran into " + RunRotationTests(rotOrders) + " issues when doing rotation conversions");
    }

    // Returns all possible combinations of axis conventions
    // and rotation orders if rotations == true
    List<FrameConversions.AxisSet> GetAxisConventions(bool rotations = false) {
        var axes = new List<FrameConversions.AxisSet>();
        string[] str = new string[] { "X", "Y", "Z" };

        foreach(var a in str)
            foreach(var b in str)
                foreach(var c in str) {
                    int matches = 0;
                    if (a == b) matches++;
                    if (c == b) matches++;
                    if (c == a) matches++;

                    if (a != b && b != c && (rotations || c != a)) {
                        for (int a2 = 0; a2 < 2; a2++)
                            for (int b2 = 0; b2 < 2; b2++)
                                for (int c2 = 0; c2 < 2; c2++) {
                                    axes.Add(
                                        new FrameConversions.AxisSet(
                                            (a2 == 1 ? "-" : "+") + a +
                                            (b2 == 1 ? "-" : "+") + b +
                                            (c2 == 1 ? "-" : "+") + c
                                        )
                                    );
                                }
                    }
                }

        return axes;
    }
    
    // Transforms to and from all conventions to make sure the
    // resultant position is the same
    int RunPositionTests(List<FrameConversions.AxisSet> conventions) {
        int issues = 0;
        foreach (var c1 in conventions)
            foreach(var c2 in conventions) {
                Vector3 v = new Vector3(1, 2, 3);

                Vector3 to = FrameConversions.ToPosition(c1, c2, v);
                Vector3 back = FrameConversions.ToPosition(c2, c1, to);

                if (!AreVectorsEquivalent(v, back)) {
                    Debug.Log(c1.ToString(true) + " could not convert to " + c2.ToString(true));
                    issues++;
                }
            }

        return issues;
    }

    // Transforms euler orders back and forth between all rotation orders
    // to make sure the conversions result in the same rotation
    int RunRotationTests(List<FrameConversions.AxisSet> orders) {
        int issues = 0;

        foreach(var o1 in orders)
            foreach(var o2 in orders) {
                Vector3 e = new Vector3(20, 40, 80);
                Vector3 to = FrameConversions.ToEulerOrder(o1, o2, e);
                Vector3 back = FrameConversions.ToEulerOrder(o2, o1, to);

                // TODO: The extractions for rotation orders like +X-Z+X can result
                // non unique euler angles for a rotation, so this check fails. We should
                // transform basis vectors by the formed quaternion to make sure they're 
                // pointing in the same direction if the initial test fails

                if (!AreVectorsEquivalent(e, back)) {
                    Debug.Log(o1.ToString(true) + " could not convert to " + o2.ToString(true));
                    Debug.Log(e.ToString("0.0000") + " > " + to.ToString("0.0000") + " > " + back.ToString("0.0000"));
                    issues++;
                }
            }
        
        return issues;
    }

    // Checks whether or not the vectors are equivalent, affording some epsilon
    // for vectors representing euler rotations in particular.
    bool AreVectorsEquivalent(Vector3 a, Vector3 b) {
        Vector3 delta = a - b;
        delta.x = Mathf.Abs(delta.x);
        delta.y = Mathf.Abs(delta.y);
        delta.z = Mathf.Abs(delta.z);

        float eps = 1e-4f;
        return delta.x < eps && delta.y < eps && delta.z < eps;
    }
}
