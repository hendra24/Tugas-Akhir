using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.RGDL;
using System.Collections;
using System.Threading.Tasks;

namespace org.RGDL
{
    class HierarchicalClustering
    {
        private static double[][] distances = null;
        public static ClasteringResults Clasterize(double[][] rawData, int numberOfClusters, double maxDistance)
        {
            ClasteringResults cr = new ClasteringResults();
            distances = new double[rawData.Length][];
            for (int a = 0; a < distances.Length; a++)
            {
                distances[a] = new double[rawData.Length];
                for (int b = 0; b < distances[a].Length; b++)
                {
                    distances[a][b] = Distance(rawData[a], rawData[b]);
                }
            }
            cr.clustering = new int[rawData.Length];
            ArrayList allClusters = new ArrayList();
            int clusterId = 1;
            for (int a = 0; a < rawData.Length; a++)
            {
                cluster c = new cluster();
                dataRecord dr = new dataRecord();
                dr.data = new double[rawData[0].Length];
                for (int b = 0; b < dr.data.Length; b++)
                {
                    dr.data[b] = rawData[a][b];
                }
                dr.index = a;
                c.children.Add(dr);
                c.clusterId = clusterId;
                clusterId++;
                allClusters.Add(c);
            }
            cluster c1, c2;
            cluster c1ToJoin = null, c2ToJoin = null;
            while (allClusters.Count > 1)
            {
                c1ToJoin = null;
                c2ToJoin = null;
                double minDistance = double.MaxValue;
                double valueHelp = minDistance;
                for (int a = 0; a < allClusters.Count; a++)
                {
                    c1 = (cluster)allClusters[a];
                    for (int b = a + 1; b < allClusters.Count; b++)
                    {
                        //if (a != b)
                        {
                            c2 = (cluster)allClusters[b];
                            valueHelp = UPGMA(c1, c2);
                            if (valueHelp < minDistance)
                            {
                                c1ToJoin = c1;
                                c2ToJoin = c2;
                                minDistance = valueHelp;
                            }
                        }
                    }
                }
                cluster joinCluster = new cluster();
                joinCluster.c1 = c1ToJoin;
                joinCluster.c2 = c2ToJoin;
                joinCluster.distance = minDistance;
                joinCluster.children.AddRange(c1ToJoin.children);
                joinCluster.children.AddRange(c2ToJoin.children);
                joinCluster.clusterId = clusterId;
                clusterId++;
                //c2ToJoin.parent = c1ToJoin;
                //c1ToJoin.childrenHierarchy.Add(c2ToJoin);
                //c1ToJoin.children.Add(c2ToJoin);
                //c2ToJoin.parentDistance = minDistance;
                //c1ToJoin.children.AddRange(c2ToJoin.children);
                //AddAllChilderToArrayList(c1ToJoin, c2ToJoin);
                allClusters.Remove(c1ToJoin);
                allClusters.Remove(c2ToJoin);
                allClusters.Add(joinCluster);
            }
            cluster resultClusteringCluster = (cluster)allClusters[0];
            //resultClusteringCluster.parentDistance = double.MaxValue;
            GenerateClusters(resultClusteringCluster, cr.clustering, 50);
            return cr;
        }

        private static void GenerateClusters(cluster c, int[]clustering, double distance)
        {
            if (c.distance > distance)
            {
                if (c.c1 != null)
                {
                    GenerateClusters(c.c1, clustering, distance);
                    GenerateClusters(c.c2, clustering, distance);
                }
            }
            else
            {
                for (int a = 0; a < c.children.Count; a++)
                {
                    dataRecord dR = (dataRecord)c.children[a];
                    clustering[dR.index] = c.clusterId;
                }

            }
        }

        class dataRecord
        {
            public double[] data = null;
            public int index = 0;
        }

        class cluster
        {
            //public cluster child = null;
            //public cluster parent = null;
            public int clusterId = 0;
            public double distance = 0;
            //public ArrayList childrenHierarchy = new ArrayList();
            public cluster c1 = null;
            public cluster c2 = null;
            public ArrayList children = new ArrayList();
        }



        //UPGMA (Unweighted Pair Group Method with Arithmetic Mean)
        static double UPGMA(cluster c1, cluster c2)
        {
            double resultsV = 0;
            dataRecord dr1 = null;
            dataRecord dr2 = null;
            //c1.children.Add(c1);
            //c2.children.Add(c2);
            //object monitor = new object();
            /*Parallel.ForEach(c1.children.Cast<dataRecord>(), child =>
            {
                double helpResult = 0;
                for (int b = 0; b < c2.children.Count; b++)
                {
                    dr2 = (dataRecord)c2.children[b];
                    helpResult += Distance(child.data, dr2.data);
                }
                lock (monitor)
                {
                    resultsV += helpResult;
                }
            }
            );*/
            for (int a = 0; a < c1.children.Count; a++)
            {
                dr1 = (dataRecord)c1.children[a];
                for (int b = 0; b < c2.children.Count; b++)
                {
                    dr2 = (dataRecord)c2.children[b];
                    //resultsV += Distance(dr1.data, dr2.data);
                    resultsV += distances[dr1.index][dr2.index];
                }
            }
            resultsV /= (c1.children.Count * c2.children.Count);
            //c1.children.Remove(c1);
            //c2.children.Remove(c2);
            /*double resultsV = 0;
            cluster c1Help = c1;
            int c1Count = 0;
            int c2Count = 0;
            while (c1Help != null)
            {
                c1Count++;
                cluster c2Help = c2;
                while (c2Help != null)
                {
                    resultsV += Distance(c1Help.data, c2Help.data);
                    c2Help = c2Help.child;
                    c2Count++;
                }
                c1Help = c1Help.child;
            }
            resultsV /= (c1Count * c2Count);*/
            return resultsV;
        }

        static double Distance(double[] tuple, double[] vector)
        {
            double sumSquaredDiffs = 0.0;
            for (int j = 0; j < tuple.Length; ++j)
                sumSquaredDiffs += (tuple[j] - vector[j]) * (tuple[j] - vector[j]);
            return Math.Sqrt(sumSquaredDiffs);
        }
    }
}
