using UnityEngine;
using System.Collections;
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
            Conversions.ToQuaternion(new AxisSet("-Z-X-Y", true), new EulerAngles(30, 10, 20)) == Quaternion.Euler(10, 20, 30)
        );

        Debug.Assert(ExpectException(() => new AxisSet("-Z-XZ", false)));
        Debug.Assert(ExpectException(() => new AxisSet("-Z-ZY", false)));
        Debug.Assert(ExpectException(() => new AxisSet("X--YZ", false)));
        Debug.Assert(ExpectException(() => new AxisSet("-+XYZ", false)));
        Debug.Assert(ExpectException(() => new AxisSet("-+XUZ", false)));
        Debug.Assert(ExpectException(() => new AxisSet("-Z-ZY", true)));

        Debug.Assert(ExpectException(() => new CoordinateFrame("XYZ", "XXZ")));
        Debug.Assert(ExpectException(() => new CoordinateFrame("XYX", "XYZ")));
        
        // Here to test issue #1
        // https://github.com/gkjohnson/coordinate-frame-converter/issues/1
        EulerAngles e = new EulerAngles(170, 90, -27);
        EulerAngles to = Conversions.ConvertEulerOrder(Frames.Unity.RotationOrder, Frames.Unity.RotationOrder, e);
        Debug.Assert(AreRotationsEquivalent(Frames.Unity.RotationOrder, e, to));

        StartCoroutine(RunConversionTests());
    }

    #region Helpers
    bool ExpectException(System.Action func) {
        try {
            func();
        } catch {
            return true;
        }
        return false;
    }

    EulerAngles GetRandomAngles() {
        float x = (Random.value * 2 - 1.0f) * 180;
        float y = (Random.value * 2 - 1.0f) * 180;
        float z = (Random.value * 2 - 1.0f) * 180;
        
        return new EulerAngles(x, y, z);
    }
    #endregion

    #region Generate Axes
    // Returns all possible combinations of axis conventions
    // and rotation orders if rotations == true
    List<AxisSet> GetAxisConventions(bool rotationOrders = false) {
        var axes = new List<AxisSet>();
        string[] str = new string[] { "X", "Y", "Z" };

        foreach(var a in str)
            foreach(var b in str)
                foreach(var c in str) {
                    int matches = 0;
                    if (a == b) matches++;
                    if (c == b) matches++;
                    if (c == a) matches++;

                    if (a != b && b != c && (rotationOrders || c != a)) {
                        for (int a2 = 0; a2 < 2; a2++)
                            for (int b2 = 0; b2 < 2; b2++)
                                for (int c2 = 0; c2 < 2; c2++) {
                                    axes.Add(
                                        new AxisSet(
                                            (a2 == 1 ? "-" : "+") + a +
                                            (b2 == 1 ? "-" : "+") + b +
                                            (c2 == 1 ? "-" : "+") + c,
                                            rotationOrders
                                        )
                                    );
                                }
                    }
                }

        return axes;
    }

    // Returns all possible coordinate frame converters
    List<CoordinateFrame> GetCoordinateFrames() {
        var frames = new List<CoordinateFrame>();
        var conventions = GetAxisConventions();
        var rotOrders = GetAxisConventions(true);

        foreach (var c in conventions)
            foreach (var ro in rotOrders)
                frames.Add(new CoordinateFrame(c.ToString(true), ro.ToString(true)));

        return frames;
    }
    #endregion

    #region Run Tests
    IEnumerator RunConversionTests() {
        var conventions = GetAxisConventions();

        yield return null;

        var rotOrders = GetAxisConventions(true);

        yield return null;

        var coordinateFrames = GetCoordinateFrames();

        yield return RunPositionTests(conventions);
        yield return RunRotationTests(rotOrders);
        yield return RunCoordinateFrameTests(coordinateFrames, 500);
    }

    // Transforms to and from all conventions to make sure the
    // resultant position is the same
    IEnumerator RunPositionTests(List<AxisSet> conventions) {
        int issues = 0;
        foreach (var c1 in conventions) {
            foreach (var c2 in conventions) {
                Vector3 v = new Vector3(1, 2, 3);

                Vector3 to = Conversions.ConvertPosition(c1, c2, v);
                Vector3 back = Conversions.ConvertPosition(c2, c1, to);

                if (!AreVectorsEquivalent(v, back)) {
                    Debug.LogError(c1.ToString(true) + " could not convert to " + c2.ToString(true));
                    issues++;
                }
            }

            yield return null;
        }

        Debug.Log("Ran into " + issues + " issues when doing position conversions");
    }

    // Transforms euler orders back and forth between all rotation orders
    // to make sure the conversions result in the same rotation
    IEnumerator RunRotationTests(List<AxisSet> orders) {
        int issues = 0;
        foreach (var o1 in orders) {
            foreach (var o2 in orders) {
                EulerAngles e = GetRandomAngles();
                EulerAngles to = Conversions.ConvertEulerOrder(o1, o2, e);
                EulerAngles back = Conversions.ConvertEulerOrder(o2, o1, to);

                // Check if the rotations are the same or if the rotations
                // have the equivalent effect on transforming vectors
                bool equal = AreRotationsEquivalent(o1, e, back);

                if (!equal) {
                    Debug.LogError("Error converting " + o1.ToString(true) + " > " + o2.ToString(true));
                    Debug.Log(e.ToString("0.00000") + " > " + to.ToString("0.00000") + " > " + back.ToString("0.00000"));

                    AreRotationsEquivalent(o1, e, back, true);

                    issues++;
                }
            }
            yield return null;
        } 

        Debug.Log("Ran into " + issues + " issues when doing rotation conversions");
    }

    // Run back and forth conversion tests against a sampling of
    // coordinate frames
    IEnumerator RunCoordinateFrameTests(List<CoordinateFrame> candidateFrames, int framesToTest) {
        framesToTest = Mathf.Min(candidateFrames.Count, framesToTest);

        List<CoordinateFrame> remainingFrames = new List<CoordinateFrame>(candidateFrames);
        List<CoordinateFrame> frames = new List<CoordinateFrame>();

        for(int i = 0; i < framesToTest; i ++) {
            int index = Mathf.FloorToInt(Random.value * (remainingFrames.Count)) % remainingFrames.Count;
            frames.Add(remainingFrames[index]);
            remainingFrames.RemoveAt(index);
        }

        int issues = 0;
        foreach (var fr1 in frames) {
            foreach (var fr2 in frames) {
                // Verify that the inverse frames work as expected
                var cfc = new CoordinateFrameConverter(fr1, fr2);
                if (cfc != cfc.inverse.inverse || cfc.from != cfc.inverse.to || cfc.to != cfc.inverse.from) issues++;

                // Position Tests
                Vector3 v = new Vector3(1, 2, 3);
                Vector3 to = fr1.ConvertPosition(fr2, v);
                Vector3 back = fr2.ConvertPosition(fr1, to);

                if (!AreVectorsEquivalent(v, back)) issues++;

                to = cfc.ConvertPosition(v);
                back = cfc.inverse.ConvertPosition(to);

                if (!AreVectorsEquivalent(v, back)) issues++;

                // Rotation Conversions
                EulerAngles e = GetRandomAngles();
                EulerAngles eTo = Conversions.ConvertEulerAngles(fr1, fr2, e);
                EulerAngles eBack = Conversions.ConvertEulerAngles(fr2, fr1, eTo);

                bool equal = AreRotationsEquivalent(fr1.Axes, fr1.RotationOrder, e, eBack);

                if (!equal) { 
                    Debug.LogError("Error converting " + fr1.ToString(true) + " > " + fr2.ToString(true));
                    Debug.Log(e.ToString("0.00000") + " > " + eTo.ToString("0.00000") + " > " + eBack.ToString("0.00000"));
                    AreRotationsEquivalent(fr1.Axes, fr1.RotationOrder, e, eBack, true);

                    issues++;
                }
            }

            yield return null;
        }

        Debug.Log("Ran into " + issues + " issues when doing coordinate frame conversions");
    }
    #endregion

    // Checks whether or not the vectors are equivalent, affording some epsilon
    // for vectors representing euler rotations in particular.
    public static bool AreRotationsEquivalent(AxisSet rotOrder, EulerAngles a, EulerAngles b, bool debug = false) {
        return AreRotationsEquivalent(new AxisSet("XY-Z", false), rotOrder, a, b, debug);
    }

    public static bool AreRotationsEquivalent(AxisSet axes, AxisSet rotOrder, EulerAngles a, EulerAngles b, bool debug = false) {
        AxisSet equivOrder = Conversions.ToEquivelentRotationOrder(axes, new AxisSet("XY-Z", true), rotOrder);

        Quaternion qe = Conversions.ToQuaternion(equivOrder, a);
        Quaternion qback = Conversions.ToQuaternion(equivOrder, b);
        return AreQuaternionsEquivalent(qe, qback, debug);
    }

    public static bool AreQuaternionsEquivalent(Quaternion q1, Quaternion q2, bool debug = false) { 
        // TODO: floating point math errors mean that the margin on
        // comparisons is super high
        float eps = 1e-2f;

        Vector3 r = q1 * Vector3.right;
        Vector3 r2 = q2 * Vector3.right;
        bool rcheck = AreVectorsEquivalent(r, r2, eps);

        Vector3 u = q1 * Vector3.up;
        Vector3 u2 = q2 * Vector3.up;
        bool ucheck = AreVectorsEquivalent(u, u2, eps);

        Vector3 f = q1 * Vector3.forward;
        Vector3 f2 = q2 * Vector3.forward;
        bool fcheck = AreVectorsEquivalent(f, f2, eps);

        if (debug) {
            if (!rcheck) Debug.LogError("Right: " + r.ToString("0.00000000") + " == " + r2.ToString("0.00000000"));
            if (!ucheck) Debug.LogError("Up: " + u.ToString("0.00000000") + " == " + u2.ToString("0.00000000"));
            if (!fcheck) Debug.LogError("Forward: " + f.ToString("0.00000000") + " == " + f2.ToString("0.00000000"));
        }

        return rcheck && ucheck && fcheck;
    }

    static bool AreVectorsEquivalent(Vector3 a, Vector3 b, float eps = 1e-40f) {
        Vector3 delta = a - b;
        delta.x = Mathf.Abs(delta.x);
        delta.y = Mathf.Abs(delta.y);
        delta.z = Mathf.Abs(delta.z);

        return delta.x < eps && delta.y < eps && delta.z < eps;
    }
}
