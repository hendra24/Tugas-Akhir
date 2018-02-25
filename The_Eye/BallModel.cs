using org.TKinect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;


namespace Microsoft.Samples.Kinect.BodyBasics
{
    class BallModel
    {
        public ArrayList jointIndexAray = new ArrayList();
        public List<ModelVisual3D> _models = new List<ModelVisual3D>();
        double LargeBallSize = 0.05;
        double SmallBllSize = 0.035;
        public ArrayList balls = new ArrayList();
        public BallModel(List<ModelVisual3D> allModels, Color jointsColors, Color limbsColors)
        {
            //Main joints
            for (int b = 0; b < 21; b++)
            {
                _models.Add(GraphicPrimitiveGenerator.CreateSphere(
                    new Point3D(0, 0, 0), LargeBallSize, 7, 7, jointsColors));
                balls.Add(new BallBoundingBox(0, 0, 0, LargeBallSize, b, b));
            }
            //hip knee
            generateHelpJoints(16, 17, 9, limbsColors);
            //knee ankle
            generateHelpJoints(17, 18, 9, limbsColors);
            //ankle foot
            generateHelpJoints(18, 19, 3, limbsColors);
            //spine base hip right
            generateHelpJoints(0, 16, 1, limbsColors);

            //hip knee
            generateHelpJoints(13, 12, 9, limbsColors);
            //knee ankle
            generateHelpJoints(13, 14, 9, limbsColors);
            //ankle foot
            generateHelpJoints(14, 15, 3, limbsColors);
            //spine base hip right
            generateHelpJoints(0, 12, 1, limbsColors);


            //base mid
            generateHelpJoints(0, 1, 7, limbsColors);
            //mid shoulder
            generateHelpJoints(1, 20, 5, limbsColors);
            //shoulder neck
            //generateHelpJoints(2, 20, 10);
            //head neck
            generateHelpJoints(2, 3, 3, limbsColors);



            //shoulder shoulder
            generateHelpJoints(20, 8, 5, limbsColors);
            //shoulder elbow
            generateHelpJoints(8, 9, 9, limbsColors);
            //elbow wrist
            generateHelpJoints(9, 10, 9, limbsColors);
            //hand wrist
            /*generateHelpJoints(10, 11, 1);
            //thumb
            generateHelpJoints(11, 24, 1);
            //hand tip
            generateHelpJoints(11, 23, 1);*/

            //shoulder shoulder
            generateHelpJoints(20, 4, 5, limbsColors);
            //shoulder elbow
            generateHelpJoints(4, 5, 9, limbsColors);
            //elbow wrist
            generateHelpJoints(5, 6, 9, limbsColors);

            //t3DModels = new Transform3D[_models.Count];
            allModels.AddRange(_models);
        }
        private void generateHelpJoints(int index1, int index2, int count, Color limbColor)
        {
            ModelVisual3D mv3d = null;

            JointIndex ji = new JointIndex();
            ji.joint1 = index1;
            ji.joint2 = index2;
            for (int b = 0; b < count; b++)
            {
                mv3d = GraphicPrimitiveGenerator.CreateSphere(
                    new Point3D(0, 0, 0), SmallBllSize, 4, 4, limbColor);
                _models.Add(mv3d);
                ji.visuals.Add(mv3d);

                balls.Add(new BallBoundingBox(0, 0, 0, SmallBllSize, index1, index2));
            }
            jointIndexAray.Add(ji);
        }


        
        public void DetectCollision(BallModel ballModel, ArrayList collisions)
        {
            for (int a = 0; a < balls.Count; a++)
            {
                for (int b = a; b < ballModel.balls.Count; b++)
                {
                    BallBoundingBox bbb = (BallBoundingBox)balls[a];
                    BallBoundingBox bbb2 = (BallBoundingBox)ballModel.balls[b];
                    if (bbb.Collide(bbb2))
                    {
                        if (collisions != null)
                        {
                            CollisionExemplar ce = new CollisionExemplar(bbb.JointType1, bbb.JointType2, bbb2.JointType1, bbb2.JointType2);
                            if (!collisions.Contains(ce))
                                collisions.Add(ce);
                        }
                    }
                }
            }
        }
        //Transform3D[] t3DModels = null;

