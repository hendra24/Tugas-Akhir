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

class expintegrals
{
    /*************************************************************************
    Exponential integral Ei(x)

                  x
                   -     t
                  | |   e
       Ei(x) =   -|-   ---  dt .
                | |     t
                 -
                -inf

    Not defined for x <= 0.
    See also expn.c.



    ACCURACY:

                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE       0,100       50000      8.6e-16     1.3e-16

    Cephes Math Library Release 2.8:  May, 1999
    Copyright 1999 by Stephen L. Moshier
    *************************************************************************/
    public static double exponentialintegralei(double x)
    {
        double result = 0;
        double eul = 0;
        double f = 0;
        double f1 = 0;
        double f2 = 0;
        double w = 0;

        eul = 0.5772156649015328606065;
        if( x<=0 )
        {
            result = 0;
            return result;
        }
        if( x<2 )
        {
            f1 = -5.350447357812542947283;
            f1 = f1*x+218.5049168816613393830;
            f1 = f1*x-4176.572384826693777058;
            f1 = f1*x+55411.76756393557601232;
            f1 = f1*x-331338.1331178144034309;
            f1 = f1*x+1592627.163384945414220;
            f2 = 1.000000000000000000000;
            f2 = f2*x-52.50547959112862969197;
            f2 = f2*x+1259.616186786790571525;
            f2 = f2*x-17565.49581973534652631;
            f2 = f2*x+149306.2117002725991967;
            f2 = f2*x-729494.9239640527645655;
            f2 = f2*x+1592627.163384945429726;
            f = f1/f2;
            result = eul+Math.Log(x)+x*f;
            return result;
        }
        if( x<4 )
        {
            w = 1/x;
            f1 = 1.981808503259689673238E-2;
            f1 = f1*w-1.271645625984917501326;
            f1 = f1*w-2.088160335681228318920;
            f1 = f1*w+2.755544509187936721172;
            f1 = f1*w-4.409507048701600257171E-1;
            f1 = f1*w+4.665623805935891391017E-2;
            f1 = f1*w-1.545042679673485262580E-3;
            f1 = f1*w+7.059980605299617478514E-5;
            f2 = 1.000000000000000000000;
            f2 = f2*w+1.476498670914921440652;
            f2 = f2*w+5.629177174822436244827E-1;
            f2 = f2*w+1.699017897879307263248E-1;
            f2 = f2*w+2.291647179034212017463E-2;
            f2 = f2*w+4.450150439728752875043E-3;
            f2 = f2*w+1.727439612206521482874E-4;
            f2 = f2*w+3.953167195549672482304E-5;
            f = f1/f2;
            result = Math.Exp(x)*w*(1+w*f);
            return result;
        }
        if( x<8 )
        {
            w = 1/x;
            f1 = -1.373215375871208729803;
            f1 = f1*w-7.084559133740838761406E-1;
            f1 = f1*w+1.580806855547941010501;
            f1 = f1*w-2.601500427425622944234E-1;
            f1 = f1*w+2.994674694113713763365E-2;
            f1 = f1*w-1.038086040188744005513E-3;
            f1 = f1*w+4.371064420753005429514E-5;
            f1 = f1*w+2.141783679522602903795E-6;
            f2 = 1.000000000000000000000;
            f2 = f2*w+8.585231423622028380768E-1;
            f2 = f2*w+4.483285822873995129957E-1;
            f2 = f2*w+7.687932158124475434091E-2;
            f2 = f2*w+2.449868241021887685904E-2;
            f2 = f2*w+8.832165941927796567926E-4;
            f2 = f2*w+4.590952299511353531215E-4;
            f2 = f2*w+-4.729848351866523044863E-6;
            f2 = f2*w+2.665195537390710170105E-6;
            f = f1/f2;
            result = Math.Exp(x)*w*(1+w*f);
            return result;
        }
        if( x<16 )
        {
            w = 1/x;
            f1 = -2.106934601691916512584;
            f1 = f1*w+1.732733869664688041885;
            f1 = f1*w-2.423619178935841904839E-1;
            f1 = f1*w+2.322724180937565842585E-2;
            f1 = f1*w+2.372880440493179832059E-4;
            f1 = f1*w-8.343219561192552752335E-5;
            f1 = f1*w+1.363408795605250394881E-5;
            f1 = f1*w-3.655412321999253963714E-7;
            f1 = f1*w+1.464941733975961318456E-8;
            f1 = f1*w+6.176407863710360207074E-10;
            f2 = 1.000000000000000000000;
            f2 = f2*w-2.298062239901678075778E-1;
            f2 = f2*w+1.105077041474037862347E-1;
            f2 = f2*w-1.566542966630792353556E-2;
            f2 = f2*w+2.761106850817352773874E-3;
            f2 = f2*w-2.089148012284048449115E-4;
            f2 = f2*w+1.708528938807675304186E-5;
            f2 = f2*w-4.459311796356686423199E-7;
            f2 = f2*w+1.394634930353847498145E-8;
            f2 = f2*w+6.150865933977338354138E-10;
            f = f1/f2;
            result = Math.Exp(x)*w*(1+w*f);
            return result;
        }
        if( x<32 )
        {
            w = 1/x;
            f1 = -2.458119367674020323359E-1;
            f1 = f1*w-1.483382253322077687183E-1;
            f1 = f1*w+7.248291795735551591813E-2;
            f1 = f1*w-1.348315687380940523823E-2;
            f1 = f1*w+1.342775069788636972294E-3;
            f1 = f1*w-7.942465637159712264564E-5;
            f1 = f1*w+2.644179518984235952241E-6;
            f1 = f1*w-4.239473659313765177195E-8;
            f2 = 1.000000000000000000000;
            f2 = f2*w-1.044225908443871106315E-1;
            f2 = f2*w-2.676453128101402655055E-1;
            f2 = f2*w+9.695000254621984627876E-2;
            f2 = f2*w-1.601745692712991078208E-2;
            f2 = f2*w+1.496414899205908021882E-3;
            f2 = f2*w-8.462452563778485013756E-5;
            f2 = f2*w+2.728938403476726394024E-6;
            f2 = f2*w-4.239462431819542051337E-8;
            f = f1/f2;
            result = Math.Exp(x)*w*(1+w*f);
            return result;
        }
        if( x<64 )
        {
            w = 1/x;
            f1 = 1.212561118105456670844E-1;
            f1 = f1*w-5.823133179043894485122E-1;
            f1 = f1*w+2.348887314557016779211E-1;
            f1 = f1*w-3.040034318113248237280E-2;
            f1 = f1*w+1.510082146865190661777E-3;
            f1 = f1*w-2.523137095499571377122E-5;
            f2 = 1.000000000000000000000;
            f2 = f2*w-1.002252150365854016662;
            f2 = f2*w+2.928709694872224144953E-1;
            f2 = f2*w-3.337004338674007801307E-2;
            f2 = f2*w+1.560544881127388842819E-3;
            f2 = f2*w-2.523137093603234562648E-5;
            f = f1/f2;
            result = Math.Exp(x)*w*(1+w*f);
            return result;
        }
        w = 1/x;
        f1 = -7.657847078286127362028E-1;
        f1 = f1*w+6.886192415566705051750E-1;
        f1 = f1*w-2.132598113545206124553E-1;
        f1 = f1*w+3.346107552384193813594E-2;
        f1 = f1*w-3.076541477344756050249E-3;
        f1 = f1*w+1.747119316454907477380E-4;
        f1 = f1*w-6.103711682274170530369E-6;
        f1 = f1*w+1.218032765428652199087E-7;
        f1 = f1*w-1.086076102793290233007E-9;
        f2 = 1.000000000000000000000;
        f2 = f2*w-1.888802868662308731041;
        f2 = f2*w+1.066691687211408896850;
        f2 = f2*w-2.751915982306380647738E-1;
        f2 = f2*w+3.930852688233823569726E-2;
        f2 = f2*w-3.414684558602365085394E-3;
        f2 = f2*w+1.866844370703555398195E-4;
        f2 = f2*w-6.345146083130515357861E-6;
        f2 = f2*w+1.239754287483206878024E-7;
        f2 = f2*w-1.086076102793126632978E-9;
        f = f1/f2;
        result = Math.Exp(x)*w*(1+w*f);
        return result;
    }


