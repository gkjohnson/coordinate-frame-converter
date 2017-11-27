using UnityEngine;
using System.Collections.Generic;
using FrameConversions;

public class Tests : MonoBehaviour {

    // Run the tests
    void Start () {
        Debug.Assert(AreVectorsEquivalent(
            Conversions.ConvertPosition("+X+Y-Z", "+Y-Z+X", new Vector3(1,2,3)),
            new Vector3(-3, 1, -2)
        ));

        Debug.Assert(AreVectorsEquivalent(
            Conversions.ConvertPosition("-Z-X+Y", "+X-Z+Y", new Vector3(1, 2, 3)),
            new Vector3(-3, 2, 1)
        ));

        Debug.Assert(
            Conversions.ToQuaternion(new AxisSet("-Z-X-Y"), new EulerAngles(30, 10, 20)) == Quaternion.Euler(10, 20, 30)
        );
        var conventions = GetAxisConventions();
        var rotOrders = GetAxisConventions(true);
        //var coordinateFrames = GetCoordinateFrames();

        Debug.Log("Ran into " + RunPositionTests(conventions) + " issues when doing position conversions");
        Debug.Log("Ran into " + RunRotationTests(rotOrders) + " issues when doing rotation conversions");
        //Debug.Log("Ran into " + RunCoordinateFrameTests(coordinateFrames) + " issues when doing coordinate frame conversions");
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

    // Returns all possible coordinate frame converters
    List<FrameConversions.CoordinateFrame> GetCoordinateFrames() {
        var frames = new List<FrameConversions.CoordinateFrame>();
        var conventions = GetAxisConventions();
        var rotOrders = GetAxisConventions(true);

        foreach (var c in conventions)
            foreach (var ro in rotOrders)
                frames.Add(new FrameConversions.CoordinateFrame(c.ToString(true), ro.ToString(true)));

        return frames;
    }
    
    // Transforms to and from all conventions to make sure the
    // resultant position is the same
    int RunPositionTests(List<FrameConversions.AxisSet> conventions) {
        int issues = 0;
        foreach (var c1 in conventions)
            foreach(var c2 in conventions) {
                Vector3 v = new Vector3(1, 2, 3);

                Vector3 to = Conversions.ConvertPosition(c1, c2, v);
                Vector3 back = Conversions.ConvertPosition(c2, c1, to);

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
                EulerAngles e = new EulerAngles(20, 40, 80);
                EulerAngles to = Conversions.ConvertEulerOrder(o1, o2, e);
                EulerAngles back = Conversions.ConvertEulerOrder(o2, o1, to);
                
                Quaternion qe = Conversions.ToQuaternion(o1, e);
                Quaternion qback = Conversions.ToQuaternion(o1, back);

                // Check if the rotations are the same or if the rotations
                // have the equivalent effect on transforming vectors
                bool equal =
                    AreVectorsEquivalent(e, back, 1e-4f) ||
                    AreVectorsEquivalent(qe * Vector3.forward,  qback * Vector3.forward,    1e-6f) &&
                    AreVectorsEquivalent(qe * Vector3.up,       qback * Vector3.up,         1e-6f) &&
                    AreVectorsEquivalent(qe * Vector3.right,    qback * Vector3.right,      1e-6f);

                if (!equal) {
                    Debug.Log(o1.ToString(true) + " > " + o2.ToString(true));
                    Debug.Log(e.ToString("0.0000") + " > " + to.ToString("0.0000") + " > " + back.ToString("0.0000"));
                    issues++;
                }
            }
        
        return issues;
    }

    int RunCoordinateFrameTests(List<FrameConversions.CoordinateFrame> frames) {
        int issues = 0;

        foreach (var fr1 in frames) {
            foreach (var fr2 in frames) {

                var cfc = new FrameConversions.CoordinateFrameConverter(fr1, fr2);
                if (cfc != cfc.inverse.inverse || cfc.from != cfc.inverse.to || cfc.to != cfc.inverse.from) issues++;

                Vector3 v = new Vector3(1, 2, 3);
                Vector3 to = fr1.ToPosition(fr2, v);
                Vector3 back = fr2.ToPosition(fr1, to);

                if (!AreVectorsEquivalent(v, back)) issues++;

                to = cfc.ConvertPosition(v);
                back = cfc.inverse.ConvertPosition(to);

                if (!AreVectorsEquivalent(v, back)) issues++;

                // TODO: rotation conversions
            }
        }

        return issues;
    }

    // Checks whether or not the vectors are equivalent, affording some epsilon
    // for vectors representing euler rotations in particular.
    bool AreVectorsEquivalent(EulerAngles a, EulerAngles b, float eps = 1e-40f) {
        return AreVectorsEquivalent(new Vector3(a[0], a[1], a[2]), new Vector3(b[0], b[1], b[2]));
    }

    bool AreVectorsEquivalent(Vector3 a, Vector3 b, float eps = 1e-40f)
        {
            Vector3 delta = a - b;
        delta.x = Mathf.Abs(delta.x);
        delta.y = Mathf.Abs(delta.y);
        delta.z = Mathf.Abs(delta.z);

        return delta.x < eps && delta.y < eps && delta.z < eps;
    }
}
