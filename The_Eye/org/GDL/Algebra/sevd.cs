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

class sevd
{
    /*************************************************************************
    Finding the eigenvalues and eigenvectors of a symmetric matrix

    The algorithm finds eigen pairs of a symmetric matrix by reducing it to
    tridiagonal form and using the QL/QR algorithm.

    Input parameters:
        A       -   symmetric matrix which is given by its upper or lower
                    triangular part.
                    Array whose indexes range within [0..N-1, 0..N-1].
        N       -   size of matrix A.
        IsUpper -   storage format.
        ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                    If ZNeeded is equal to:
                     * 0, the eigenvectors are not returned;
                     * 1, the eigenvectors are returned.

    Output parameters:
        D       -   eigenvalues in ascending order.
                    Array whose index ranges within [0..N-1].
        Z       -   if ZNeeded is equal to:
                     * 0, Z hasn’t changed;
                     * 1, Z contains the eigenvectors.
                    Array whose indexes range within [0..N-1, 0..N-1].
                    The eigenvectors are stored in the matrix columns.

    Result:
        True, if the algorithm has converged.
        False, if the algorithm hasn't converged (rare case).

      -- ALGLIB --
         Copyright 2005-2008 by Bochkanov Sergey
    *************************************************************************/
    public static bool smatrixevd(double[,] a,
        int n,
        int zneeded,
        bool isupper,
        ref double[] d,
        ref double[,] z)
    {
        bool result = new bool();
        double[] tau = new double[0];
        double[] e = new double[0];

        a = (double[,])a.Clone();

        System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "SMatrixEVD: incorrect ZNeeded");
        tridiagonal.smatrixtd(ref a, n, isupper, ref tau, ref d, ref e);
        if( zneeded==1 )
        {
            tridiagonal.smatrixtdunpackq(ref a, n, isupper, ref tau, ref z);
        }
        result = tdevd.smatrixtdevd(ref d, e, n, zneeded, ref z);
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static bool symmetricevd(double[,] a,
        int n,
        int zneeded,
        bool isupper,
        ref double[] d,
        ref double[,] z)
    {
        bool result = new bool();
        double[] tau = new double[0];
        double[] e = new double[0];

        a = (double[,])a.Clone();

        System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "SymmetricEVD: incorrect ZNeeded");
        tridiagonal.totridiagonal(ref a, n, isupper, ref tau, ref d, ref e);
        if( zneeded==1 )
        {
            tridiagonal.unpackqfromtridiagonal(ref a, n, isupper, ref tau, ref z);
        }
        result = tdevd.tridiagonalevd(ref d, e, n, zneeded, ref z);
        return result;
    }
}
