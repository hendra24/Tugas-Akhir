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

class sdet
{
    /*************************************************************************
    Determinant calculation of the matrix given by LDLT decomposition.

    Input parameters:
        A       -   LDLT-decomposition of the matrix,
                    output of subroutine SMatrixLDLT.
        Pivots  -   table of permutations which were made during
                    LDLT decomposition, output of subroutine SMatrixLDLT.
        N       -   size of matrix A.
        IsUpper -   matrix storage format. The value is equal to the input
                    parameter of subroutine SMatrixLDLT.

    Result:
        matrix determinant.

      -- ALGLIB --
         Copyright 2005-2008 by Bochkanov Sergey
    *************************************************************************/
    public static double smatrixldltdet(ref double[,] a,
        ref int[] pivots,
        int n,
        bool isupper)
    {
        double result = 0;
        int k = 0;

        result = 1;
        if( isupper )
        {
            k = 0;
            while( k<n )
            {
                if( pivots[k]>=0 )
                {
                    result = result*a[k,k];
                    k = k+1;
                }
                else
                {
                    result = result*(a[k,k]*a[k+1,k+1]-a[k,k+1]*a[k,k+1]);
                    k = k+2;
                }
            }
        }
        else
        {
            k = n-1;
            while( k>=0 )
            {
                if( pivots[k]>=0 )
                {
                    result = result*a[k,k];
                    k = k-1;
                }
                else
                {
                    result = result*(a[k-1,k-1]*a[k,k]-a[k,k-1]*a[k,k-1]);
                    k = k-2;
                }
            }
        }
        return result;
    }


    /*************************************************************************
    Determinant calculation of the symmetric matrix

    Input parameters:
        A       -   matrix. Array with elements [0..N-1, 0..N-1].
        N       -   size of matrix A.
        IsUpper -   if IsUpper = True, then symmetric matrix A is given by its
                    upper triangle, and the lower triangle isn’t used by
                    subroutine. Similarly, if IsUpper = False, then A is given
                    by its lower triangle.

    Result:
        determinant of matrix A.

      -- ALGLIB --
         Copyright 2005-2008 by Bochkanov Sergey
    *************************************************************************/
    public static double smatrixdet(double[,] a,
        int n,
        bool isupper)
    {
        double result = 0;
        int[] pivots = new int[0];

        a = (double[,])a.Clone();

        ldlt.smatrixldlt(ref a, n, isupper, ref pivots);
        result = smatrixldltdet(ref a, ref pivots, n, isupper);
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static double determinantldlt(ref double[,] a,
        ref int[] pivots,
        int n,
        bool isupper)
    {
        double result = 0;
        int k = 0;

        result = 1;
        if( isupper )
        {
            k = 1;
            while( k<=n )
            {
                if( pivots[k]>0 )
                {
                    result = result*a[k,k];
                    k = k+1;
                }
                else
                {
                    result = result*(a[k,k]*a[k+1,k+1]-a[k,k+1]*a[k,k+1]);
                    k = k+2;
                }
            }
        }
        else
        {
            k = n;
            while( k>=1 )
            {
                if( pivots[k]>0 )
                {
                    result = result*a[k,k];
                    k = k-1;
                }
                else
                {
                    result = result*(a[k-1,k-1]*a[k,k]-a[k,k-1]*a[k,k-1]);
                    k = k-2;
                }
            }
        }
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static double determinantsymmetric(double[,] a,
        int n,
        bool isupper)
    {
        double result = 0;
        int[] pivots = new int[0];

        a = (double[,])a.Clone();

        ldlt.ldltdecomposition(ref a, n, isupper, ref pivots);
        result = determinantldlt(ref a, ref pivots, n, isupper);
        return result;
    }
}
