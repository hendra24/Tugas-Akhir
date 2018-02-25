/*************************************************************************
Copyright (c) 1992-2007 The University of Tennessee. All rights reserved.

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

class clu
{
    /*************************************************************************
    LU decomposition of a complex general matrix of size MxN

    The subroutine calculates the LU decomposition of a rectangular general
    matrix with partial pivoting (with row permutations).

    Input parameters:
        A   -   matrix A whose indexes range within [0..M-1, 0..N-1].
        M   -   number of rows in matrix A.
        N   -   number of columns in matrix A.

    Output parameters:
        A   -   matrices L and U in compact form (see below).
                Array whose indexes range within [0..M-1, 0..N-1].
        Pivots - permutation matrix in compact form (see below).
                Array whose index ranges within [0..Min(M-1,N-1)].

    Matrix A is represented as A = P * L * U, where P is a permutation matrix,
    matrix L - lower triangular (or lower trapezoid, if M>N) matrix,
    U - upper triangular (or upper trapezoid, if M<N) matrix.

    Let M be equal to 4 and N be equal to 3:

                       (  1          )    ( U11 U12 U13  )
    A = P1 * P2 * P3 * ( L21  1      )  * (     U22 U23  )
                       ( L31 L32  1  )    (         U33  )
                       ( L41 L42 L43 )

    Matrix L has size MxMin(M,N), matrix U has size Min(M,N)xN, matrix P(i) is
    a permutation of the identity matrix of size MxM with numbers I and Pivots[I].

    The algorithm returns array Pivots and the following matrix which replaces
    matrix A and contains matrices L and U in compact form (the example applies
    to M=4, N=3).

     ( U11 U12 U13 )
     ( L21 U22 U23 )
     ( L31 L32 U33 )
     ( L41 L42 L43 )

    As we can see, the unit diagonal isn't stored.

      -- LAPACK routine (version 3.0) --
         Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
         Courant Institute, Argonne National Lab, and Rice University
         June 30, 1992
    *************************************************************************/
    public static void cmatrixlu(ref AP.Complex[,] a,
        int m,
        int n,
        ref int[] pivots)
    {
        int i = 0;
        int j = 0;
        int jp = 0;
        AP.Complex[] t1 = new AP.Complex[0];
        AP.Complex s = 0;
        int i_ = 0;

        pivots = new int[Math.Min(m-1, n-1)+1];
        t1 = new AP.Complex[Math.Max(m-1, n-1)+1];
        System.Diagnostics.Debug.Assert(m>=0 & n>=0, "Error in LUDecomposition: incorrect function arguments");
        
        //
        // Quick return if possible
        //
        if( m==0 | n==0 )
        {
            return;
        }
        for(j=0; j<=Math.Min(m-1, n-1); j++)
        {
            
            //
            // Find pivot and test for singularity.
            //
            jp = j;
            for(i=j+1; i<=m-1; i++)
            {
                if( AP.Math.AbsComplex(a[i,j])>AP.Math.AbsComplex(a[jp,j]) )
                {
                    jp = i;
                }
            }
            pivots[j] = jp;
            if( a[jp,j]!=0 )
            {
                
                //
                //Apply the interchange to rows
                //
                if( jp!=j )
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        t1[i_] = a[j,i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[j,i_] = a[jp,i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[jp,i_] = t1[i_];
                    }
                }
                
                //
                //Compute elements J+1:M of J-th column.
                //
                if( j<m )
                {
                    jp = j+1;
                    s = 1/a[j,j];
                    for(i_=jp; i_<=m-1;i_++)
                    {
                        a[i_,j] = s*a[i_,j];
                    }
                }
            }
            if( j<Math.Min(m, n)-1 )
            {
                
                //
                //Update trailing submatrix.
                //
                jp = j+1;
                for(i=j+1; i<=m-1; i++)
                {
                    s = a[i,j];
                    for(i_=jp; i_<=n-1;i_++)
                    {
                        a[i,i_] = a[i,i_] - s*a[j,i_];
                    }
                }
            }
        }
    }


    /*************************************************************************
    Obsolete 1-based subroutine, left for backward compatibility.
    See CMatrixLU for 0-based replacement.
    *************************************************************************/
    public static void complexludecomposition(ref AP.Complex[,] a,
        int m,
        int n,
        ref int[] pivots)
    {
        int i = 0;
        int j = 0;
        int jp = 0;
        AP.Complex[] t1 = new AP.Complex[0];
        AP.Complex s = 0;
        int i_ = 0;

        pivots = new int[Math.Min(m, n)+1];
        t1 = new AP.Complex[Math.Max(m, n)+1];
        System.Diagnostics.Debug.Assert(m>=0 & n>=0, "Error in ComplexLUDecomposition: incorrect function arguments");
        
        //
        // Quick return if possible
        //
        if( m==0 | n==0 )
        {
            return;
        }
        for(j=1; j<=Math.Min(m, n); j++)
        {
            
            //
            // Find pivot and test for singularity.
            //
            jp = j;
            for(i=j+1; i<=m; i++)
            {
                if( AP.Math.AbsComplex(a[i,j])>AP.Math.AbsComplex(a[jp,j]) )
                {
                    jp = i;
                }
            }
            pivots[j] = jp;
            if( a[jp,j]!=0 )
            {
                
                //
                //Apply the interchange to rows
                //
                if( jp!=j )
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        t1[i_] = a[j,i_];
                    }
                    for(i_=1; i_<=n;i_++)
                    {
                        a[j,i_] = a[jp,i_];
                    }
                    for(i_=1; i_<=n;i_++)
                    {
                        a[jp,i_] = t1[i_];
                    }
                }
                
                //
                //Compute elements J+1:M of J-th column.
                //
                if( j<m )
                {
                    jp = j+1;
                    s = 1/a[j,j];
                    for(i_=jp; i_<=m;i_++)
                    {
                        a[i_,j] = s*a[i_,j];
                    }
                }
            }
            if( j<Math.Min(m, n) )
            {
                
                //
                //Update trailing submatrix.
                //
                jp = j+1;
                for(i=j+1; i<=m; i++)
                {
                    s = a[i,j];
                    for(i_=jp; i_<=n;i_++)
                    {
                        a[i,i_] = a[i,i_] - s*a[j,i_];
                    }
                }
            }
        }
    }


    /*************************************************************************
    Obsolete 1-based subroutine, left for backward compatibility.
    See CMatrixLU for 0-based replacement.
    *************************************************************************/
    public static void complexludecompositionunpacked(AP.Complex[,] a,
        int m,
        int n,
        ref AP.Complex[,] l,
        ref AP.Complex[,] u,
        ref int[] pivots)
    {
        int i = 0;
        int j = 0;
        int minmn = 0;

        a = (AP.Complex[,])a.Clone();

        if( m==0 | n==0 )
        {
            return;
        }
        minmn = Math.Min(m, n);
        l = new AP.Complex[m+1, minmn+1];
        u = new AP.Complex[minmn+1, n+1];
        complexludecomposition(ref a, m, n, ref pivots);
        for(i=1; i<=m; i++)
        {
            for(j=1; j<=minmn; j++)
            {
                if( j>i )
                {
                    l[i,j] = 0;
                }
                if( j==i )
                {
                    l[i,j] = 1;
                }
                if( j<i )
                {
                    l[i,j] = a[i,j];
                }
            }
        }
        for(i=1; i<=minmn; i++)
        {
            for(j=1; j<=n; j++)
            {
                if( j<i )
                {
                    u[i,j] = 0;
                }
                if( j>=i )
                {
                    u[i,j] = a[i,j];
                }
            }
        }
    }
}
