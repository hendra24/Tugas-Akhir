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

class hevd
{
    /*************************************************************************
    Finding the eigenvalues and eigenvectors of a Hermitian matrix

    The algorithm finds eigen pairs of a Hermitian matrix by  reducing  it  to
    real tridiagonal form and using the QL/QR algorithm.

    Input parameters:
        A       -   Hermitian matrix which is given  by  its  upper  or  lower
                    triangular part.
                    Array whose indexes range within [0..N-1, 0..N-1].
        N       -   size of matrix A.
        IsUpper -   storage format.
        ZNeeded -   flag controlling whether the eigenvectors  are  needed  or
                    not. If ZNeeded is equal to:
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

    Note:
        eigen vectors of Hermitian matrix are defined up to multiplication  by
        a complex number L, such as |L|=1.

      -- ALGLIB --
         Copyright 2005, 23 March 2007 by Bochkanov Sergey
    *************************************************************************/
    public static bool hmatrixevd(AP.Complex[,] a,
        int n,
        int zneeded,
        bool isupper,
        ref double[] d,
        ref AP.Complex[,] z)
    {
        bool result = new bool();
        AP.Complex[] tau = new AP.Complex[0];
        double[] e = new double[0];
        double[] work = new double[0];
        double[,] t = new double[0,0];
        AP.Complex[,] q = new AP.Complex[0,0];
        int i = 0;
        int k = 0;
        double v = 0;
        int i_ = 0;

        a = (AP.Complex[,])a.Clone();

        System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "HermitianEVD: incorrect ZNeeded");
        
        //
        // Reduce to tridiagonal form
        //
        htridiagonal.hmatrixtd(ref a, n, isupper, ref tau, ref d, ref e);
        if( zneeded==1 )
        {
            htridiagonal.hmatrixtdunpackq(ref a, n, isupper, ref tau, ref q);
            zneeded = 2;
        }
        
        //
        // TDEVD
        //
        result = tdevd.smatrixtdevd(ref d, e, n, zneeded, ref t);
        
        //
        // Eigenvectors are needed
        // Calculate Z = Q*T = Re(Q)*T + i*Im(Q)*T
        //
        if( result & zneeded!=0 )
        {
            work = new double[n-1+1];
            z = new AP.Complex[n-1+1, n-1+1];
            for(i=0; i<=n-1; i++)
            {
                
                //
                // Calculate real part
                //
                for(k=0; k<=n-1; k++)
                {
                    work[k] = 0;
                }
                for(k=0; k<=n-1; k++)
                {
                    v = q[i,k].x;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        work[i_] = work[i_] + v*t[k,i_];
                    }
                }
                for(k=0; k<=n-1; k++)
                {
                    z[i,k].x = work[k];
                }
                
                //
                // Calculate imaginary part
                //
                for(k=0; k<=n-1; k++)
                {
                    work[k] = 0;
                }
                for(k=0; k<=n-1; k++)
                {
                    v = q[i,k].y;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        work[i_] = work[i_] + v*t[k,i_];
                    }
                }
                for(k=0; k<=n-1; k++)
                {
                    z[i,k].y = work[k];
                }
            }
        }
        return result;
    }


    /*************************************************************************
    Obsolete 1-based subroutine

      -- ALGLIB --
         Copyright 2005, 23 March 2007 by Bochkanov Sergey
    *************************************************************************/
    public static bool hermitianevd(AP.Complex[,] a,
        int n,
        int zneeded,
        bool isupper,
        ref double[] d,
        ref AP.Complex[,] z)
    {
        bool result = new bool();
        AP.Complex[] tau = new AP.Complex[0];
        double[] e = new double[0];
        double[] work = new double[0];
        double[,] t = new double[0,0];
        AP.Complex[,] q = new AP.Complex[0,0];
        int i = 0;
        int k = 0;
        double v = 0;
        int i_ = 0;

        a = (AP.Complex[,])a.Clone();

        System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "HermitianEVD: incorrect ZNeeded");
        
        //
        // Reduce to tridiagonal form
        //
        htridiagonal.hermitiantotridiagonal(ref a, n, isupper, ref tau, ref d, ref e);
        if( zneeded==1 )
        {
            htridiagonal.unpackqfromhermitiantridiagonal(ref a, n, isupper, ref tau, ref q);
            zneeded = 2;
        }
        
        //
        // TDEVD
        //
        result = tdevd.tridiagonalevd(ref d, e, n, zneeded, ref t);
        
        //
        // Eigenvectors are needed
        // Calculate Z = Q*T = Re(Q)*T + i*Im(Q)*T
        //
        if( result & zneeded!=0 )
        {
            work = new double[n+1];
            z = new AP.Complex[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                
                //
                // Calculate real part
                //
                for(k=1; k<=n; k++)
                {
                    work[k] = 0;
                }
                for(k=1; k<=n; k++)
                {
                    v = q[i,k].x;
                    for(i_=1; i_<=n;i_++)
                    {
                        work[i_] = work[i_] + v*t[k,i_];
                    }
                }
                for(k=1; k<=n; k++)
                {
                    z[i,k].x = work[k];
                }
                
                //
                // Calculate imaginary part
                //
                for(k=1; k<=n; k++)
                {
                    work[k] = 0;
                }
                for(k=1; k<=n; k++)
                {
                    v = q[i,k].y;
                    for(i_=1; i_<=n;i_++)
                    {
                        work[i_] = work[i_] + v*t[k,i_];
                    }
                }
                for(k=1; k<=n; k++)
                {
                    z[i,k].y = work[k];
                }
            }
        }
        return result;
    }
}
