/*************************************************************************
Copyright 2008 by Sergey Bochkanov (ALGLIB project).

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

class tsort
{
    public static void tagsort(ref double[] a,
        int n,
        ref int[] p1,
        ref int[] p2)
    {
        int i = 0;
        //int j = 0;
        //int k = 0;
        //int t = 0;
        //double tmp = 0;
        //int tmpi = 0;
        int[] pv = new int[0];
        int[] vp = new int[0];
        int lv = 0;
        int lp = 0;
        int rv = 0;
        int rp = 0;

        
        //
        // Special cases
        //
        if( n<=0 )
        {
            return;
        }
        if( n==1 )
        {
            p1 = new int[0+1];
            p2 = new int[0+1];
            p1[0] = 0;
            p2[0] = 0;
            return;
        }
        
        //
        // General case, N>1: prepare permutations table P1
        //
        p1 = new int[n-1+1];
        for(i=0; i<=n-1; i++)
        {
            p1[i] = i;
        }
        
        //
        // General case, N>1: sort, update P1
        //
        tagsortfasti(ref a, ref p1, n);
        
        //
        // General case, N>1: fill permutations table P2
        //
        // To fill P2 we maintain two arrays:
        // * PV, Position(Value). PV[i] contains position of I-th key at the moment
        // * VP, Value(Position). VP[i] contains key which has position I at the moment
        //
        // At each step we making permutation of two items:
        //   Left, which is given by position/value pair LP/LV
        //   and Right, which is given by RP/RV
        // and updating PV[] and VP[] correspondingly.
        //
        pv = new int[n-1+1];
        vp = new int[n-1+1];
        p2 = new int[n-1+1];
        for(i=0; i<=n-1; i++)
        {
            pv[i] = i;
            vp[i] = i;
        }
        for(i=0; i<=n-1; i++)
        {
            
            //
            // calculate LP, LV, RP, RV
            //
            lp = i;
            lv = vp[lp];
            rv = p1[i];
            rp = pv[rv];
            
            //
            // Fill P2
            //
            p2[i] = rp;
            
            //
            // update PV and VP
            //
            vp[lp] = rv;
            vp[rp] = lv;
            pv[lv] = rp;
            pv[rv] = lp;
        }
    }


    public static void tagsortfasti(ref double[] a,
        ref int[] b,
        int n)
    {
        int i = 0;
        //int j = 0;
        int k = 0;
        int t = 0;
        double tmp = 0;
        int tmpi = 0;

        
        //
        // Special cases
        //
        if( n<=1 )
        {
            return;
        }
        
        //
        // General case, N>1: sort, update B
        //
        i = 2;
        do
        {
            t = i;
            while( t!=1 )
            {
                k = t/2;
                if( a[k-1]>=a[t-1] )
                {
                    t = 1;
                }
                else
                {
                    tmp = a[k-1];
                    a[k-1] = a[t-1];
                    a[t-1] = tmp;
                    tmpi = b[k-1];
                    b[k-1] = b[t-1];
                    b[t-1] = tmpi;
                    t = k;
                }
            }
            i = i+1;
        }
        while( i<=n );
        i = n-1;
        do
        {
            tmp = a[i];
            a[i] = a[0];
            a[0] = tmp;
            tmpi = b[i];
            b[i] = b[0];
            b[0] = tmpi;
            t = 1;
            while( t!=0 )
            {
                k = 2*t;
                if( k>i )
                {
                    t = 0;
                }
                else
                {
                    if( k<i )
                    {
                        if( a[k]>a[k-1] )
                        {
                            k = k+1;
                        }
                    }
                    if( a[t-1]>=a[k-1] )
                    {
                        t = 0;
                    }
                    else
                    {
                        tmp = a[k-1];
                        a[k-1] = a[t-1];
                        a[t-1] = tmp;
                        tmpi = b[k-1];
                        b[k-1] = b[t-1];
                        b[t-1] = tmpi;
                        t = k;
                    }
                }
            }
            i = i-1;
        }
        while( i>=1 );
    }


    public static void tagsortfastr(ref double[] a,
        ref double[] b,
        int n)
    {
        int i = 0;
        //int j = 0;
        int k = 0;
        int t = 0;
        double tmp = 0;
        double tmpr = 0;

        
        //
        // Special cases
        //
        if( n<=1 )
        {
            return;
        }
        
        //
        // General case, N>1: sort, update B
        //
        i = 2;
        do
        {
            t = i;
            while( t!=1 )
            {
                k = t/2;
                if( a[k-1]>=a[t-1] )
                {
                    t = 1;
                }
                else
                {
                    tmp = a[k-1];
                    a[k-1] = a[t-1];
                    a[t-1] = tmp;
                    tmpr = b[k-1];
                    b[k-1] = b[t-1];
                    b[t-1] = tmpr;
                    t = k;
                }
            }
            i = i+1;
        }
        while( i<=n );
        i = n-1;
        do
        {
            tmp = a[i];
            a[i] = a[0];
            a[0] = tmp;
            tmpr = b[i];
            b[i] = b[0];
            b[0] = tmpr;
            t = 1;
            while( t!=0 )
            {
                k = 2*t;
                if( k>i )
                {
                    t = 0;
                }
                else
                {
                    if( k<i )
                    {
                        if( a[k]>a[k-1] )
                        {
                            k = k+1;
                        }
                    }
                    if( a[t-1]>=a[k-1] )
                    {
                        t = 0;
                    }
                    else
                    {
                        tmp = a[k-1];
                        a[k-1] = a[t-1];
                        a[t-1] = tmp;
                        tmpr = b[k-1];
                        b[k-1] = b[t-1];
                        b[t-1] = tmpr;
                        t = k;
                    }
                }
            }
            i = i-1;
        }
        while( i>=1 );
    }


    public static void tagsortfast(ref double[] a,
        int n)
    {
        int i = 0;
        //int j = 0;
        int k = 0;
        int t = 0;
        double tmp = 0;
        //double tmpr = 0;

        
        //
        // Special cases
        //
        if( n<=1 )
        {
            return;
        }
        
        //
        // General case, N>1: sort, update B
        //
        i = 2;
        do
        {
            t = i;
            while( t!=1 )
            {
                k = t/2;
                if( a[k-1]>=a[t-1] )
                {
                    t = 1;
                }
                else
                {
                    tmp = a[k-1];
                    a[k-1] = a[t-1];
                    a[t-1] = tmp;
                    t = k;
                }
            }
            i = i+1;
        }
        while( i<=n );
        i = n-1;
        do
        {
            tmp = a[i];
            a[i] = a[0];
            a[0] = tmp;
            t = 1;
            while( t!=0 )
            {
                k = 2*t;
                if( k>i )
                {
                    t = 0;
                }
                else
                {
                    if( k<i )
                    {
                        if( a[k]>a[k-1] )
                        {
                            k = k+1;
                        }
                    }
                    if( a[t-1]>=a[k-1] )
                    {
                        t = 0;
                    }
                    else
                    {
                        tmp = a[k-1];
                        a[k-1] = a[t-1];
                        a[t-1] = tmp;
                        t = k;
                    }
                }
            }
            i = i-1;
        }
        while( i>=1 );
    }
}