        private void MoveBalls(TSkeleton skeleton)
        {
            for (int a = 0; a < 21; a++)
            {
                BallBoundingBox bbb = (BallBoundingBox)balls[a];
                bbb.X = skeleton.Position[a].X;
                bbb.Y = skeleton.Position[a].Y;
                bbb.Z = skeleton.Position[a].Z;
            }
            for (int a = 0; a < jointIndexAray.Count; a++)
            {
                BallBoundingBox bbb = (BallBoundingBox)balls[a + 21];
                JointIndex ji = (JointIndex)jointIndexAray[a];
                int divHelper = ji.visuals.Count + 1;
                for (int b = 0; b < ji.visuals.Count; b++)
                {
                    ModelVisual3D mv3d = (ModelVisual3D)ji.visuals[b];
                    float f16X = skeleton.Position[ji.joint1].X;
                    float f16Y = skeleton.Position[ji.joint1].Y;
                    float f16Z = skeleton.Position[ji.joint1].Z;

                    float f17X = skeleton.Position[ji.joint2].X;
                    float f17Y = skeleton.Position[ji.joint2].Y;
                    float f17Z = skeleton.Position[ji.joint2].Z;

                    float aa = (float)(b + 1) / divHelper;
                    //float aa = (float)b / ji.visuals.Count;
                    bbb.X = (aa * f16X) + ((1 - aa) * f17X);
                    bbb.Y = (aa * f16Y) + ((1 - aa) * f17Y);
                    bbb.Z = (aa * f16Z) + ((1 - aa) * f17Z);
                }
            }
        }

        public void RenderModel(bool CheckBoxRelativeMoveIsChecked, TSkeleton skeleton)
        {
            MoveBalls(skeleton);
            for (int a = 0; a < 21; a++)
            {
                ModelVisual3D mv3d = _models[a];
                //Transform3DGroup transGroup = new Transform3DGroup();
                TranslateTransform3D translationTraansorm;
                if (CheckBoxRelativeMoveIsChecked == true)
                {
                    translationTraansorm = new TranslateTransform3D(skeleton.Position[a].X - skeleton.Position[0].X,
                    skeleton.Position[a].Y,// - skeletons[0].Position[0].Y,
                    skeleton.Position[a].Z - skeleton.Position[0].Z);
                }
                else
                {
                    translationTraansorm = new TranslateTransform3D(skeleton.Position[a].X,
                    skeleton.Position[a].Y,// - skeletons[0].Position[0].Y,
                    skeleton.Position[a].Z);
                }
                var myTransform3DGroup = new Transform3DGroup();
                myTransform3DGroup.Children.Add(translationTraansorm);
                mv3d.Transform = myTransform3DGroup;
            }
            
            for (int a = 0; a < jointIndexAray.Count; a++)
            {
                JointIndex ji = (JointIndex)jointIndexAray[a];
                int divHelper = ji.visuals.Count + 1;
                for (int b = 0; b < ji.visuals.Count; b++)
                {
                    ModelVisual3D mv3d = (ModelVisual3D)ji.visuals[b];
                    float f16X = skeleton.Position[ji.joint1].X;
                    float f16Y = skeleton.Position[ji.joint1].Y;
                    float f16Z = skeleton.Position[ji.joint1].Z;

                    float f17X = skeleton.Position[ji.joint2].X;
                    float f17Y = skeleton.Position[ji.joint2].Y;
                    float f17Z = skeleton.Position[ji.joint2].Z;

                    float aa = (float)(b + 1) / divHelper;
                    //float aa = (float)b / ji.visuals.Count;
                    TranslateTransform3D translationTraansorm;
                    if (CheckBoxRelativeMoveIsChecked == true)
                    {
                        translationTraansorm = new TranslateTransform3D((aa * f16X) + ((1 - aa) * f17X) - skeleton.Position[0].X,
                        (aa * f16Y) + ((1 - aa) * f17Y),// - skeletons[0].Position[0].Y,
                        (aa * f16Z) + ((1 - aa) * f17Z) - skeleton.Position[0].Z);
                    }
                    else
                    {
                        translationTraansorm = new TranslateTransform3D((aa * f16X) + ((1 - aa) * f17X),
                        (aa * f16Y) + ((1 - aa) * f17Y),// - skeletons[0].Position[0].Y,
                        (aa * f16Z) + ((1 - aa) * f17Z));
                    }
                    var myTransform3DGroup = new Transform3DGroup();
                    myTransform3DGroup.Children.Add(translationTraansorm);
                    mv3d.Transform = myTransform3DGroup;
                }
            }
        }
    }
}
