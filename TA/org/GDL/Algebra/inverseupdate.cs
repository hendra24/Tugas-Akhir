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

class inverseupdate
{
    /*************************************************************************
    Inverse matrix update by the Sherman-Morrison formula

    The algorithm updates matrix A^-1 when adding a number to an element
    of matrix A.

    Input parameters:
        InvA    -   inverse of matrix A.
                    Array whose indexes range within [0..N-1, 0..N-1].
        N       -   size of matrix A.
        UpdRow  -   row where the element to be updated is stored.
        UpdColumn - column where the element to be updated is stored.
        UpdVal  -   a number to be added to the element.


    Output parameters:
        InvA    -   inverse of modified matrix A.

      -- ALGLIB --
         Copyright 2005 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixinvupdatesimple(ref double[,] inva,
        int n,
        int updrow,
        int updcolumn,
        double updval)
    {
        double[] t1 = new double[0];
        double[] t2 = new double[0];
        int i = 0;
        //int j = 0;
        double lambda = 0;
        double vt = 0;
        int i_ = 0;

        System.Diagnostics.Debug.Assert(updrow>=0 & updrow<n, "RMatrixInvUpdateSimple: incorrect UpdRow!");
        System.Diagnostics.Debug.Assert(updcolumn>=0 & updcolumn<n, "RMatrixInvUpdateSimple: incorrect UpdColumn!");
        t1 = new double[n-1+1];
        t2 = new double[n-1+1];
        
        //
        // T1 = InvA * U
        //
        for(i_=0; i_<=n-1;i_++)
        {
            t1[i_] = inva[i_,updrow];
        }
        
        //
        // T2 = v*InvA
        //
        for(i_=0; i_<=n-1;i_++)
        {
            t2[i_] = inva[updcolumn,i_];
        }
        
        //
        // Lambda = v * InvA * U
        //
        lambda = updval*inva[updcolumn,updrow];
        
        //
        // InvA = InvA - correction
        //
        for(i=0; i<=n-1; i++)
        {
            vt = updval*t1[i];
            vt = vt/(1+lambda);
            for(i_=0; i_<=n-1;i_++)
            {
                inva[i,i_] = inva[i,i_] - vt*t2[i_];
            }
        }
    }


    /*************************************************************************
    Inverse matrix update by the Sherman-Morrison formula

    The algorithm updates matrix A^-1 when adding a vector to a row
    of matrix A.

    Input parameters:
        InvA    -   inverse of matrix A.
                    Array whose indexes range within [0..N-1, 0..N-1].
        N       -   size of matrix A.
        UpdRow  -   the row of A whose vector V was added.
                    0 <= Row <= N-1
        V       -   the vector to be added to a row.
                    Array whose index ranges within [0..N-1].

    Output parameters:
        InvA    -   inverse of modified matrix A.

      -- ALGLIB --
         Copyright 2005 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixinvupdaterow(ref double[,] inva,
        int n,
        int updrow,
        ref double[] v)
    {
        double[] t1 = new double[0];
        double[] t2 = new double[0];
        int i = 0;
        int j = 0;
        double lambda = 0;
        double vt = 0;
        int i_ = 0;

        t1 = new double[n-1+1];
        t2 = new double[n-1+1];
        
        //
        // T1 = InvA * U
        //
        for(i_=0; i_<=n-1;i_++)
        {
            t1[i_] = inva[i_,updrow];
        }
        
        //
        // T2 = v*InvA
        // Lambda = v * InvA * U
        //
        for(j=0; j<=n-1; j++)
        {
            vt = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                vt += v[i_]*inva[i_,j];
            }
            t2[j] = vt;
        }
        lambda = t2[updrow];
        
        //
        // InvA = InvA - correction
        //
        for(i=0; i<=n-1; i++)
        {
            vt = t1[i]/(1+lambda);
            for(i_=0; i_<=n-1;i_++)
            {
                inva[i,i_] = inva[i,i_] - vt*t2[i_];
            }
        }
    }


    /*************************************************************************
    Inverse matrix update by the Sherman-Morrison formula

    The algorithm updates matrix A^-1 when adding a vector to a column
    of matrix A.

    Input parameters:
        InvA        -   inverse of matrix A.
                        Array whose indexes range within [0..N-1, 0..N-1].
        N           -   size of matrix A.
        UpdColumn   -   the column of A whose vector U was added.
                        0 <= UpdColumn <= N-1
        U           -   the vector to be added to a column.
                        Array whose index ranges within [0..N-1].

    Output parameters:
        InvA        -   inverse of modified matrix A.

      -- ALGLIB --
         Copyright 2005 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixinvupdatecolumn(ref double[,] inva,
        int n,
        int updcolumn,
        ref double[] u)
    {
        double[] t1 = new double[0];
        double[] t2 = new double[0];
        int i = 0;
        //int j = 0;
        double lambda = 0;
        double vt = 0;
        int i_ = 0;

        t1 = new double[n-1+1];
        t2 = new double[n-1+1];
        
        //
        // T1 = InvA * U
        // Lambda = v * InvA * U
        //
        for(i=0; i<=n-1; i++)
        {
            vt = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                vt += inva[i,i_]*u[i_];
            }
            t1[i] = vt;
        }
        lambda = t1[updcolumn];
        
        //
        // T2 = v*InvA
        //
        for(i_=0; i_<=n-1;i_++)
        {
            t2[i_] = inva[updcolumn,i_];
        }
        
        //
        // InvA = InvA - correction
        //
        for(i=0; i<=n-1; i++)
        {
            vt = t1[i]/(1+lambda);
            for(i_=0; i_<=n-1;i_++)
            {
                inva[i,i_] = inva[i,i_] - vt*t2[i_];
            }
        }
    }


    /*************************************************************************
    Inverse matrix update by the Sherman-Morrison formula

    The algorithm computes the inverse of matrix A+u*v’ by using the given matrix
    A^-1 and the vectors u and v.

    Input parameters:
        InvA    -   inverse of matrix A.
                    Array whose indexes range within [0..N-1, 0..N-1].
        N       -   size of matrix A.
        U       -   the vector modifying the matrix.
                    Array whose index ranges within [0..N-1].
        V       -   the vector modifying the matrix.
                    Array whose index ranges within [0..N-1].

    Output parameters:
        InvA - inverse of matrix A + u*v'.

      -- ALGLIB --
         Copyright 2005 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixinvupdateuv(ref double[,] inva,
        int n,
        ref double[] u,
        ref double[] v)
    {
        double[] t1 = new double[0];
        double[] t2 = new double[0];
        int i = 0;
        int j = 0;
        double lambda = 0;
        double vt = 0;
        int i_ = 0;

        t1 = new double[n-1+1];
        t2 = new double[n-1+1];
        
        //
        // T1 = InvA * U
        // Lambda = v * T1
        //
        for(i=0; i<=n-1; i++)
        {
            vt = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                vt += inva[i,i_]*u[i_];
            }
            t1[i] = vt;
        }
        lambda = 0.0;
        for(i_=0; i_<=n-1;i_++)
        {
            lambda += v[i_]*t1[i_];
        }
        
        //
        // T2 = v*InvA
        //
        for(j=0; j<=n-1; j++)
        {
            vt = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                vt += v[i_]*inva[i_,j];
            }
            t2[j] = vt;
        }
        
        //
        // InvA = InvA - correction
        //
        for(i=0; i<=n-1; i++)
        {
            vt = t1[i]/(1+lambda);
            for(i_=0; i_<=n-1;i_++)
            {
                inva[i,i_] = inva[i,i_] - vt*t2[i_];
            }
        }
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static void shermanmorrisonsimpleupdate(ref double[,] inva,
        int n,
        int updrow,
        int updcolumn,
        double updval)
    {
        double[] t1 = new double[0];
        double[] t2 = new double[0];
        int i = 0;
        //int j = 0;
        double lambda = 0;
        double vt = 0;
        int i_ = 0;

        t1 = new double[n+1];
        t2 = new double[n+1];
        
        //
        // T1 = InvA * U
        //
        for(i_=1; i_<=n;i_++)
        {
            t1[i_] = inva[i_,updrow];
        }
        
        //
        // T2 = v*InvA
        //
        for(i_=1; i_<=n;i_++)
        {
            t2[i_] = inva[updcolumn,i_];
        }
        
        //
        // Lambda = v * InvA * U
        //
        lambda = updval*inva[updcolumn,updrow];
        
        //
        // InvA = InvA - correction
        //
        for(i=1; i<=n; i++)
        {
            vt = updval*t1[i];
            vt = vt/(1+lambda);
            for(i_=1; i_<=n;i_++)
            {
                inva[i,i_] = inva[i,i_] - vt*t2[i_];
            }
        }
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static void shermanmorrisonupdaterow(ref double[,] inva,
        int n,
        int updrow,
        ref double[] v)
    {
        double[] t1 = new double[0];
        double[] t2 = new double[0];
        int i = 0;
        int j = 0;
        double lambda = 0;
        double vt = 0;
        int i_ = 0;

        t1 = new double[n+1];
        t2 = new double[n+1];
        
        //
        // T1 = InvA * U
        //
        for(i_=1; i_<=n;i_++)
        {
            t1[i_] = inva[i_,updrow];
        }
        
        //
        // T2 = v*InvA
        // Lambda = v * InvA * U
        //
        for(j=1; j<=n; j++)
        {
            vt = 0.0;
            for(i_=1; i_<=n;i_++)
            {
                vt += v[i_]*inva[i_,j];
            }
            t2[j] = vt;
        }
        lambda = t2[updrow];
        
        //
        // InvA = InvA - correction
        //
        for(i=1; i<=n; i++)
        {
            vt = t1[i]/(1+lambda);
            for(i_=1; i_<=n;i_++)
            {
                inva[i,i_] = inva[i,i_] - vt*t2[i_];
            }
        }
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static void shermanmorrisonupdatecolumn(ref double[,] inva,
        int n,
        int updcolumn,
        ref double[] u)
    {
        double[] t1 = new double[0];
        double[] t2 = new double[0];
        int i = 0;
        //int j = 0;
        double lambda = 0;
        double vt = 0;
        int i_ = 0;

        t1 = new double[n+1];
        t2 = new double[n+1];
        
        //
        // T1 = InvA * U
        // Lambda = v * InvA * U
        //
        for(i=1; i<=n; i++)
        {
            vt = 0.0;
            for(i_=1; i_<=n;i_++)
            {
                vt += inva[i,i_]*u[i_];
            }
            t1[i] = vt;
        }
        lambda = t1[updcolumn];
        
        //
        // T2 = v*InvA
        //
        for(i_=1; i_<=n;i_++)
        {
            t2[i_] = inva[updcolumn,i_];
        }
        
        //
        // InvA = InvA - correction
        //
        for(i=1; i<=n; i++)
        {
            vt = t1[i]/(1+lambda);
            for(i_=1; i_<=n;i_++)
            {
                inva[i,i_] = inva[i,i_] - vt*t2[i_];
            }
        }
    }


    /*************************************************************************
    Obsolete 1-based subroutine
    *************************************************************************/
    public static void shermanmorrisonupdateuv(ref double[,] inva,
        int n,
        ref double[] u,
        ref double[] v)
    {
        double[] t1 = new double[0];
        double[] t2 = new double[0];
        int i = 0;
        int j = 0;
        double lambda = 0;
        double vt = 0;
        int i_ = 0;

        t1 = new double[n+1];
        t2 = new double[n+1];
        
        //
        // T1 = InvA * U
        // Lambda = v * T1
        //
        for(i=1; i<=n; i++)
        {
            vt = 0.0;
            for(i_=1; i_<=n;i_++)
            {
                vt += inva[i,i_]*u[i_];
            }
            t1[i] = vt;
        }
        lambda = 0.0;
        for(i_=1; i_<=n;i_++)
        {
            lambda += v[i_]*t1[i_];
        }
        
        //
        // T2 = v*InvA
        //
        for(j=1; j<=n; j++)
        {
            vt = 0.0;
            for(i_=1; i_<=n;i_++)
            {
                vt += v[i_]*inva[i_,j];
            }
            t2[j] = vt;
        }
        
        //
        // InvA = InvA - correction
        //
        for(i=1; i<=n; i++)
        {
            vt = t1[i]/(1+lambda);
            for(i_=1; i_<=n;i_++)
            {
                inva[i,i_] = inva[i,i_] - vt*t2[i_];
            }
        }
    }
}
