using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;

namespace org.GDL
{
    public class GDLInterpreter
    {
        public ParserToken[] RuleTable = null;
        public ParserToken[] FeatureTable = null;
        public int HeapSize = 300;
        public ArrayList Heap = new ArrayList();
        public void ResetHeap()
        {
            while (Heap.Count > 0)
                Heap.RemoveAt(0);
        }
        public GDLInterpreter(ParserToken[] FeatureTable, ParserToken[] RuleTable)
        {
            this.RuleTable = RuleTable;
            this.FeatureTable = FeatureTable;
        }
        public GDLInterpreter(ParserToken[] FeatureTable, ParserToken[] RuleTable, int HeapSize)
        {
            this.RuleTable = RuleTable;
            this.FeatureTable = FeatureTable;
            this.HeapSize = HeapSize;
        }
        public bool ReplaceExceptionByZeroFalseAndInfinity = true;

        private double ReturnBodyPartValue(int position, String text)
        {
            int index = -1;
            int indicator = -1;
            for (int a = 0; a < ParserToken.BodyParts.Length; a++)
            {
                if (ParserToken.BodyParts[a] == text)
                    index = a;
            }
            indicator = index % 4;
            index = index / 4;
            if (position >= Heap.Count)
                position = Heap.Count - 1;
            TrackingMemory tm = (TrackingMemory)Heap[position];
            if (tm.BodyParts == null) return 0;
            if (indicator == 0)
                return tm.BodyParts[index].X;
            if (indicator == 1)
                return tm.BodyParts[index].Y;
            if (indicator == 2)
                return tm.BodyParts[index].Z;
            if (indicator == 3)
            {
                /*
                MyVector3D v1 = new MyVector3D();
                MyVector3D v2 = new MyVector3D();
                ////////////////////
                //RIGHT
                ////////////////////
                if (text.ToLower().Contains("ElbowRight".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.ShoulderRight].X - tm.BodyParts[(int)JointTypeDictionary.ElbowRight].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.ShoulderRight].Y - tm.BodyParts[(int)JointTypeDictionary.ElbowRight].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.ShoulderRight].Z - tm.BodyParts[(int)JointTypeDictionary.ElbowRight].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.WristRight].X - tm.BodyParts[(int)JointTypeDictionary.ElbowRight].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.WristRight].Y - tm.BodyParts[(int)JointTypeDictionary.ElbowRight].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.WristRight].Z - tm.BodyParts[(int)JointTypeDictionary.ElbowRight].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }

                if (text.ToLower().Contains("WristRight".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.ElbowRight].X - tm.BodyParts[(int)JointTypeDictionary.WristRight].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.ElbowRight].Y - tm.BodyParts[(int)JointTypeDictionary.WristRight].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.ElbowRight].Z - tm.BodyParts[(int)JointTypeDictionary.WristRight].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.HandRight].X - tm.BodyParts[(int)JointTypeDictionary.WristRight].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.HandRight].Y - tm.BodyParts[(int)JointTypeDictionary.WristRight].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.HandRight].Z - tm.BodyParts[(int)JointTypeDictionary.WristRight].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }

                if (text.ToLower().Contains("ShoulderRight".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.ElbowRight].X - tm.BodyParts[(int)JointTypeDictionary.ShoulderRight].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.ElbowRight].Y - tm.BodyParts[(int)JointTypeDictionary.ShoulderRight].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.ElbowRight].Z - tm.BodyParts[(int)JointTypeDictionary.ShoulderRight].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.ShoulderCenter].X - tm.BodyParts[(int)JointTypeDictionary.ShoulderRight].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.ShoulderCenter].Y - tm.BodyParts[(int)JointTypeDictionary.ShoulderRight].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.ShoulderCenter].Z - tm.BodyParts[(int)JointTypeDictionary.ShoulderRight].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }


                if (text.ToLower().Contains("KneeRight".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.HipRight].X - tm.BodyParts[(int)JointTypeDictionary.KneeRight].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.HipRight].Y - tm.BodyParts[(int)JointTypeDictionary.KneeRight].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.HipRight].Z - tm.BodyParts[(int)JointTypeDictionary.KneeRight].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.AnkleRight].X - tm.BodyParts[(int)JointTypeDictionary.KneeRight].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.AnkleRight].Y - tm.BodyParts[(int)JointTypeDictionary.KneeRight].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.AnkleRight].Z - tm.BodyParts[(int)JointTypeDictionary.KneeRight].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }

                if (text.ToLower().Contains("HipRight".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.HipCenter].X - tm.BodyParts[(int)JointTypeDictionary.HipRight].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.HipCenter].Y - tm.BodyParts[(int)JointTypeDictionary.HipRight].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.HipCenter].Z - tm.BodyParts[(int)JointTypeDictionary.HipRight].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.KneeRight].X - tm.BodyParts[(int)JointTypeDictionary.HipRight].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.KneeRight].Y - tm.BodyParts[(int)JointTypeDictionary.HipRight].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.KneeRight].Z - tm.BodyParts[(int)JointTypeDictionary.HipRight].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }

                ////////////////////
                //LEFT
                ////////////////////
                if (text.ToLower().Contains("ElbowLeft".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.ShoulderLeft].X - tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.ShoulderLeft].Y - tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.ShoulderLeft].Z - tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.WristLeft].X - tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.WristLeft].Y - tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.WristLeft].Z - tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }

                if (text.ToLower().Contains("WristLeft".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].X - tm.BodyParts[(int)JointTypeDictionary.WristLeft].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].Y - tm.BodyParts[(int)JointTypeDictionary.WristLeft].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].Z - tm.BodyParts[(int)JointTypeDictionary.WristLeft].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.HandLeft].X - tm.BodyParts[(int)JointTypeDictionary.WristLeft].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.HandLeft].Y - tm.BodyParts[(int)JointTypeDictionary.WristLeft].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.HandLeft].Z - tm.BodyParts[(int)JointTypeDictionary.WristLeft].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }

                if (text.ToLower().Contains("ShoulderLeft".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].X - tm.BodyParts[(int)JointTypeDictionary.ShoulderLeft].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].Y - tm.BodyParts[(int)JointTypeDictionary.ShoulderLeft].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.ElbowLeft].Z - tm.BodyParts[(int)JointTypeDictionary.ShoulderLeft].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.ShoulderCenter].X - tm.BodyParts[(int)JointTypeDictionary.ShoulderLeft].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.ShoulderCenter].Y - tm.BodyParts[(int)JointTypeDictionary.ShoulderLeft].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.ShoulderCenter].Z - tm.BodyParts[(int)JointTypeDictionary.ShoulderLeft].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }

                if (text.ToLower().Contains("KneeLeft".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.HipLeft].X - tm.BodyParts[(int)JointTypeDictionary.KneeLeft].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.HipLeft].Y - tm.BodyParts[(int)JointTypeDictionary.KneeLeft].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.HipLeft].Z - tm.BodyParts[(int)JointTypeDictionary.KneeLeft].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.AnkleLeft].X - tm.BodyParts[(int)JointTypeDictionary.KneeLeft].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.AnkleLeft].Y - tm.BodyParts[(int)JointTypeDictionary.KneeLeft].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.AnkleLeft].Z - tm.BodyParts[(int)JointTypeDictionary.KneeLeft].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }

                if (text.ToLower().Contains("HipLeft".ToLower()))
                {
                    v1.X = tm.BodyParts[(int)JointTypeDictionary.HipCenter].X - tm.BodyParts[(int)JointTypeDictionary.HipLeft].X;
                    v1.Y = tm.BodyParts[(int)JointTypeDictionary.HipCenter].Y - tm.BodyParts[(int)JointTypeDictionary.HipLeft].Y;
                    v1.Z = tm.BodyParts[(int)JointTypeDictionary.HipCenter].Z - tm.BodyParts[(int)JointTypeDictionary.HipLeft].Z;

                    v2.X = tm.BodyParts[(int)JointTypeDictionary.KneeLeft].X - tm.BodyParts[(int)JointTypeDictionary.HipLeft].X;
                    v2.Y = tm.BodyParts[(int)JointTypeDictionary.KneeLeft].Y - tm.BodyParts[(int)JointTypeDictionary.HipLeft].Y;
                    v2.Z = tm.BodyParts[(int)JointTypeDictionary.KneeLeft].Z - tm.BodyParts[(int)JointTypeDictionary.HipLeft].Z;

                    return (MyVector3D.AngleBetween(v1, v2));
                }*/
            }
            return 0;
        }

