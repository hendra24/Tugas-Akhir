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

class estnorm
{
    /*************************************************************************
    Matrix norm estimation

    The algorithm estimates the 1-norm of square matrix A  on  the  assumption
    that the multiplication of matrix  A  by  the  vector  is  available  (the
    iterative method is used). It is recommended to use this algorithm  if  it
    is hard  to  calculate  matrix  elements  explicitly  (for  example,  when
    estimating the inverse matrix norm).

    The algorithm uses back communication for multiplying the  vector  by  the
    matrix.  If  KASE=0  after  returning from a subroutine, its execution was
    completed successfully, otherwise it is required to multiply the  returned
    vector by matrix A and call the subroutine again.

    The DemoIterativeEstimateNorm subroutine shows a simple example.

    Parameters:
        N       -   size of matrix A.
        V       -   vector.   It is initialized by the subroutine on the first
                    call. It is then passed into it on repeated calls.
        X       -   if KASE<>0, it contains the vector to be replaced by:
                        A * X,      if KASE=1
                        A^T * X,    if KASE=2
                    Array whose index ranges within [1..N].
        ISGN    -   vector. It is initialized by the subroutine on  the  first
                    call. It is then passed into it on repeated calls.
        EST     -   if KASE=0, it contains the lower boundary of the matrix
                    norm estimate.
        KASE    -   on the first call, it should be equal to 0. After the last
                    return, it is equal to 0 (EST contains the  matrix  norm),
                    on intermediate returns it can be equal to 1 or 2 depending
                    on the operation to be performed on vector X.

      -- LAPACK auxiliary routine (version 3.0) --
         Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
         Courant Institute, Argonne National Lab, and Rice University
         February 29, 1992
    *************************************************************************/
    public static void iterativeestimate1norm(int n,
        ref double[] v,
        ref double[] x,
        ref int[] isgn,
        ref double est,
        ref int kase)
    {
        int itmax = 0;
        int i = 0;
        double t = 0;
        bool flg = new bool();
        int positer = 0;
        int posj = 0;
        int posjlast = 0;
        int posjump = 0;
        int posaltsgn = 0;
        int posestold = 0;
        int postemp = 0;
        int i_ = 0;

        itmax = 5;
        posaltsgn = n+1;
        posestold = n+2;
        postemp = n+3;
        positer = n+1;
        posj = n+2;
        posjlast = n+3;
        posjump = n+4;
        if( kase==0 )
        {
            v = new double[n+3+1];
            x = new double[n+1];
            isgn = new int[n+4+1];
            t = (double)(1)/(double)(n);
            for(i=1; i<=n; i++)
            {
                x[i] = t;
            }
            kase = 1;
            isgn[posjump] = 1;
            return;
        }
        
        //
        //     ................ ENTRY   (JUMP = 1)
        //     FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY A*X.
        //
        if( isgn[posjump]==1 )
        {
            if( n==1 )
            {
                v[1] = x[1];
                est = Math.Abs(v[1]);
                kase = 0;
                return;
            }
            est = 0;
            for(i=1; i<=n; i++)
            {
                est = est+Math.Abs(x[i]);
            }
            for(i=1; i<=n; i++)
            {
                if( x[i]>=0 )
                {
                    x[i] = 1;
                }
                else
                {
                    x[i] = -1;
                }
                isgn[i] = Math.Sign(x[i]);
            }
            kase = 2;
            isgn[posjump] = 2;
            return;
        }
        
        //
        //     ................ ENTRY   (JUMP = 2)
        //     FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY TRANDPOSE(A)*X.
        //
        if( isgn[posjump]==2 )
        {
            isgn[posj] = 1;
            for(i=2; i<=n; i++)
            {
                if( Math.Abs(x[i])>Math.Abs(x[isgn[posj]]) )
                {
                    isgn[posj] = i;
                }
            }
            isgn[positer] = 2;
            
            //
            // MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
            //
            for(i=1; i<=n; i++)
            {
                x[i] = 0;
            }
            x[isgn[posj]] = 1;
            kase = 1;
            isgn[posjump] = 3;
            return;
        }
        
        //
        //     ................ ENTRY   (JUMP = 3)
        //     X HAS BEEN OVERWRITTEN BY A*X.
        //
        if( isgn[posjump]==3 )
        {
            for(i_=1; i_<=n;i_++)
            {
                v[i_] = x[i_];
            }
            v[posestold] = est;
            est = 0;
            for(i=1; i<=n; i++)
            {
                est = est+Math.Abs(v[i]);
            }
            flg = false;
            for(i=1; i<=n; i++)
            {
                if( x[i]>=0 & isgn[i]<0 | x[i]<0 & isgn[i]>=0 )
                {
                    flg = true;
                }
            }
            
            //
            // REPEATED SIGN VECTOR DETECTED, HENCE ALGORITHM HAS CONVERGED.
            // OR MAY BE CYCLING.
            //
            if( !flg | est<=v[posestold] )
            {
                v[posaltsgn] = 1;
                for(i=1; i<=n; i++)
                {
                    x[i] = v[posaltsgn]*(1+((double)(i-1))/((double)(n-1)));
                    v[posaltsgn] = -v[posaltsgn];
                }
                kase = 1;
                isgn[posjump] = 5;
                return;
            }
            for(i=1; i<=n; i++)
            {
                if( x[i]>=0 )
                {
                    x[i] = 1;
                    isgn[i] = 1;
                }
                else
                {
                    x[i] = -1;
                    isgn[i] = -1;
                }
            }
            kase = 2;
            isgn[posjump] = 4;
            return;
        }
        
        //
        //     ................ ENTRY   (JUMP = 4)
        //     X HAS BEEN OVERWRITTEN BY TRANDPOSE(A)*X.
        //
        if( isgn[posjump]==4 )
        {
            isgn[posjlast] = isgn[posj];
            isgn[posj] = 1;
            for(i=2; i<=n; i++)
            {
                if( Math.Abs(x[i])>Math.Abs(x[isgn[posj]]) )
                {
                    isgn[posj] = i;
                }
            }
            if( x[isgn[posjlast]]!=Math.Abs(x[isgn[posj]]) & isgn[positer]<itmax )
            {
                isgn[positer] = isgn[positer]+1;
                for(i=1; i<=n; i++)
                {
                    x[i] = 0;
                }
                x[isgn[posj]] = 1;
                kase = 1;
                isgn[posjump] = 3;
                return;
            }
            
            //
            // ITERATION COMPLETE.  FINAL STAGE.
            //
            v[posaltsgn] = 1;
            for(i=1; i<=n; i++)
            {
                x[i] = v[posaltsgn]*(1+((double)(i-1))/((double)(n-1)));
                v[posaltsgn] = -v[posaltsgn];
            }
            kase = 1;
            isgn[posjump] = 5;
            return;
        }
        
        //
        //     ................ ENTRY   (JUMP = 5)
        //     X HAS BEEN OVERWRITTEN BY A*X.
        //
        if( isgn[posjump]==5 )
        {
            v[postemp] = 0;
            for(i=1; i<=n; i++)
            {
                v[postemp] = v[postemp]+Math.Abs(x[i]);
            }
            v[postemp] = 2*v[postemp]/(3*n);
            if( v[postemp]>est )
            {
                for(i_=1; i_<=n;i_++)
                {
                    v[i_] = x[i_];
                }
                est = v[postemp];
            }
            kase = 0;
            return;
        }
    }


