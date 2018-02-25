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

class dawson
{
    /*************************************************************************
    Dawson's Integral

    Approximates the integral

                                x
                                -
                         2     | |        2
     dawsn(x)  =  exp( -x  )   |    exp( t  ) dt
                             | |
                              -
                              0

    Three different rational approximations are employed, for
    the intervals 0 to 3.25; 3.25 to 6.25; and 6.25 up.

    ACCURACY:

                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE      0,10        10000       6.9e-16     1.0e-16

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double dawsonintegral(double x)
    {
        double result = 0;
        double x2 = 0;
        double y = 0;
        int sg = 0;
        double an = 0;
        double ad = 0;
        double bn = 0;
        double bd = 0;
        double cn = 0;
        double cd = 0;

        sg = 1;
        if( x<0 )
        {
            sg = -1;
            x = -x;
        }
        if( x<3.25 )
        {
            x2 = x*x;
            an = 1.13681498971755972054E-11;
            an = an*x2+8.49262267667473811108E-10;
            an = an*x2+1.94434204175553054283E-8;
            an = an*x2+9.53151741254484363489E-7;
            an = an*x2+3.07828309874913200438E-6;
            an = an*x2+3.52513368520288738649E-4;
            an = an*x2+-8.50149846724410912031E-4;
            an = an*x2+4.22618223005546594270E-2;
            an = an*x2+-9.17480371773452345351E-2;
            an = an*x2+9.99999999999999994612E-1;
            ad = 2.40372073066762605484E-11;
            ad = ad*x2+1.48864681368493396752E-9;
            ad = ad*x2+5.21265281010541664570E-8;
            ad = ad*x2+1.27258478273186970203E-6;
            ad = ad*x2+2.32490249820789513991E-5;
            ad = ad*x2+3.25524741826057911661E-4;
            ad = ad*x2+3.48805814657162590916E-3;
            ad = ad*x2+2.79448531198828973716E-2;
            ad = ad*x2+1.58874241960120565368E-1;
            ad = ad*x2+5.74918629489320327824E-1;
            ad = ad*x2+1.00000000000000000539E0;
            y = x*an/ad;
            result = sg*y;
            return result;
        }
        x2 = 1.0/(x*x);
        if( x<6.25 )
        {
            bn = 5.08955156417900903354E-1;
            bn = bn*x2-2.44754418142697847934E-1;
            bn = bn*x2+9.41512335303534411857E-2;
            bn = bn*x2-2.18711255142039025206E-2;
            bn = bn*x2+3.66207612329569181322E-3;
            bn = bn*x2-4.23209114460388756528E-4;
            bn = bn*x2+3.59641304793896631888E-5;
            bn = bn*x2-2.14640351719968974225E-6;
            bn = bn*x2+9.10010780076391431042E-8;
            bn = bn*x2-2.40274520828250956942E-9;
            bn = bn*x2+3.59233385440928410398E-11;
            bd = 1.00000000000000000000E0;
            bd = bd*x2-6.31839869873368190192E-1;
            bd = bd*x2+2.36706788228248691528E-1;
            bd = bd*x2-5.31806367003223277662E-2;
            bd = bd*x2+8.48041718586295374409E-3;
            bd = bd*x2-9.47996768486665330168E-4;
            bd = bd*x2+7.81025592944552338085E-5;
            bd = bd*x2-4.55875153252442634831E-6;
            bd = bd*x2+1.89100358111421846170E-7;
            bd = bd*x2-4.91324691331920606875E-9;
            bd = bd*x2+7.18466403235734541950E-11;
            y = 1.0/x+x2*bn/(bd*x);
            result = sg*0.5*y;
            return result;
        }
        if( x>1.0E9 )
        {
            result = sg*0.5/x;
            return result;
        }
        cn = -5.90592860534773254987E-1;
        cn = cn*x2+6.29235242724368800674E-1;
        cn = cn*x2-1.72858975380388136411E-1;
        cn = cn*x2+1.64837047825189632310E-2;
        cn = cn*x2-4.86827613020462700845E-4;
        cd = 1.00000000000000000000E0;
        cd = cd*x2-2.69820057197544900361E0;
        cd = cd*x2+1.73270799045947845857E0;
        cd = cd*x2-3.93708582281939493482E-1;
        cd = cd*x2+3.44278924041233391079E-2;
        cd = cd*x2-9.73655226040941223894E-4;
        y = 1.0/x+x2*cn/(cd*x);
        result = sg*0.5*y;
        return result;
    }


    private static void testdawson()
    {
        double t = 0;
        double h = 0;
        double mdiff = 0;
        double err = 0;
        double toterr = 0;
        int i = 0;

        System.Console.Write("TESTING DAWSON INTEGRAL");
        System.Console.WriteLine();
        toterr = 0;
        err = 0;
        t = 0;
        while( t<=100 )
        {
            err = Math.Max(err, Math.Abs(dawsonintegral(t)+dawsonintegral(-t)));
            t = t+0.125;
        }
        toterr = Math.Max(err, toterr);
        System.Console.Write("SYMMETRY TEST ERROR:   ");
        System.Console.Write("{0,5:E2}",err);
        System.Console.WriteLine();
        System.Console.Write("TESTING dF/dX (TEST FOR CONTINUITY)");
        System.Console.WriteLine();
        h = 1;
        for(i=1; i<=2; i++)
        {
            mdiff = 0;
            h = h*1.0E-2;
            t = 0;
            while( t<=10 )
            {
                mdiff = Math.Max(mdiff, Math.Abs(dawsonintegral(t)-dawsonintegral(t+h))/h);
                t = t+h;
            }
            System.Console.Write("max dF/dX (h");
            System.Console.Write("{0,0:d}",i);
            System.Console.Write(")    ");
            System.Console.Write("{0,10:F6}",mdiff);
            System.Console.WriteLine();
        }
        err = 0;
        err = Math.Max(err, Math.Abs(dawsonintegral(0.02)-0.0199946675));
        err = Math.Max(err, Math.Abs(dawsonintegral(0.38)-0.3454471562));
        err = Math.Max(err, Math.Abs(dawsonintegral(1.00)-0.5380795069));
        err = Math.Max(err, Math.Abs(dawsonintegral(2.00)-0.3013403889));
        err = Math.Max(err, Math.Abs(3.0860669992418382*dawsonintegral(3.0860669992418382)-0.5323347470));
        toterr = Math.Max(err, toterr);
        System.Console.Write("INTERVAL 1 TEST ERROR: ");
        System.Console.Write("{0,5:E2}",err);
        System.Console.WriteLine();
        err = 0;
        err = Math.Max(err, Math.Abs(dawsonintegral(1/Math.Sqrt(0.075))/Math.Sqrt(0.075)-0.5214267490));
        err = Math.Max(err, Math.Abs(dawsonintegral(1/Math.Sqrt(0.045))/Math.Sqrt(0.045)-0.5121119710));
        err = Math.Max(err, Math.Abs(dawsonintegral(1/Math.Sqrt(0.035))/Math.Sqrt(0.035)-0.5092554660));
        err = Math.Max(err, Math.Abs(dawsonintegral(1/Math.Sqrt(0.030))/Math.Sqrt(0.030)-0.5078659030));
        toterr = Math.Max(err, toterr);
        System.Console.Write("INTERVAL 2 TEST ERROR: ");
        System.Console.Write("{0,5:E2}",err);
        System.Console.WriteLine();
        err = 0;
        err = Math.Max(err, Math.Abs(dawsonintegral(1/Math.Sqrt(0.020))/Math.Sqrt(0.020)-0.5051580780));
        err = Math.Max(err, Math.Abs(dawsonintegral(1/Math.Sqrt(0.015))/Math.Sqrt(0.015)-0.5038377170));
        err = Math.Max(err, Math.Abs(dawsonintegral(1/Math.Sqrt(0.010))/Math.Sqrt(0.010)-0.5025384710));
        err = Math.Max(err, Math.Abs(dawsonintegral(1/Math.Sqrt(0.005))/Math.Sqrt(0.005)-0.5012594940));
        toterr = Math.Max(err, toterr);
        System.Console.Write("INTERVAL 3 TEST ERROR: ");
        System.Console.Write("{0,5:E2}",err);
        System.Console.WriteLine();
    }
}
