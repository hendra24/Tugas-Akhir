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

class betaf
{
    /*************************************************************************
    Beta function


                      -     -
                     | (a) | (b)
    beta( a, b )  =  -----------.
                        -
                       | (a+b)

    For large arguments the logarithm of the function is
    evaluated using lgam(), then exponentiated.

    ACCURACY:

                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE       0,30       30000       8.1e-14     1.1e-14

    Cephes Math Library Release 2.0:  April, 1987
    Copyright 1984, 1987 by Stephen L. Moshier
    *************************************************************************/
    public static double beta(double a,
        double b)
    {
        double result = 0;
        double y = 0;
        double sg = 0;
        double s = 0;

        sg = 1;
        System.Diagnostics.Debug.Assert(a>0 | a!=(int)Math.Floor(a), "Overflow in Beta");
        System.Diagnostics.Debug.Assert(b>0 | b!=(int)Math.Floor(b), "Overflow in Beta");
        y = a+b;
        if( Math.Abs(y)>171.624376956302725 )
        {
            y = gammaf.lngamma(y, ref s);
            sg = sg*s;
            y = gammaf.lngamma(b, ref s)-y;
            sg = sg*s;
            y = gammaf.lngamma(a, ref s)+y;
            sg = sg*s;
            System.Diagnostics.Debug.Assert(y<=Math.Log(AP.Math.MaxRealNumber), "Overflow in Beta");
            result = sg*Math.Exp(y);
            return result;
        }
        y = gammaf.gamma(y);
        System.Diagnostics.Debug.Assert(y!=0, "Overflow in Beta");
        if( a>b )
        {
            y = gammaf.gamma(a)/y;
            y = y*gammaf.gamma(b);
        }
        else
        {
            y = gammaf.gamma(b)/y;
            y = y*gammaf.gamma(a);
        }
        result = y;
        return result;
    }
}
