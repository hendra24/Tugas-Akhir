using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.RGDL
{
    class DimensionalityReduction
    {
        public static double[][] ReduceDimensionality(double [][]data, int newDimensionality)
        {
            double[][] returnArray = null;
            double[,] Covariance = new double[data[0].Length, data[0].Length];
            
            double[] means = new double[data[0].Length];
            for (int b = 0; b < data[0].Length; b++)
            {
                for (int a = 0; a < data.Length; a++)
                {
                    means[b] += data[a][b];
                }
                means[b] /= data.Length;
            }

            for (int a = 0; a < data[0].Length; a++)
                for (int b = 0; b < data[0].Length; b++)
                {
                    for (int c = 0; c < data.Length; c++)
                    {
                        Covariance[a, b] += (data[c][a] - means[a]) * (data[c][b] - means[b]);
                    }
                    Covariance[a, b] /= (data.Length - 1);
                }

            double[] wr = null;
            double[] wi = null;
            double[,] vl = null;
            double[,] vr = null;
            nsevd.rmatrixevd(Covariance, //matrix
                data[0].Length, //dimensions
                1, //vales and right vectors
                ref wr, //real values
                ref wi, //imaginery values
                ref vl, //left vectors
                ref vr); //right vecotrs
            //sortowanie kolumn względem rosnącej wartości własnej
            int minIndex = 0;
            double[] helpMatrix = new double[data[0].Length];
            double helpDouble = 0;
            for (int a = 0; a < wr.Length -1; a++)
            {
                minIndex = a;
                for (int b = a + 1; b < wr.Length; b++)
                {
                    if (wr[minIndex] < wr[b])
                    {
                        minIndex = b;
                    }
                }
                for (int c = 0; c < helpMatrix.Length; c++)
                {
                    helpMatrix[c] = vr[c, a];
                }
                for (int c = 0; c < helpMatrix.Length; c++)
                {
                    vr[c, a] = vr[c, minIndex];
                }
                for (int c = 0; c < helpMatrix.Length; c++)
                {
                    vr[c, minIndex] = helpMatrix[c];
                }
                helpDouble = wr[a];
                wr[a] = wr[minIndex];
                wr[minIndex] = helpDouble;
            }

            returnArray = new double[data.Length][];
            double[][] helpArray = new double[data.Length][];
            for (int a = 0; a < returnArray.Length; a++)
            {
                returnArray[a] = new double[newDimensionality];
                helpArray[a] = new double[data[0].Length];
            }
            for (int a = 0; a < helpArray.Length; a++)
                for (int b = 0; b < helpArray[a].Length; b++)
                    for (int c = 0; c < helpArray[a].Length; c++)
                    {
                        helpArray[a][b] += (data[a][c] - means[c]) * vr[c,b];
                    }
            for (int a = 0; a < helpArray.Length; a++)
                for (int b = 0; b < newDimensionality; b++)
                {
                    returnArray[a][b] = helpArray[a][b];
                }



            return returnArray;
        }
    }
}
