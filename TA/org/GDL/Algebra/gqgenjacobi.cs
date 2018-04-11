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

class gqgenjacobi
{
    /*************************************************************************
    Computation of nodes and weights for a Gauss-Jacobi quadrature formula

    The algorithm  calculates  the  nodes  and  weights  of  the  Gauss-Jacobi
    quadrature    formula    on    domain   [-1, 1]   with   weight   function
    W(x)=Power(1-x,Alpha)*Power(1+x,Beta).

    Input parameters:
        n       –   a required number of nodes.
                    n>=1.
        alpha   –   power of the first factor of the weighting function.
                    alpha > -1
        beta    –   power of the second factor of the weighting function.
                    beta > -1

    Output parameters:
        x       -   array of nodes.
                    Array whose index ranges from 0 to N-1.
        w       -   array of weighting coefficients.
                    Array whose index ranges from 0 to N-1.

    The algorithm was designed by using information from the QUADRULE library.
    *************************************************************************/
    public static void buildgaussjacobiquadrature(int n,
        double alpha,
        double beta,
        ref double[] x,
        ref double[] w)
    {
        int i = 0;
        int j = 0;
        double r = 0;
        double r1 = 0;
        double t1 = 0;
        double t2 = 0;
        double t3 = 0;
        double p1 = 0;
        double p2 = 0;
        double p3 = 0;
        double pp = 0;
        double an = 0;
        double bn = 0;
        double a = 0;
        double b = 0;
        double c = 0;
        double tmpsgn = 0;
        double alfbet = 0;
        double temp = 0;
        //int its = 0;

        x = new double[n-1+1];
        w = new double[n-1+1];
        for(i=0; i<=n-1; i++)
        {
            if( i==0 )
            {
                an = alpha/n;
                bn = beta/n;
                t1 = (1+alpha)*(2.78/(4+n*n)+0.768*an/n);
                t2 = 1+1.48*an+0.96*bn+0.452*an*an+0.83*an*bn;
                r = (t2-t1)/t2;
            }
            else
            {
                if( i==1 )
                {
                    t1 = (4.1+alpha)/((1+alpha)*(1+0.156*alpha));
                    t2 = 1+0.06*(n-8)*(1+0.12*alpha)/n;
                    t3 = 1+0.012*beta*(1+0.25*Math.Abs(alpha))/n;
                    r = r-t1*t2*t3*(1-r);
                }
                else
                {
                    if( i==2 )
                    {
                        t1 = (1.67+0.28*alpha)/(1+0.37*alpha);
                        t2 = 1+0.22*(n-8)/n;
                        t3 = 1+8*beta/((6.28+beta)*n*n);
                        r = r-t1*t2*t3*(x[0]-r);
                    }
                    else
                    {
                        if( i<n-2 )
                        {
                            r = 3*x[i-1]-3*x[i-2]+x[i-3];
                        }
                        else
                        {
                            if( i==n-2 )
                            {
                                t1 = (1+0.235*beta)/(0.766+0.119*beta);
                                t2 = 1/(1+0.639*(n-4)/(1+0.71*(n-4)));
                                t3 = 1/(1+20*alpha/((7.5+alpha)*n*n));
                                r = r+t1*t2*t3*(r-x[i-2]);
                            }
                            else
                            {
                                if( i==n-1 )
                                {
                                    t1 = (1+0.37*beta)/(1.67+0.28*beta);
                                    t2 = 1/(1+0.22*(n-8)/n);
                                    t3 = 1/(1+8*alpha/((6.28+alpha)*n*n));
                                    r = r+t1*t2*t3*(r-x[i-2]);
                                }
                            }
                        }
                    }
                }
            }
            alfbet = alpha+beta;
            do
            {
                temp = 2+alfbet;
                p1 = (alpha-beta+temp*r)*0.5;
                p2 = 1;
                for(j=2; j<=n; j++)
                {
                    p3 = p2;
                    p2 = p1;
                    temp = 2*j+alfbet;
                    a = 2*j*(j+alfbet)*(temp-2);
                    b = (temp-1)*(alpha*alpha-beta*beta+temp*(temp-2)*r);
                    c = 2*(j-1+alpha)*(j-1+beta)*temp;
                    p1 = (b*p2-c*p3)/a;
                }
                pp = (n*(alpha-beta-temp*r)*p1+2*(n+alpha)*(n+beta)*p2)/(temp*(1-r*r));
                r1 = r;
                r = r1-p1/pp;
            }
            while( Math.Abs(r-r1)>=AP.Math.MachineEpsilon*(1+Math.Abs(r))*100 );
            x[i] = r;
            w[i] = Math.Exp(gammaf.lngamma(alpha+n, ref tmpsgn)+gammaf.lngamma(beta+n, ref tmpsgn)-gammaf.lngamma(n+1, ref tmpsgn)-gammaf.lngamma(n+alfbet+1, ref tmpsgn))*temp*Math.Pow(2, alfbet)/(pp*p2);
        }
    }