        private Point3D ReturnBodyPartValue3D(int position, String text)
        {
            int index = -1;
            //int indicator = -1;
            for (int a = 0; a < ParserToken.BodyParts3D.Length; a++)
            {
                if (ParserToken.BodyParts3D[a] == text)
                    index = a;
            }
            if (position >= Heap.Count)
                position = Heap.Count - 1;
            TrackingMemory tm = (TrackingMemory)Heap[position];
            if (tm.BodyParts == null) return new Point3D(0,0,0);
            return tm.BodyParts[index];
        }

        private bool StringInArray(String stringHelp, String[] stringArray)
        {
            if (stringArray == null) return false;
            for (int a = 0; a < stringArray.Length; a++)
                if (stringHelp == stringArray[a])
                    return true;
            return false;
        }

        private bool CheckRulePersistence(String conclusion, double persistaceTime, double persistancePercentage)
        {
            int c = 0;
            TrackingMemory tm = null;
            double timePeriod = 0;
            bool stringInArray = false;
            double conclusionsCount = 0;
            do
            {
                //jeœli wiêcje ni¿ rozmiar stosu - koniec
                if (c >= Heap.Count)
                    timePeriod = persistaceTime + 1;
                else
                {
                    //pobieramy ze stosu tak d³ugo, a¿ znajdiezmy sekwencjê, albo przekroczymy czas (ale zawsze conajmniej raz)
                    tm = (TrackingMemory)Heap[c];
                    c++;
                    timePeriod += tm.TimePeriod;
                    stringInArray = StringInArray(conclusion, tm.Conclusions);
                    if (stringInArray)
                        conclusionsCount++;
                }
            }
            while (timePeriod < persistaceTime);
            double percentageResult = conclusionsCount / (double)c;
            if (percentageResult >= persistancePercentage)
                return true;
            return false;
        }
        /*
        private Point3D ComputeCheckCircularity(String BodyPart3DType, double timeOnHeap, double minimalAcceptedCollinearityCoefficent, double minimalRadius)
        {
            int maxDepth = 0;
            bool end = false;
            double timePeriod = 0;
            while (!end)
            {
                if (maxDepth < Heap.Count)
                {
                    TrackingMemory tm = (TrackingMemory)Heap[maxDepth];
                    timePeriod += tm.TimePeriod;
                    if (timePeriod > timeOnHeap)
                        end = true;
                    else
                        maxDepth++;

                }
                else
                {
                    end = true;
                }
            }
            if (maxDepth < 5)
                return new Point3D(0, 0, 0);
            double[] X = new double[maxDepth];
            double[] Y = new double[maxDepth];
            double[] Z = new double[maxDepth];
            Point3D helpValue = null;
            for (int a = 0; a < maxDepth; a++)
            {
                helpValue = ReturnBodyPartValue3D(a, BodyPart3DType);
                X[a] = helpValue.X;
                Y[a] = helpValue.Y;
                Z[a] = helpValue.Z;
            }
            double radius = 0;
            Point3D circleCenter = CircleFitting.ComputeFittedCircleCenterAndRadius(X, Y, Z, ref radius, minimalAcceptedCollinearityCoefficent);
            if (radius < minimalRadius)
                return new Point3D(double.NaN, double.NaN, double.NaN);
            return circleCenter;
        }

        private Point3D ComputeCheckcollinearity(String BodyPart3DType, double timeOnHeap, double minimalAcceptedCollinearityCoefficent)
        {
            int maxDepth = 0;
            bool end = false;
            double timePeriod = 0;
            while (!end)
            {
                if (maxDepth < Heap.Count)
                {
                    TrackingMemory tm = (TrackingMemory)Heap[maxDepth];
                    timePeriod += tm.TimePeriod;
                    if (timePeriod > timeOnHeap)
                        end = true;
                    else
                        maxDepth++;

                }
                else
                {
                    end = true;
                }
            }
            if (maxDepth < 5)
                return new Point3D(0, 0, 0);
            double []X = new double[maxDepth];
            double []Y = new double[maxDepth];
            double []Z = new double[maxDepth];
            Point3D helpValue = null;
            for (int a = 0; a < maxDepth; a++)
            {
                helpValue = ReturnBodyPartValue3D(a, BodyPart3DType);
                X[a] = helpValue.X;
                Y[a] = helpValue.Y;
                Z[a] = helpValue.Z;
            }
            return LineFitting.ComputeFittedLineDirection(X, Y, Z, minimalAcceptedCollinearityCoefficent);
        }
        */
        class ScoreHelper{
            public SequenceToken st = null;
            public int[] EndTime = null;
            public int[] StartTime = null;
        }
        private ArrayList sequenceScore = new ArrayList();

