/*************************************************************************
Copyright (c) 2005-2007, Sergey Bochkanov (ALGLIB project).

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above copyright
  notice, this list of conditions and the following disclaimer listed
  in this license in the documentation and/or other materials
  provided with the distribution.

- Neither the name of the copyright holders nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*************************************************************************/

using System;

class sbisinv
{
    /*************************************************************************
    Subroutine for finding the eigenvalues (and eigenvectors) of  a  symmetric
    matrix  in  a  given half open interval (A, B] by using  a  bisection  and
    inverse iteration

    Input parameters:
        A       -   symmetric matrix which is given by its upper or lower
                    triangular part. Array [0..N-1, 0..N-1].
        N       -   size of matrix A.
        ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                    If ZNeeded is equal to:
                     * 0, the eigenvectors are not returned;
                     * 1, the eigenvectors are returned.
        IsUpperA -  storage format of matrix A.
        B1, B2 -    half open interval (B1, B2] to search eigenvalues in.

    Output parameters:
        M       -   number of eigenvalues found in a given half-interval (M>=0).
        W       -   array of the eigenvalues found.
                    Array whose index ranges within [0..M-1].
        Z       -   if ZNeeded is equal to:
                     * 0, Z hasn’t changed;
                     * 1, Z contains eigenvectors.
                    Array whose indexes range within [0..N-1, 0..M-1].
                    The eigenvectors are stored in the matrix columns.

    Result:
        True, if successful. M contains the number of eigenvalues in the given
        half-interval (could be equal to 0), W contains the eigenvalues,
        Z contains the eigenvectors (if needed).

        False, if the bisection method subroutine wasn't able to find the
        eigenvalues in the given interval or if the inverse iteration subroutine
        wasn't able to find all the corresponding eigenvectors.
        In that case, the eigenvalues and eigenvectors are not returned,
        M is equal to 0.

      -- ALGLIB --
         Copyright 07.01.2006 by Bochkanov Sergey
    *************************************************************************/
    public static bool smatrixevdr(double[,] a,
        int n,
        int zneeded,
        bool isupper,
        double b1,
        double b2,
        ref int m,
        ref double[] w,
        ref double[,] z)
    {
        bool result = new bool();
        double[] tau = new double[0];
        double[] e = new double[0];

        a = (double[,])a.Clone();

        System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "SMatrixTDEVDR: incorrect ZNeeded");
        tridiagonal.smatrixtd(ref a, n, isupper, ref tau, ref w, ref e);
        if( zneeded==1 )
        {
            tridiagonal.smatrixtdunpackq(ref a, n, isupper, ref tau, ref z);
        }
        result = tdbisinv.smatrixtdevdr(ref w, ref e, n, zneeded, b1, b2, ref m, ref z);
        return result;
    }


    /*************************************************************************
    Subroutine for finding the eigenvalues and  eigenvectors  of  a  symmetric
    matrix with given indexes by using bisection and inverse iteration methods.

    Input parameters:
        A       -   symmetric matrix which is given by its upper or lower
                    triangular part. Array whose indexes range within [0..N-1, 0..N-1].
        N       -   size of matrix A.
        ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                    If ZNeeded is equal to:
                     * 0, the eigenvectors are not returned;
                     * 1, the eigenvectors are returned.
        IsUpperA -  storage format of matrix A.
        I1, I2 -    index interval for searching (from I1 to I2).
                    0 <= I1 <= I2 <= N-1.

    Output parameters:
        W       -   array of the eigenvalues found.
                    Array whose index ranges within [0..I2-I1].
        Z       -   if ZNeeded is equal to:
                     * 0, Z hasn’t changed;
                     * 1, Z contains eigenvectors.
                    Array whose indexes range within [0..N-1, 0..I2-I1].
                    In that case, the eigenvectors are stored in the matrix columns.

    Result:
        True, if successful. W contains the eigenvalues, Z contains the
        eigenvectors (if needed).

        False, if the bisection method subroutine wasn't able to find the
        eigenvalues in the given interval or if the inverse iteration subroutine
        wasn't able to find all the corresponding eigenvectors.
        In that case, the eigenvalues and eigenvectors are not returned.

      -- ALGLIB --
         Copyright 07.01.2006 by Bochkanov Sergey
    *************************************************************************/
    public static bool smatrixevdi(double[,] a,
        int n,
        int zneeded,
        bool isupper,
        int i1,
        int i2,
        ref double[] w,
        ref double[,] z)
    {
        bool result = new bool();
        double[] tau = new double[0];
        double[] e = new double[0];

        a = (double[,])a.Clone();

        System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "SMatrixEVDI: incorrect ZNeeded");
        tridiagonal.smatrixtd(ref a, n, isupper, ref tau, ref w, ref e);
        if( zneeded==1 )
        {
            tridiagonal.smatrixtdunpackq(ref a, n, isupper, ref tau, ref z);
        }
        result = tdbisinv.smatrixtdevdi(ref w, ref e, n, zneeded, i1, i2, ref z);
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static bool symmetriceigenvaluesandvectorsininterval(double[,] a,
        int n,
        int zneeded,
        bool isupper,
        double b1,
        double b2,
        ref int m,
        ref double[] w,
        ref double[,] z)
    {
        bool result = new bool();
        double[] tau = new double[0];
        double[] e = new double[0];

        a = (double[,])a.Clone();

        System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "SymmetricEigenValuesAndVectorsInInterval: incorrect ZNeeded");
        tridiagonal.totridiagonal(ref a, n, isupper, ref tau, ref w, ref e);
        if( zneeded==1 )
        {
            tridiagonal.unpackqfromtridiagonal(ref a, n, isupper, ref tau, ref z);
        }
        result = tdbisinv.tridiagonaleigenvaluesandvectorsininterval(ref w, ref e, n, zneeded, b1, b2, ref m, ref z);
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static bool symmetriceigenvaluesandvectorsbyindexes(double[,] a,
        int n,
        int zneeded,
        bool isupper,
        int i1,
        int i2,
        ref double[] w,
        ref double[,] z)
    {
        bool result = new bool();
        double[] tau = new double[0];
        double[] e = new double[0];

        a = (double[,])a.Clone();

        System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "SymmetricEigenValuesAndVectorsInInterval: incorrect ZNeeded");
        tridiagonal.totridiagonal(ref a, n, isupper, ref tau, ref w, ref e);
        if( zneeded==1 )
        {
            tridiagonal.unpackqfromtridiagonal(ref a, n, isupper, ref tau, ref z);
        }
        result = tdbisinv.tridiagonaleigenvaluesandvectorsbyindexes(ref w, ref e, n, zneeded, i1, i2, ref z);
        return result;
    }
}
