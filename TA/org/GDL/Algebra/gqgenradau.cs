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

class gqgenradau
{
    /*************************************************************************
    Computation of nodes and weights for a Gauss-Radua quadrature formula

    The algorithm generates the N-point Gauss-Radau  quadrature  formula  with
    weight function given by the coefficients alpha and  beta  of a recurrence
    which generates a system of orthogonal polynomials.

    P-1(x)   =  0
    P0(x)    =  1
    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

    and zero moment Mu0

    Mu0 = integral(W(x)dx,a,b)


    Input parameters:
        Alpha   –   array of coefficients.
                    Array whose index ranges within [0..N-2].
        Beta    –   array of coefficients.
                    Array whose index ranges within [0..N-1].
                    Zero-indexed element is not used.
                    Beta[I]>0
        Mu0     –   zero moment of the weighting function.
        A       –   left boundary of the integration interval.
        N       –   number of nodes of the quadrature formula, N>=2
                    (including the left boundary node).

    Output parameters:
        X       –   array of nodes of the quadrature formula (in ascending order).
                    Array whose index ranges within [0..N-1].
        W       –   array of weights of the quadrature formula.
                    Array whose index ranges within [0..N-1].

    Result:
        True, if the algorithm finished its work successfully.
        False, if the algorithm of finding eigenvalues has not converged, or
    one or more of the Beta coefficients is less or equal to 0.

      -- ALGLIB --
         Copyright 2005 by Bochkanov Sergey
    *************************************************************************/
    public static bool generategaussradauquadrature(double[] alpha,
        double[] beta,
        double mu0,
        double a,
        int n,
        ref double[] x,
        ref double[] w)
    {
        bool result = new bool();
        int i = 0;
        double[] d = new double[0];
        double[] e = new double[0];
        double[,] z = new double[0,0];
        double polim1 = 0;
        double poli = 0;
        double t = 0;

        alpha = (double[])alpha.Clone();
        beta = (double[])beta.Clone();

        if( n<2 )
        {
            result = false;
            return result;
        }
        
        //
        // Initialize, D[1:N], E[1:N]
        //
        n = n-1;
        d = new double[n+1+1];
        e = new double[n+1];
        for(i=1; i<=n; i++)
        {
            d[i] = alpha[i-1];
            if( beta[i]<=0 )
            {
                result = false;
                return result;
            }
            e[i] = Math.Sqrt(beta[i]);
        }
        
        //
        // Caclulate Pn(a), Pn-1(a), and D[N+1]
        //
        beta[0] = 0;
        polim1 = 0;
        poli = 1;
        for(i=1; i<=n; i++)
        {
            t = (a-alpha[i-1])*poli-beta[i-1]*polim1;
            polim1 = poli;
            poli = t;
        }
        d[n+1] = a-beta[n]*polim1/poli;
        
        //
        // EVD
        //
        result = tdevd.tridiagonalevd(ref d, e, n+1, 3, ref z);
        if( !result )
        {
            return result;
        }
        
        //
        // Generate
        //
        x = new double[n+1];
        w = new double[n+1];
        for(i=1; i<=n+1; i++)
        {
            x[i-1] = d[i];
            w[i-1] = mu0*AP.Math.Sqr(z[1,i]);
        }
        return result;
    }


    private static void testgenerategaussradauquadrature()
    {
        double[] alpha = new double[0];
        double[] beta = new double[0];
        double[] x = new double[0];
        double[] w = new double[0];
        double err = 0;
        int i = 0;

        System.Console.Write("TESTING GAUSS-RADAU QUADRATURES GENERATION");
        System.Console.WriteLine();
        
        //
        // first test
        //
        alpha = new double[0+1];
        beta = new double[1+1];
        alpha[0] = 0;
        beta[0] = 0;
        beta[1] = (double)(1*1)/((double)(4*1*1-1));
        generategaussradauquadrature(alpha, beta, 2.0, -1, 2, ref x, ref w);
        err = 0;
        err = Math.Max(err, Math.Abs(x[0]+1));
        err = Math.Max(err, Math.Abs(x[1]-(double)(1)/(double)(3)));
        err = Math.Max(err, Math.Abs(w[0]-0.5));
        err = Math.Max(err, Math.Abs(w[1]-1.5));
        System.Console.Write("First test (Gauss-Radau 2): error is ");
        System.Console.Write("{0,6:E3}",err);
        System.Console.WriteLine();
        
        //
        // second test
        //
        alpha = new double[1+1];
        beta = new double[2+1];
        alpha[0] = 0;
        alpha[1] = 0;
        for(i=0; i<=2; i++)
        {
            beta[i] = AP.Math.Sqr(i)/(4*AP.Math.Sqr(i)-1);
        }
        generategaussradauquadrature(alpha, beta, 2.0, -1, 3, ref x, ref w);
        err = 0;
        err = Math.Max(err, Math.Abs(x[0]+1));
        err = Math.Max(err, Math.Abs(x[1]-(1-Math.Sqrt(6))/5));
        err = Math.Max(err, Math.Abs(x[2]-(1+Math.Sqrt(6))/5));
        err = Math.Max(err, Math.Abs(w[0]-(double)(2)/(double)(9)));
        err = Math.Max(err, Math.Abs(w[1]-(16+Math.Sqrt(6))/18));
        err = Math.Max(err, Math.Abs(w[2]-(16-Math.Sqrt(6))/18));
        System.Console.Write("Second test (Gauss-Radau 3): error is ");
        System.Console.Write("{0,6:E3}",err);
        System.Console.WriteLine();
        
        //
        // third test
        //
        alpha = new double[1+1];
        beta = new double[2+1];
        alpha[0] = 0;
        alpha[1] = 0;
        for(i=0; i<=2; i++)
        {
            beta[i] = AP.Math.Sqr(i)/(4*AP.Math.Sqr(i)-1);
        }
        generategaussradauquadrature(alpha, beta, 2.0, +1, 3, ref x, ref w);
        err = 0;
        err = Math.Max(err, Math.Abs(x[2]-1));
        err = Math.Max(err, Math.Abs(x[1]+(1-Math.Sqrt(6))/5));
        err = Math.Max(err, Math.Abs(x[0]+(1+Math.Sqrt(6))/5));
        err = Math.Max(err, Math.Abs(w[2]-(double)(2)/(double)(9)));
        err = Math.Max(err, Math.Abs(w[1]-(16+Math.Sqrt(6))/18));
        err = Math.Max(err, Math.Abs(w[0]-(16-Math.Sqrt(6))/18));
        System.Console.Write("Second test (Gauss-Radau 3, at point b): error is ");
        System.Console.Write("{0,6:E3}",err);
        System.Console.WriteLine();
    }
}
