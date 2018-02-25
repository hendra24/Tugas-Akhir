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

class spddet
{
    /*************************************************************************
    Determinant calculation of the matrix given by the Cholesky decomposition.

    Input parameters:
        A   -   Cholesky decomposition,
                output of SMatrixCholesky subroutine.
        N   -   size of matrix A.

    As the determinant is equal to the product of squares of diagonal elements,
    it’s not necessary to specify which triangle - lower or upper - the matrix
    is stored in.

    Result:
        matrix determinant.

      -- ALGLIB --
         Copyright 2005-2008 by Bochkanov Sergey
    *************************************************************************/
    public static double spdmatrixcholeskydet(ref double[,] a,
        int n)
    {
        double result = 0;
        int i = 0;

        result = 1;
        for(i=0; i<=n-1; i++)
        {
            result = result*AP.Math.Sqr(a[i,i]);
        }
        return result;
    }


    /*************************************************************************
    Determinant calculation of the symmetric positive definite matrix.

    Input parameters:
        A       -   matrix. Array with elements [0..N-1, 0..N-1].
        N       -   size of matrix A.
        IsUpper -   if IsUpper = True, then the symmetric matrix A is given by
                    its upper triangle, and the lower triangle isn’t used by
                    subroutine. Similarly, if IsUpper = False, then A is given
                    by its lower triangle.

    Result:
        determinant of matrix A.
        If matrix A is not positive definite, then subroutine returns -1.

      -- ALGLIB --
         Copyright 2005-2008 by Bochkanov Sergey
    *************************************************************************/
    public static double spdmatrixdet(double[,] a,
        int n,
        bool isupper)
    {
        double result = 0;

        a = (double[,])a.Clone();

        if( !cholesky.spdmatrixcholesky(ref a, n, isupper) )
        {
            result = -1;
        }
        else
        {
            result = spdmatrixcholeskydet(ref a, n);
        }
        return result;
    }


    /*************************************************************************
    Obsolete subroutine
    *************************************************************************/
    public static double determinantcholesky(ref double[,] a,
        int n)
    {
        double result = 0;
        int i = 0;

        result = 1;
        for(i=1; i<=n; i++)
        {
            result = result*AP.Math.Sqr(a[i,i]);
        }
        return result;
    }


    /*************************************************************************
    Obsolete subroutine
    *************************************************************************/
    public static double determinantspd(double[,] a,
        int n,
        bool isupper)
    {
        double result = 0;

        a = (double[,])a.Clone();

        if( !cholesky.choleskydecomposition(ref a, n, isupper) )
        {
            result = -1;
        }
        else
        {
            result = determinantcholesky(ref a, n);
        }
        return result;
    }
}
