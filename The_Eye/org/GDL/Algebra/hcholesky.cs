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

class hcholesky
{
    /*************************************************************************
    Cholesky decomposition

    The algorithm computes Cholesky decomposition  of  a  Hermitian  positive-
    definite matrix.

    The result of an algorithm is a representation of matrix A as A = U'*U  or
    A = L*L' (here X' detones conj(X^T)).

    Input parameters:
        A       -   upper or lower triangle of a factorized matrix.
                    array with elements [0..N-1, 0..N-1].
        N       -   size of matrix A.
        IsUpper -   if IsUpper=True, then A contains an upper triangle of
                    a symmetric matrix, otherwise A contains a lower one.

    Output parameters:
        A       -   the result of factorization. If IsUpper=True, then
                    the upper triangle contains matrix U, so that A = U'*U,
                    and the elements below the main diagonal are not modified.
                    Similarly, if IsUpper = False.

    Result:
        If the matrix is positive-definite, the function returns True.
        Otherwise, the function returns False. This means that the
        factorization could not be carried out.

      -- LAPACK routine (version 3.0) --
         Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
         Courant Institute, Argonne National Lab, and Rice University
         February 29, 1992
    *************************************************************************/
    public static bool hmatrixcholesky(ref AP.Complex[,] a,
        int n,
        bool isupper)
    {
        bool result = new bool();
        int j = 0;
        double ajj = 0;
        AP.Complex v = 0;
        double r = 0;
        AP.Complex[] t = new AP.Complex[0];
        AP.Complex[] t2 = new AP.Complex[0];
        AP.Complex[] t3 = new AP.Complex[0];
        int i = 0;
        AP.Complex[,] a1 = new AP.Complex[0,0];
        int i_ = 0;

        if( !isupper )
        {
            a1 = new AP.Complex[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a1[i,j] = a[i-1,j-1];
                }
            }
            result = hermitiancholeskydecomposition(ref a1, n, isupper);
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a[i-1,j-1] = a1[i,j];
                }
            }
            return result;
        }
        t = new AP.Complex[n-1+1];
        t2 = new AP.Complex[n-1+1];
        t3 = new AP.Complex[n-1+1];
        result = true;
        if( n<0 )
        {
            result = false;
            return result;
        }
        if( n==0 )
        {
            return result;
        }
        if( isupper )
        {
            for(j=0; j<=n-1; j++)
            {
                v = 0.0;
                for(i_=0; i_<=j-1;i_++)
                {
                    v += AP.Math.Conj(a[i_,j])*a[i_,j];
                }
                ajj = (a[j,j]-v).x;
                if( ajj<=0 )
                {
                    a[j,j] = ajj;
                    result = false;
                    return result;
                }
                ajj = Math.Sqrt(ajj);
                a[j,j] = ajj;
                if( j<n-1 )
                {
                    for(i_=0; i_<=j-1;i_++)
                    {
                        t2[i_] = AP.Math.Conj(a[i_,j]);
                    }
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        t3[i_] = a[j,i_];
                    }
                    cblas.complexmatrixvectormultiply(ref a, 0, j-1, j+1, n-1, true, false, ref t2, 0, j-1, -1.0, ref t3, j+1, n-1, 1.0, ref t);
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        a[j,i_] = t3[i_];
                    }
                    r = 1/ajj;
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        a[j,i_] = r*a[j,i_];
                    }
                }
            }
        }
        else
        {
            for(j=0; j<=n-1; j++)
            {
                v = 0.0;
                for(i_=0; i_<=j-1;i_++)
                {
                    v += AP.Math.Conj(a[j,i_])*a[j,i_];
                }
                ajj = (a[j,j]-v).x;
                if( ajj<=0 )
                {
                    a[j,j] = ajj;
                    result = false;
                    return result;
                }
                ajj = Math.Sqrt(ajj);
                a[j,j] = ajj;
                if( j<n-1 )
                {
                    for(i_=0; i_<=j-1;i_++)
                    {
                        t2[i_] = AP.Math.Conj(a[j,i_]);
                    }
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        t3[i_] = a[i_,j];
                    }
                    cblas.complexmatrixvectormultiply(ref a, j+1, n-1, 0, j-1, false, false, ref t2, 0, j-1, -1.0, ref t3, j+1, n-1, 1.0, ref t);
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        a[i_,j] = t3[i_];
                    }
                    r = 1/ajj;
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        a[i_,j] = r*a[i_,j];
                    }
                }
            }
        }
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static bool hermitiancholeskydecomposition(ref AP.Complex[,] a,
        int n,
        bool isupper)
    {
        bool result = new bool();
        int j = 0;
        double ajj = 0;
        AP.Complex v = 0;
        double r = 0;
        AP.Complex[] t = new AP.Complex[0];
        AP.Complex[] t2 = new AP.Complex[0];
        AP.Complex[] t3 = new AP.Complex[0];
        int i_ = 0;

        t = new AP.Complex[n+1];
        t2 = new AP.Complex[n+1];
        t3 = new AP.Complex[n+1];
        result = true;
        if( n<0 )
        {
            result = false;
            return result;
        }
        if( n==0 )
        {
            return result;
        }
        if( isupper )
        {
            for(j=1; j<=n; j++)
            {
                v = 0.0;
                for(i_=1; i_<=j-1;i_++)
                {
                    v += AP.Math.Conj(a[i_,j])*a[i_,j];
                }
                ajj = (a[j,j]-v).x;
                if( ajj<=0 )
                {
                    a[j,j] = ajj;
                    result = false;
                    return result;
                }
                ajj = Math.Sqrt(ajj);
                a[j,j] = ajj;
                if( j<n )
                {
                    for(i_=1; i_<=j-1;i_++)
                    {
                        a[i_,j] = AP.Math.Conj(a[i_,j]);
                    }
                    for(i_=1; i_<=j-1;i_++)
                    {
                        t2[i_] = a[i_,j];
                    }
                    for(i_=j+1; i_<=n;i_++)
                    {
                        t3[i_] = a[j,i_];
                    }
                    cblas.complexmatrixvectormultiply(ref a, 1, j-1, j+1, n, true, false, ref t2, 1, j-1, -1.0, ref t3, j+1, n, 1.0, ref t);
                    for(i_=j+1; i_<=n;i_++)
                    {
                        a[j,i_] = t3[i_];
                    }
                    for(i_=1; i_<=j-1;i_++)
                    {
                        a[i_,j] = AP.Math.Conj(a[i_,j]);
                    }
                    r = 1/ajj;
                    for(i_=j+1; i_<=n;i_++)
                    {
                        a[j,i_] = r*a[j,i_];
                    }
                }
            }
        }
        else
        {
            for(j=1; j<=n; j++)
            {
                v = 0.0;
                for(i_=1; i_<=j-1;i_++)
                {
                    v += AP.Math.Conj(a[j,i_])*a[j,i_];
                }
                ajj = (a[j,j]-v).x;
                if( ajj<=0 )
                {
                    a[j,j] = ajj;
                    result = false;
                    return result;
                }
                ajj = Math.Sqrt(ajj);
                a[j,j] = ajj;
                if( j<n )
                {
                    for(i_=1; i_<=j-1;i_++)
                    {
                        a[j,i_] = AP.Math.Conj(a[j,i_]);
                    }
                    for(i_=1; i_<=j-1;i_++)
                    {
                        t2[i_] = a[j,i_];
                    }
                    for(i_=j+1; i_<=n;i_++)
                    {
                        t3[i_] = a[i_,j];
                    }
                    cblas.complexmatrixvectormultiply(ref a, j+1, n, 1, j-1, false, false, ref t2, 1, j-1, -1.0, ref t3, j+1, n, 1.0, ref t);
                    for(i_=j+1; i_<=n;i_++)
                    {
                        a[i_,j] = t3[i_];
                    }
                    for(i_=1; i_<=j-1;i_++)
                    {
                        a[j,i_] = AP.Math.Conj(a[j,i_]);
                    }
                    r = 1/ajj;
                    for(i_=j+1; i_<=n;i_++)
                    {
                        a[i_,j] = r*a[i_,j];
                    }
                }
            }
        }
        return result;
    }
}
