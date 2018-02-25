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

class spline2d
{
    /*************************************************************************
    This subroutine builds bilinear spline coefficients table.

    Input parameters:
        X   -   spline abscissas, array[0..N-1]
        Y   -   spline ordinates, array[0..M-1]
        F   -   function values, array[0..M-1,0..N-1]
        M,N -   grid size, M>=2, N>=2

    Output parameters:
        C   -   coefficients table.  Used  by  SplineInterpolation2D and other
                subroutines from this file.

      -- ALGLIB PROJECT --
         Copyright 05.07.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void buildbilinearspline(double[] x,
        double[] y,
        double[,] f,
        int m,
        int n,
        ref double[] c)
    {
        int i = 0;
        int j = 0;
        int k = 0;
        int tblsize = 0;
        int shift = 0;
        double t = 0;
        double[,] dx = new double[0,0];
        double[,] dy = new double[0,0];
        double[,] dxy = new double[0,0];

        x = (double[])x.Clone();
        y = (double[])y.Clone();
        f = (double[,])f.Clone();

        System.Diagnostics.Debug.Assert(n>=2 & m>=2, "BuildBilinearSpline: N<2 or M<2!");
        
        //
        // Sort points
        //
        for(j=0; j<=n-1; j++)
        {
            k = j;
            for(i=j+1; i<=n-1; i++)
            {
                if( x[i]<x[k] )
                {
                    k = i;
                }
            }
            if( k!=j )
            {
                for(i=0; i<=m-1; i++)
                {
                    t = f[i,j];
                    f[i,j] = f[i,k];
                    f[i,k] = t;
                }
                t = x[j];
                x[j] = x[k];
                x[k] = t;
            }
        }
        for(i=0; i<=m-1; i++)
        {
            k = i;
            for(j=i+1; j<=m-1; j++)
            {
                if( y[j]<y[k] )
                {
                    k = j;
                }
            }
            if( k!=i )
            {
                for(j=0; j<=n-1; j++)
                {
                    t = f[i,j];
                    f[i,j] = f[k,j];
                    f[k,j] = t;
                }
                t = y[i];
                y[i] = y[k];
                y[k] = t;
            }
        }
        
        //
        // Fill C:
        //  C[0]            -   length(C)
        //  C[1]            -   type(C):
        //                      -1 = bilinear interpolant
        //                      -3 = general cubic spline
        //                           (see BuildBicubicSpline)
        //  C[2]:
        //      N (x count)
        //  C[3]:
        //      M (y count)
        //  C[4]...C[4+N-1]:
        //      x[i], i = 0...N-1
        //  C[4+N]...C[4+N+M-1]:
        //      y[i], i = 0...M-1
        //  C[4+N+M]...C[4+N+M+(N*M-1)]:
        //      f(i,j) table. f(0,0), f(0, 1), f(0,2) and so on...
        //
        tblsize = 4+n+m+n*m;
        c = new double[tblsize-1+1];
        c[0] = tblsize;
        c[1] = -1;
        c[2] = n;
        c[3] = m;
        for(i=0; i<=n-1; i++)
        {
            c[4+i] = x[i];
        }
        for(i=0; i<=m-1; i++)
        {
            c[4+n+i] = y[i];
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                shift = i*n+j;
                c[4+n+m+shift] = f[i,j];
            }
        }
    }


    /*************************************************************************
    This subroutine builds bicubic spline coefficients table.

    Input parameters:
        X   -   spline abscissas, array[0..N-1]
        Y   -   spline ordinates, array[0..M-1]
        F   -   function values, array[0..M-1,0..N-1]
        M,N -   grid size, M>=2, N>=2

    Output parameters:
        C   -   coefficients table.  Used  by  SplineInterpolation2D and other
                subroutines from this file.

      -- ALGLIB PROJECT --
         Copyright 05.07.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void buildbicubicspline(double[] x,
        double[] y,
        //double[,] f,
        double[][] f,
        int m,
        int n,
        ref double[] c)
    {
        int i = 0;
        int j = 0;
        int k = 0;
        int tblsize = 0;
        int shift = 0;
        double t = 0;
        double[,] dx = new double[0,0];
        double[,] dy = new double[0,0];
        double[,] dxy = new double[0,0];

        x = (double[])x.Clone();
        y = (double[])y.Clone();
        //f = (double[,])f.Clone();
        f = (double[][])f.Clone();

        System.Diagnostics.Debug.Assert(n>=2 & m>=2, "BuildBicubicSpline: N<2 or M<2!");
        
        //
        // Sort points
        //
        for(j=0; j<=n-1; j++)
        {
            k = j;
            for(i=j+1; i<=n-1; i++)
            {
                if( x[i]<x[k] )
                {
                    k = i;
                }
            }
            if( k!=j )
            {
                for(i=0; i<=m-1; i++)
                {
                    /*t = f[i,j];
                    f[i,j] = f[i,k];
                    f[i,k] = t;*/
                    t = f[i][j];
                    f[i][j] = f[i][k];
                    f[i][k] = t;
                }
                t = x[j];
                x[j] = x[k];
                x[k] = t;
            }
        }
        for(i=0; i<=m-1; i++)
        {
            k = i;
            for(j=i+1; j<=m-1; j++)
            {
                if( y[j]<y[k] )
                {
                    k = j;
                }
            }
            if( k!=i )
            {
                for(j=0; j<=n-1; j++)
                {
                    /*t = f[i,j];
                    f[i,j] = f[k,j];
                    f[k,j] = t;*/
                    t = f[i][j];
                    f[i][j] = f[k][j];
                    f[k][j] = t;
                }
                t = y[i];
                y[i] = y[k];
                y[k] = t;
            }
        }
        
        //
        // Fill C:
        //  C[0]            -   length(C)
        //  C[1]            -   type(C):
        //                      -1 = bilinear interpolant
        //                           (see BuildBilinearInterpolant)
        //                      -3 = general cubic spline
        //  C[2]:
        //      N (x count)
        //  C[3]:
        //      M (y count)
        //  C[4]...C[4+N-1]:
        //      x[i], i = 0...N-1
        //  C[4+N]...C[4+N+M-1]:
        //      y[i], i = 0...M-1
        //  C[4+N+M]...C[4+N+M+(N*M-1)]:
        //      f(i,j) table. f(0,0), f(0, 1), f(0,2) and so on...
        //  C[4+N+M+N*M]...C[4+N+M+(2*N*M-1)]:
        //      df(i,j)/dx table.
        //  C[4+N+M+2*N*M]...C[4+N+M+(3*N*M-1)]:
        //      df(i,j)/dy table.
        //  C[4+N+M+3*N*M]...C[4+N+M+(4*N*M-1)]:
        //      d2f(i,j)/dxdy table.
        //
        tblsize = 4+n+m+4*n*m;
        c = new double[tblsize-1+1];
        c[0] = tblsize;
        c[1] = -3;
        c[2] = n;
        c[3] = m;
        for(i=0; i<=n-1; i++)
        {
            c[4+i] = x[i];
        }
        for(i=0; i<=m-1; i++)
        {
            c[4+n+i] = y[i];
        }
        //bicubiccalcderivatives(ref f, ref x, ref y, m, n, ref dx, ref dy, ref dxy);
        bicubiccalcderivatives(ref f, ref x, ref y, m, n, ref dx, ref dy, ref dxy);

        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                shift = i*n+j;
                //c[4+n+m+shift] = f[i,j];
                c[4 + n + m + shift] = f[i][j];
                c[4+n+m+n*m+shift] = dx[i,j];
                c[4+n+m+2*n*m+shift] = dy[i,j];
                c[4+n+m+3*n*m+shift] = dxy[i,j];
            }
        }
    }


    /*************************************************************************
    This subroutine calculates the value of the bilinear or bicubic spline  at
    the given point X.

    Input parameters:
        C   -   coefficients table.
                Built by BuildBilinearSpline or BuildBicubicSpline.
        X, Y-   point

    Result:
        S(x,y)

      -- ALGLIB PROJECT --
         Copyright 05.07.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double splineinterpolation2d(ref double[] c,
        double x,
        double y)
    {
        double result = 0;
        double v = 0;
        double vx = 0;
        double vy = 0;
        double vxy = 0;

        splinedifferentiation2d(ref c, x, y, ref v, ref vx, ref vy, ref vxy);
        result = v;
        return result;
    }












    public static void buildbicubicspline(double[] x,
        double[] y,
        double[,] f,
        int m,
        int n,
        ref double[] c)
    {
        int i = 0;
        int j = 0;
        int k = 0;
        int tblsize = 0;
        int shift = 0;
        double t = 0;
        double[,] dx = new double[0, 0];
        double[,] dy = new double[0, 0];
        double[,] dxy = new double[0, 0];

        x = (double[])x.Clone();
        y = (double[])y.Clone();
        f = (double[,])f.Clone();

        System.Diagnostics.Debug.Assert(n >= 2 & m >= 2, "BuildBicubicSpline: N<2 or M<2!");

        //
        // Sort points
        //
        for (j = 0; j <= n - 1; j++)
        {
            k = j;
            for (i = j + 1; i <= n - 1; i++)
            {
                if (x[i] < x[k])
                {
                    k = i;
                }
            }
            if (k != j)
            {
                for (i = 0; i <= m - 1; i++)
                {
                    t = f[i,j];
                    f[i,j] = f[i,k];
                    f[i,k] = t;
                }
                t = x[j];
                x[j] = x[k];
                x[k] = t;
            }
        }
        for (i = 0; i <= m - 1; i++)
        {
            k = i;
            for (j = i + 1; j <= m - 1; j++)
            {
                if (y[j] < y[k])
                {
                    k = j;
                }
            }
            if (k != i)
            {
                for (j = 0; j <= n - 1; j++)
                {
                    t = f[i,j];
                    f[i,j] = f[k,j];
                    f[k,j] = t;
                }
                t = y[i];
                y[i] = y[k];
                y[k] = t;
            }
        }

        //
        // Fill C:
        //  C[0]            -   length(C)
        //  C[1]            -   type(C):
        //                      -1 = bilinear interpolant
        //                           (see BuildBilinearInterpolant)
        //                      -3 = general cubic spline
        //  C[2]:
        //      N (x count)
        //  C[3]:
        //      M (y count)
        //  C[4]...C[4+N-1]:
        //      x[i], i = 0...N-1
        //  C[4+N]...C[4+N+M-1]:
        //      y[i], i = 0...M-1
        //  C[4+N+M]...C[4+N+M+(N*M-1)]:
        //      f(i,j) table. f(0,0), f(0, 1), f(0,2) and so on...
        //  C[4+N+M+N*M]...C[4+N+M+(2*N*M-1)]:
        //      df(i,j)/dx table.
        //  C[4+N+M+2*N*M]...C[4+N+M+(3*N*M-1)]:
        //      df(i,j)/dy table.
        //  C[4+N+M+3*N*M]...C[4+N+M+(4*N*M-1)]:
        //      d2f(i,j)/dxdy table.
        //
        tblsize = 4 + n + m + 4 * n * m;
        c = new double[tblsize - 1 + 1];
        c[0] = tblsize;
        c[1] = -3;
        c[2] = n;
        c[3] = m;
        for (i = 0; i <= n - 1; i++)
        {
            c[4 + i] = x[i];
        }
        for (i = 0; i <= m - 1; i++)
        {
            c[4 + n + i] = y[i];
        }
        //bicubiccalcderivatives(ref f, ref x, ref y, m, n, ref dx, ref dy, ref dxy);
        bicubiccalcderivatives(ref f, ref x, ref y, m, n, ref dx, ref dy, ref dxy);

        for (i = 0; i <= m - 1; i++)
        {
            for (j = 0; j <= n - 1; j++)
            {
                shift = i * n + j;
                c[4+n+m+shift] = f[i,j];
                c[4 + n + m + n * m + shift] = dx[i, j];
                c[4 + n + m + 2 * n * m + shift] = dy[i, j];
                c[4 + n + m + 3 * n * m + shift] = dxy[i, j];
            }
        }
    }


    /*************************************************************************
    This subroutine calculates the value of the bilinear or bicubic spline  at
    the given point X.

    Input parameters:
        C   -   coefficients table.
                Built by BuildBilinearSpline or BuildBicubicSpline.
        X, Y-   point

    Result:
        S(x,y)

      -- ALGLIB PROJECT --
         Copyright 05.07.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double splineinterpolation2dDMD(ref double[] c,
        double x,
        double y)
    {
        double result = 0;
        double v = 0;
        double vx = 0;
        double vy = 0;
        double vxy = 0;

        splinedifferentiation2d(ref c, x, y, ref v, ref vx, ref vy, ref vxy);
        result = v;
        return result;
    }














    /*************************************************************************
    This subroutine calculates the value of the bilinear or bicubic spline  at
    the given point X and its derivatives.

    Input parameters:
        C   -   coefficients table.
                Built by BuildBilinearSpline or BuildBicubicSpline.
        X, Y-   point

    Output parameters:
        F   -   S(x,y)
        FX  -   dS(x,y)/dX
        FY  -   dS(x,y)/dY
        FXY -   d2S(x,y)/dXdY

      -- ALGLIB PROJECT --
         Copyright 05.07.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void splinedifferentiation2d(ref double[] c,
        double x,
        double y,
        ref double f,
        ref double fx,
        ref double fy,
        ref double fxy)
    {
        int n = 0;
        int m = 0;
        double t = 0;
        double dt = 0;
        double u = 0;
        double du = 0;
        //int i = 0;
        //int j = 0;
        int ix = 0;
        int iy = 0;
        int l = 0;
        int r = 0;
        int h = 0;
        int shift1 = 0;
        int s1 = 0;
        int s2 = 0;
        int s3 = 0;
        int s4 = 0;
        int sf = 0;
        int sfx = 0;
        int sfy = 0;
        int sfxy = 0;
        double y1 = 0;
        double y2 = 0;
        double y3 = 0;
        double y4 = 0;
        double v = 0;
        double t0 = 0;
        double t1 = 0;
        double t2 = 0;
        double t3 = 0;
        double u0 = 0;
        double u1 = 0;
        double u2 = 0;
        double u3 = 0;

        System.Diagnostics.Debug.Assert((int)Math.Round(c[1])==-1 | (int)Math.Round(c[1])==-3, "TwoDimensionalInterpolation: incorrect C!");
        n = (int)Math.Round(c[2]);
        m = (int)Math.Round(c[3]);
        
        //
        // Binary search in the [ x[0], ..., x[n-2] ] (x[n-1] is not included)
        //
        l = 4;
        r = 4+n-2+1;
        while( l!=r-1 )
        {
            h = (l+r)/2;
            if( c[h]>=x )
            {
                r = h;
            }
            else
            {
                l = h;
            }
        }
        t = (x-c[l])/(c[l+1]-c[l]);
        dt = 1.0/(c[l+1]-c[l]);
        ix = l-4;
        
        //
        // Binary search in the [ y[0], ..., y[m-2] ] (y[m-1] is not included)
        //
        l = 4+n;
        r = 4+n+(m-2)+1;
        while( l!=r-1 )
        {
            h = (l+r)/2;
            if( c[h]>=y )
            {
                r = h;
            }
            else
            {
                l = h;
            }
        }
        u = (y-c[l])/(c[l+1]-c[l]);
        du = 1.0/(c[l+1]-c[l]);
        iy = l-(4+n);
        
        //
        // Prepare F, dF/dX, dF/dY, d2F/dXdY
        //
        f = 0;
        fx = 0;
        fy = 0;
        fxy = 0;
        
        //
        // Bilinear interpolation
        //
        if( (int)Math.Round(c[1])==-1 )
        {
            shift1 = 4+n+m;
            y1 = c[shift1+n*iy+ix];
            y2 = c[shift1+n*iy+(ix+1)];
            y3 = c[shift1+n*(iy+1)+(ix+1)];
            y4 = c[shift1+n*(iy+1)+ix];
            f = (1-t)*(1-u)*y1+t*(1-u)*y2+t*u*y3+(1-t)*u*y4;
            fx = (-((1-u)*y1)+(1-u)*y2+u*y3-u*y4)*dt;
            fy = (-((1-t)*y1)-t*y2+t*y3+(1-t)*y4)*du;
            fxy = (y1-y2+y3-y4)*du*dt;
            return;
        }
        
        //
        // Bicubic interpolation
        //
        if( (int)Math.Round(c[1])==-3 )
        {
            
            //
            // Prepare info
            //
            t0 = 1;
            t1 = t;
            t2 = AP.Math.Sqr(t);
            t3 = t*t2;
            u0 = 1;
            u1 = u;
            u2 = AP.Math.Sqr(u);
            u3 = u*u2;
            sf = 4+n+m;
            sfx = 4+n+m+n*m;
            sfy = 4+n+m+2*n*m;
            sfxy = 4+n+m+3*n*m;
            s1 = n*iy+ix;
            s2 = n*iy+(ix+1);
            s3 = n*(iy+1)+(ix+1);
            s4 = n*(iy+1)+ix;
            
            //
            // Calculate
            //
            v = +(1*c[sf+s1]);
            f = f+v*t0*u0;
            v = +(1*c[sfy+s1]/du);
            f = f+v*t0*u1;
            fy = fy+1*v*t0*u0*du;
            v = -(3*c[sf+s1])+3*c[sf+s4]-2*c[sfy+s1]/du-1*c[sfy+s4]/du;
            f = f+v*t0*u2;
            fy = fy+2*v*t0*u1*du;
            v = +(2*c[sf+s1])-2*c[sf+s4]+1*c[sfy+s1]/du+1*c[sfy+s4]/du;
            f = f+v*t0*u3;
            fy = fy+3*v*t0*u2*du;
            v = +(1*c[sfx+s1]/dt);
            f = f+v*t1*u0;
            fx = fx+1*v*t0*u0*dt;
            v = +(1*c[sfxy+s1]/(dt*du));
            f = f+v*t1*u1;
            fx = fx+1*v*t0*u1*dt;
            fy = fy+1*v*t1*u0*du;
            fxy = fxy+1*v*t0*u0*dt*du;
            v = -(3*c[sfx+s1]/dt)+3*c[sfx+s4]/dt-2*c[sfxy+s1]/(dt*du)-1*c[sfxy+s4]/(dt*du);
            f = f+v*t1*u2;
            fx = fx+1*v*t0*u2*dt;
            fy = fy+2*v*t1*u1*du;
            fxy = fxy+2*v*t0*u1*dt*du;
            v = +(2*c[sfx+s1]/dt)-2*c[sfx+s4]/dt+1*c[sfxy+s1]/(dt*du)+1*c[sfxy+s4]/(dt*du);
            f = f+v*t1*u3;
            fx = fx+1*v*t0*u3*dt;
            fy = fy+3*v*t1*u2*du;
            fxy = fxy+3*v*t0*u2*dt*du;
            v = -(3*c[sf+s1])+3*c[sf+s2]-2*c[sfx+s1]/dt-1*c[sfx+s2]/dt;
            f = f+v*t2*u0;
            fx = fx+2*v*t1*u0*dt;
            v = -(3*c[sfy+s1]/du)+3*c[sfy+s2]/du-2*c[sfxy+s1]/(dt*du)-1*c[sfxy+s2]/(dt*du);
            f = f+v*t2*u1;
            fx = fx+2*v*t1*u1*dt;
            fy = fy+1*v*t2*u0*du;
            fxy = fxy+2*v*t1*u0*dt*du;
            v = +(9*c[sf+s1])-9*c[sf+s2]+9*c[sf+s3]-9*c[sf+s4]+6*c[sfx+s1]/dt+3*c[sfx+s2]/dt-3*c[sfx+s3]/dt-6*c[sfx+s4]/dt+6*c[sfy+s1]/du-6*c[sfy+s2]/du-3*c[sfy+s3]/du+3*c[sfy+s4]/du+4*c[sfxy+s1]/(dt*du)+2*c[sfxy+s2]/(dt*du)+1*c[sfxy+s3]/(dt*du)+2*c[sfxy+s4]/(dt*du);
            f = f+v*t2*u2;
            fx = fx+2*v*t1*u2*dt;
            fy = fy+2*v*t2*u1*du;
            fxy = fxy+4*v*t1*u1*dt*du;
            v = -(6*c[sf+s1])+6*c[sf+s2]-6*c[sf+s3]+6*c[sf+s4]-4*c[sfx+s1]/dt-2*c[sfx+s2]/dt+2*c[sfx+s3]/dt+4*c[sfx+s4]/dt-3*c[sfy+s1]/du+3*c[sfy+s2]/du+3*c[sfy+s3]/du-3*c[sfy+s4]/du-2*c[sfxy+s1]/(dt*du)-1*c[sfxy+s2]/(dt*du)-1*c[sfxy+s3]/(dt*du)-2*c[sfxy+s4]/(dt*du);
            f = f+v*t2*u3;
            fx = fx+2*v*t1*u3*dt;
            fy = fy+3*v*t2*u2*du;
            fxy = fxy+6*v*t1*u2*dt*du;
            v = +(2*c[sf+s1])-2*c[sf+s2]+1*c[sfx+s1]/dt+1*c[sfx+s2]/dt;
            f = f+v*t3*u0;
            fx = fx+3*v*t2*u0*dt;
            v = +(2*c[sfy+s1]/du)-2*c[sfy+s2]/du+1*c[sfxy+s1]/(dt*du)+1*c[sfxy+s2]/(dt*du);
            f = f+v*t3*u1;
            fx = fx+3*v*t2*u1*dt;
            fy = fy+1*v*t3*u0*du;
            fxy = fxy+3*v*t2*u0*dt*du;
            v = -(6*c[sf+s1])+6*c[sf+s2]-6*c[sf+s3]+6*c[sf+s4]-3*c[sfx+s1]/dt-3*c[sfx+s2]/dt+3*c[sfx+s3]/dt+3*c[sfx+s4]/dt-4*c[sfy+s1]/du+4*c[sfy+s2]/du+2*c[sfy+s3]/du-2*c[sfy+s4]/du-2*c[sfxy+s1]/(dt*du)-2*c[sfxy+s2]/(dt*du)-1*c[sfxy+s3]/(dt*du)-1*c[sfxy+s4]/(dt*du);
            f = f+v*t3*u2;
            fx = fx+3*v*t2*u2*dt;
            fy = fy+2*v*t3*u1*du;
            fxy = fxy+6*v*t2*u1*dt*du;
            v = +(4*c[sf+s1])-4*c[sf+s2]+4*c[sf+s3]-4*c[sf+s4]+2*c[sfx+s1]/dt+2*c[sfx+s2]/dt-2*c[sfx+s3]/dt-2*c[sfx+s4]/dt+2*c[sfy+s1]/du-2*c[sfy+s2]/du-2*c[sfy+s3]/du+2*c[sfy+s4]/du+1*c[sfxy+s1]/(dt*du)+1*c[sfxy+s2]/(dt*du)+1*c[sfxy+s3]/(dt*du)+1*c[sfxy+s4]/(dt*du);
            f = f+v*t3*u3;
            fx = fx+3*v*t2*u3*dt;
            fy = fy+3*v*t3*u2*du;
            fxy = fxy+9*v*t2*u2*dt*du;
            return;
        }
    }


    /*************************************************************************
    This subroutine unpacks two-dimensional spline into the coefficients table

    Input parameters:
        C   -   coefficients table.

    Result:
        M, N-   grid size (x-axis and y-axis)
        Tbl -   coefficients table, unpacked format,
                [0..(N-1)*(M-1)-1, 0..19].
                For I = 0...M-2, J=0..N-2:
                    K =  I*(N-1)+J
                    Tbl[K,0] = X[j]
                    Tbl[K,1] = X[j+1]
                    Tbl[K,2] = Y[i]
                    Tbl[K,3] = Y[i+1]
                    Tbl[K,4] = C00
                    Tbl[K,5] = C01
                    Tbl[K,6] = C02
                    Tbl[K,7] = C03
                    Tbl[K,8] = C10
                    Tbl[K,9] = C11
                    ...
                    Tbl[K,19] = C33
                On each grid square spline is equals to:
                    S(x) = SUM(c[i,j]*(x^i)*(y^j), i=0..3, j=0..3)
                    t = x-x[j]
                    u = y-y[i]

      -- ALGLIB PROJECT --
         Copyright 29.06.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void splineunpack2d(ref double[] c,
        ref int m,
        ref int n,
        ref double[,] tbl)
    {
        int i = 0;
        int j = 0;
        int ci = 0;
        int cj = 0;
        int k = 0;
        int p = 0;
        int shift = 0;
        int s1 = 0;
        int s2 = 0;
        int s3 = 0;
        int s4 = 0;
        int sf = 0;
        int sfx = 0;
        int sfy = 0;
        int sfxy = 0;
        double y1 = 0;
        double y2 = 0;
        double y3 = 0;
        double y4 = 0;
        double dt = 0;
        double du = 0;

        System.Diagnostics.Debug.Assert((int)Math.Round(c[1])==-3 | (int)Math.Round(c[1])==-1, "SplineUnpack2D: incorrect C!");
        n = (int)Math.Round(c[2]);
        m = (int)Math.Round(c[3]);
        tbl = new double[(n-1)*(m-1)-1+1, 19+1];
        
        //
        // Fill
        //
        for(i=0; i<=m-2; i++)
        {
            for(j=0; j<=n-2; j++)
            {
                p = i*(n-1)+j;
                tbl[p,0] = c[4+j];
                tbl[p,1] = c[4+j+1];
                tbl[p,2] = c[4+n+i];
                tbl[p,3] = c[4+n+i+1];
                dt = 1/(tbl[p,1]-tbl[p,0]);
                du = 1/(tbl[p,3]-tbl[p,2]);
                
                //
                // Bilinear interpolation
                //
                if( (int)Math.Round(c[1])==-1 )
                {
                    for(k=4; k<=19; k++)
                    {
                        tbl[p,k] = 0;
                    }
                    shift = 4+n+m;
                    y1 = c[shift+n*i+j];
                    y2 = c[shift+n*i+(j+1)];
                    y3 = c[shift+n*(i+1)+(j+1)];
                    y4 = c[shift+n*(i+1)+j];
                    tbl[p,4] = y1;
                    tbl[p,4+1*4+0] = y2-y1;
                    tbl[p,4+0*4+1] = y4-y1;
                    tbl[p,4+1*4+1] = y3-y2-y4+y1;
                }
                
                //
                // Bicubic interpolation
                //
                if( (int)Math.Round(c[1])==-3 )
                {
                    sf = 4+n+m;
                    sfx = 4+n+m+n*m;
                    sfy = 4+n+m+2*n*m;
                    sfxy = 4+n+m+3*n*m;
                    s1 = n*i+j;
                    s2 = n*i+(j+1);
                    s3 = n*(i+1)+(j+1);
                    s4 = n*(i+1)+j;
                    tbl[p,4+0*4+0] = +(1*c[sf+s1]);
                    tbl[p,4+0*4+1] = +(1*c[sfy+s1]/du);
                    tbl[p,4+0*4+2] = -(3*c[sf+s1])+3*c[sf+s4]-2*c[sfy+s1]/du-1*c[sfy+s4]/du;
                    tbl[p,4+0*4+3] = +(2*c[sf+s1])-2*c[sf+s4]+1*c[sfy+s1]/du+1*c[sfy+s4]/du;
                    tbl[p,4+1*4+0] = +(1*c[sfx+s1]/dt);
                    tbl[p,4+1*4+1] = +(1*c[sfxy+s1]/(dt*du));
                    tbl[p,4+1*4+2] = -(3*c[sfx+s1]/dt)+3*c[sfx+s4]/dt-2*c[sfxy+s1]/(dt*du)-1*c[sfxy+s4]/(dt*du);
                    tbl[p,4+1*4+3] = +(2*c[sfx+s1]/dt)-2*c[sfx+s4]/dt+1*c[sfxy+s1]/(dt*du)+1*c[sfxy+s4]/(dt*du);
                    tbl[p,4+2*4+0] = -(3*c[sf+s1])+3*c[sf+s2]-2*c[sfx+s1]/dt-1*c[sfx+s2]/dt;
                    tbl[p,4+2*4+1] = -(3*c[sfy+s1]/du)+3*c[sfy+s2]/du-2*c[sfxy+s1]/(dt*du)-1*c[sfxy+s2]/(dt*du);
                    tbl[p,4+2*4+2] = +(9*c[sf+s1])-9*c[sf+s2]+9*c[sf+s3]-9*c[sf+s4]+6*c[sfx+s1]/dt+3*c[sfx+s2]/dt-3*c[sfx+s3]/dt-6*c[sfx+s4]/dt+6*c[sfy+s1]/du-6*c[sfy+s2]/du-3*c[sfy+s3]/du+3*c[sfy+s4]/du+4*c[sfxy+s1]/(dt*du)+2*c[sfxy+s2]/(dt*du)+1*c[sfxy+s3]/(dt*du)+2*c[sfxy+s4]/(dt*du);
                    tbl[p,4+2*4+3] = -(6*c[sf+s1])+6*c[sf+s2]-6*c[sf+s3]+6*c[sf+s4]-4*c[sfx+s1]/dt-2*c[sfx+s2]/dt+2*c[sfx+s3]/dt+4*c[sfx+s4]/dt-3*c[sfy+s1]/du+3*c[sfy+s2]/du+3*c[sfy+s3]/du-3*c[sfy+s4]/du-2*c[sfxy+s1]/(dt*du)-1*c[sfxy+s2]/(dt*du)-1*c[sfxy+s3]/(dt*du)-2*c[sfxy+s4]/(dt*du);
                    tbl[p,4+3*4+0] = +(2*c[sf+s1])-2*c[sf+s2]+1*c[sfx+s1]/dt+1*c[sfx+s2]/dt;
                    tbl[p,4+3*4+1] = +(2*c[sfy+s1]/du)-2*c[sfy+s2]/du+1*c[sfxy+s1]/(dt*du)+1*c[sfxy+s2]/(dt*du);
                    tbl[p,4+3*4+2] = -(6*c[sf+s1])+6*c[sf+s2]-6*c[sf+s3]+6*c[sf+s4]-3*c[sfx+s1]/dt-3*c[sfx+s2]/dt+3*c[sfx+s3]/dt+3*c[sfx+s4]/dt-4*c[sfy+s1]/du+4*c[sfy+s2]/du+2*c[sfy+s3]/du-2*c[sfy+s4]/du-2*c[sfxy+s1]/(dt*du)-2*c[sfxy+s2]/(dt*du)-1*c[sfxy+s3]/(dt*du)-1*c[sfxy+s4]/(dt*du);
                    tbl[p,4+3*4+3] = +(4*c[sf+s1])-4*c[sf+s2]+4*c[sf+s3]-4*c[sf+s4]+2*c[sfx+s1]/dt+2*c[sfx+s2]/dt-2*c[sfx+s3]/dt-2*c[sfx+s4]/dt+2*c[sfy+s1]/du-2*c[sfy+s2]/du-2*c[sfy+s3]/du+2*c[sfy+s4]/du+1*c[sfxy+s1]/(dt*du)+1*c[sfxy+s2]/(dt*du)+1*c[sfxy+s3]/(dt*du)+1*c[sfxy+s4]/(dt*du);
                }
                
                //
                // Rescale Cij
                //
                for(ci=0; ci<=3; ci++)
                {
                    for(cj=0; cj<=3; cj++)
                    {
                        tbl[p,4+ci*4+cj] = tbl[p,4+ci*4+cj]*Math.Pow(dt, ci)*Math.Pow(du, cj);
                    }
                }
            }
        }
    }


    /*************************************************************************
    This subroutine performs linear transformation of the spline argument.

    Input parameters:
        C       -   coefficients table.
        AX, BX  -   transformation coefficients: x = A*t + B
        AY, BY  -   transformation coefficients: y = A*u + B
    Result:
        C   -   transformed spline

      -- ALGLIB PROJECT --
         Copyright 30.06.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void spline2dlintransxy(ref double[] c,
        double ax,
        double bx,
        double ay,
        double by)
    {
        int i = 0;
        int j = 0;
        int n = 0;
        int m = 0;
        double v = 0;
        double[] x = new double[0];
        double[] y = new double[0];
        double[,] f = new double[0,0];
        int typec = 0;

        typec = (int)Math.Round(c[1]);
        System.Diagnostics.Debug.Assert(typec==-3 | typec==-1, "Spline2DLinTransXY: incorrect C!");
        n = (int)Math.Round(c[2]);
        m = (int)Math.Round(c[3]);
        x = new double[n-1+1];
        y = new double[m-1+1];
        f = new double[m-1+1, n-1+1];
        for(j=0; j<=n-1; j++)
        {
            x[j] = c[4+j];
        }
        for(i=0; i<=m-1; i++)
        {
            y[i] = c[4+n+i];
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                f[i,j] = c[4+n+m+i*n+j];
            }
        }
        
        //
        // Special case: AX=0 or AY=0
        //
        if( ax==0 )
        {
            for(i=0; i<=m-1; i++)
            {
                v = splineinterpolation2d(ref c, bx, y[i]);
                for(j=0; j<=n-1; j++)
                {
                    f[i,j] = v;
                }
            }
            if( typec==-3 )
            {
                buildbicubicspline(x, y, f, m, n, ref c);
            }
            if( typec==-1 )
            {
                buildbilinearspline(x, y, f, m, n, ref c);
            }
            ax = 1;
            bx = 0;
        }
        if( ay==0 )
        {
            for(j=0; j<=n-1; j++)
            {
                v = splineinterpolation2d(ref c, x[j], by);
                for(i=0; i<=m-1; i++)
                {
                    f[i,j] = v;
                }
            }
            if( typec==-3 )
            {
                buildbicubicspline(x, y, f, m, n, ref c);
            }
            if( typec==-1 )
            {
                buildbilinearspline(x, y, f, m, n, ref c);
            }
            ay = 1;
            by = 0;
        }
        
        //
        // General case: AX<>0, AY<>0
        // Unpack, scale and pack again.
        //
        for(j=0; j<=n-1; j++)
        {
            x[j] = (x[j]-bx)/ax;
        }
        for(i=0; i<=m-1; i++)
        {
            y[i] = (y[i]-by)/ay;
        }
        if( typec==-3 )
        {
            buildbicubicspline(x, y, f, m, n, ref c);
        }
        if( typec==-1 )
        {
            buildbilinearspline(x, y, f, m, n, ref c);
        }
    }


    /*************************************************************************
    This subroutine performs linear transformation of the spline.

    Input parameters:
        C   -   coefficients table. Built by BuildLinearSpline,
                BuildHermiteSpline, BuildCubicSpline, BuildAkimaSpline.
        A, B-   transformation coefficients: S2(x,y) = A*S(x,y) + B
        
    Output parameters:
        C   -   transformed spline

      -- ALGLIB PROJECT --
         Copyright 30.06.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void spline2dlintransf(ref double[] c,
        double a,
        double b)
    {
        int i = 0;
        int j = 0;
        int n = 0;
        int m = 0;
        //double v = 0;
        double[] x = new double[0];
        double[] y = new double[0];
        double[,] f = new double[0,0];
        int typec = 0;

        typec = (int)Math.Round(c[1]);
        System.Diagnostics.Debug.Assert(typec==-3 | typec==-1, "Spline2DLinTransXY: incorrect C!");
        n = (int)Math.Round(c[2]);
        m = (int)Math.Round(c[3]);
        x = new double[n-1+1];
        y = new double[m-1+1];
        f = new double[m-1+1, n-1+1];
        for(j=0; j<=n-1; j++)
        {
            x[j] = c[4+j];
        }
        for(i=0; i<=m-1; i++)
        {
            y[i] = c[4+n+i];
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                f[i,j] = a*c[4+n+m+i*n+j]+b;
            }
        }
        if( typec==-3 )
        {
            buildbicubicspline(x, y, f, m, n, ref c);
        }
        if( typec==-1 )
        {
            buildbilinearspline(x, y, f, m, n, ref c);
        }
    }


    /*************************************************************************
    This subroutine makes the copy of the spline.

    Input parameters:
        C   -   coefficients table.

    Output parameters:
        CC  -   spline copy

      -- ALGLIB PROJECT --
         Copyright 29.06.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void spline2dcopy(ref double[] c,
        ref double[] cc)
    {
        spline3.splinecopy(ref c, ref cc);
    }


    /*************************************************************************
    Bicubic spline resampling

    Input parameters:
        A           -   function values at the old grid,
                        array[0..OldHeight-1, 0..OldWidth-1]
        OldHeight   -   old grid height, OldHeight>1
        OldWidth    -   old grid width, OldWidth>1
        NewHeight   -   new grid height, NewHeight>1
        NewWidth    -   new grid width, NewWidth>1
        
    Output parameters:
        B           -   function values at the new grid,
                        array[0..NewHeight-1, 0..NewWidth-1]

      -- ALGLIB routine --
         15 May, 2007
         Copyright by Bochkanov Sergey
    *************************************************************************/
    public static void bicubicresamplecartesian(ref double[,] a,
        int oldheight,
        int oldwidth,
        ref double[,] b,
        int newheight,
        int newwidth)
    {
        double[,] buf = new double[0,0];
        double[] x = new double[0];
        double[] y = new double[0];
        double[] c = new double[0];
        int i = 0;
        int j = 0;
        int mw = 0;
        int mh = 0;

        System.Diagnostics.Debug.Assert(oldwidth>1 & oldheight>1, "BicubicResampleCartesian: width/height less than 1");
        System.Diagnostics.Debug.Assert(newwidth>1 & newheight>1, "BicubicResampleCartesian: width/height less than 1");
        
        //
        // Prepare
        //
        mw = Math.Max(oldwidth, newwidth);
        mh = Math.Max(oldheight, newheight);
        b = new double[newheight-1+1, newwidth-1+1];
        buf = new double[oldheight-1+1, newwidth-1+1];
        x = new double[Math.Max(mw, mh)-1+1];
        y = new double[Math.Max(mw, mh)-1+1];
        
        //
        // Horizontal interpolation
        //
        for(i=0; i<=oldheight-1; i++)
        {
            
            //
            // Fill X, Y
            //
            for(j=0; j<=oldwidth-1; j++)
            {
                x[j] = (double)(j)/((double)(oldwidth-1));
                y[j] = a[i,j];
            }
            
            //
            // Interpolate and place result into temporary matrix
            //
            spline3.buildcubicspline(x, y, oldwidth, 0, 0.0, 0, 0.0, ref c);
            for(j=0; j<=newwidth-1; j++)
            {
                buf[i,j] = spline3.splineinterpolation(ref c, (double)(j)/((double)(newwidth-1)));
            }
        }
        
        //
        // Vertical interpolation
        //
        for(j=0; j<=newwidth-1; j++)
        {
            
            //
            // Fill X, Y
            //
            for(i=0; i<=oldheight-1; i++)
            {
                x[i] = (double)(i)/((double)(oldheight-1));
                y[i] = buf[i,j];
            }
            
            //
            // Interpolate and place result into B
            //
            spline3.buildcubicspline(x, y, oldheight, 0, 0.0, 0, 0.0, ref c);
            for(i=0; i<=newheight-1; i++)
            {
                b[i,j] = spline3.splineinterpolation(ref c, (double)(i)/((double)(newheight-1)));
            }
        }
    }


    /*************************************************************************
    Bilinear spline resampling

    Input parameters:
        A           -   function values at the old grid,
                        array[0..OldHeight-1, 0..OldWidth-1]
        OldHeight   -   old grid height, OldHeight>1
        OldWidth    -   old grid width, OldWidth>1
        NewHeight   -   new grid height, NewHeight>1
        NewWidth    -   new grid width, NewWidth>1

    Output parameters:
        B           -   function values at the new grid,
                        array[0..NewHeight-1, 0..NewWidth-1]

      -- ALGLIB routine --
         09.07.2007
         Copyright by Bochkanov Sergey
    *************************************************************************/
    public static void bilinearresamplecartesian(ref double[,] a,
        int oldheight,
        int oldwidth,
        ref double[,] b,
        int newheight,
        int newwidth)
    {
        int i = 0;
        int j = 0;
        int l = 0;
        int c = 0;
        double t = 0;
        double u = 0;

        b = new double[newheight-1+1, newwidth-1+1];
        for(i=0; i<=newheight-1; i++)
        {
            for(j=0; j<=newwidth-1; j++)
            {
                l = i*(oldheight-1)/(newheight-1);
                if( l==oldheight-1 )
                {
                    l = oldheight-2;
                }
                u = (double)(i)/((double)(newheight-1))*(oldheight-1)-l;
                c = j*(oldwidth-1)/(newwidth-1);
                if( c==oldwidth-1 )
                {
                    c = oldwidth-2;
                }
                t = (double)(j*(oldwidth-1))/((double)(newwidth-1))-c;
                b[i,j] = (1-t)*(1-u)*a[l,c]+t*(1-u)*a[l,c+1]+t*u*a[l+1,c+1]+(1-t)*u*a[l+1,c];
            }
        }
    }


    /*************************************************************************
    Obsolete subroutine for backwards compatibility
    *************************************************************************/
    public static void bicubicresample(int oldwidth,
        int oldheight,
        int newwidth,
        int newheight,
        ref double[,] a,
        ref double[,] b)
    {
        bicubicresamplecartesian(ref a, oldheight, oldwidth, ref b, newheight, newwidth);
    }


    /*************************************************************************
    Obsolete subroutine for backwards compatibility
    *************************************************************************/
    public static void bilinearresample(int oldwidth,
        int oldheight,
        int newwidth,
        int newheight,
        ref double[,] a,
        ref double[,] b)
    {
        bilinearresamplecartesian(ref a, oldheight, oldwidth, ref b, newheight, newwidth);
    }


    /*************************************************************************
    Internal subroutine.
    Calculation of the first derivatives and the cross-derivative.
    *************************************************************************/
    private static void bicubiccalcderivatives(ref double[,] a,
        ref double[] x,
        ref double[] y,
        int m,
        int n,
        ref double[,] dx,
        ref double[,] dy,
        ref double[,] dxy)
    {
        int i = 0;
        int j = 0;
        //int k = 0;
        double[] xt = new double[0];
        double[] ft = new double[0];
        double[] c = new double[0];
        double s = 0;
        double ds = 0;
        double d2s = 0;
        //double v = 0;

        dx = new double[m-1+1, n-1+1];
        dy = new double[m-1+1, n-1+1];
        dxy = new double[m-1+1, n-1+1];
        
        //
        // dF/dX
        //
        xt = new double[n-1+1];
        ft = new double[n-1+1];
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                xt[j] = x[j];
                ft[j] = a[i,j];
            }
            spline3.buildcubicspline(xt, ft, n, 0, 0.0, 0, 0.0, ref c);
            for(j=0; j<=n-1; j++)
            {
                spline3.splinedifferentiation(ref c, x[j], ref s, ref ds, ref d2s);
                dx[i,j] = ds;
            }
        }
        
        //
        // dF/dY
        //
        xt = new double[m-1+1];
        ft = new double[m-1+1];
        for(j=0; j<=n-1; j++)
        {
            for(i=0; i<=m-1; i++)
            {
                xt[i] = y[i];
                ft[i] = a[i,j];
            }
            spline3.buildcubicspline(xt, ft, m, 0, 0.0, 0, 0.0, ref c);
            for(i=0; i<=m-1; i++)
            {
                spline3.splinedifferentiation(ref c, y[i], ref s, ref ds, ref d2s);
                dy[i,j] = ds;
            }
        }
        
        //
        // d2F/dXdY
        //
        xt = new double[n-1+1];
        ft = new double[n-1+1];
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                xt[j] = x[j];
                ft[j] = dy[i,j];
            }
            spline3.buildcubicspline(xt, ft, n, 0, 0.0, 0, 0.0, ref c);
            for(j=0; j<=n-1; j++)
            {
                spline3.splinedifferentiation(ref c, x[j], ref s, ref ds, ref d2s);
                dxy[i,j] = ds;
            }
        }
    }






    private static void bicubiccalcderivatives(ref double[][] a,
        ref double[] x,
        ref double[] y,
        int m,
        int n,
        ref double[,] dx,
        ref double[,] dy,
        ref double[,] dxy)
    {
        int i = 0;
        int j = 0;
        //int k = 0;
        double[] xt = new double[0];
        double[] ft = new double[0];
        double[] c = new double[0];
        double s = 0;
        double ds = 0;
        double d2s = 0;
        //double v = 0;

        dx = new double[m - 1 + 1, n - 1 + 1];
        dy = new double[m - 1 + 1, n - 1 + 1];
        dxy = new double[m - 1 + 1, n - 1 + 1];

        //
        // dF/dX
        //
        xt = new double[n - 1 + 1];
        ft = new double[n - 1 + 1];
        for (i = 0; i <= m - 1; i++)
        {
            for (j = 0; j <= n - 1; j++)
            {
                xt[j] = x[j];
                ft[j] = a[i][j];
            }
            spline3.buildcubicspline(xt, ft, n, 0, 0.0, 0, 0.0, ref c);
            for (j = 0; j <= n - 1; j++)
            {
                spline3.splinedifferentiation(ref c, x[j], ref s, ref ds, ref d2s);
                dx[i, j] = ds;
            }
        }

        //
        // dF/dY
        //
        xt = new double[m - 1 + 1];
        ft = new double[m - 1 + 1];
        for (j = 0; j <= n - 1; j++)
        {
            for (i = 0; i <= m - 1; i++)
            {
                xt[i] = y[i];
                ft[i] = a[i][j];
            }
            spline3.buildcubicspline(xt, ft, m, 0, 0.0, 0, 0.0, ref c);
            for (i = 0; i <= m - 1; i++)
            {
                spline3.splinedifferentiation(ref c, y[i], ref s, ref ds, ref d2s);
                dy[i, j] = ds;
            }
        }

        //
        // d2F/dXdY
        //
        xt = new double[n - 1 + 1];
        ft = new double[n - 1 + 1];
        for (i = 0; i <= m - 1; i++)
        {
            for (j = 0; j <= n - 1; j++)
            {
                xt[j] = x[j];
                ft[j] = dy[i, j];
            }
            spline3.buildcubicspline(xt, ft, n, 0, 0.0, 0, 0.0, ref c);
            for (j = 0; j <= n - 1; j++)
            {
                spline3.splinedifferentiation(ref c, x[j], ref s, ref ds, ref d2s);
                dxy[i, j] = ds;
            }
        }
    }

}
