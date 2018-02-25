using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.RGDL
{
    class DBSCAN
    {
        class Point
        {
            public const int NOISE = -1;
            public const int UNCLASSIFIED = 0;
            //public int X, Y, ClusterId;
            public double[] data = null;
            public int id = 0;
            public int ClusterId;

            public bool visited = false;

            public Point(double[] data, int id)
            {
                this.data = data;
                this.id = id;
            }
            /*public override string ToString()
            {
                return String.Format("({0}, {1})", X, Y);
            }*/
            public static double DistanceSquared(Point p1, Point p2)
            {
                double returnValue = 0;
                for (int a = 0; a < p1.data.Length; a++)
                {
                    returnValue += (p2.data[a] - p1.data[a]) * (p2.data[a] - p1.data[a]);
                }
                return returnValue;
            }
        }

        //double eps = 60;
        //int minPts = 3;
        public static ClasteringResults Clasterize(double[][] rawData, double eps, int minPts)
        {
            ClasteringResults cr = new ClasteringResults();
            List<Point> points = new List<Point>();
            // sample data

            for (int a = 0; a < rawData.Length; a++)
            {
                points.Add(new Point(rawData[a], a));
            }

            cr.clustering = new int[rawData.Length];
            List<List<Point>> clusters = GetClusters(points, eps, minPts);
            for (int a = 0; a < clusters.Count; a++)
            {
                foreach (Point p in clusters[a])
                {
                    cr.clustering[p.id] = p.ClusterId;
                }
            }
            return cr;
        }

        private static List<List<Point>> GetClusters(List<Point> points, double eps, int minPts)
        {
            List<List<Point>> clusters = new List<List<Point>>();
            int clusterId = 0;
            //foreach (Point p in points)
            for (int a = 0; a < points.Count; a++)
            {
                Point p = (Point)points[a];

                if (p.visited == false)
                {
                    p.visited = true;
                    List<Point> NeighborPts = regionQuery(points, p, eps);
                    if (NeighborPts.Count < minPts)
                    {
                        p.ClusterId = Point.NOISE;
                    }
                    else
                    {
                        clusterId++;
                        List<Point> cluster = expandCluster(p, NeighborPts, clusterId, eps, minPts, points);
                        if (cluster != null)
                            clusters.Add(cluster);
                    }
                }
            }
            /*DBSCAN(D, eps, MinPts)
       C = 0
       for each unvisited point P in dataset D
          mark P as visited
          NeighborPts = regionQuery(P, eps)
          if sizeof(NeighborPts) < MinPts
             mark P as NOISE
          else
             C = next cluster
             expandCluster(P, NeighborPts, C, eps, MinPts)*/
            return clusters;
        }

        private static List<Point> expandCluster(Point p, List<Point> NeighborPts, int clusterId, double eps, int minPts, List<Point> points)
        {
            List<Point> cluster = new List<Point>();
            cluster.Add(p);
            p.ClusterId = clusterId;
            //foreach (Point pp in NeighborPts)
            for (int a = 0; a < NeighborPts.Count; a++)
            {
                Point pp = (Point)NeighborPts[a];
                if (pp.visited == false)
                {
                    pp.visited = true;
                    List<Point> NeighborPtsP = regionQuery(points, pp, eps);
                    if (NeighborPtsP.Count >= minPts)
                    {
                        NeighborPts.AddRange(NeighborPtsP);
                    }
                }
                if (pp.ClusterId == Point.UNCLASSIFIED)
                {
                        cluster.Add(pp);
                        pp.ClusterId = clusterId;
                }
            }
            return cluster;
            /*
             * add P to cluster C
   for each point P' in NeighborPts 
      if P' is not visited
         mark P' as visited
         NeighborPts' = regionQuery(P', eps)
         if sizeof(NeighborPts') >= MinPts
            NeighborPts = NeighborPts joined with NeighborPts'
      if P' is not yet member of any cluster
         add P' to cluster C
             */
        }

        private static List<Point> regionQuery(List<Point> points, Point p, double eps)
        {
            List<Point> NeighborPts = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                double distSquared = Point.DistanceSquared(p, points[i]);
                if (distSquared <= eps) NeighborPts.Add(points[i]);
            }
            return NeighborPts;
        //return all points within P's eps-neighborhood (including P)
        }
        /*
        //double eps = 60;
        //int minPts = 3;
        public static ClasteringResults Clasterize(double[][] rawData, double eps, int minPts)
        {
            ClasteringResults cr = new ClasteringResults();
            List<Point> points = new List<Point>();
            // sample data
            
            for (int a = 0; a < rawData.Length; a++)
            {
                points.Add(new Point(rawData[a], a));
            }
            
            cr.clustering = new int[rawData.Length];
            List<List<Point>> clusters = GetClusters(points, eps, minPts);
            for (int a = 0; a < clusters.Count; a++)
            {
                foreach (Point p in clusters[a])
                {
                    cr.clustering[p.id] = p.ClusterId;
                }
            }
            return cr;
        }

        private static List<List<Point>> GetClusters(List<Point> points, double eps, int minPts)
        {
            if (points == null) return null;
            List<List<Point>> clusters = new List<List<Point>>();
            eps *= eps; // square eps
            int clusterId = 1;
            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                if (p.ClusterId == Point.UNCLASSIFIED)
                {
                    if (ExpandCluster(points, p, clusterId, eps, minPts)) clusterId++;
                }
            }
            // sort out points into their clusters, if any
            int maxClusterId = points.OrderBy(p => p.ClusterId).Last().ClusterId;
            if (maxClusterId < 1) return clusters; // no clusters, so list is empty
            for (int i = 0; i < maxClusterId; i++) clusters.Add(new List<Point>());
            foreach (Point p in points)
            {
                if (p.ClusterId > 0) clusters[p.ClusterId - 1].Add(p);
            }
            return clusters;
        }
        static List<Point> GetRegion(List<Point> points, Point p, double eps)
        {
            List<Point> region = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                double distSquared = Point.DistanceSquared(p, points[i]);
                if (distSquared <= eps) region.Add(points[i]);
            }
            return region;
        }
        static bool ExpandCluster(List<Point> points, Point p, int clusterId, double eps, int minPts)
        {
            List<Point> seeds = GetRegion(points, p, eps);
            if (seeds.Count < minPts) // no core point
            {
                p.ClusterId = Point.NOISE;
                return false;
            }
            else // all points in seeds are density reachable from point 'p'
            {
                for (int i = 0; i < seeds.Count; i++) seeds[i].ClusterId = clusterId;
                seeds.Remove(p);
                while (seeds.Count > 0)
                {
                    Point currentP = seeds[0];
                    List<Point> result = GetRegion(points, currentP, eps);
                    if (result.Count >= minPts)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            Point resultP = result[i];
                            if (resultP.ClusterId == Point.UNCLASSIFIED || resultP.ClusterId == Point.NOISE)
                            {
                                if (resultP.ClusterId == Point.UNCLASSIFIED) seeds.Add(resultP);
                                resultP.ClusterId = clusterId;
                            }
                        }
                    }
                    seeds.Remove(currentP);
                }
                return true;
            }
        }*/
    }
}