        private double ComputeSequenceScore(ArrayList sequence)
        {
            double timePeriod = 0;
            double result = double.PositiveInfinity;
            String conclusionHelp = "";
            TrackingMemory tm = null;
            int c = 0;
            int maxC = 0, maxCTemp = 0;

            bool stringInArray = false;
            //przeszukujemy ca³¹ sekwencjê
            sequenceScore.Clear();
            //ScoreHelper prevSh = null;
            ScoreHelper sh = null;
            for (int a = 0; a < sequence.Count; a++)
            {
                //pobieramy pierwszy element
                SequenceToken st = (SequenceToken)sequence[a];

                sh = new ScoreHelper();
                sh.st = st;
                sh.EndTime = new int[st.Conclusions.Length];
                sh.StartTime = new int[st.Conclusions.Length];
                sequenceScore.Add(sh);
                /*if (st.Features != null)
                {    
                    sequenceScore.Add(sh);
                }*/
                /*if (prevSh != null)
                {
                    for (int d = 0; d < prevSh.StartTime.Length; d++)
                    {
                    }
                }*/
                //pobieramy wszytskie konkluzje
                for (int b = 0; b < st.Conclusions.Length; b++)
                {
                    //zerujemy okres czasu
                    timePeriod = 0;
                    //startujemy od pozycji na stosie, na której zakoñczyliœmy analizê poprzedniego tokena sekwencji
                    c = maxC;
                    /*if (st.Features != null)
                    {
                        for (int d = 0; d < st.Features.Length; d++)
                        {
                            sh.EndTime = 
                        }
                    }*/
                    conclusionHelp = st.Conclusions[b];
                    stringInArray = false;
                    do
                    {

                        //jeœli wiêcje ni¿ rozmiar stosu - koniec
                        if (c >= Heap.Count)
                            timePeriod = st.TimeConstraintSeconds + 1;
                        else
                        {
                            //pobieramy ze stosu tak d³ugo, a¿ znajdiezmy sekwencjê, albo przekroczymy czas (ale zawsze conajmniej raz)
                            tm = (TrackingMemory)Heap[c];
                            c++;
                            timePeriod += tm.TimePeriod;
                            stringInArray = StringInArray(conclusionHelp, tm.Conclusions);
                            
                            if (stringInArray)
                            {
                                //result += (double)tm.Features[st.Features[b]];
                                sh.EndTime[b] = c - 1;
                            }
                        }
                    }
                    while (!stringInArray && timePeriod <= st.TimeConstraintSeconds);
                    //jeœli nie znaleŸlismy konkluzji, to znaczy, ¿e przekroczyliœmy limit czasu
                    if (!stringInArray && st.Prefixes[b] == "") return result;
                    //jeœli jest zaprzeczenie - to przy znalezieniu konkluzji jest to nieprawda
                    if (stringInArray && st.Prefixes[b] == "!") return result;
                    //zapamiêtujemy najd³u¿szy do to pory czas (to znaczy ile musieliœmy przegl¹dn¹æ na g³êbokoœæ w stosie, ¿eby znaleŸæ t¹
                    //sekwencjê                   
                    if (maxCTemp < c) maxCTemp = c;
                }
                //ustaw startow¹ pozycjê stosu na ostatnio u¿yt¹ +1
                maxC = maxCTemp + 1;
            }
            for (int a = 0; a < sequenceScore.Count -1; a++)
            {
                ScoreHelper s1 = (ScoreHelper)sequenceScore[a];
                ScoreHelper s2 = (ScoreHelper)sequenceScore[a +1];
                int maxTime = 0;
                for (int b = 0; b < s2.EndTime.Length; b++)
                {
                    if (s2.EndTime[b] > maxTime)
                        maxTime = s2.EndTime[b];
                }
                for (int b = 0; b < s1.StartTime.Length; b++)
                {
                    s1.StartTime[b] = maxTime;
                }
            }
            ScoreHelper lastSHelper = (ScoreHelper)sequenceScore[sequenceScore.Count - 1];
            timePeriod = 0;
            c = maxC;
            do
            {
                if (c >= Heap.Count)
                {
                    timePeriod = lastSHelper.st.TimeConstraintSeconds + 1;
                    maxC = c - 1;
                }
                else
                {
                    //pobieramy ze stosu tak d³ugo, a¿ znajdiezmy sekwencjê, albo przekroczymy czas (ale zawsze conajmniej raz)
                    tm = (TrackingMemory)Heap[c];
                    c++;
                    timePeriod += tm.TimePeriod;
                    maxC = c;
                }
            }
            while (timePeriod <= lastSHelper.st.TimeConstraintSeconds);
            for (int b = 0; b < lastSHelper.StartTime.Length; b++)
            {
                lastSHelper.StartTime[b] = maxC;
            }
            int minIndex = 0;
            for (int a = 0; a < sequenceScore.Count; a++)
            {
                sh = (ScoreHelper)sequenceScore[a];
                if (sh.st.Features != null)
                {
                    for (int b = 0; b < sh.st.Features.Length; b++)
                    {
                        //double minValue = double.PositiveInfinity;
                        minIndex = sh.EndTime[b];
                        conclusionHelp = sh.st.Conclusions[b];
                        String featureName = sh.st.Features[b];
                        /*if (sh.st.FeaturesToTakesValuesFrom != null)
                            featureName = sh.st.FeaturesToTakesValuesFrom[b];
                        else
                            featureName = sh.st.Features[b];*/
                        for (c = sh.EndTime[b]; c <= sh.StartTime[b]; c++)
                        {
                            tm = (TrackingMemory)Heap[c];
                            if (StringInArray(conclusionHelp, tm.Conclusions))
                            {
                                double aa = (double)tm.Features[featureName];
                                if ((double)((TrackingMemory)Heap[minIndex]).Features[featureName] > (double)tm.Features[featureName])
                                {
                                    minIndex = c;
                                    //minValue = (double)tm.Features[featureName];
                                }
                            }
                        }
                        if (sh.st.FeaturesToTakesValuesFrom != null)
                            featureName = sh.st.FeaturesToTakesValuesFrom[b];
                        if (double.IsInfinity(result))
                        {
                            result = (double)(((TrackingMemory)Heap[minIndex]).Features[featureName]);
                        }
                        else
                        {
                            result += (double)(((TrackingMemory)Heap[minIndex]).Features[featureName]);
                        }
                    }
                }
            }
            return result;
        }

