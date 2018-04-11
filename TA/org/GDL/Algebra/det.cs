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

class det
{
    /*************************************************************************
    Determinant calculation of the matrix given by its LU decomposition.

    Input parameters:
        A       -   LU decomposition of the matrix (output of
                    RMatrixLU subroutine).
        Pivots  -   table of permutations which were made during
                    the LU decomposition.
                    Output of RMatrixLU subroutine.
        N       -   size of matrix A.

    Result: matrix determinant.

      -- ALGLIB --
         Copyright 2005 by Bochkanov Sergey
    *************************************************************************/
    public static double rmatrixludet(ref double[,] a,
        ref int[] pivots,
        int n)
    {
        double result = 0;
        int i = 0;
        int s = 0;

        result = 1;
        s = 1;
        for(i=0; i<=n-1; i++)
        {
            result = result*a[i,i];
            if( pivots[i]!=i )
            {
                s = -s;
            }
        }
        result = result*s;
        return result;
    }


    /*************************************************************************
    Calculation of the determinant of a general matrix

    Input parameters:
        A       -   matrix, array[0..N-1, 0..N-1]
        N       -   size of matrix A.

    Result: determinant of matrix A.

      -- ALGLIB --
         Copyright 2005 by Bochkanov Sergey
    *************************************************************************/
    public static double rmatrixdet(double[,] a,
        int n)
    {
        double result = 0;
        int[] pivots = new int[0];

        a = (double[,])a.Clone();

        lu.rmatrixlu(ref a, n, n, ref pivots);
        result = rmatrixludet(ref a, ref pivots, n);
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine.
    See RMatrixDetLU for 0-based replacement.
    *************************************************************************/
    public static double determinantlu(ref double[,] a,
        ref int[] pivots,
        int n)
    {
        double result = 0;
        int i = 0;
        int s = 0;

        result = 1;
        s = 1;
        for(i=1; i<=n; i++)
        {
            result = result*a[i,i];
            if( pivots[i]!=i )
            {
                s = -s;
            }
        }
        result = result*s;
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine.
    See RMatrixDet for 0-based replacement.
    *************************************************************************/
    public static double determinant(double[,] a,
        int n)
    {
        double result = 0;
        int[] pivots = new int[0];

        a = (double[,])a.Clone();

        lu.ludecomposition(ref a, n, n, ref pivots);
        result = determinantlu(ref a, ref pivots, n);
        return result;
    }
}
