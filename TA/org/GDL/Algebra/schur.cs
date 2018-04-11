/*************************************************************************
Copyright (c) 1992-2007 The University of Tennessee.  All rights reserved.

Contributors:
    * Sergey Bochkanov (ALGLIB project). Translation from FORTRAN to
      pseudocode.

See subroutines comments for additional copyrights.

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

class schur
{
    /*************************************************************************
    Subroutine performing the Schur decomposition of a general matrix by using
    the QR algorithm with multiple shifts.

    The source matrix A is represented as S'*A*S = T, where S is an orthogonal
    matrix (Schur vectors), T - upper quasi-triangular matrix (with blocks of
    sizes 1x1 and 2x2 on the main diagonal).

    Input parameters:
        A   -   matrix to be decomposed.
                Array whose indexes range within [0..N-1, 0..N-1].
        N   -   size of A, N>=0.


    Output parameters:
        A   -   contains matrix T.
                Array whose indexes range within [0..N-1, 0..N-1].
        S   -   contains Schur vectors.
                Array whose indexes range within [0..N-1, 0..N-1].

    Note 1:
        The block structure of matrix T can be easily recognized: since all
        the elements below the blocks are zeros, the elements a[i+1,i] which
        are equal to 0 show the block border.

    Note 2:
        The algorithm performance depends on the value of the internal parameter
        NS of the InternalSchurDecomposition subroutine which defines the number
        of shifts in the QR algorithm (similarly to the block width in block-matrix
        algorithms in linear algebra). If you require maximum performance on
        your machine, it is recommended to adjust this parameter manually.

    Result:
        True,
            if the algorithm has converged and parameters A and S contain the result.
        False,
            if the algorithm has not converged.

    Algorithm implemented on the basis of the DHSEQR subroutine (LAPACK 3.0 library).
    *************************************************************************/
    public static bool rmatrixschur(ref double[,] a,
        int n,
        ref double[,] s)
    {
        bool result = new bool();
        double[] tau = new double[0];
        double[] wi = new double[0];
        double[] wr = new double[0];
        double[,] a1 = new double[0,0];
        double[,] s1 = new double[0,0];
        int info = 0;
        int i = 0;
        int j = 0;

        
        //
        // Upper Hessenberg form of the 0-based matrix
        //
        hessenberg.rmatrixhessenberg(ref a, n, ref tau);
        hessenberg.rmatrixhessenbergunpackq(ref a, n, ref tau, ref s);
        
        //
        // Convert from 0-based arrays to 1-based,
        // then call InternalSchurDecomposition
        // Awkward, of course, but Schur decompisiton subroutine
        // is too complex to fix it.
        //
        //
        a1 = new double[n+1, n+1];
        s1 = new double[n+1, n+1];
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=n; j++)
            {
                a1[i,j] = a[i-1,j-1];
                s1[i,j] = s[i-1,j-1];
            }
        }
        hsschur.internalschurdecomposition(ref a1, n, 1, 1, ref wr, ref wi, ref s1, ref info);
        result = info==0;
        
        //
        // convert from 1-based arrays to -based
        //
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=n; j++)
            {
                a[i-1,j-1] = a1[i,j];
                s[i-1,j-1] = s1[i,j];
            }
        }
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine.
    See RMatrixSchur for 0-based replacement.
    *************************************************************************/
    public static bool schurdecomposition(ref double[,] a,
        int n,
        ref double[,] s)
    {
        bool result = new bool();
        double[] tau = new double[0];
        double[] wi = new double[0];
        double[] wr = new double[0];
        int info = 0;

        hessenberg.toupperhessenberg(ref a, n, ref tau);
        hessenberg.unpackqfromupperhessenberg(ref a, n, ref tau, ref s);
        hsschur.internalschurdecomposition(ref a, n, 1, 1, ref wr, ref wi, ref s, ref info);
        result = info==0;
        return result;
    }
}