        /*
        private double ComputeSequenceScore(ArrayList sequence)
        {
            double timePeriod = 0;
            double result = 0;
            String conclusionHelp = "";
            TrackingMemory tm = null;
            int c = 0;
            int maxC = 0, maxCTemp = 0;

            bool stringInArray = false;
            //przeszukujemy ca³¹ sekwencjê
            for (int a = 0; a < sequence.Count; a++)
            {
                //pobieramy pierwszy element
                SequenceToken st = (SequenceToken)sequence[a];
                
                
                //pobieramy wszytskie konkluzje
                for (int b = 0; b < st.Conclusions.Length; b++)
                {
                    //zerujemy okres czasu
                    timePeriod = 0;
                    //startujemy od pozycji na stosie, na której zakoñczyliœmy analizê poprzedniego tokena sekwencji
                    c = maxC;
                    conclusionHelp = st.Conclusions[b];
                    stringInArray = false;
                    do
                    {
                        
                        //jeœli wiêcje ni¿ rozmiar stosu - koniec
                        if (c >= Heap.Count)
                            timePeriod = st.TimeConstraintSeconds + 1;
                        else
                        {
                            //pobieramy ze stosu tak d³ugo, a¿ znajdiezmy sekwencjê, albo przekroczymy czas (ale zawsze conajmniej raz)
                            tm = (TrackingMemory)Heap[c];
                            c++;
                            timePeriod += tm.TimePeriod;
                            stringInArray = StringInArray(conclusionHelp, tm.Conclusions);
                            if (st.Features != null && stringInArray)
                            {
                                result += (double)tm.Features[st.Features[b]];
                            }
                        }
                    }
                    while (!stringInArray && timePeriod <= st.TimeConstraintSeconds);
                    //jeœli nie znaleŸlismy konkluzji, to znaczy, ¿e przekroczyliœmy limit czasu
                    if (!stringInArray && st.Prefixes[b] == "") return result;
                    //jeœli jest zaprzeczenie - to przy znalezieniu konkluzji jest to nieprawda
                    if (stringInArray && st.Prefixes[b] == "!") return result;
                    //zapamiêtujemy najd³u¿szy do to pory czas (to znaczy ile musieliœmy przegl¹dn¹æ na g³êbokoœæ w stosie, ¿eby znaleŸæ t¹
                    //sekwencjê
                    if (maxCTemp < c) maxCTemp = c;
                }
                //ustaw startow¹ pozycjê stosu na ostatnio u¿yt¹ +1
                maxC = maxCTemp + 1;
            }
            return result;
        }*/

