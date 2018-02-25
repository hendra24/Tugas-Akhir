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

class gqgenlobatto
{
    /*************************************************************************
    Computation of nodes and weights for a Gauss-Lobatto quadrature formula

    The algorithm generates the N-point Gauss-Lobatto quadrature formula  with
    weight function given by coefficients alpha and beta of a recurrence which
    generates a system of orthogonal polynomials.

    P-1(x)   =  0
    P0(x)    =  1
    Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

    and zero moment Mu0

    Mu0 = integral(W(x)dx,a,b)


    Input parameters:
        Alpha   –   array of coefficients.
                    Array whose index ranges within [0..N-2].
        Beta    –   array of coefficients.
                    Array whose index ranges within [0..N-2].
                    Zero-indexed element is not used.
                    Beta[I]>0
        Mu0     –   zero moment of the weighting function.
        A       –   left boundary of the integration interval.
        B       –   right boundary of the integration interval.
        N       –   number of nodes of the quadrature formula, N>=3
                    (including the left and right boundary nodes).

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
    public static bool generategausslobattoquadrature(double[] alpha,
        double[] beta,
        double mu0,
        double a,
        double b,
        int n,
        ref double[] x,
        ref double[] w)
    {
        bool result = new bool();
        int i = 0;
        double[] d = new double[0];
        double[] e = new double[0];
        double[,] z = new double[0,0];
        double pim1a = 0;
        double pia = 0;
        double pim1b = 0;
        double pib = 0;
        double t = 0;
        double a11 = 0;
        double a12 = 0;
        double a21 = 0;
        double a22 = 0;
        double b1 = 0;
        double b2 = 0;
        double alph = 0;
        double bet = 0;

        alpha = (double[])alpha.Clone();
        beta = (double[])beta.Clone();

        if( n<=2 )
        {
            result = false;
            return result;
        }
        
        //
        // Initialize, D[1:N+1], E[1:N]
        //
        n = n-2;
        d = new double[n+2+1];
        e = new double[n+1+1];
        for(i=1; i<=n+1; i++)
        {
            d[i] = alpha[i-1];
        }
        for(i=1; i<=n; i++)
        {
            if( beta[i]<=0 )
            {
                result = false;
                return result;
            }
            e[i] = Math.Sqrt(beta[i]);
        }
        
        //
        // Caclulate Pn(a), Pn+1(a), Pn(b), Pn+1(b)
        //
        beta[0] = 0;
        pim1a = 0;
        pia = 1;
        pim1b = 0;
        pib = 1;
        for(i=1; i<=n+1; i++)
        {
            
            //
            // Pi(a)
            //
            t = (a-alpha[i-1])*pia-beta[i-1]*pim1a;
            pim1a = pia;
            pia = t;
            
            //
            // Pi(b)
            //
            t = (b-alpha[i-1])*pib-beta[i-1]*pim1b;
            pim1b = pib;
            pib = t;
        }
        
        //
        // Calculate alpha'(n+1), beta'(n+1)
        //
        a11 = pia;
        a12 = pim1a;
        a21 = pib;
        a22 = pim1b;
        b1 = a*pia;
        b2 = b*pib;
        if( Math.Abs(a11)>Math.Abs(a21) )
        {
            a22 = a22-a12*a21/a11;
            b2 = b2-b1*a21/a11;
            bet = b2/a22;
            alph = (b1-bet*a12)/a11;
        }
        else
        {
            a12 = a12-a22*a11/a21;
            b1 = b1-b2*a11/a21;
            bet = b1/a12;
            alph = (b2-bet*a22)/a21;
        }
        if( bet<0 )
        {
            result = false;
            return result;
        }
        d[n+2] = alph;
        e[n+1] = Math.Sqrt(bet);
        
        //
        // EVD
        //
        result = tdevd.tridiagonalevd(ref d, e, n+2, 3, ref z);
        if( !result )
        {
            return result;
        }
        
        //
        // Generate
        //
        x = new double[n+1+1];
        w = new double[n+1+1];
        for(i=1; i<=n+2; i++)
        {
            x[i-1] = d[i];
            w[i-1] = mu0*AP.Math.Sqr(z[1,i]);
        }
        return result;
    }


    private static void testgenerategausslobattoquadrature()
    {
        double[] alpha = new double[0];
        double[] beta = new double[0];
        double[] x = new double[0];
        double[] w = new double[0];
        double err = 0;
        //int i = 0;

        System.Console.Write("TESTING GAUSS-LOBATTO QUADRATURES GENERATION");
        System.Console.WriteLine();
        
        //
        // first test
        //
        alpha = new double[1+1];
        beta = new double[1+1];
        alpha[0] = 0;
        alpha[1] = 0;
        beta[0] = 0;
        beta[1] = (double)(1*1)/((double)(4*1*1-1));
        generategausslobattoquadrature(alpha, beta, 2.0, -1, +1, 3, ref x, ref w);
        err = 0;
        err = Math.Max(err, Math.Abs(x[0]+1));
        err = Math.Max(err, Math.Abs(x[1]));
        err = Math.Max(err, Math.Abs(x[2]-1));
        err = Math.Max(err, Math.Abs(w[0]-(double)(1)/(double)(3)));
        err = Math.Max(err, Math.Abs(w[1]-(double)(4)/(double)(3)));
        err = Math.Max(err, Math.Abs(w[2]-(double)(1)/(double)(3)));
        System.Console.Write("First test (Gauss-Lobatto 2): error is ");
        System.Console.Write("{0,6:E3}",err);
        System.Console.WriteLine();
        
        //
        // second test
        //
        alpha = new double[2+1];
        beta = new double[2+1];
        alpha[0] = 0;
        alpha[1] = 0;
        alpha[2] = 0;
        beta[0] = 0;
        beta[1] = (double)(1*1)/((double)(4*1*1-1));
        beta[2] = (double)(2*2)/((double)(4*2*2-1));
        generategausslobattoquadrature(alpha, beta, 2.0, -1, +1, 4, ref x, ref w);
        err = 0;
        err = Math.Max(err, Math.Abs(x[0]+1));
        err = Math.Max(err, Math.Abs(x[1]+Math.Sqrt(5)/5));
        err = Math.Max(err, Math.Abs(x[2]-Math.Sqrt(5)/5));
        err = Math.Max(err, Math.Abs(x[3]-1));
        err = Math.Max(err, Math.Abs(w[0]-(double)(1)/(double)(6)));
        err = Math.Max(err, Math.Abs(w[1]-(double)(5)/(double)(6)));
        err = Math.Max(err, Math.Abs(w[2]-(double)(5)/(double)(6)));
        err = Math.Max(err, Math.Abs(w[3]-(double)(1)/(double)(6)));
        System.Console.Write("Second test (Gauss-Lobatto 4): error is ");
        System.Console.Write("{0,6:E3}",err);
        System.Console.WriteLine();
        
        //
        // third test
        //
        alpha = new double[4+1];
        beta = new double[4+1];
        alpha[0] = 0;
        alpha[1] = 0;
        alpha[2] = 0;
        alpha[3] = 0;
        alpha[4] = 0;
        beta[0] = 0;
        beta[1] = (double)(1*1)/((double)(4*1*1-1));
        beta[2] = (double)(2*2)/((double)(4*2*2-1));
        beta[3] = (double)(3*3)/((double)(4*3*3-1));
        beta[4] = (double)(4*4)/((double)(4*4*4-1));
        generategausslobattoquadrature(alpha, beta, 2.0, -1, +1, 6, ref x, ref w);
        err = 0;
        err = Math.Max(err, Math.Abs(x[0]+1));
        err = Math.Max(err, Math.Abs(x[1]+Math.Sqrt((7+2*Math.Sqrt(7))/21)));
        err = Math.Max(err, Math.Abs(x[2]+Math.Sqrt((7-2*Math.Sqrt(7))/21)));
        err = Math.Max(err, Math.Abs(x[3]-Math.Sqrt((7-2*Math.Sqrt(7))/21)));
        err = Math.Max(err, Math.Abs(x[4]-Math.Sqrt((7+2*Math.Sqrt(7))/21)));
        err = Math.Max(err, Math.Abs(x[5]-1));
        err = Math.Max(err, Math.Abs(w[0]-(double)(1)/(double)(15)));
        err = Math.Max(err, Math.Abs(w[1]-(14-Math.Sqrt(7))/30));
        err = Math.Max(err, Math.Abs(w[2]-(14+Math.Sqrt(7))/30));
        err = Math.Max(err, Math.Abs(w[3]-(14+Math.Sqrt(7))/30));
        err = Math.Max(err, Math.Abs(w[4]-(14-Math.Sqrt(7))/30));
        err = Math.Max(err, Math.Abs(w[5]-(double)(1)/(double)(15)));
        System.Console.Write("Third test (Gauss-Lobatto 6): error is ");
        System.Console.Write("{0,6:E3}",err);
        System.Console.WriteLine();
    }
}
