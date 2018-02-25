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

class spdinverse
{
    /*************************************************************************
    Inversion of a symmetric positive definite matrix which is given
    by Cholesky decomposition.

    Input parameters:
        A       -   Cholesky decomposition of the matrix to be inverted:
                    A=U’*U or A = L*L'.
                    Output of  CholeskyDecomposition subroutine.
                    Array with elements [0..N-1, 0..N-1].
        N       -   size of matrix A.
        IsUpper –   storage format.
                    If IsUpper = True, then matrix A is given as A = U'*U
                    (matrix contains upper triangle).
                    Similarly, if IsUpper = False, then A = L*L'.

    Output parameters:
        A       -   upper or lower triangle of symmetric matrix A^-1, depending
                    on the value of IsUpper.

    Result:
        True, if the inversion succeeded.
        False, if matrix A contains zero elements on its main diagonal.
        Matrix A could not be inverted.

    The algorithm is the modification of DPOTRI and DLAUU2 subroutines from
    LAPACK library.
    *************************************************************************/
    public static bool spdmatrixcholeskyinverse(ref double[,] a,
        int n,
        bool isupper)
    {
        bool result = new bool();
        int i = 0;
        int j = 0;
        int k = 0;
        double v = 0;
        double ajj = 0;
        double aii = 0;
        double[] t = new double[0];
        double[,] a1 = new double[0,0];
        int i_ = 0;

        result = true;
        
        //
        // Test the input parameters.
        //
        t = new double[n-1+1];
        if( isupper )
        {
            
            //
            // Compute inverse of upper triangular matrix.
            //
            for(j=0; j<=n-1; j++)
            {
                if( a[j,j]==0 )
                {
                    result = false;
                    return result;
                }
                a[j,j] = 1/a[j,j];
                ajj = -a[j,j];
                
                //
                // Compute elements 1:j-1 of j-th column.
                //
                for(i_=0; i_<=j-1;i_++)
                {
                    t[i_] = a[i_,j];
                }
                for(i=0; i<=j-1; i++)
                {
                    v = 0.0;
                    for(i_=i; i_<=j-1;i_++)
                    {
                        v += a[i,i_]*t[i_];
                    }
                    a[i,j] = v;
                }
                for(i_=0; i_<=j-1;i_++)
                {
                    a[i_,j] = ajj*a[i_,j];
                }
            }
            
            //
            // InvA = InvU * InvU'
            //
            for(i=0; i<=n-1; i++)
            {
                aii = a[i,i];
                if( i<n-1 )
                {
                    v = 0.0;
                    for(i_=i; i_<=n-1;i_++)
                    {
                        v += a[i,i_]*a[i,i_];
                    }
                    a[i,i] = v;
                    for(k=0; k<=i-1; k++)
                    {
                        v = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            v += a[k,i_]*a[i,i_];
                        }
                        a[k,i] = a[k,i]*aii+v;
                    }
                }
                else
                {
                    for(i_=0; i_<=i;i_++)
                    {
                        a[i_,i] = aii*a[i_,i];
                    }
                }
            }
        }
        else
        {
            
            //
            // Compute inverse of lower triangular matrix.
            //
            for(j=n-1; j>=0; j--)
            {
                if( a[j,j]==0 )
                {
                    result = false;
                    return result;
                }
                a[j,j] = 1/a[j,j];
                ajj = -a[j,j];
                if( j<n-1 )
                {
                    
                    //
                    // Compute elements j+1:n of j-th column.
                    //
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        t[i_] = a[i_,j];
                    }
                    for(i=j+1+1; i<=n; i++)
                    {
                        v = 0.0;
                        for(i_=j+1; i_<=i-1;i_++)
                        {
                            v += a[i-1,i_]*t[i_];
                        }
                        a[i-1,j] = v;
                    }
                    for(i_=j+1; i_<=n-1;i_++)
                    {
                        a[i_,j] = ajj*a[i_,j];
                    }
                }
            }
            