        private bool CheckSequence(ArrayList sequence)
        {
            double timePeriod = 0;
            String conclusionHelp = "";
            TrackingMemory tm = null;
            int c = 0;
            int maxC = 0, maxCTemp = 0;

            bool stringInArray = false;
            //przeszukujemy ca³¹ sekwencjê
            for (int a = 0; a < sequence.Count; a++)
            {
                //pobieramy pierwszy element
                SequenceToken st = (SequenceToken)sequence[a];
                
                
                //pobieramy wszytskie konkluzje
                for (int b = 0; b < st.Conclusions.Length; b++)
                {
                    //zerujemy okres czasu
                    timePeriod = 0;
                    //startujemy od pozycji na stosie, na której zakoñczyliœmy analizê poprzedniego tokena sekwencji
                    c = maxC;
                    conclusionHelp = st.Conclusions[b];
                    stringInArray = false;
                    do
                    {
                        
                        //jeœli wiêcje ni¿ rozmiar stosu - koniec
                        if (c >= Heap.Count)
                            timePeriod = st.TimeConstraintSeconds + 1;
                        else
                        {
                            //pobieramy ze stosu tak d³ugo, a¿ znajdiezmy sekwencjê, albo przekroczymy czas (ale zawsze conajmniej raz)
                            tm = (TrackingMemory)Heap[c];
                            c++;
                            timePeriod += tm.TimePeriod;
                            stringInArray = StringInArray(conclusionHelp, tm.Conclusions);
                        }
                    }
                    while (!stringInArray && timePeriod <= st.TimeConstraintSeconds);
                    //jeœli nie znaleŸlismy konkluzji, to znaczy, ¿e przekroczyliœmy limit czasu
                    if (!stringInArray && st.Prefixes[b] == "") return false;
                    //jeœli jest zaprzeczenie - to przy znalezieniu konkluzji jest to nieprawda
                    if (stringInArray && st.Prefixes[b] == "!") return false;
                    //zapamiêtujemy najd³u¿szy do to pory czas (to znaczy ile musieliœmy przegl¹dn¹æ na g³êbokoœæ w stosie, ¿eby znaleŸæ t¹
                    //sekwencjê
                    if (maxCTemp < c) maxCTemp = c;
                }
                //ustaw startow¹ pozycjê stosu na ostatnio u¿yt¹ +1
                maxC = maxCTemp + 1;
            }
            return true;
        }

        private object FindFeatureValue(Dictionary<String, object> features, String featureName, int col, int ln)
        {
            /*bool found = false;
            for (int a = 0; a < rRVarray.Length; a++)
                if (rRVarray[a].TextValue == featureName)
                    return rRVarray[a];
            if (!found)*/
            if (features.ContainsKey(featureName))
            {
                return features[featureName];
            }
                throw new ParserException("GDL Interpreter exception: unknown feature name: " + featureName, col, ln);
            return null;
        }

