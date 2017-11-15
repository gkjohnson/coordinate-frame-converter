// Geometric Tools, LLC
// Copyright (c) 1998-2012
// Distributed under the Boost Software License, Version 1.0.
// http://www.boost.org/LICENSE_1_0.txt
// http://www.geometrictools.com/License/Boost/LICENSE_1_0.txt
//
// File Version: 5.0.2 (2012/07/29)

// Ported file from WildMagic5 for extracting euler angles
using UnityEngine;
class AngleExtraction {
    public enum EulerResult {
        EA_UNIQUE,
        EA_NOT_UNIQUE_DIF,
        EA_NOT_UNIQUE_SUM
    }

    static float Get3x3ElementAtIndex(Matrix4x4 mat, int i) {

        // TODO: Apparently we reverse the row and column values? Is Unity
        // or WildMagic not doing what we expect?
        int col = i / 3;
        int row = i % 3;

        return mat[row, col];
    }

    public static Vector3 ExtractEulerXYZ(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                        -+
        // | r00 r01 r02 |   |  cy*cz           -cy*sz            sy    |
        // | r10 r11 r12 | = |  cz*sx*sy+cx*sz   cx*cz-sx*sy*sz  -cy*sx |
        // | r20 r21 r22 |   | -cx*cz*sy+sx*sz   cz*sx+cx*sy*sz   cx*cy |
        // +-           -+   +-                                        -+

        if (Get3x3ElementAtIndex(mat, 2) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 2) > -1)
            {
                // y_angle = asin(r02)
                // x_angle = atan2(-r12,r22)
                // z_angle = atan2(-r01,r00)
                res[1] = Mathf.Asin(Get3x3ElementAtIndex(mat, 2));
                res[0] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 5), Get3x3ElementAtIndex(mat, 8));
                res[2] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 1), Get3x3ElementAtIndex(mat, 0));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // y_angle = -pi/2
                // z_angle - x_angle = atan2(r10,r11)
                // WARNING.  The solution is not unique.  Choosing z_angle = 0.
                res[1] = -Mathf.PI / 2.0f;
                res[0] = -Mathf.Atan2(Get3x3ElementAtIndex(mat, 3), Get3x3ElementAtIndex(mat, 4));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // y_angle = +pi/2
            // z_angle + x_angle = atan2(r10,r11)
            // WARNING.  The solutions is not unique.  Choosing z_angle = 0.
            res[1] = Mathf.PI / 2.0f;
            res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 3), Get3x3ElementAtIndex(mat, 4));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerXZY(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                        -+
        // | r00 r01 r02 |   |  cy*cz           -sz      cz*sy          |
        // | r10 r11 r12 | = |  sx*sy+cx*cy*sz   cx*cz  -cy*sx+cx*sy*sz |
        // | r20 r21 r22 |   | -cx*sy+cy*sx*sz   cz*sx   cx*cy+sx*sy*sz |
        // +-           -+   +-                                        -+

        if (Get3x3ElementAtIndex(mat, 1) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 1) > -1)
            {
                // z_angle = asin(-r01)
                // x_angle = atan2(r21,r11)
                // y_angle = atan2(r02,r00)
                res[1] = Mathf.Asin(-Get3x3ElementAtIndex(mat, 1));
                res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 7), Get3x3ElementAtIndex(mat, 4));
                res[2] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 2), Get3x3ElementAtIndex(mat, 0));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // z_angle = +pi/2
                // y_angle - x_angle = atan2(-r20,r22)
                // WARNING.  The solution is not unique.  Choosing y_angle = 0.
                res[1] = Mathf.PI / 2.0f;
                res[0] = -Mathf.Atan2(-Get3x3ElementAtIndex(mat, 6) ,Get3x3ElementAtIndex(mat, 8));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // z_angle = -pi/2
            // y_angle + x_angle = atan2(-r20,r22)
            // WARNING.  The solution is not unique.  Choosing y_angle = 0.
            res[1] = -Mathf.PI / 2.0f;
            res[0] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 6), Get3x3ElementAtIndex(mat, 8));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerYXZ(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                       -+
        // | r00 r01 r02 |   |  cy*cz+sx*sy*sz  cz*sx*sy-cy*sz   cx*sy |
        // | r10 r11 r12 | = |  cx*sz           cx*cz           -sx    |
        // | r20 r21 r22 |   | -cz*sy+cy*sx*sz  cy*cz*sx+sy*sz   cx*cy |
        // +-           -+   +-                                       -+

        if (Get3x3ElementAtIndex(mat, 5) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 5) > -1)
            {
                // x_angle = asin(-r12)
                // y_angle = atan2(r02,r22)
                // z_angle = atan2(r10,r11)
                res[1] = Mathf.Asin(-Get3x3ElementAtIndex(mat, 5));
                res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 2), Get3x3ElementAtIndex(mat, 8));
                res[2] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 3), Get3x3ElementAtIndex(mat, 4));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // x_angle = +pi/2
                // z_angle - y_angle = atan2(-r01,r00)
                // WARNING.  The solution is not unique.  Choosing z_angle = 0.
                res[1] = Mathf.PI / 2.0f;
                res[0] = -Mathf.Atan2(-Get3x3ElementAtIndex(mat, 1), Get3x3ElementAtIndex(mat, 0));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // x_angle = -pi/2
            // z_angle + y_angle = atan2(-r01,r00)
            // WARNING.  The solution is not unique.  Choosing z_angle = 0.
            res[1] = -Mathf.PI / 2.0f;
            res[0] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 1), Get3x3ElementAtIndex(mat, 0));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerYZX(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                       -+
        // | r00 r01 r02 |   |  cy*cz  sx*sy-cx*cy*sz   cx*sy+cy*sx*sz |
        // | r10 r11 r12 | = |  sz     cx*cz           -cz*sx          |
        // | r20 r21 r22 |   | -cz*sy  cy*sx+cx*sy*sz   cx*cy-sx*sy*sz |
        // +-           -+   +-                                       -+

        if (Get3x3ElementAtIndex(mat, 3) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 3) > -1)
            {
                // z_angle = asin(r10)
                // y_angle = atan2(-r20,r00)
                // x_angle = atan2(-r12,r11)
                res[1] = Mathf.Asin(Get3x3ElementAtIndex(mat, 3));
                res[0] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 6), Get3x3ElementAtIndex(mat, 0));
                res[2] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 5), Get3x3ElementAtIndex(mat, 4));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // z_angle = -pi/2
                // x_angle - y_angle = atan2(r21,r22)
                // WARNING.  The solution is not unique.  Choosing x_angle = 0.
                res[1] = -Mathf.PI / 2.0f;
                res[0] = -Mathf.Atan2(Get3x3ElementAtIndex(mat, 7), Get3x3ElementAtIndex(mat, 8));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // z_angle = +pi/2
            // x_angle + y_angle = atan2(r21,r22)
            // WARNING.  The solution is not unique.  Choosing x_angle = 0.
            res[1] = Mathf.PI / 2.0f;
            res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 7), Get3x3ElementAtIndex(mat, 8));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerZXY(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                        -+
        // | r00 r01 r02 |   |  cy*cz-sx*sy*sz  -cx*sz   cz*sy+cy*sx*sz |
        // | r10 r11 r12 | = |  cz*sx*sy+cy*sz   cx*cz  -cy*cz*sx+sy*sz |
        // | r20 r21 r22 |   | -cx*sy            sx      cx*cy          |
        // +-           -+   +-                                        -+

        if (Get3x3ElementAtIndex(mat, 7) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 7) > -1)
            {
                // x_angle = asin(r21)
                // z_angle = atan2(-r01,r11)
                // y_angle = atan2(-r20,r22)
                res[1] = Mathf.Asin(Get3x3ElementAtIndex(mat, 7));
                res[0] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 1), Get3x3ElementAtIndex(mat, 4));
                res[2] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 6), Get3x3ElementAtIndex(mat, 8));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // x_angle = -pi/2
                // y_angle - z_angle = atan2(r02,r00)
                // WARNING.  The solution is not unique.  Choosing y_angle = 0.
                res[1] = -Mathf.PI / 2.0f;
                res[0] = -Mathf.Atan2(Get3x3ElementAtIndex(mat, 2), Get3x3ElementAtIndex(mat, 0));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // x_angle = +pi/2
            // y_angle + z_angle = atan2(r02,r00)
            // WARNING.  The solution is not unique.  Choosing y_angle = 0.
            res[1] = Mathf.PI / 2.0f;
            res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 2), Get3x3ElementAtIndex(mat, 0));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerZYX(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                      -+
        // | r00 r01 r02 |   |  cy*cz  cz*sx*sy-cx*sz  cx*cz*sy+sx*sz |
        // | r10 r11 r12 | = |  cy*sz  cx*cz+sx*sy*sz -cz*sx+cx*sy*sz |
        // | r20 r21 r22 |   | -sy     cy*sx           cx*cy          |
        // +-           -+   +-                                      -+

        if (Get3x3ElementAtIndex(mat, 6) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 6) > -1)
            {
                // y_angle = asin(-r20)
                // z_angle = atan2(r10,r00)
                // x_angle = atan2(r21,r22)
                res[1] = Mathf.Asin(-Get3x3ElementAtIndex(mat, 6));
                res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 3), Get3x3ElementAtIndex(mat, 0));
                res[2] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 7), Get3x3ElementAtIndex(mat, 8));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // y_angle = +pi/2
                // x_angle - z_angle = atan2(r01,r02)
                // WARNING.  The solution is not unique.  Choosing x_angle = 0.
                res[1] = Mathf.PI / 2.0f;
                res[0] = -Mathf.Atan2(Get3x3ElementAtIndex(mat, 1), Get3x3ElementAtIndex(mat, 2));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // y_angle = -pi/2
            // x_angle + z_angle = atan2(-r01,-r02)
            // WARNING.  The solution is not unique.  Choosing x_angle = 0;
            res[1] = -Mathf.PI / 2.0f;
            res[0] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 1), -Get3x3ElementAtIndex(mat, 2));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerXYX(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                                -+
        // | r00 r01 r02 |   |  cy      sy*sx1               sy*cx1             |
        // | r10 r11 r12 | = |  sy*sx0  cx0*cx1-cy*sx0*sx1  -cy*cx1*sx0-cx0*sx1 |
        // | r20 r21 r22 |   | -sy*cx0  cx1*sx0+cy*cx0*sx1   cy*cx0*cx1-sx0*sx1 |
        // +-           -+   +-                                                -+

        if (Get3x3ElementAtIndex(mat, 0) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 0) > -1)
            {
                // y_angle  = acos(r00)
                // x0_angle = atan2(r10,-r20)
                // x1_angle = atan2(r01,r02)
                res[1] = Mathf.Acos(Get3x3ElementAtIndex(mat, 0));
                res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 3), -Get3x3ElementAtIndex(mat, 6));
                res[2] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 1), Get3x3ElementAtIndex(mat, 2));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // Not a unique solution:  x1_angle - x0_angle = atan2(-r12,r11)
                res[1] = Mathf.PI;
                res[0] = -Mathf.Atan2(-Get3x3ElementAtIndex(mat, 5), Get3x3ElementAtIndex(mat, 4));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // Not a unique solution:  x1_angle + x0_angle = atan2(-r12,r11)
            res[1] = 0;
            res[0] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 5), Get3x3ElementAtIndex(mat, 4));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerXZX(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                                -+
        // | r00 r01 r02 |   | cz      -sz*cx1               sz*sx1             |
        // | r10 r11 r12 | = | sz*cx0   cz*cx0*cx1-sx0*sx1  -cx1*sx0-cz*cx0*sx1 |
        // | r20 r21 r22 |   | sz*sx0   cz*cx1*sx0+cx0*sx1   cx0*cx1-cz*sx0*sx1 |
        // +-           -+   +-                                                -+

        if (Get3x3ElementAtIndex(mat, 0) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 0) > -1)
            {
                // z_angle  = acos(r00)
                // x0_angle = atan2(r20,r10)
                // x1_angle = atan2(r02,-r01)
                res[1] = Mathf.Acos(Get3x3ElementAtIndex(mat, 0));
                res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 6), Get3x3ElementAtIndex(mat, 3));
                res[2] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 2), -Get3x3ElementAtIndex(mat, 1));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // Not a unique solution:  x1_angle - x0_angle = atan2(r21,r22)
                res[1] = Mathf.PI;
                res[0] = -Mathf.Atan2(Get3x3ElementAtIndex(mat, 7), Get3x3ElementAtIndex(mat, 8));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // Not a unique solution:  x1_angle + x0_angle = atan2(r21,r22)
            res[1] = 0;
            res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 7), Get3x3ElementAtIndex(mat, 8));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerYXY(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                                -+
        // | r00 r01 r02 |   |  cy0*cy1-cx*sy0*sy1  sx*sy0   cx*cy1*sy0+cy0*sy1 |
        // | r10 r11 r12 | = |  sx*sy1              cx      -sx*cy1             |
        // | r20 r21 r22 |   | -cy1*sy0-cx*cy0*sy1  sx*cy0   cx*cy0*cy1-sy0*sy1 |
        // +-           -+   +-                                                -+

        if (Get3x3ElementAtIndex(mat, 4) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 4) > -1)
            {
                // x_angle  = acos(r11)
                // y0_angle = atan2(r01,r21)
                // y1_angle = atan2(r10,-r12)
                res[1] = Mathf.Acos(Get3x3ElementAtIndex(mat, 4));
                res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 1), Get3x3ElementAtIndex(mat, 7));
                res[2] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 3), -Get3x3ElementAtIndex(mat, 5));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // Not a unique solution:  y1_angle - y0_angle = atan2(r02,r00)
                res[1] = Mathf.PI;
                res[0] = -Mathf.Atan2(Get3x3ElementAtIndex(mat, 2), Get3x3ElementAtIndex(mat, 0));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // Not a unique solution:  y1_angle + y0_angle = atan2(r02,r00)
            res[1] = 0;
            res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 2), Get3x3ElementAtIndex(mat, 0));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerYZY(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                                -+
        // | r00 r01 r02 |   |  cz*cy0*cy1-sy0*sy1  -sz*cy0  cy1*sy0+cz*cy0*sy1 |
        // | r10 r11 r12 | = |  sz*cy1               cz      sz*sy1             |
        // | r20 r21 r22 |   | -cz*cy1*sy0-cy0*sy1   sz*sy0  cy0*cy1-cz*sy0*sy1 |
        // +-           -+   +-                                                -+

        if (Get3x3ElementAtIndex(mat, 4) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 4) > -1)
            {
                // z_angle  = acos(r11)
                // y0_angle = atan2(r21,-r01)
                // y1_angle = atan2(r12,r10)
                res[1] = Mathf.Acos(Get3x3ElementAtIndex(mat, 4));
                res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 7), -Get3x3ElementAtIndex(mat, 1));
                res[2] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 5), Get3x3ElementAtIndex(mat, 3));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // Not a unique solution:  y1_angle - y0_angle = atan2(-r20,r22)
                res[1] = Mathf.PI;
                res[0] = -Mathf.Atan2(-Get3x3ElementAtIndex(mat, 6), Get3x3ElementAtIndex(mat, 8));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // Not a unique solution:  y1_angle + y0_angle = atan2(-r20,r22)
            res[1] = 0;
            res[0] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 6), Get3x3ElementAtIndex(mat, 8));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerZXZ(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                                -+
        // | r00 r01 r02 |   | cz0*cz1-cx*sz0*sz1  -cx*cz1*sz0-cz0*sz1   sx*sz0 |
        // | r10 r11 r12 | = | cz1*sz0+cx*cz0*sz1   cx*cz0*cz1-sz0*sz1  -sz*cz0 |
        // | r20 r21 r22 |   | sx*sz1               sx*cz1               cx     |
        // +-           -+   +-                                                -+

        if (Get3x3ElementAtIndex(mat, 8) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 8) > -1)
            {
                // x_angle  = acos(r22)
                // z0_angle = atan2(r02,-r12)
                // z1_angle = atan2(r20,r21)
                res[1] = Mathf.Acos(Get3x3ElementAtIndex(mat, 8));
                res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 2), -Get3x3ElementAtIndex(mat, 5));
                res[2] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 6), Get3x3ElementAtIndex(mat, 7));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else
            {
                // Not a unique solution:  z1_angle - z0_angle = atan2(-r01,r00)
                res[1] = Mathf.PI;
                res[0] = -Mathf.Atan2(-Get3x3ElementAtIndex(mat, 1), Get3x3ElementAtIndex(mat, 0));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else
        {
            // Not a unique solution:  z1_angle + z0_angle = atan2(-r01,r00)
            res[1] = 0;
            res[0] = Mathf.Atan2(-Get3x3ElementAtIndex(mat, 1), Get3x3ElementAtIndex(mat, 0));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }

    public static Vector3 ExtractEulerZYZ(Matrix4x4 mat, out EulerResult eulerRes)
    {
        Vector3 res = new Vector3();

        // +-           -+   +-                                                -+
        // | r00 r01 r02 |   |  cy*cz0*cz1-sz0*sz1  -cz1*sz0-cy*cz0*sz1  sy*cz0 |
        // | r10 r11 r12 | = |  cy*cz1*sz0+cz0*sz1   cz0*cz1-cy*sz0*sz1  sy*sz0 |
        // | r20 r21 r22 |   | -sy*cz1               sy*sz1              cy     |
        // +-           -+   +-                                                -+

        if (Get3x3ElementAtIndex(mat, 8) < 1)
        {
            if (Get3x3ElementAtIndex(mat, 8) > -1)
            {
                // y_angle  = acos(r22)
                // z0_angle = atan2(r12,r02)
                // z1_angle = atan2(r21,-r20)
                res[1] = Mathf.Acos(Get3x3ElementAtIndex(mat, 8));
                res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 5), Get3x3ElementAtIndex(mat, 2));
                res[2] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 7), -Get3x3ElementAtIndex(mat, 6));
                eulerRes = EulerResult.EA_UNIQUE;
                return res;
            }
            else // r22 = -1
            {
                // Not a unique solution:  z1_angle - z0_angle = atan2(r10,r11)
                res[1] = Mathf.PI;
                res[0] = -Mathf.Atan2(Get3x3ElementAtIndex(mat, 3), Get3x3ElementAtIndex(mat, 4));
                res[2] = 0;
                eulerRes = EulerResult.EA_NOT_UNIQUE_DIF;
                return res;
            }
        }
        else // r22 = +1
        {
            // Not a unique solution:  z1_angle + z0_angle = atan2(r10,r11)
            res[1] = 0;
            res[0] = Mathf.Atan2(Get3x3ElementAtIndex(mat, 3), Get3x3ElementAtIndex(mat, 4));
            res[2] = 0;
            eulerRes = EulerResult.EA_NOT_UNIQUE_SUM;
            return res;
        }
    }
}