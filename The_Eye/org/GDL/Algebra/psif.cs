/*************************************************************************
Cephes Math Library Release 2.8:  June, 2000
Copyright by Stephen L. Moshier

Contributors:
    * Sergey Bochkanov (ALGLIB project). Translation from C to
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

class psif
{
    /*************************************************************************
    Psi (digamma) function

                 d      -
      psi(x)  =  -- ln | (x)
                 dx

    is the logarithmic derivative of the gamma function.
    For integer x,
                      n-1
                       -
    psi(n) = -EUL  +   >  1/k.
                       -
                      k=1

    This formula is used for 0 < n <= 10.  If x is negative, it
    is transformed to a positive argument by the reflection
    formula  psi(1-x) = psi(x) + pi cot(pi x).
    For general positive x, the argument is made greater than 10
    using the recurrence  psi(x+1) = psi(x) + 1/x.
    Then the following asymptotic expansion is applied:

                              inf.   B
                               -      2k
    psi(x) = log(x) - 1/2x -   >   -------
                               -        2k
                              k=1   2k x

    where the B2k are Bernoulli numbers.

    ACCURACY:
       Relative error (except absolute when |psi| < 1):
    arithmetic   domain     # trials      peak         rms
       IEEE      0,30        30000       1.3e-15     1.4e-16
       IEEE      -30,0       40000       1.5e-15     2.2e-16

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1992, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double psi(double x)
    {
        double result = 0;
        double p = 0;
        double q = 0;
        double nz = 0;
        double s = 0;
        double w = 0;
        double y = 0;
        double z = 0;
        double polv = 0;
        int i = 0;
        int n = 0;
        int negative = 0;

        negative = 0;
        nz = 0.0;
        if( x<=0 )
        {
            negative = 1;
            q = x;
            p = (int)Math.Floor(q);
            if( p==q )
            {
                System.Diagnostics.Debug.Assert(false, "Singularity in Psi(x)");
                result = AP.Math.MaxRealNumber;
                return result;
            }
            nz = q-p;
            if( nz!=0.5 )
            {
                if( nz>0.5 )
                {
                    p = p+1.0;
                    nz = q-p;
                }
                nz = Math.PI/Math.Tan(Math.PI*nz);
            }
            else
            {
                nz = 0.0;
            }
            x = 1.0-x;
        }
        if( x<=10.0 & x==(int)Math.Floor(x) )
        {
            y = 0.0;
            n = (int)Math.Floor(x);
            for(i=1; i<=n-1; i++)
            {
                w = i;
                y = y+1.0/w;
            }
            y = y-0.57721566490153286061;
        }
        else
        {
            s = x;
            w = 0.0;
            while( s<10.0 )
            {
                w = w+1.0/s;
                s = s+1.0;
            }
            if( s<1.0E17 )
            {
                z = 1.0/(s*s);
                polv = 8.33333333333333333333E-2;
                polv = polv*z-2.10927960927960927961E-2;
                polv = polv*z+7.57575757575757575758E-3;
                polv = polv*z-4.16666666666666666667E-3;
                polv = polv*z+3.96825396825396825397E-3;
                polv = polv*z-8.33333333333333333333E-3;
                polv = polv*z+8.33333333333333333333E-2;
                y = z*polv;
            }
            else
            {
                y = 0.0;
            }
            y = Math.Log(s)-0.5/s-y-w;
        }
        if( negative!=0 )
        {
            y = y-nz;
        }
        result = y;
        return result;
    }
}
