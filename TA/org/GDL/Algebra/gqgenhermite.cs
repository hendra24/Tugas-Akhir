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

class gqgenhermite
{
    /*************************************************************************
    Computation of nodes and weights for a Gauss-Hermite quadrature formula

    The  algorithm  calculates  the  nodes  and  weights  of the Gauss-Hermite
    quadrature  formula on domain  (-infinity, +infinity) with weight function
    W(x)=Exp(-x*x).

    Input parameters:
        n   –   a required number of nodes.
                1 <= n <= 190.

    Output parameters:
        x   -   array of nodes.
                Array whose index ranges from 0 to N-1.
        w   -   array of weighting coefficients.
                Array whose index ranges from 0 to N-1.

    The algorithm was designed by using information from the QUADRULE library.
    *************************************************************************/
    public static void buildgausshermitequadrature(int n,
        ref double[] x,
        ref double[] w)
    {
        int i = 0;
        int j = 0;
        double r = 0;
        double r1 = 0;
        double p1 = 0;
        double p2 = 0;
        double p3 = 0;
        double dp3 = 0;
        double pipm4 = 0;

        x = new double[n-1+1];
        w = new double[n-1+1];
        pipm4 = Math.Pow(Math.PI, -0.25);
        for(i=0; i<=(n+1)/2-1; i++)
        {
            if( i==0 )
            {
                r = Math.Sqrt(2*n+1)-1.85575*Math.Pow(2*n+1, -((double)(1)/(double)(6)));
            }
            else
            {
                if( i==1 )
                {
                    r = r-1.14*Math.Pow(n, 0.426)/r;
                }
                else
                {
                    if( i==2 )
                    {
                        r = 1.86*r-0.86*x[0];
                    }
                    else
                    {
                        if( i==3 )
                        {
                            r = 1.91*r-0.91*x[1];
                        }
                        else
                        {
                            r = 2*r-x[i-2];
                        }
                    }
                }
            }
            do
            {
                p2 = 0;
                p3 = pipm4;
                for(j=0; j<=n-1; j++)
                {
                    p1 = p2;
                    p2 = p3;
                    p3 = p2*r*Math.Sqrt((double)(2)/((double)(j+1)))-p1*Math.Sqrt((double)(j)/((double)(j+1)));
                }
                dp3 = Math.Sqrt(2*j)*p2;
                r1 = r;
                r = r-p3/dp3;
            }
            while( Math.Abs(r-r1)>=AP.Math.MachineEpsilon*(1+Math.Abs(r))*100 );
            x[i] = r;
            w[i] = 2/(dp3*dp3);
            x[n-1-i] = -x[i];
            w[n-1-i] = w[i];
        }
    }


    private static double f(double x)
    {
        double result = 0;

        result = Math.Sin(x)*Math.Sin(x);
        return result;
    }


    private static void testquad()
    {
        double epsilon = 0;
        double a = 0;
        double b = 0;
        double[] x = new double[0];
        double[] w = new double[0];
        int n = 0;
        int i = 0;
        int j = 0;
        double v1 = 0;
        double v2 = 0;
        double h = 0;
        double s = 0;
        double s1 = 0;
        double s2 = 0;
        double s3 = 0;
        double tx = 0;
        int simpfunccalls = 0;
        double nodedist = 0;
        double nodesectstart = 0;
        double nodesectend = 0;
        double tmp = 0;

        n = 10;
        epsilon = 0.000000001;
        a = -1;
        b = 1;
        System.Console.Write("PRIMARY POINTS TEST");
        System.Console.WriteLine();
        v1 = 0;
        simpfunccalls = 2;
        s2 = 1;
        h = b-a;
        s = Math.Exp(-(a*a))*f(a)+Math.Exp(-(b*b))*f(b);
        do
        {
            s3 = s2;
            h = h/2;
            s1 = 0;
            tx = a+h;
            do
            {
                s1 = s1+2*Math.Exp(-(tx*tx))*f(tx);
                simpfunccalls = simpfunccalls+1;
                tx = tx+2*h;
            }
            while( tx<b );
            s = s+s1;
            s2 = (s+s1)*h/3;
            tx = Math.Abs(s3-s2)/15;
        }
        while( tx>epsilon );
        v1 = s2;
        System.Console.Write("Simpson (");
        System.Console.Write("{0,1:d}",simpfunccalls);
        System.Console.Write(" calls) result ");
        System.Console.Write("{0,12:F8}",v1);
        System.Console.WriteLine();
        buildgausshermitequadrature(n, ref x, ref w);
        v2 = 0;
        for(i=0; i<=n-1; i++)
        {
            v2 = v2+w[i]*f(x[i]);
        }
        System.Console.Write("Quadrature (");
        System.Console.Write("{0,1:d}",n);
        System.Console.Write(" points) result ");
        System.Console.Write("{0,12:F8}",v2);
        System.Console.WriteLine();
        System.Console.Write("Difference is ");
        System.Console.Write("{0,5:E3}",Math.Abs(v2-v1));
        System.Console.WriteLine();
        n = 56;
        System.Console.WriteLine();
        System.Console.WriteLine();
        System.Console.Write("MIN NODE DISTANCE TEST");
        System.Console.WriteLine();
        buildgausshermitequadrature(n, ref x, ref w);
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
            nodedist = x[n-1]-x[0];
            nodesectstart = nodedist*(0.2*i-0.5);
            nodesectend = nodedist*(0.2*(i+1)-0.5);
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
            System.Console.Write("{0,7:F2}",nodesectstart);
            System.Console.Write(", ");
            System.Console.Write("{0,7:F2}",nodesectend);
            System.Console.Write("] is ");
            System.Console.Write("{0,7:E3}",nodedist);
            System.Console.WriteLine();
        }
        for(i=0; i<=4; i++)
        {
            tmp = x[n-1]-x[0];
            nodesectstart = tmp*(0.2*i-0.5);
            nodesectend = tmp*(0.2*(i+1)-0.5);
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
            System.Console.Write("{0,7:F2}",nodesectstart);
            System.Console.Write(", ");
            System.Console.Write("{0,7:F2}",nodesectend);
            System.Console.Write("] is ");
            System.Console.Write("{0,7:E3}",nodedist);
            System.Console.WriteLine();
        }
    }
}
