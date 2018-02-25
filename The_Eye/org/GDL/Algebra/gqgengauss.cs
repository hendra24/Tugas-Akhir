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

class gqgengauss
{
    /*************************************************************************
    Computation of nodes and weights for a Gauss quadrature formula

    The algorithm generates the N-point Gauss quadrature formula with weighting
    function given by coefficients alpha and beta  of  a  recurrence  relation
    which generates a system of orthogonal polynomials:

    P-1(x)   =  0
    P0(x)    =  1
    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

    and zero moment Mu0

    Mu0 = integral(W(x)dx,a,b)


    Input parameters:
        Alpha   –   array of coefficients.
                    Array whose index ranges within [0..N-1].
        Beta    –   array of coefficients.
                    Array whose index ranges within [0..N-1].
                    Zero-indexed element is not used and may be arbitrary.
                    Beta[I]>0.
        Mu0     –   zeroth moment of the weighting function.
        N       –   number of nodes of the quadrature formula, N>=1

    Output parameters:
        X       –   array of nodes of the quadrature formula (in ascending order).
                    Array whose index ranges within [0..N-1].
        W       –   array of weights of the quadrature formula.
                    Array whose index ranges within [0..N-1].

    Result:
        True, if the algorithm finished its work successfully.
        False, if the algorithm of finding the eigenvalues has not converged,
        or one or more of the Beta coefficients is less or equal to 0.

      -- ALGLIB --
         Copyright 2005 by Bochkanov Sergey
    *************************************************************************/
    public static bool generategaussquadrature(ref double[] alpha,
        ref double[] beta,
        double mu0,
        int n,
        ref double[] x,
        ref double[] w)
    {
        bool result = new bool();
        int i = 0;
        double[] d = new double[0];
        double[] e = new double[0];
        double[,] z = new double[0,0];

        if( n<1 )
        {
            result = false;
            return result;
        }
        
        //
        // Initialize
        //
        d = new double[n+1];
        e = new double[n+1];
        for(i=1; i<=n-1; i++)
        {
            d[i] = alpha[i-1];
            if( beta[i]<=0 )
            {
                result = false;
                return result;
            }
            e[i] = Math.Sqrt(beta[i]);
        }
        d[n] = alpha[n-1];
        
        //
        // EVD
        //
        result = tdevd.tridiagonalevd(ref d, e, n, 3, ref z);
        if( !result )
        {
            return result;
        }
        
        //
        // Generate
        //
        x = new double[n-1+1];
        w = new double[n-1+1];
        for(i=1; i<=n; i++)
        {
            x[i-1] = d[i];
            w[i-1] = mu0*AP.Math.Sqr(z[1,i]);
        }
        return result;
    }


    private static void testgenerategaussquadrature()
    {
        double[] alpha = new double[0];
        double[] beta = new double[0];
        double[] x = new double[0];
        double[] w = new double[0];
        double err = 0;
        int n = 0;
        int i = 0;

        System.Console.Write("TESTING GAUSS QUADRATURES GENERATION");
        System.Console.WriteLine();
        
        //
        // first test
        //
        alpha = new double[1+1];
        beta = new double[1+1];
        alpha[0] = 0;
        alpha[1] = 0;
        beta[1] = (double)(1)/((double)(4*1*1-1));
        generategaussquadrature(ref alpha, ref beta, 2.0, 2, ref x, ref w);
        err = 0;
        err = Math.Max(err, Math.Abs(x[0]+Math.Sqrt(3)/3));
        err = Math.Max(err, Math.Abs(x[1]-Math.Sqrt(3)/3));
        err = Math.Max(err, Math.Abs(w[0]-1));
        err = Math.Max(err, Math.Abs(w[1]-1));
        System.Console.Write("First test (Gauss-Legendre 2): error is ");
        System.Console.Write("{0,6:E3}",err);
        System.Console.WriteLine();
        
        //
        // second test
        //
        alpha = new double[4+1];
        beta = new double[4+1];
        alpha[0] = 0;
        alpha[1] = 0;
        alpha[2] = 0;
        alpha[3] = 0;
        alpha[4] = 0;
        for(i=1; i<=4; i++)
        {
            beta[i] = AP.Math.Sqr(i)/(4*AP.Math.Sqr(i)-1);
        }
        generategaussquadrature(ref alpha, ref beta, 2.0, 5, ref x, ref w);
        err = 0;
        err = Math.Max(err, Math.Abs(x[0]+Math.Sqrt(245+14*Math.Sqrt(70))/21));
        err = Math.Max(err, Math.Abs(x[0]+x[4]));
        err = Math.Max(err, Math.Abs(x[1]+Math.Sqrt(245-14*Math.Sqrt(70))/21));
        err = Math.Max(err, Math.Abs(x[1]+x[3]));
        err = Math.Max(err, Math.Abs(x[2]));
        err = Math.Max(err, Math.Abs(w[0]-(322-13*Math.Sqrt(70))/900));
        err = Math.Max(err, Math.Abs(w[0]-w[4]));
        err = Math.Max(err, Math.Abs(w[1]-(322+13*Math.Sqrt(70))/900));
        err = Math.Max(err, Math.Abs(w[1]-w[3]));
        err = Math.Max(err, Math.Abs(w[2]-(double)(128)/(double)(225)));
        System.Console.Write("Second test (Gauss-Legendre 5): error is ");
        System.Console.Write("{0,6:E3}",err);
        System.Console.WriteLine();
        
        //
        // chebyshev test 1, 10, 100
        //
        n = 1;
        while( n<=512 )
        {
            alpha = new double[n-1+1];
            beta = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                alpha[i] = 0;
                if( i==0 )
                {
                    beta[i] = 0;
                }
                if( i==1 )
                {
                    beta[i] = (double)(1)/(double)(2);
                }
                if( i>1 )
                {
                    beta[i] = (double)(1)/(double)(4);
                }
            }
            generategaussquadrature(ref alpha, ref beta, Math.PI, n, ref x, ref w);
            err = 0;
            for(i=0; i<=n-1; i++)
            {
                err = Math.Max(err, Math.Abs(x[i]-Math.Cos(Math.PI*(n-i-0.5)/n)));
                err = Math.Max(err, Math.Abs(w[i]-Math.PI/n));
            }
            System.Console.Write("Gauss-Chebyshev test (N=");
            System.Console.Write("{0,0:d}",n);
            System.Console.Write("): error is ");
            System.Console.Write("{0,6:E3}",err);
            System.Console.WriteLine();
            n = n*2;
        }
    }
}