            //
            // InvA = InvL' * InvL
            //
            for(i=0; i<=n-1; i++)
            {
                aii = a[i,i];
                if( i<n-1 )
                {
                    v = 0.0;
                    for(i_=i; i_<=n-1;i_++)
                    {
                        v += a[i_,i]*a[i_,i];
                    }
                    a[i,i] = v;
                    for(k=0; k<=i-1; k++)
                    {
                        v = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            v += a[i_,k]*a[i_,i];
                        }
                        a[i,k] = aii*a[i,k]+v;
                    }
                }
                else
                {
                    for(i_=0; i_<=i;i_++)
                    {
                        a[i,i_] = aii*a[i,i_];
                    }
                }
            }
        }
        return result;
    }


    /*************************************************************************
    Inversion of a symmetric positive definite matrix.

    Given an upper or lower triangle of a symmetric positive definite matrix,
    the algorithm generates matrix A^-1 and saves the upper or lower triangle
    depending on the input.

    Input parameters:
        A       -   matrix to be inverted (upper or lower triangle).
                    Array with elements [0..N-1,0..N-1].
        N       -   size of matrix A.
        IsUpper -   storage format.
                    If IsUpper = True, then the upper triangle of matrix A is
                    given, otherwise the lower triangle is given.

    Output parameters:
        A       -   inverse of matrix A.
                    Array with elements [0..N-1,0..N-1].
                    If IsUpper = True, then the upper triangle of matrix A^-1
                    is used, and the elements below the main diagonal are not
                    used nor changed. The same applies if IsUpper = False.

    Result:
        True, if the matrix is positive definite.
        False, if the matrix is not positive definite (and it could not be
        inverted by this algorithm).
    *************************************************************************/
    public static bool spdmatrixinverse(ref double[,] a,
        int n,
        bool isupper)
    {
        bool result = new bool();

        result = false;
        if( cholesky.spdmatrixcholesky(ref a, n, isupper) )
        {
            if( spdmatrixcholeskyinverse(ref a, n, isupper) )
            {
                result = true;
            }
        }
        return result;
    }


    /*************************************************************************
    Obsolete subroutine.
    *************************************************************************/
    public static bool inversecholesky(ref double[,] a,
        int n,
        bool isupper)
    {
        bool result = new bool();
        int i = 0;
        int j = 0;
        int k = 0;
        int nmj = 0;
        int jm1 = 0;
        int jp1 = 0;
        int ip1 = 0;
        double v = 0;
        double ajj = 0;
        double aii = 0;
        double[] t = new double[0];
        double[] d = new double[0];
        int i_ = 0;

        result = true;
        
        //
        // Test the input parameters.
        //
        t = new double[n+1];
        d = new double[n+1];
        if( isupper )
        {
            
            //
            // Compute inverse of upper triangular matrix.
            //
            for(j=1; j<=n; j++)
            {
                if( a[j,j]==0 )
                {
                    result = false;
                    return result;
                }
                jm1 = j-1;
                a[j,j] = 1/a[j,j];
                ajj = -a[j,j];
                
                //
                // Compute elements 1:j-1 of j-th column.
                //
                for(i_=1; i_<=jm1;i_++)
                {
                    t[i_] = a[i_,j];
                }
                for(i=1; i<=j-1; i++)
                {
                    v = 0.0;
                    for(i_=i; i_<=jm1;i_++)
                    {
                        v += a[i,i_]*a[i_,j];
                    }
                    a[i,j] = v;
                }
                for(i_=1; i_<=jm1;i_++)
                {
                    a[i_,j] = ajj*a[i_,j];
                }
            }
            
            //
            // InvA = InvU * InvU'
            //
            for(i=1; i<=n; i++)
            {
                aii = a[i,i];
                if( i<n )
                {
                    v = 0.0;
                    for(i_=i; i_<=n;i_++)
                    {
                        v += a[i,i_]*a[i,i_];
                    }
                    a[i,i] = v;
                    ip1 = i+1;
                    for(k=1; k<=i-1; k++)
                    {
                        v = 0.0;
                        for(i_=ip1; i_<=n;i_++)
                        {
                            v += a[k,i_]*a[i,i_];
                        }
                        a[k,i] = a[k,i]*aii+v;
                    }
                }
                else
                {
                    for(i_=1; i_<=i;i_++)
                    {
                        a[i_,i] = aii*a[i_,i];
                    }
                }
            }
        }
        else
        {
            
            //
            // Compute inverse of lower triangular matrix.
            //
            for(j=n; j>=1; j--)
            {
                if( a[j,j]==0 )
                {
                    result = false;
                    return result;
                }
                a[j,j] = 1/a[j,j];
                ajj = -a[j,j];
                if( j<n )
                {
                    
                    //
                    // Compute elements j+1:n of j-th column.
                    //
                    nmj = n-j;
                    jp1 = j+1;
                    for(i_=jp1; i_<=n;i_++)
                    {
                        t[i_] = a[i_,j];
                    }
                    for(i=j+1; i<=n; i++)
                    {
                        v = 0.0;
                        for(i_=jp1; i_<=i;i_++)
                        {
                            v += a[i,i_]*t[i_];
                        }
                        a[i,j] = v;
                    }
                    for(i_=jp1; i_<=n;i_++)
                    {
                        a[i_,j] = ajj*a[i_,j];
                    }
                }
            }
            
            //
            // InvA = InvL' * InvL
            //
            for(i=1; i<=n; i++)
            {
                aii = a[i,i];
                if( i<n )
                {
                    v = 0.0;
                    for(i_=i; i_<=n;i_++)
                    {
                        v += a[i_,i]*a[i_,i];
                    }
                    a[i,i] = v;
                    ip1 = i+1;
                    for(k=1; k<=i-1; k++)
                    {
                        v = 0.0;
                        for(i_=ip1; i_<=n;i_++)
                        {
                            v += a[i_,k]*a[i_,i];
                        }
                        a[i,k] = aii*a[i,k]+v;
                    }
                }
                else
                {
                    for(i_=1; i_<=i;i_++)
                    {
                        a[i,i_] = aii*a[i,i_];
                    }
                }
            }
        }
        return result;
    }


    /*************************************************************************
    Obsolete subroutine.
    *************************************************************************/
    public static bool inversesymmetricpositivedefinite(ref double[,] a,
        int n,
        bool isupper)
    {
        bool result = new bool();

        result = false;
        if( cholesky.choleskydecomposition(ref a, n, isupper) )
        {
            if( inversecholesky(ref a, n, isupper) )
            {
                result = true;
            }
        }
        return result;
    }


    private static void testinversecholesky()
    {
        double[,] l = new double[0,0];
        double[,] a = new double[0,0];
        double[,] inva = new double[0,0];
        int n = 0;
        int pass = 0;
        int passcount = 0;
        int i = 0;
        int j = 0;
        int minij = 0;
        bool upperin = new bool();
        bool cr = new bool();
        double err = 0;
        double v = 0;
        int i_ = 0;

        err = 0;
        passcount = 100;
        for(pass=1; pass<=passcount; pass++)
        {
            n = 1+AP.Math.RandomInteger(50);
            upperin = AP.Math.RandomReal()>0.5;
            l = new double[n+1, n+1];
            a = new double[n+1, n+1];
            inva = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=i+1; j<=n; j++)
                {
                    l[i,j] = AP.Math.RandomReal();
                    l[j,i] = l[i,j];
                }
                l[i,i] = 1.1+AP.Math.RandomReal();
            }
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    minij = Math.Min(i, j);
                    v = 0.0;
                    for(i_=1; i_<=minij;i_++)
                    {
                        v += l[i,i_]*l[i_,j];
                    }
                    a[i,j] = v;
                    a[j,i] = v;
                    inva[i,j] = v;
                    inva[j,i] = v;
                }
            }
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    if( upperin )
                    {
                        if( j<i )
                        {
                            inva[i,j] = 0;
                        }
                    }
                    else
                    {
                        if( i<j )
                        {
                            inva[i,j] = 0;
                        }
                    }
                }
            }
            cr = inversesymmetricpositivedefinite(ref inva, n, upperin);
            System.Diagnostics.Debug.Assert(cr, "Something strange");
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    if( upperin )
                    {
                        if( j<i )
                        {
                            inva[i,j] = inva[j,i];
                        }
                    }
                    else
                    {
                        if( i<j )
                        {
                            inva[i,j] = inva[j,i];
                        }
                    }
                }
            }
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    v = 0.0;
                    for(i_=1; i_<=n;i_++)
                    {
                        v += a[i,i_]*inva[i_,j];
                    }
                    if( j==i )
                    {
                        err = Math.Max(err, Math.Abs(v-1));
                    }
                    else
                    {
                        err = Math.Max(err, Math.Abs(v));
                    }
                }
            }
        }
        System.Console.Write("Pass count ");
        System.Console.Write("{0,0:d}",passcount);
        System.Console.WriteLine();
        System.Console.Write("InverseSymmetricPositiveDefinite error is ");
        System.Console.Write("{0,5:E3}",err);
        System.Console.WriteLine();
    }
}
