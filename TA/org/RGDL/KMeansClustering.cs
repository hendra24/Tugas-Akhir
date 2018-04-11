using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.RGDL
{
    class KMeansClustering
    {
        //string[] attributes = new string[] { "Height", "Weight" };
        //double[][] rawData = new double[20][];

        //raw, 3, 30
        public static ClasteringResults Clasterize(double[][] rawData, int numberOfClusters, int maxCount, int randomSeed)
        {
            if (rawData == null)
                throw new RGDLException("Empty structure to clasterize", new Exception());
            if (rawData.Length < 1)
                throw new RGDLException("Empty structure to clasterize", new Exception());
            if (rawData[0].Length < 1)
                throw new RGDLException("Empty structure to clasterize", new Exception());
            int numAttributes = rawData[0].Length;
            int numClusters = numberOfClusters;
            //Console.WriteLine("\nk = " + numClusters + " and maxCount = " + maxCount);
            int[] clustering = Cluster(rawData, numClusters, numAttributes, maxCount, randomSeed);

            double[][] means = Allocate(numClusters, numAttributes);
            UpdateMeans(rawData, clustering, means);
            double[][] standardDeviations = Allocate(numClusters, numAttributes);
            UpdateStandardDeviations(rawData, clustering, means, standardDeviations);
            ClasteringResults cr = new ClasteringResults();
            cr.clustering = clustering;
            cr.means = means;
            cr.standardDeviations = standardDeviations;
            return cr;
            /*Console.WriteLine("\nClustering complete");
            Console.WriteLine("\nClustering in internal format: \n");
            ShowVector(clustering);
            Console.WriteLine("\nClustered data:");
            ShowClustering(rawData, numClusters, clustering);
            Console.WriteLine("\nCentroids:");
            double[][] means = Allocate(numClusters, attributes.Length);
            UpdateMeans(rawData, clustering, means);
            for (int a = 0; a < numClusters; a++)
            {
                double[] centroid = ComputeCentroid(rawData, clustering, a, means);
                Console.Write("Claster (" + a + "): ");
                for (int b = 0; b < centroid.Length; b++)
                    Console.Write(centroid[b] + " ");
                Console.WriteLine("");
            }
            double[] outlier = Outlier(rawData, clustering, numClusters, 0);
            Console.WriteLine("Outlier for cluster 0 is:");
            ShowVector(outlier);
            Console.WriteLine("\nEnd demo\n");*/
        }

        static void UpdateMeans(double[][] rawData, int[] clustering,
        double[][] means)
        {
            int numClusters = means.Length;
            for (int k = 0; k < means.Length; ++k)
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] = 0.0;
            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < rawData.Length; ++i)
            {
                int cluster = clustering[i];
                ++clusterCounts[cluster];
                for (int j = 0; j < rawData[i].Length; ++j)
                    means[cluster][j] += rawData[i][j];
            }
            for (int k = 0; k < means.Length; ++k)
                for (int j = 0; j < means[k].Length; ++j)
                    means[k][j] /= clusterCounts[k]; // danger
            return;
        }

        static void UpdateStandardDeviations(double[][] rawData, int[] clustering,
        double[][] means, double[][] standardDeviations)
        {
            int numClusters = means.Length;
            for (int k = 0; k < standardDeviations.Length; ++k)
                for (int j = 0; j < standardDeviations[k].Length; ++j)
                    standardDeviations[k][j] = 0.0;

            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < rawData.Length; ++i)
            {
                int cluster = clustering[i];
                ++clusterCounts[cluster];
                for (int j = 0; j < rawData[i].Length; ++j)
                    standardDeviations[cluster][j] += (rawData[i][j] - means[cluster][j]) * (rawData[i][j] - means[cluster][j]);
            }

            for (int k = 0; k < standardDeviations.Length; ++k)
                for (int j = 0; j < standardDeviations[k].Length; ++j)
                {
                    double denominator = clusterCounts[k] - 1;
                    if (denominator >0.01)
                        standardDeviations[k][j] /= denominator;
                    else
                        standardDeviations[k][j] = 0;
                    standardDeviations[k][j] = Math.Sqrt(standardDeviations[k][j]);
                }
            return;
        }

        static double[][] Allocate(int numClusters, int numAttributes)
        {
            double[][] result = new double[numClusters][];
            for (int k = 0; k < numClusters; ++k)
                result[k] = new double[numAttributes];
            return result;
        }

        static double[] ComputeCentroid(double[][] rawData, int[] clustering,
      int cluster, double[][] means)
        {
            int numAttributes = means[0].Length;
            double[] centroid = new double[numAttributes];
            double minDist = double.MaxValue;
            for (int i = 0; i < rawData.Length; ++i) // walk thru each data tuple
            {
                int c = clustering[i];
                if (c != cluster) continue;
                double currDist = Distance(rawData[i], means[cluster]);
                if (currDist < minDist)
                {
                    minDist = currDist;
                    for (int j = 0; j < centroid.Length; ++j)
                        centroid[j] = rawData[i][j];
                }
            }
            return centroid;
        }

        static void UpdateCentroids(double[][] rawData, int[] clustering,
      double[][] means, double[][] centroids)
        {
            for (int k = 0; k < centroids.Length; ++k)
            {
                double[] centroid = ComputeCentroid(rawData, clustering, k, means);
                centroids[k] = centroid;
            }
        }

        static double Distance(double[] tuple, double[] vector)
        {
            double sumSquaredDiffs = 0.0;
            for (int j = 0; j < tuple.Length; ++j)
                sumSquaredDiffs += Math.Pow((tuple[j] - vector[j]), 2);
            return Math.Sqrt(sumSquaredDiffs);
        }

        static bool Assign(double[][] rawData, int[] clustering, double[][] centroids)
        {
            int numClusters = centroids.Length;
            bool changed = false;
            double[] distances = new double[numClusters];
            for (int i = 0; i < rawData.Length; ++i)
            {
                for (int k = 0; k < numClusters; ++k)
                    distances[k] = Distance(rawData[i], centroids[k]);
                int newCluster = MinIndex(distances);
                if (newCluster != clustering[i])
                {
                    changed = true;
                    clustering[i] = newCluster;
                }
            }
            return changed;
        }

        static int MinIndex(double[] distances)
        {
            int indexOfMin = 0;
            double smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k]; indexOfMin = k;
                }
            }
            return indexOfMin;
        }

        static int[] Cluster(double[][] rawData, int numClusters,
      int numAttributes, int maxCount, int randomSeed)
        {
            bool changed = true;
            int ct = 0;
            int numTuples = rawData.Length;
            int[] clustering = InitClustering(numTuples, numClusters, randomSeed);
            double[][] means = Allocate(numClusters, numAttributes);
            double[][] centroids = Allocate(numClusters, numAttributes);
            UpdateMeans(rawData, clustering, means);
            UpdateCentroids(rawData, clustering, means, centroids);
            while (changed == true && ct < maxCount)
            {
                ++ct;
                changed = Assign(rawData, clustering, centroids);
                UpdateMeans(rawData, clustering, means);
                UpdateCentroids(rawData, clustering, means, centroids);
            }
            return clustering;
        }

        static int[] InitClustering(int numTuples,
      int numClusters, int randomSeed)
        {
            Random random = new Random(randomSeed);
            int[] clustering = new int[numTuples];
            for (int i = 0; i < numClusters; ++i)
                clustering[i] = i;
            for (int i = numClusters; i < clustering.Length; ++i)
                clustering[i] = random.Next(0, numClusters);
            return clustering;
        }

        static double[] Outlier(double[][] rawData, int[] clustering,
      int numClusters, int cluster)
        {
            int numAttributes = rawData[0].Length;
            double[] outlier = new double[numAttributes];
            double maxDist = 0.0;
            double[][] means = Allocate(numClusters, numAttributes);
            double[][] centroids = Allocate(numClusters, numAttributes);
            UpdateMeans(rawData, clustering, means);
            UpdateCentroids(rawData, clustering, means, centroids);
            for (int i = 0; i < rawData.Length; ++i)
            {
                int c = clustering[i];
                if (c != cluster) continue;
                double dist = Distance(rawData[i], centroids[cluster]);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    Array.Copy(rawData[i], outlier, rawData[i].Length);
                }
            }
            return outlier;
        }

        //Display Routines

        //For the sake of completeness, here are some simplified display routines. The code download has slightly fancier versions. If you use these simplified routines, you’ll have to modify their calls in the Main method. To display raw data, means and centroids you can use:

        static void ShowMatrix(double[][] matrix)
        {
            for (int i = 0; i < matrix.Length; ++i)
            {
                Console.Write("[" + i.ToString().PadLeft(2) + "]  ");
                for (int j = 0; j < matrix[i].Length; ++j)
                    Console.Write(matrix[i][j].ToString("F1") + "  ");
                Console.WriteLine("");
            }
        }

        //To display the clustering array you can use:

        static void ShowVector(int[] vector)
        {
            for (int i = 0; i < vector.Length; ++i)
                Console.Write(vector[i] + " ");
            Console.WriteLine("");
        }

        //To display an outlier’s values you can use:

        static void ShowVector(double[] vector)
        {
            for (int i = 0; i < vector.Length; ++i)
                Console.Write(vector[i].ToString("F1") + " ");
            Console.WriteLine("");
        }

        //And to display raw data grouped by cluster you can use:

        static void ShowClustering(double[][] rawData,
          int numClusters, int[] clustering)
        {
            for (int k = 0; k < numClusters; ++k) // Each cluster
            {
                for (int i = 0; i < rawData.Length; ++i) // Each tuple
                    if (clustering[i] == k)
                    {
                        for (int j = 0; j < rawData[i].Length; ++j)
                            Console.Write(rawData[i][j].ToString("F1") + " ");
                        Console.WriteLine("");
                    }
                Console.WriteLine("");
            }
        }
    }
}
