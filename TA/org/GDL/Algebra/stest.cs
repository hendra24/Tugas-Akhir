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

class stest
{
    /*************************************************************************
    Sign test

    This test checks three hypotheses about the median of  the  given  sample.
    The following tests are performed:
        * two-tailed test (null hypothesis - the median is equal to the  given
          value)
        * left-tailed test (null hypothesis - the median is  greater  than  or
          equal to the given value)
        * right-tailed test (null hypothesis - the  median  is  less  than  or
          equal to the given value)

    Requirements:
        * the scale of measurement should be ordinal, interval or ratio  (i.e.
          the test could not be applied to nominal variables).

    The test is non-parametric and doesn't require distribution X to be normal

    Input parameters:
        X       -   sample. Array whose index goes from 0 to N-1.
        N       -   size of the sample.
        Median  -   assumed median value.

    Output parameters:
        BothTails   -   p-value for two-tailed test.
                        If BothTails is less than the given significance level
                        the null hypothesis is rejected.
        LeftTail    -   p-value for left-tailed test.
                        If LeftTail is less than the given significance level,
                        the null hypothesis is rejected.
        RightTail   -   p-value for right-tailed test.
                        If RightTail is less than the given significance level
                        the null hypothesis is rejected.

    While   calculating   p-values   high-precision   binomial    distribution
    approximation is used, so significance levels have about 15 exact digits.

      -- ALGLIB --
         Copyright 08.09.2006 by Bochkanov Sergey
    *************************************************************************/
    public static void onesamplesigntest(ref double[] x,
        int n,
        double median,
        ref double bothtails,
        ref double lefttail,
        ref double righttail)
    {
        int i = 0;
        int gtcnt = 0;
        int necnt = 0;

        if( n<=1 )
        {
            bothtails = 1.0;
            lefttail = 1.0;
            righttail = 1.0;
            return;
        }
        
        //
        // Calculate:
        // GTCnt - count of x[i]>Median
        // NECnt - count of x[i]<>Median
        //
        gtcnt = 0;
        necnt = 0;
        for(i=0; i<=n-1; i++)
        {
            if( x[i]>median )
            {
                gtcnt = gtcnt+1;
            }
            if( x[i]!=median )
            {
                necnt = necnt+1;
            }
        }
        if( necnt==0 )
        {
            
            //
            // all x[i] are equal to Median.
            // So we can conclude that Median is a true median :)
            //
            bothtails = 0.0;
            lefttail = 0.0;
            righttail = 0.0;
            return;
        }
        bothtails = 2*binomialdistr.binomialdistribution(Math.Min(gtcnt, necnt-gtcnt), necnt, 0.5);
        lefttail = binomialdistr.binomialdistribution(gtcnt, necnt, 0.5);
        righttail = binomialdistr.binomialcdistribution(gtcnt-1, necnt, 0.5);
    }
}