        private RuleReturnValue IsSatisfied(ParserToken rule, ArrayList conclusions, Dictionary<String, object> features)
        {
            RuleReturnValue rRV = new RuleReturnValue();
            if (rule.Children.Count == 0)
            {
                if (rule.TokenTypeMemory == ParserToken.TokenTypeNumeric)
                {
                    rRV.DoubleValue = Double.Parse(rule.TokenString, CultureInfo.InvariantCulture);
                    rRV.RuleType = ParserToken.RuleTypeNumeric;
                }
                else if (rule.TokenTypeMemory == ParserToken.TokenTypeConclusion)
                {//TUTUTUTU
                    //feature liczba
                    if (rule.RuleType == ParserToken.RuleTypeNumeric)
                    {
                        rRV.DoubleValue = (double)FindFeatureValue(features, rule.TokenString, rule.PositionCol, rule.PositionLn);
                        rRV.RuleType = ParserToken.RuleTypeNumeric;
                    }
                    //feature liczba3D
                    else if (rule.RuleType == ParserToken.RuleTypeNumeric3D)
                    {
                        rRV.Point3DValue = (Point3D)FindFeatureValue(features, rule.TokenString, rule.PositionCol, rule.PositionLn);
                        rRV.RuleType = ParserToken.RuleTypeNumeric3D;
                    }
                    //konkluzja z regu³y
                    else
                    {
                        rRV.BoolValue = ContainsConclusion(conclusions, rule.TokenString);
                        rRV.RuleType = ParserToken.RuleTypeLogical;
                    }
                }
                else if (rule.TokenTypeMemory == ParserToken.RuleTypeNumeric3D)
                {
                    rRV.Point3DValue = new Point3D(0,0,0);
                    rRV.RuleType = ParserToken.RuleTypeNumeric3D;
                }
                else
                {
                    throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
                }
                return rRV;
            }
            //licz sekwencyjn¹ 2013-09-23
            if (rule.TokenTypeMemory == ParserToken.TokenTypeSequentialFunction)
            {
                if (rule.TokenString == "sequenceexists(")
                {
                    ArrayList sequence = ((ParserToken)rule.Children[0]).Sequence;
                    rRV.BoolValue = CheckSequence(sequence);
                }
                else if (rule.TokenString == "sequencescore(")
                {
                    ArrayList sequence = ((ParserToken)rule.Children[0]).Sequence;
                    rRV.DoubleValue = ComputeSequenceScore(sequence);
                    rRV.RuleType = ParserToken.RuleTypeNumeric;
                    return rRV;
                }
                else if (rule.TokenString == "rulepersists(")
                {
                    //TUTUTUT rulepersists
                    double d1 = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).DoubleValue;
                    double d2 = IsSatisfied((ParserToken)rule.Children[2], conclusions, features).DoubleValue;
                    rRV.BoolValue = CheckRulePersistence(((ParserToken)rule.Children[0]).TokenString, d1, d2);
                }
                else
                {
                    throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
                }
                rRV.RuleType = ParserToken.RuleTypeLogical;
                return rRV;
            }
            //funkcja liczbowa
            if (rule.TokenTypeMemory == ParserToken.TokenTypeNumericFunction)
            {
                rRV.RuleType = ParserToken.RuleTypeNumeric;
                if (rule.TokenString == "sqrt(")
                {
                    double returnDouble = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).DoubleValue;
                    try
                    {
                        rRV.DoubleValue = Math.Sqrt(returnDouble);
                        if (double.IsNaN(rRV.DoubleValue))
                        {
                            if (ReplaceExceptionByZeroFalseAndInfinity) rRV.DoubleValue = 0;
                            else throw new ParserException("Line: " + rule.Position + " SQRT from negative value " + returnDouble, rule.PositionCol, rule.PositionLn);
                        }
                    } catch
                    {
                        throw new ParserException("Line: " + rule.Position + " SQRT from negative value " + returnDouble, rule.PositionCol, rule.PositionLn); 
                    }
                }
                else if (rule.TokenString == "abs(")
                {
                    double returnDouble = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).DoubleValue;
                    rRV.DoubleValue = Math.Abs(returnDouble);

                }
                else if (rule.TokenString == "sgn(")
                {
                    double returnDouble = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).DoubleValue;
                    if (returnDouble < 0)
                        returnDouble = -1;
                    else if (returnDouble > 0)
                        returnDouble = 1;
                    else
                        returnDouble = 0;
                    rRV.DoubleValue = returnDouble;

                }
                else if (rule.TokenString == "sgnfuzzy(")
                {
                    double returnDouble = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).DoubleValue;
                    double returnDouble2 = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).DoubleValue;
                    //double fuzzyBorderHalf = returnDouble2 / 2;
                    if (Math.Abs(returnDouble2) < 0.00001 || Math.Abs(returnDouble) > Math.Abs(returnDouble2))
                    {
                        if (returnDouble < 0)
                            returnDouble = -1;
                        else if (returnDouble > 0)
                            returnDouble = 1;
                        else
                            returnDouble = 0;
                    }
                    else
                    {
                        returnDouble = returnDouble / returnDouble2;
                    }
                    rRV.DoubleValue = returnDouble;

                }
                else
                {
                    throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
                }
                rRV.RuleType = ParserToken.RuleTypeNumeric;
                return rRV;
            }

            //funkcja liczbowa 3D
            if (rule.TokenTypeMemory == ParserToken.TokenTypeNumericFunction3D)
            {
                rRV.RuleType = ParserToken.RuleTypeLogical;
                if (rule.TokenString == "cross(")
                {
                    Point3D l13D = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).Point3DValue;
                    Point3D l23D = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).Point3DValue;
                    MyVector3D v1 = new MyVector3D(l13D.X, l13D.Y, l13D.Z);
                    MyVector3D v2 = new MyVector3D(l23D.X, l23D.Y, l23D.Z);
                    MyVector3D cross = MyVector3D.CrossProduct(v1, v2);
                    rRV.Point3DValue = new Point3D(cross.X, cross.Y, cross.Z);
                    rRV.RuleType = ParserToken.RuleTypeNumeric3D;
                }
                else if (rule.TokenString == "distance(")
                {
                    Point3D l13D = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).Point3DValue;
                    Point3D l23D = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).Point3DValue;
                    try
                    {
                        rRV.DoubleValue = Math.Sqrt(((l13D.X - l23D.X) * (l13D.X - l23D.X))
                        + ((l13D.Y - l23D.Y) * (l13D.Y - l23D.Y))
                        + ((l13D.Z - l23D.Z) * (l13D.Z - l23D.Z)));
                    }
                    catch
                    {
                        throw new ParserException("Line: " + rule.Position + " error while computing distance between 3D points", rule.PositionCol, rule.PositionLn);
                    }
                    rRV.RuleType = ParserToken.RuleTypeNumeric;
                }
                else if (rule.TokenString == "angle(")
                {
                    Point3D l13D = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).Point3DValue;
                    Point3D l23D = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).Point3DValue;
                    MyVector3D v1 = new MyVector3D(l13D.X, l13D.Y, l13D.Z);
                    MyVector3D v2 = new MyVector3D(l23D.X, l23D.Y, l23D.Z);
                    rRV.DoubleValue = MyVector3D.AngleBetween(v1, v2);
                    rRV.RuleType = ParserToken.RuleTypeNumeric;
                }
                /*else if (rule.TokenString == "checkcollinearity(")
                {
                    //Point3D l13D = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).Point3DValue;
                    //Point3D l23D = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).Point3DValue;
                    ParserToken pt = (ParserToken)rule.Children[0];
                    if (pt.TokenTypeMemory != ParserToken.TokenTypeBodyPart3D)
                        throw new ParserException("Different type of CheckCollinearity argument than TokenTypeBodyPart3D. This exception seems critical ;-) ", 0, 0);
                    //TUTUTUTUTU
                    //if (Heap.Count < )
                        //not enough points to make it valuable
                    double timeOnHeap = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).DoubleValue;
                    double minimalAcceptedCollinearityCoefficent = IsSatisfied((ParserToken)rule.Children[2], conclusions, features).DoubleValue;
                    Point3D returnValue = ComputeCheckcollinearity(pt.TokenString, timeOnHeap, minimalAcceptedCollinearityCoefficent);
                    //Point3D l23D = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).Point3DValue;
                    //MyVector3D v1 = new MyVector3D(l13D.X, l13D.Y, l13D.Z);
                    //MyVector3D v2 = new MyVector3D(l23D.X, l23D.Y, l23D.Z);
                    //rRV.DoubleValue = MyVector3D.AngleBetween(v1, v2);
                    rRV.Point3DValue = returnValue;
                    rRV.RuleType = ParserToken.RuleTypeNumeric3D;
                    return rRV;
                }
                else if (rule.TokenString == "checkcircularity(")
                {
                    //Point3D l13D = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).Point3DValue;
                    //Point3D l23D = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).Point3DValue;
                    ParserToken pt = (ParserToken)rule.Children[0];
                    if (pt.TokenTypeMemory != ParserToken.TokenTypeBodyPart3D)
                        throw new ParserException("Different type of CheckCircularity argument than TokenTypeBodyPart3D. This exception seems critical ;-) ", 0, 0);
                    //TUTUTUTUTU
                    //if (Heap.Count < )
                    //not enough points to make it valuable
                    double timeOnHeap = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).DoubleValue;
                    double minimalAcceptedCircularityCoefficent = IsSatisfied((ParserToken)rule.Children[2], conclusions, features).DoubleValue;
                    double miniamalRaius = IsSatisfied((ParserToken)rule.Children[3], conclusions, features).DoubleValue;
                    //Point3D returnValue = Computecheckcollinearity(pt.TokenString, timeOnHeap, minimalAcceptedCollinearityCoefficent);
                    Point3D returnValue = ComputeCheckCircularity(pt.TokenString, timeOnHeap, minimalAcceptedCircularityCoefficent, miniamalRaius);
                    //Point3D l23D = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).Point3DValue;
                    //MyVector3D v1 = new MyVector3D(l13D.X, l13D.Y, l13D.Z);
                    //MyVector3D v2 = new MyVector3D(l23D.X, l23D.Y, l23D.Z);
                    //rRV.DoubleValue = MyVector3D.AngleBetween(v1, v2);
                    rRV.Point3DValue = returnValue;
                    rRV.RuleType = ParserToken.RuleTypeNumeric3D;
                    return rRV;
                }*/
                else
                {
                    throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
                }
                
                return rRV;
            }
            //funkcja logiczna
            if (rule.TokenTypeMemory == ParserToken.TokenTypeLogicalFunction)
            {
                rRV.RuleType = ParserToken.RuleTypeLogical;
                if (rule.TokenString == "not(")
                {
                    bool returnBool = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).BoolValue;
                    rRV.BoolValue = !returnBool;
                }
                else
                {
                    throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
                }
                rRV.RuleType = ParserToken.RuleTypeLogical;
                return rRV;
            }
            //czesc ciala
            //funkcja logiczna
            if (rule.TokenTypeMemory == ParserToken.TokenTypeBodyPart)
            {
                rRV.RuleType = ParserToken.RuleTypeNumeric;
                double l1 = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).DoubleValue;
                rRV.DoubleValue = ReturnBodyPartValue((int)l1, rule.TokenString);
                return rRV;
            }
            //punkt 3D
            if (rule.TokenTypeMemory == ParserToken.TokenTypeOpenSquareBracket)
            {
                rRV.RuleType = ParserToken.RuleTypeNumeric3D;
                double l1 = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).DoubleValue;
                double l2 = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).DoubleValue;
                double l3 = IsSatisfied((ParserToken)rule.Children[2], conclusions, features).DoubleValue;
                rRV.Point3DValue = new Point3D((float)l1, (float)l2, (float)l3);
                return rRV;
            }
            //czesc ciala 3D
            if (rule.TokenTypeMemory == ParserToken.TokenTypeBodyPart3D)
            {
                rRV.RuleType = ParserToken.RuleTypeNumeric3D;
                double l1 = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).DoubleValue;
                if (rule.TokenString[0] == '-')
                {
                    String helpValueString = rule.TokenString.Substring(1);
                    rRV.Point3DValue = ReturnBodyPartValue3D((int)l1, helpValueString);
                    rRV.Point3DValue.X *= -1;
                    rRV.Point3DValue.Y *= -1;
                    rRV.Point3DValue.Z *= -1;
                }
                else
                    rRV.Point3DValue = ReturnBodyPartValue3D((int)l1, rule.TokenString);
                return rRV;
            }
            if (rule.TokenTypeMemory == ParserToken.TokenTypeLogicalOperator)
            {
                bool l1 = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).BoolValue;
                bool l2 = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).BoolValue;
                if (rule.TokenString == "&")
                    rRV.BoolValue = l1 && l2;
                else if (rule.TokenString == "|")
                    rRV.BoolValue = l1 || l2;
                else
                {
                    throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
                }
                rRV.RuleType = ParserToken.RuleTypeLogical;
                return rRV;
            }
            if (rule.TokenTypeMemory == ParserToken.TokenTypeRelationalOperator)
            {
                double l1 = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).DoubleValue;
                double l2 = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).DoubleValue;
                if (rule.TokenString == "<")
                    rRV.BoolValue = l1 < l2;
                else if (rule.TokenString == ">")
                    rRV.BoolValue = l1 > l2;
                else if (rule.TokenString == "=")
                    rRV.BoolValue = l1 == l2;
                else if (rule.TokenString == "<=")
                    rRV.BoolValue = l1 <= l2;
                else if (rule.TokenString == ">=")
                    rRV.BoolValue = l1 >= l2;
                else if (rule.TokenString == "!=")
                    rRV.BoolValue = l1 != l2;
                else
                {
                    throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
                }
                rRV.RuleType = ParserToken.RuleTypeLogical;
                return rRV;
            }

            if (rule.TokenTypeMemory == ParserToken.TokenTypeNumericOperator3D)
            {
                Point3D l13D = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).Point3DValue;
                Point3D l23D = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).Point3DValue;
                if (rule.TokenString == "+")
                {
                    rRV.Point3DValue.X = l13D.X + l23D.X;
                    rRV.Point3DValue.Y = l13D.Y + l23D.Y;
                    rRV.Point3DValue.Z = l13D.Z + l23D.Z;
                }
                else if (rule.TokenString == "-")
                {
                    rRV.Point3DValue.X = l13D.X - l23D.X;
                    rRV.Point3DValue.Y = l13D.Y - l23D.Y;
                    rRV.Point3DValue.Z = l13D.Z - l23D.Z;
                }
                else
                {
                    throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
                }
                rRV.RuleType = ParserToken.RuleTypeNumeric3D;
                return rRV;
            }

            if (rule.TokenTypeMemory == ParserToken.TokenTypeNumericOperator)
            {
                double l1 = IsSatisfied((ParserToken)rule.Children[0], conclusions, features).DoubleValue;
                double l2 = IsSatisfied((ParserToken)rule.Children[1], conclusions, features).DoubleValue;
                if (rule.TokenString == "+")
                    rRV.DoubleValue = l1 + l2;
                else if (rule.TokenString == "-")
                    rRV.DoubleValue = l1 - l2;
                else if (rule.TokenString == "*")
                    rRV.DoubleValue = l1 * l2;
                else if (rule.TokenString == "/")
                    try
                    {
                        rRV.DoubleValue = l1 / l2;
                        if (double.IsInfinity(rRV.DoubleValue))
                        {
                            if (ReplaceExceptionByZeroFalseAndInfinity)
                            {
                                //OK
                                //rRV.DoubleValue = rRV.DoubleValue;
                            }
                            else throw new ParserException("Line " + rule.Position + ", division by zero", rule.PositionCol, rule.PositionLn);
                        }
                    }
                    catch
                    {
                        throw new ParserException("Line " + rule.Position + ", division by zero", rule.PositionCol, rule.PositionLn);
                    }
                else if (rule.TokenString == "%")
                {
                    rRV.DoubleValue = l1 % l2;
                    if (double.IsNaN(rRV.DoubleValue))
                        if (ReplaceExceptionByZeroFalseAndInfinity) rRV.DoubleValue = 0;
                        else throw new ParserException("Line " + rule.Position + ", modulo by zero", rule.PositionCol, rule.PositionLn);
                }
                else if (rule.TokenString == "^")
                {
                    rRV.DoubleValue = Math.Pow(l1, l2);
                }
                else
                {
                    throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
                }
                rRV.RuleType = ParserToken.RuleTypeNumeric;
                return rRV;
            }
            throw new ParserException("Line " + rule.Position + ", unrecognized rule " + rule.TokenString, rule.PositionCol, rule.PositionLn);
            //return null;
        }

        private bool ContainsConclusion(ArrayList aL, String conclusion)
        {
            for (int a = 0; a < aL.Count; a++)
                if ((String)aL[a] == conclusion)
                    return true;
            return false;
        }
        /*
        public String[] ReturnConclusions(Point3D[] skeleton, int user, double timePeriod)
        {
            bool newConclusion = false;
            ArrayList conclusions = new ArrayList();
            TrackingMemory tm = new TrackingMemory();
            tm.BodyParts = skeleton;//GenerateBodyPartArray(skeleton, user);
            tm.TimePeriod = timePeriod;
            Heap.Insert(0, tm);
            if (Heap.Count >= HeapSize)
                Heap.RemoveAt(HeapSize - 1);

            do
            {
                newConclusion = false;
                for (int a = 0; a < RuleTable.Length; a++)
                {
                    if (!ContainsConclusion(conclusions, RuleTable[a].Conclusion))
                        if (IsSatisfied(RuleTable[a], conclusions).BoolValue)
                            {
                            conclusions.Add(RuleTable[a].Conclusion);
                            newConclusion = true;
                            }
                }
            } while (newConclusion);
            String[] sConclusions = new String[conclusions.Count];
            for (int a = 0; a < sConclusions.Length; a++)
                sConclusions[a] = (String)conclusions[a];
            tm.Conclusions = sConclusions;
            return sConclusions;
        }*/

        public String[] ReturnConclusions(Point3D[] BodyPartsArray, double timePeriod)
        {
            //if (RuleTable == null)
            //    return null;
            bool newConclusion = false;
            ArrayList conclusions = new ArrayList();
            TrackingMemory tm = new TrackingMemory();
            tm.BodyParts = BodyPartsArray;
            tm.TimePeriod = timePeriod;
            tm.Features = new Dictionary<string, object>();
            Heap.Insert(0, tm);
            if (Heap.Count >= HeapSize)
                Heap.RemoveAt(HeapSize - 1);

            //RuleReturnValue[] rRV = null;
            //compute all features
            if (FeatureTable != null)
            {
                //rRV = new RuleReturnValue[FeatureTable.Length];
                for (int a = 0; a < FeatureTable.Length; a++)
                {
                    //rRV[a] = IsSatisfied(FeatureTable[a], conclusions, rRV);
                    RuleReturnValue rrV = IsSatisfied(FeatureTable[a], conclusions, tm.Features);
                    if (rrV.RuleType == ParserToken.RuleTypeNumeric)
                    {
                        tm.Features.Add(FeatureTable[a].Conclusion, rrV.DoubleValue);
                    }
                    if (rrV.RuleType == ParserToken.RuleTypeNumeric3D)
                    {
                        tm.Features.Add(FeatureTable[a].Conclusion, rrV.Point3DValue);
                    }
                    //rRV[a].TextValue = FeatureTable[a].Conclusion;
                }
            }
            if (RuleTable == null)
            {
                return null;
            }
            do
            {
                newConclusion = false;
                for (int a = 0; a < RuleTable.Length; a++)
                {
                    if (!ContainsConclusion(conclusions, RuleTable[a].Conclusion))
                        if (IsSatisfied(RuleTable[a], conclusions, tm.Features).BoolValue)
                        {
                            conclusions.Add(RuleTable[a].Conclusion);
                            newConclusion = true;
                        }
                }
            } while (newConclusion);
            String[] sConclusions = new String[conclusions.Count];
            for (int a = 0; a < sConclusions.Length; a++)
                sConclusions[a] = (String)conclusions[a];
            tm.Conclusions = sConclusions;
            return sConclusions;
        }
    }
}