    private static void testquad()
    {
        double alpha = 0;
        double beta = 0;
        double[] x = new double[0];
        double[] w = new double[0];
        double[] xleg = new double[0];
        double[] wleg = new double[0];
        int n = 0;
        int i = 0;
        int j = 0;
        double tmp = 0;
        //double valjac = 0;
        //double valsim = 0;
        double nodedist = 0;
        double nodesectstart = 0;
        double nodesectend = 0;
        double err = 0;
        double nodeerr = 0;
        double weighterr = 0;

        n = 56;
        alpha = -0.5;
        beta = -0.5;
        System.Console.Write("CHEBYSHEV TEST");
        System.Console.WriteLine();
        buildgaussjacobiquadrature(n, alpha, beta, ref x, ref w);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-2-i; j++)
            {
                if( x[j]>=x[j+1] )
                {
                    tmp = x[j];
                    x[j] = x[j+1];
                    x[j+1] = tmp;
                    tmp = w[j];
                    w[j] = w[j+1];
                    w[j+1] = tmp;
                }
            }
        }
        nodeerr = 0;
        weighterr = 0;
        for(i=0; i<=n-1; i++)
        {
            err = Math.Abs(Math.PI/n-w[i]);
            if( err>weighterr )
            {
                weighterr = err;
            }
            err = Math.Abs(x[n-1-i]-Math.Cos(Math.PI*(i+0.5)/n));
            if( err>nodeerr )
            {
                nodeerr = err;
            }
        }
        System.Console.Write("Node error is ");
        System.Console.Write("{0,5:E3}",nodeerr);
        System.Console.WriteLine();
        System.Console.Write("Weight error is ");
        System.Console.Write("{0,5:E3}",weighterr);
        System.Console.WriteLine();
        n = 56;
        alpha = -0.95;
        beta = -0.95;
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("MIN NODE DISTANCE AND WEIGHT HISTOGRAM TEST");
        System.Console.WriteLine();
        buildgaussjacobiquadrature(n, alpha, beta, ref x, ref w);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-2-i; j++)
            {
                if( x[j]>=x[j+1] )
                {
                    tmp = x[j];
                    x[j] = x[j+1];
                    x[j+1] = tmp;
                    tmp = w[j];
                    w[j] = w[j+1];
                    w[j+1] = tmp;
                }
            }
        }
        for(i=0; i<=4; i++)
        {
            nodesectstart = 0.2*i*2-1;
            nodesectend = 0.2*(i+1)*2-1;
            nodedist = 2;
            for(j=0; j<=n-2; j++)
            {
                if( x[j]>=nodesectstart & x[j]<=nodesectend | x[j+1]>=nodesectstart & x[j+1]<=nodesectend )
                {
                    if( nodedist>x[j+1]-x[j] )
                    {
                        nodedist = x[j+1]-x[j];
                    }
                }
            }
            System.Console.Write("Node distance at [");
            System.Console.Write("{0,5:F2}",nodesectstart);
            System.Console.Write(", ");
            System.Console.Write("{0,5:F2}",nodesectend);
            System.Console.Write("] is ");
            System.Console.Write("{0,5:E3}",nodedist);
            System.Console.WriteLine();
        }
        for(i=0; i<=4; i++)
        {
            nodesectstart = 0.2*i*2-1;
            nodesectend = 0.2*(i+1)*2-1;
            nodedist = 0;
            for(j=0; j<=n-1; j++)
            {
                if( x[j]>=nodesectstart & x[j]<=nodesectend )
                {
                    if( nodedist<Math.Abs(w[j]) )
                    {
                        nodedist = Math.Abs(w[j]);
                    }
                }
            }
            System.Console.Write("Max weight at [");
            System.Console.Write("{0,5:F2}",nodesectstart);
            System.Console.Write(", ");
            System.Console.Write("{0,5:F2}",nodesectend);
            System.Console.Write("] is ");
            System.Console.Write("{0,5:E3}",nodedist);
            System.Console.WriteLine();
        }
    }
}
