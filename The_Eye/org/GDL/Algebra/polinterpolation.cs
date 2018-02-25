/*************************************************************************
Copyright (c) 2007, Sergey Bochkanov (ALGLIB project).

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

class polinterpolation
{
    /*************************************************************************
    Interpolation using barycentric formula

    F(t) = SUM(i=0,n-1,w[i]*f[i]/(t-x[i])) / SUM(i=0,n-1,w[i]/(t-x[i]))

    Input parameters:
        X   -   interpolation nodes, array[0..N-1]
        F   -   function values, array[0..N-1]
        W   -   barycentric weights, array[0..N-1]
        N   -   nodes count, N>0
        T   -   interpolation point
        
    Result:
        barycentric interpolant F(t)

      -- ALGLIB --
         Copyright 28.05.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double barycentricinterpolation(ref double[] x,
        ref double[] f,
        ref double[] w,
        int n,
        double t)
    {
        double result = 0;
        double s1 = 0;
        double s2 = 0;
        double v = 0;
        double threshold = 0;
        double s = 0;
        int i = 0;
        int j = 0;

        System.Diagnostics.Debug.Assert(n>0, "BarycentricInterpolation: N<=0!");
        threshold = Math.Sqrt(AP.Math.MinRealNumber);
        
        //
        // First, decide: should we use "safe" formula (guarded
        // against overflow) or fast one?
        //
        j = 0;
        s = t-x[0];
        for(i=1; i<=n-1; i++)
        {
            if( Math.Abs(t-x[i])<Math.Abs(s) )
            {
                s = t-x[i];
                j = i;
            }
        }
        if( s==0 )
        {
            result = f[j];
            return result;
        }
        if( Math.Abs(s)>threshold )
        {
            
            //
            // use fast formula
            //
            j = -1;
            s = 1.0;
        }
        
        //
        // Calculate using safe or fast barycentric formula
        //
        s1 = 0;
        s2 = 0;
        for(i=0; i<=n-1; i++)
        {
            if( i!=j )
            {
                v = s*w[i]/(t-x[i]);
                s1 = s1+v*f[i];
                s2 = s2+v;
            }
            else
            {
                v = w[i];
                s1 = s1+v*f[i];
                s2 = s2+v;
            }
        }
        result = s1/s2;
        return result;
    }


    /*************************************************************************
    Polynomial interpolation on the equidistant nodes using barycentric
    formula. O(N) complexity.

    Input parameters:
        A,B -   interpolation interval [A,B]
        F   -   function values, array[0..N-1].
                F[i] = F(A+(B-A)*i/(N-1))
        N   -   nodes count
        T   -   interpolation point

    Result:
        the value of the interpolation polynomial F(t)

      -- ALGLIB --
         Copyright 28.05.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double equidistantpolynomialinterpolation(double a,
        double b,
        ref double[] f,
        int n,
        double t)
    {
        double result = 0;
        double s1 = 0;
        double s2 = 0;
        double v = 0;
        double threshold = 0;
        double s = 0;
        int i = 0;
        int j = 0;
        double w = 0;
        double x = 0;

        System.Diagnostics.Debug.Assert(n>0, "BarycentricInterpolation: N<=0!");
        threshold = Math.Sqrt(AP.Math.MinRealNumber);
        
        //
        // Special case: N=1
        //
        if( n==1 )
        {
            result = f[0];
            return result;
        }
        
        //
        // First, decide: should we use "safe" formula (guarded
        // against overflow) or fast one?
        //
        j = 0;
        s = t-a;
        for(i=1; i<=n-1; i++)
        {
            x = a+(double)(i)/((double)(n-1))*(b-a);
            if( Math.Abs(t-x)<Math.Abs(s) )
            {
                s = t-x;
                j = i;
            }
        }
        if( s==0 )
        {
            result = f[j];
            return result;
        }
        if( Math.Abs(s)>threshold )
        {
            
            //
            // use fast formula
            //
            j = -1;
            s = 1.0;
        }
        
        //
        // Calculate using safe or fast barycentric formula
        //
        s1 = 0;
        s2 = 0;
        w = 1.0;
        for(i=0; i<=n-1; i++)
        {
            if( i!=j )
            {
                v = s*w/(t-(a+(double)(i)/((double)(n-1))*(b-a)));
                s1 = s1+v*f[i];
                s2 = s2+v;
            }
            else
            {
                v = w;
                s1 = s1+v*f[i];
                s2 = s2+v;
            }
            w = -(w*(n-1-i));
            w = w/(i+1);
        }
        result = s1/s2;
        return result;
    }


    /*************************************************************************
    Polynomial interpolation on the Chebyshev nodes (first kind) using
    barycentric formula. O(N) complexity.

    Input parameters:
        A,B -   interpolation interval [A,B]
        F   -   function values, array[0..N-1].
                F[i] = F(0.5*(B+A) + 0.5*(B-A)*Cos(PI*(2*i+1)/(2*n)))
        N   -   nodes count
        T   -   interpolation point

    Result:
        the value of the interpolation polynomial F(t)

      -- ALGLIB --
         Copyright 28.05.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double chebyshev1interpolation(double a,
        double b,
        ref double[] f,
        int n,
        double t)
    {
        double result = 0;
        double s1 = 0;
        double s2 = 0;
        double v = 0;
        double threshold = 0;
        double s = 0;
        int i = 0;
        int j = 0;
        double a0 = 0;
        double delta = 0;
        double alpha = 0;
        double beta = 0;
        double ca = 0;
        double sa = 0;
        double tempc = 0;
        double temps = 0;
        double x = 0;
        double w = 0;
        double p1 = 0;

        System.Diagnostics.Debug.Assert(n>0, "Chebyshev1Interpolation: N<=0!");
        threshold = Math.Sqrt(AP.Math.MinRealNumber);
        t = (t-0.5*(a+b))/(0.5*(b-a));
        
        //
        // Prepare information for the recurrence formula
        // used to calculate sin(pi*(2j+1)/(2n+2)) and
        // cos(pi*(2j+1)/(2n+2)):
        //
        // A0    = pi/(2n+2)
        // Delta = pi/(n+1)
        // Alpha = 2 sin^2 (Delta/2)
        // Beta  = sin(Delta)
        //
        // so that sin(..) = sin(A0+j*delta) and cos(..) = cos(A0+j*delta).
        // Then we use
        //
        // sin(x+delta) = sin(x) - (alpha*sin(x) - beta*cos(x))
        // cos(x+delta) = cos(x) - (alpha*cos(x) - beta*sin(x))
        //
        // to repeatedly calculate sin(..) and cos(..).
        //
        a0 = Math.PI/(2*(n-1)+2);
        delta = 2*Math.PI/(2*(n-1)+2);
        alpha = 2*AP.Math.Sqr(Math.Sin(delta/2));
        beta = Math.Sin(delta);
        
        //
        // First, decide: should we use "safe" formula (guarded
        // against overflow) or fast one?
        //
        ca = Math.Cos(a0);
        sa = Math.Sin(a0);
        j = 0;
        x = ca;
        s = t-x;
        for(i=1; i<=n-1; i++)
        {
            
            //
            // Next X[i]
            //
            temps = sa-(alpha*sa-beta*ca);
            tempc = ca-(alpha*ca+beta*sa);
            sa = temps;
            ca = tempc;
            x = ca;
            
            //
            // Use X[i]
            //
            if( Math.Abs(t-x)<Math.Abs(s) )
            {
                s = t-x;
                j = i;
            }
        }
        if( s==0 )
        {
            result = f[j];
            return result;
        }
        if( Math.Abs(s)>threshold )
        {
            
            //
            // use fast formula
            //
            j = -1;
            s = 1.0;
        }
        
        //
        // Calculate using safe or fast barycentric formula
        //
        s1 = 0;
        s2 = 0;
        ca = Math.Cos(a0);
        sa = Math.Sin(a0);
        p1 = 1.0;
        for(i=0; i<=n-1; i++)
        {
            
            //
            // Calculate X[i], W[i]
            //
            x = ca;
            w = p1*sa;
            
            //
            // Proceed
            //
            if( i!=j )
            {
                v = s*w/(t-x);
                s1 = s1+v*f[i];
                s2 = s2+v;
            }
            else
            {
                v = w;
                s1 = s1+v*f[i];
                s2 = s2+v;
            }
            
            //
            // Next CA, SA, P1
            //
            temps = sa-(alpha*sa-beta*ca);
            tempc = ca-(alpha*ca+beta*sa);
            sa = temps;
            ca = tempc;
            p1 = -p1;
        }
        result = s1/s2;
        return result;
    }


    /*************************************************************************
    Polynomial interpolation on the Chebyshev nodes (second kind) using
    barycentric formula. O(N) complexity.

    Input parameters:
        A,B -   interpolation interval [A,B]
        F   -   function values, array[0..N-1].
                F[i] = F(0.5*(B+A) + 0.5*(B-A)*Cos(PI*i/(n-1)))
        N   -   nodes count
        T   -   interpolation point

    Result:
        the value of the interpolation polynomial F(t)

      -- ALGLIB --
         Copyright 28.05.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double chebyshev2interpolation(double a,
        double b,
        ref double[] f,
        int n,
        double t)
    {
        double result = 0;
        double s1 = 0;
        double s2 = 0;
        double v = 0;
        double threshold = 0;
        double s = 0;
        int i = 0;
        int j = 0;
        double a0 = 0;
        double delta = 0;
        double alpha = 0;
        double beta = 0;
        double ca = 0;
        double sa = 0;
        double tempc = 0;
        double temps = 0;
        double x = 0;
        double w = 0;
        double p1 = 0;

        System.Diagnostics.Debug.Assert(n>1, "Chebyshev2Interpolation: N<=1!");
        threshold = Math.Sqrt(AP.Math.MinRealNumber);
        t = (t-0.5*(a+b))/(0.5*(b-a));
        
        //
        // Prepare information for the recurrence formula
        // used to calculate sin(pi*i/n) and
        // cos(pi*i/n):
        //
        // A0    = 0
        // Delta = pi/n
        // Alpha = 2 sin^2 (Delta/2)
        // Beta  = sin(Delta)
        //
        // so that sin(..) = sin(A0+j*delta) and cos(..) = cos(A0+j*delta).
        // Then we use
        //
        // sin(x+delta) = sin(x) - (alpha*sin(x) - beta*cos(x))
        // cos(x+delta) = cos(x) - (alpha*cos(x) - beta*sin(x))
        //
        // to repeatedly calculate sin(..) and cos(..).
        //
        a0 = 0.0;
        delta = Math.PI/(n-1);
        alpha = 2*AP.Math.Sqr(Math.Sin(delta/2));
        beta = Math.Sin(delta);
        
        //
        // First, decide: should we use "safe" formula (guarded
        // against overflow) or fast one?
        //
        ca = Math.Cos(a0);
        sa = Math.Sin(a0);
        j = 0;
        x = ca;
        s = t-x;
        for(i=1; i<=n-1; i++)
        {
            
            //
            // Next X[i]
            //
            temps = sa-(alpha*sa-beta*ca);
            tempc = ca-(alpha*ca+beta*sa);
            sa = temps;
            ca = tempc;
            x = ca;
            
            //
            // Use X[i]
            //
            if( Math.Abs(t-x)<Math.Abs(s) )
            {
                s = t-x;
                j = i;
            }
        }
        if( s==0 )
        {
            result = f[j];
            return result;
        }
        if( Math.Abs(s)>threshold )
        {
            
            //
            // use fast formula
            //
            j = -1;
            s = 1.0;
        }
        
        //
        // Calculate using safe or fast barycentric formula
        //
        s1 = 0;
        s2 = 0;
        ca = Math.Cos(a0);
        sa = Math.Sin(a0);
        p1 = 1.0;
        for(i=0; i<=n-1; i++)
        {
            
            //
            // Calculate X[i], W[i]
            //
            x = ca;
            if( i==0 | i==n-1 )
            {
                w = 0.5*p1;
            }
            else
            {
                w = 1.0*p1;
            }
            
            //
            // Proceed
            //
            if( i!=j )
            {
                v = s*w/(t-x);
                s1 = s1+v*f[i];
                s2 = s2+v;
            }
            else
            {
                v = w;
                s1 = s1+v*f[i];
                s2 = s2+v;
            }
            
            //
            // Next CA, SA, P1
            //
            temps = sa-(alpha*sa-beta*ca);
            tempc = ca-(alpha*ca+beta*sa);
            sa = temps;
            ca = tempc;
            p1 = -p1;
        }
        result = s1/s2;
        return result;
    }


    /*************************************************************************
    Polynomial interpolation on the arbitrary nodes using Neville's algorithm.
    O(N^2) complexity.

    Input parameters:
        X   -   interpolation nodes, array[0..N-1].
        F   -   function values, array[0..N-1].
        N   -   nodes count
        T   -   interpolation point

    Result:
        the value of the interpolation polynomial F(t)

      -- ALGLIB --
         Copyright 28.05.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double nevilleinterpolation(ref double[] x,
        double[] f,
        int n,
        double t)
    {
        double result = 0;
        int m = 0;
        int i = 0;

        f = (double[])f.Clone();

        n = n-1;
        for(m=1; m<=n; m++)
        {
            for(i=0; i<=n-m; i++)
            {
                f[i] = ((t-x[i+m])*f[i]+(x[i]-t)*f[i+1])/(x[i]-x[i+m]);
            }
        }
        result = f[0];
        return result;
    }


    /*************************************************************************
    Polynomial interpolation on the arbitrary nodes using Neville's algorithm.
    O(N^2) complexity. Subroutine  returns  the  value  of  the  interpolation
    polynomial, the first and the second derivative.

    Input parameters:
        X   -   interpolation nodes, array[0..N-1].
        F   -   function values, array[0..N-1].
        N   -   nodes count
        T   -   interpolation point

    Output parameters:
        P   -   the value of the interpolation polynomial F(t)
        DP  -   the first derivative of the interpolation polynomial dF(t)/dt
        D2P -   the second derivative of the interpolation polynomial d2F(t)/dt2

      -- ALGLIB --
         Copyright 28.05.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void nevilledifferentiation(ref double[] x,
        double[] f,
        int n,
        double t,
        ref double p,
        ref double dp,
        ref double d2p)
    {
        int m = 0;
        int i = 0;
        double[] df = new double[0];
        double[] d2f = new double[0];

        f = (double[])f.Clone();

        n = n-1;
        df = new double[n+1];
        d2f = new double[n+1];
        for(i=0; i<=n; i++)
        {
            d2f[i] = 0;
            df[i] = 0;
        }
        for(m=1; m<=n; m++)
        {
            for(i=0; i<=n-m; i++)
            {
                d2f[i] = ((t-x[i+m])*d2f[i]+(x[i]-t)*d2f[i+1]+2*df[i]-2*df[i+1])/(x[i]-x[i+m]);
                df[i] = ((t-x[i+m])*df[i]+f[i]+(x[i]-t)*df[i+1]-f[i+1])/(x[i]-x[i+m]);
                f[i] = ((t-x[i+m])*f[i]+(x[i]-t)*f[i+1])/(x[i]-x[i+m]);
            }
        }
        p = f[0];
        dp = df[0];
        d2p = d2f[0];
    }


    /*************************************************************************
    Obsolete algorithm, replaced by NevilleInterpolation.
    *************************************************************************/
    public static double lagrangeinterpolate(int n,
        ref double[] x,
        double[] f,
        double t)
    {
        double result = 0;

        f = (double[])f.Clone();

        result = nevilleinterpolation(ref x, f, n, t);
        return result;
    }


    /*************************************************************************
    Obsolete algorithm, replaced by NevilleInterpolationWithDerivative
    *************************************************************************/
    public static void lagrangederivative(int n,
        ref double[] x,
        double[] f,
        double t,
        ref double p,
        ref double dp)
    {
        double d2p = 0;

        f = (double[])f.Clone();

        nevilledifferentiation(ref x, f, n, t, ref p, ref dp, ref d2p);
    }
}
