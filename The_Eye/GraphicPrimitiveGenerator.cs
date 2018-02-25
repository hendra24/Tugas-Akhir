using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class GraphicPrimitiveGenerator
    {
        public static ModelVisual3D GetCylinder(MaterialGroup materialGroup, Point3D midPoint, double radius, double depth)
        {
            var cylinder = new Model3DGroup();
            var nearCircle = new CircleAssitor();
            var farCircle = new CircleAssitor();

            var twoPi = Math.PI * 2;
            var firstPass = true;

            double x;
            double y;

            var increment = 0.1d;
            for (double i = 0; i < twoPi + increment; i = i + increment)
            {
                x = (radius * Math.Cos(i));
                y = (-radius * Math.Sin(i));

                farCircle.CurrentTriangle.P0 = midPoint;
                farCircle.CurrentTriangle.P1 = farCircle.LastPoint;
                farCircle.CurrentTriangle.P2 = new Point3D(x + midPoint.X, y + midPoint.Y, midPoint.Z);

                nearCircle.CurrentTriangle = farCircle.CurrentTriangle.Clone(depth, true);

                if (!firstPass)
                {
                    cylinder.Children.Add(CreateTriangleModel(materialGroup, farCircle.CurrentTriangle));
                    cylinder.Children.Add(CreateTriangleModel(materialGroup, nearCircle.CurrentTriangle));

                    cylinder.Children.Add(CreateTriangleModel(materialGroup, farCircle.CurrentTriangle.P2, farCircle.CurrentTriangle.P1, nearCircle.CurrentTriangle.P2));
                    cylinder.Children.Add(CreateTriangleModel(materialGroup, nearCircle.CurrentTriangle.P2, nearCircle.CurrentTriangle.P1, farCircle.CurrentTriangle.P2));
                }
                else
                {
                    farCircle.FirstPoint = farCircle.CurrentTriangle.P1;
                    nearCircle.FirstPoint = nearCircle.CurrentTriangle.P1;
                    firstPass = false;
                }
                farCircle.LastPoint = farCircle.CurrentTriangle.P2;
                nearCircle.LastPoint = nearCircle.CurrentTriangle.P2;
            }
            cylinder.Freeze();
            var model = new ModelVisual3D { Content = cylinder };
            return model;
        }


        public static ModelVisual3D CreateSphere(Point3D center, double radius, int u, int v, Color color)
        {
            Model3DGroup spear = new Model3DGroup();

            if (u < 2 || v < 2)
                return null;
            Point3D[,] pts = new Point3D[u, v];
            for (int i = 0; i < u; i++)
            {
                for (int j = 0; j < v; j++)
                {
                    pts[i, j] = GetPosition(radius,
                    i * 180 / (u - 1), j * 360 / (v - 1));
                    pts[i, j] += (Vector3D)center;
                }
            }

            Point3D[] p = new Point3D[4];
            for (int i = 0; i < u - 1; i++)
            {
                for (int j = 0; j < v - 1; j++)
                {
                    p[0] = pts[i, j];
                    p[1] = pts[i + 1, j];
                    p[2] = pts[i + 1, j + 1];
                    p[3] = pts[i, j + 1];
                    spear.Children.Add(CreateTriangleFace(p[0], p[1], p[2], color));
                    spear.Children.Add(CreateTriangleFace(p[2], p[3], p[0], color));
                }
            }
            spear.Freeze();
            ModelVisual3D model = new ModelVisual3D();
            model.Content = spear;
            
            return model;
        }


        private static Point3D GetPosition(double radius, double theta, double phi)
        {
            Point3D pt = new Point3D();
            double snt = Math.Sin(theta * Math.PI / 180);
            double cnt = Math.Cos(theta * Math.PI / 180);
            double snp = Math.Sin(phi * Math.PI / 180);
            double cnp = Math.Cos(phi * Math.PI / 180);
            pt.X = radius * snt * cnp;
            pt.Y = radius * cnt;
            pt.Z = -radius * snt * snp;
            return pt;
        }


        public static Model3DGroup CreateTriangleFace(Point3D p0, Point3D p1, Point3D p2, Color color)
        {
            MeshGeometry3D mesh = new MeshGeometry3D(); mesh.Positions.Add(p0); mesh.Positions.Add(p1); mesh.Positions.Add(p2); mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(2);

            Vector3D normal = VectorHelper.CalcNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Freeze();
            Material material = new DiffuseMaterial(
                new SolidColorBrush(color));
            GeometryModel3D model = new GeometryModel3D(
                mesh, material);
            model.Freeze();
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);
            group.Freeze();
            return group;
        }


        private class VectorHelper
        {
            public static Vector3D CalcNormal(Point3D p0, Point3D p1, Point3D p2)
            {
                Vector3D v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
                Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
                return Vector3D.CrossProduct(v0, v1);
            }
        }

        public static MaterialGroup GetSurfaceMaterial(Color colour)
        {
            var materialGroup = new MaterialGroup();
            var emmMat = new EmissiveMaterial(new SolidColorBrush(colour));
            materialGroup.Children.Add(emmMat);
            materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(colour)));
            var specMat = new SpecularMaterial(new SolidColorBrush(Colors.White), 30);
            materialGroup.Children.Add(specMat);
            return materialGroup;
        }


        public static ModelVisual3D GetCube(MaterialGroup materialGroup, Point3D point, Size3D size)
        {
            var farPoint = new Point3D(point.X - (size.X / 2), point.Y - (size.Y / 2), point.Z - (size.Z / 2));
            var nearPoint = new Point3D(point.X + (size.X / 2), point.Y + (size.Y / 2), point.Z + (size.Z / 2));

            var cube = new Model3DGroup();
            var p0 = new Point3D(farPoint.X, farPoint.Y, farPoint.Z);
            var p1 = new Point3D(nearPoint.X, farPoint.Y, farPoint.Z);
            var p2 = new Point3D(nearPoint.X, farPoint.Y, nearPoint.Z);
            var p3 = new Point3D(farPoint.X, farPoint.Y, nearPoint.Z);
            var p4 = new Point3D(farPoint.X, nearPoint.Y, farPoint.Z);
            var p5 = new Point3D(nearPoint.X, nearPoint.Y, farPoint.Z);
            var p6 = new Point3D(nearPoint.X, nearPoint.Y, nearPoint.Z);
            var p7 = new Point3D(farPoint.X, nearPoint.Y, nearPoint.Z);
            //front side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p3, p2, p6));
            cube.Children.Add(CreateTriangleModel(materialGroup, p3, p6, p7));
            //right side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p2, p1, p5));
            cube.Children.Add(CreateTriangleModel(materialGroup, p2, p5, p6));
            //back side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p1, p0, p4));
            cube.Children.Add(CreateTriangleModel(materialGroup, p1, p4, p5));
            //left side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p0, p3, p7));
            cube.Children.Add(CreateTriangleModel(materialGroup, p0, p7, p4));
            //top side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p7, p6, p5));
            cube.Children.Add(CreateTriangleModel(materialGroup, p7, p5, p4));
            //bottom side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p2, p3, p0));
            cube.Children.Add(CreateTriangleModel(materialGroup, p2, p0, p1));
            cube.Freeze();
            var model = new ModelVisual3D();
            model.Content = cube;
            return model;
        }


        public static ModelVisual3D GetTexturedCube(DiffuseMaterial materialGroup, Point3D point, Size3D size)
        {
            var farPoint = new Point3D(point.X - (size.X / 2), point.Y - (size.Y / 2), point.Z - (size.Z / 2));
            var nearPoint = new Point3D(point.X + (size.X / 2), point.Y + (size.Y / 2), point.Z + (size.Z / 2));

            var cube = new Model3DGroup();
            var p0 = new Point3D(farPoint.X, farPoint.Y, farPoint.Z);
            var p1 = new Point3D(nearPoint.X, farPoint.Y, farPoint.Z);
            var p2 = new Point3D(nearPoint.X, farPoint.Y, nearPoint.Z);
            var p3 = new Point3D(farPoint.X, farPoint.Y, nearPoint.Z);
            var p4 = new Point3D(farPoint.X, nearPoint.Y, farPoint.Z);
            var p5 = new Point3D(nearPoint.X, nearPoint.Y, farPoint.Z);
            var p6 = new Point3D(nearPoint.X, nearPoint.Y, nearPoint.Z);
            var p7 = new Point3D(farPoint.X, nearPoint.Y, nearPoint.Z);
            //front side triangles
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p3, p2, p6, 0, 0, 1, 0, 1, -1));
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p3, p6, p7, 0, 0, 1, -1, 0, -1));
            //right side triangles
            //cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p2, p1, p5, 0, 1, 1, 1, 1, 0));
            //cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p2, p5, p6, 1, 1, 1, 0, 0, 0));
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p2, p1, p5, 0, 0, 1, 0, 1, -1));
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p2, p5, p6, 0, 0, 1, -1, 0, -1));

            //back side triangles
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p1, p0, p4, 0, 0, 1, 0, 1, -1));
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p1, p4, p5, 0, 0, 1, -1, 0, -1));
            //left side triangles
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p0, p3, p7, 0, 0, 1, 0, 1, -1));
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p0, p7, p4, 0, 0, 1, -1, 0, -1));
            //top side triangles
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p7, p6, p5, 0, 0, 1, 0, 1, -1));
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p7, p5, p4, 0, 0, 1, -1, 0, -1));
            //bottom side triangles
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p2, p3, p0, 0, 0, 1, 0, 1, -1));
            cube.Children.Add(CreateTexturedTriangleModel(materialGroup, p2, p0, p1, 0, 0, 1, -1, 0, -1));
            cube.Freeze();
            var model = new ModelVisual3D();
            model.Content = cube;
            return model;
        }

        private static Model3DGroup CreateTexturedTriangleModel(Material material, Point3D p0, Point3D p1, Point3D p2,
            int coord1, int coord2, int coord3, int coord4, int coord5, int coord6)
        {
            var mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TextureCoordinates.Add(new Point(coord1, coord2));
            mesh.TextureCoordinates.Add(new Point(coord3, coord4));
            mesh.TextureCoordinates.Add(new Point(coord5, coord6));


            var normal = CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Freeze();
            var model = new GeometryModel3D(mesh, material);
            model.Freeze();
            var group = new Model3DGroup();
            group.Children.Add(model);
            return group;
        }

        private static Model3DGroup CreateTriangleModel(MaterialGroup materialGroup, Triangle triangle)
        {
            return CreateTriangleModel(materialGroup, triangle.P0, triangle.P1, triangle.P2);
        }


        private static Model3DGroup CreateTriangleModel(Material material, Point3D p0, Point3D p1, Point3D p2)
        {
            var mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            var normal = CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Freeze();
            var model = new GeometryModel3D(mesh, material);
            model.Freeze();
            var group = new Model3DGroup();
            group.Children.Add(model);
            return group;
        }


        private static Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        /*
        void Transform(double adjustBy)
        {
            _angle += adjustBy;

            var rotateTransform3D = new RotateTransform3D { CenterX = 0, CenterZ = 0 };
            var axisAngleRotation3D = new AxisAngleRotation3D { Axis = new Vector3D(1, 1, 1), Angle = _angle };
            rotateTransform3D.Rotation = axisAngleRotation3D;
            var myTransform3DGroup = new Transform3DGroup();
            myTransform3DGroup.Children.Add(rotateTransform3D);
            _models.ForEach(x => x.Transform = myTransform3DGroup);
        }*/


        public static ModelVisual3D GetFloor(MaterialGroup materialGroup, Point3D midPoint)
        {
            var cylinder = new Model3DGroup();
            Triangle triangle = new Triangle();
            triangle.P0 = new Point3D(-100, midPoint.Y, 100);
            triangle.P1 = new Point3D(100, midPoint.Y, -100);
            triangle.P2 = new Point3D(-100, midPoint.Y, -100);
            
            //cylinder.Children.Add(CreateTriangleFace(triangle.P2, triangle.P1, triangle.P0, Color.FromRgb(255,0,0)));
            cylinder.Children.Add(CreateTriangleModel(materialGroup, triangle));
            triangle.P0 = new Point3D(100, midPoint.Y, -100);
            triangle.P1 = new Point3D(-100, midPoint.Y, 100);
            triangle.P2 = new Point3D(100, midPoint.Y, 100);
            //cylinder.Children.Add(CreateTriangleFace(triangle.P2, triangle.P1, triangle.P0, Color.FromRgb(255,0,0)));
            cylinder.Children.Add(CreateTriangleModel(materialGroup, triangle));

            cylinder.Freeze();
            var model = new ModelVisual3D { Content = cylinder };
            return model;
        }
    }
}