    /*************************************************************************
    Example of usage of an IterativeEstimateNorm subroutine

    Input parameters:
        A   -   matrix.
                Array whose indexes range within [1..N, 1..N].

    Return:
        Matrix norm estimated by the subroutine.

      -- ALGLIB --
         Copyright 2005 by Bochkanov Sergey
    *************************************************************************/
    public static double demoiterativeestimate1norm(ref double[,] a,
        int n)
    {
        double result = 0;
        int i = 0;
        double s = 0;
        double[] x = new double[0];
        double[] t = new double[0];
        double[] v = new double[0];
        int[] iv = new int[0];
        int kase = 0;
        int i_ = 0;

        kase = 0;
        t = new double[n+1];
        iterativeestimate1norm(n, ref v, ref x, ref iv, ref result, ref kase);
        while( kase!=0 )
        {
            if( kase==1 )
            {
                for(i=1; i<=n; i++)
                {
                    s = 0.0;
                    for(i_=1; i_<=n;i_++)
                    {
                        s += a[i,i_]*x[i_];
                    }
                    t[i] = s;
                }
            }
            else
            {
                for(i=1; i<=n; i++)
                {
                    s = 0.0;
                    for(i_=1; i_<=n;i_++)
                    {
                        s += a[i_,i]*x[i_];
                    }
                    t[i] = s;
                }
            }
            for(i_=1; i_<=n;i_++)
            {
                x[i_] = t[i_];
            }
            iterativeestimate1norm(n, ref v, ref x, ref iv, ref result, ref kase);
        }
        return result;
    }


    private static void testiterativeestimatenorm()
    {
        double[,] a = new double[0,0];
        int n = 0;
        int i = 0;
        int j = 0;
        int k = 0;
        double avg = 0;
        double nrm1a = 0;
        double estnrm1a = 0;
        double v = 0;
        int pass = 0;
        int passcount = 0;
        double maxerr = 0;
        double avgerr = 0;

        passcount = 1000;
        avg = 100;
        maxerr = 0;
        avgerr = 0;
        for(pass=1; pass<=passcount; pass++)
        {
            n = 1+AP.Math.RandomInteger(50);
            a = new double[n+1, n+1];
            
            //
            // fill
            //
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a[i,j] = Math.Sqrt(avg)*(2*AP.Math.RandomReal()-1);
                    a[i,j] = AP.Math.Sqr(a[i,j]);
                }
            }
            a[1+AP.Math.RandomInteger(n),1+AP.Math.RandomInteger(n)] = 1.5*avg*(2*AP.Math.RandomReal()-1);
            
            //
            // norm A
            //
            nrm1a = 0;
            for(k=1; k<=n; k++)
            {
                v = 0;
                for(i=1; i<=n; i++)
                {
                    v = v+Math.Abs(a[i,k]);
                }
                nrm1a = Math.Max(nrm1a, v);
            }
            estnrm1a = demoiterativeestimate1norm(ref a, n);
            
            //
            // test
            //
            maxerr = Math.Max(maxerr, Math.Abs(1-estnrm1a/nrm1a));
            avgerr = avgerr+Math.Abs(1-estnrm1a/nrm1a);
        }
        System.Console.Write("TOTAL PASS COUNT ");
        System.Console.Write("{0,0:d}",passcount);
        System.Console.WriteLine();
        System.Console.Write("1-norm: avg ");
        System.Console.Write("{0,4:F2}",100*avgerr/passcount);
        System.Console.Write(" percents of exact, max ");
        System.Console.Write("{0,4:F2}",100*maxerr);
        System.Console.WriteLine();
    }
}