    /*************************************************************************
    Exponential integral En(x)

    Evaluates the exponential integral

                    inf.
                      -
                     | |   -xt
                     |    e
         E (x)  =    |    ----  dt.
          n          |      n
                   | |     t
                    -
                     1


    Both n and x must be nonnegative.

    The routine employs either a power series, a continued
    fraction, or an asymptotic formula depending on the
    relative values of n and x.

    ACCURACY:

                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE      0, 30       10000       1.7e-15     3.6e-16

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1985, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double exponentialintegralen(double x,
        int n)
    {
        double result = 0;
        double r = 0;
        double t = 0;
        double yk = 0;
        double xk = 0;
        double pk = 0;
        double pkm1 = 0;
        double pkm2 = 0;
        double qk = 0;
        double qkm1 = 0;
        double qkm2 = 0;
        double psi = 0;
        double z = 0;
        int i = 0;
        int k = 0;
        double big = 0;
        double eul = 0;

        eul = 0.57721566490153286060;
        big = 1.44115188075855872*Math.Pow(10, 17);
        if( n<0 | x<0 | x>170 | x==0 & n<2 )
        {
            result = -1;
            return result;
        }
        if( x==0 )
        {
            result = (double)(1)/((double)(n-1));
            return result;
        }
        if( n==0 )
        {
            result = Math.Exp(-x)/x;
            return result;
        }
        if( n>5000 )
        {
            xk = x+n;
            yk = 1/(xk*xk);
            t = n;
            result = yk*t*(6*x*x-8*t*x+t*t);
            result = yk*(result+t*(t-2.0*x));
            result = yk*(result+t);
            result = (result+1)*Math.Exp(-x)/xk;
            return result;
        }
        if( x<=1 )
        {
            psi = -eul-Math.Log(x);
            for(i=1; i<=n-1; i++)
            {
                psi = psi+(double)(1)/(double)(i);
            }
            z = -x;
            xk = 0;
            yk = 1;
            pk = 1-n;
            if( n==1 )
            {
                result = 0.0;
            }
            else
            {
                result = 1.0/pk;
            }
            do
            {
                xk = xk+1;
                yk = yk*z/xk;
                pk = pk+1;
                if( pk!=0 )
                {
                    result = result+yk/pk;
                }
                if( result!=0 )
                {
                    t = Math.Abs(yk/result);
                }
                else
                {
                    t = 1;
                }
            }
            while( t>=AP.Math.MachineEpsilon );
            t = 1;
            for(i=1; i<=n-1; i++)
            {
                t = t*z/i;
            }
            result = psi*t-result;
            return result;
        }
        else
        {
            k = 1;
            pkm2 = 1;
            qkm2 = x;
            pkm1 = 1.0;
            qkm1 = x+n;
            result = pkm1/qkm1;
            do
            {
                k = k+1;
                if( k%2==1 )
                {
                    yk = 1;
                    xk = n+((double)(k-1))/(double)(2);
                }
                else
                {
                    yk = x;
                    xk = (double)(k)/(double)(2);
                }
                pk = pkm1*yk+pkm2*xk;
                qk = qkm1*yk+qkm2*xk;
                if( qk!=0 )
                {
                    r = pk/qk;
                    t = Math.Abs((result-r)/r);
                    result = r;
                }
                else
                {
                    t = 1;
                }
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if( Math.Abs(pk)>big )
                {
                    pkm2 = pkm2/big;
                    pkm1 = pkm1/big;
                    qkm2 = qkm2/big;
                    qkm1 = qkm1/big;
                }
            }
            while( t>=AP.Math.MachineEpsilon );
            result = result*Math.Exp(-x);
        }
        return result;
    }
}
