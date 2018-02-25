using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.GDL;
using System.Collections;
using org.TKinect;
using System.Globalization;
using Microsoft.Samples.Kinect.BodyBasics;

namespace org.RGDL
{
    class GDLGenerator
    {

        static private void SaveClusteringToFile(int []clutsetId, double[][]rawData, int dim)
        {
            double[][] reducedData = DimensionalityReduction.ReduceDimensionality(rawData, dim);
            //double[][] reducedData = rawData;
            System.IO.StreamWriter sw = new System.IO.StreamWriter("e://test2.txt");
            for (int a = 0; a < reducedData.Length; a++)
            {
                for (int b = 0; b < reducedData[0].Length; b++)
                    sw.Write(reducedData[a][b].ToString(CultureInfo.InvariantCulture) + " ");
                sw.Write(clutsetId[a]);
                sw.WriteLine();
            }
            sw.Close();
        }

        //public static void GenerateRules(ArrayList recording, ParserToken[] features, String oldFileName, String newFileName, int clastersCount, double minimalTimeDistance)
        public static String GenerateRules(ArrayList recording, ParserToken[] features, String oldFileContent, int clastersCount, double minimalTimeDistance, int maxIterationsCount,
            String ruleName, String GDLVersion, double epsilon, int randomSeed, ref int[]keyframes)
        {
            double[][] rawData;
            rawData = new double[recording.Count][];
            GDLInterpreter inter = new GDLInterpreter(features, null);
            String[] foo = null; 
            long[] time = new long[recording.Count];

            for (int a = 0; a < rawData.Length; a++)
            {
                rawData[a] = new double[features.Length];
                TSkeleton ts = ((TSkeleton[])recording[a])[0];
                if (a > 0)
                    //time[a] = time[a - 1] + ts.TimePeriod;
                    time[a] = ts.TimePeriod;
                Point3D[] bodyParts = HendlerHolder.GenerateBodyPartArray(ts, 0);
                foo = inter.ReturnConclusions(bodyParts, 0);
                TrackingMemory tm = (TrackingMemory)inter.Heap[0];

                for (int b = 0; b < features.Length; b++)
                {
                    double v = (double)tm.Features[features[b].Conclusion];
                    rawData[a][b] = v;
                    //RuleReturnValue rrV = IsSatisfied(FeatureTable[a], conclusions, tm.Features);
                    //if (rrV.RuleType == ParserToken.RuleTypeNumeric)
                }
            }
            
            //inter = new GDLInterpreter(AllFeatures, AllRules);
            ClasteringResults cr = KMeansClustering.Clasterize(rawData, clastersCount, maxIterationsCount, randomSeed);
            /*
            double step = 2;
            double[][] rawDataSubset = new double[(int)(rawData.Length/step)][];

            for (int a = 0; (int)step * a < rawData.Length && a < rawDataSubset.Length; a++)
            {
                rawDataSubset[a] = new double[rawData[(int)step * a].Length];
                for (int b = 0; b < rawData[(int)step * a].Length; b++)
                {
                    rawDataSubset[a][b] = rawData[a * (int)step][b];
                }
            }

            ClasteringResults cr = HierarchicalClustering.Clasterize(rawDataSubset, clastersCount, 0);
            SaveClusteringToFile(cr.clustering, rawDataSubset, 3);
            //ClasteringResults cr = HierarchicalClustering.Clasterize(rawData, clastersCount, 0);
            //SaveClusteringToFile(cr.clustering, rawData, 3);
            
            //ClasteringResults cr = DBSCAN.Clasterize(rawData, 64, 32);
            //SaveClusteringToFile(cr.clustering, rawData, 3);
            
            int abc = 0;
            if (abc == 0)
                return null;
            */
            //TimeAnalyser(cr.clustering, time,2);
            String newFeatures = "";
            double festureEPS = epsilon;
            keyframes = new int[clastersCount];
            int []sequence = new int[rawData.Length];
            for (int a = 0; a < rawData.Length; a++)
            {
                sequence[a] = -1;
                int classIndex = -1;
                //po wszytskich klastrach
                for (int c = 0; c < cr.means.Length; c++)
                {
                    bool satisfied = true;
                    for (int b = 0; b < rawData[a].Length; b++)
                    {
                        if (Math.Abs(rawData[a][b] - cr.means[c][b]) > cr.standardDeviations[c][b] + festureEPS)
                            satisfied = false;
                    }
                    if (satisfied)
                    {
                        classIndex = c;
                        keyframes[classIndex] = a;
                    }
                }
                sequence[a] = classIndex;
            }

            //SaveClusteringToFile(sequence, rawData, 3);

            string newGDLFileContent = "";
            //HowLong[] hla = GenerateSequenceFromClusteredData(cr.clustering, time);
            HowLong[] hla = GenerateSequenceFromRawData(sequence, time);

            if (GDLVersion == "1.0")
            {
                String[] featuresArray = new String[features.Length];
                int position = 0;
                for (int a = 0; a < featuresArray.Length; a++)
                {
                    int startF = oldFileContent.IndexOf("FEATURE", position, StringComparison.OrdinalIgnoreCase);
                    int endF = oldFileContent.IndexOf("AS", startF, StringComparison.OrdinalIgnoreCase);
                    int startLength = startF + "FEATURE".Length + 1;
                    featuresArray[a] = oldFileContent.Substring(startLength, endF - startLength);
                    position = endF;
                }
                String rules = "";
                for (int a = 0; a < cr.means.Length; a++)
                {
                    if (a > 0)
                        rules += "\r\n";
                    rules += "RULE ";
                    for (int b = 0; b < cr.means[0].Length; b++)
                    {
                        if (b > 0)
                            rules += "\r\n\t& ";
                        rules += "abs(" + featuresArray[b] + " -" + cr.means[a][b].ToString(CultureInfo.InvariantCulture)
                            + ") <= " + cr.standardDeviations[a][b].ToString(CultureInfo.InvariantCulture)
                            + " + " + festureEPS.ToString(CultureInfo.InvariantCulture);
                    }
                    rules += " THEN " + ruleName + "" + a;
                }
                String sequenceRules = "";
                double[] probability;
                double[][][] seqHelp = NGramAnalyser(hla, time, clastersCount, minimalTimeDistance, out probability);
                for (int grams = 0; grams < seqHelp.Length; grams++)
                {
                    if (grams > 0)
                        sequenceRules += "\r\n";
                    if (seqHelp[grams] != null)
                    {
                        sequenceRules += "//Frequency of appearance: " + probability[grams].ToString(CultureInfo.InvariantCulture) + "\r\n";
                        sequenceRules += "RULE " + ruleName + "" + (int)seqHelp[grams][0][0];
                        if (seqHelp[grams].Length > 1)
                        {
                            sequenceRules += " & sequenceexists(\"";
                            for (int a = 1; a < seqHelp[grams].Length; a++)
                            {
                                sequenceRules += "[" + ruleName + "" + (int)seqHelp[grams][a][0] + "," + (seqHelp[grams][a][1] / 1000).ToString(CultureInfo.InvariantCulture) + "]";
                            }
                        }
                        /*sequenceRules += "RULE sequenceexists(\"";
                        for (int a = 0; a < seqHelp[grams].Length; a++)
                        {
                            sequenceRules += "[" + ruleName + "" + (int)seqHelp[grams][a][0] + "," + (seqHelp[grams][a][1] / 1000).ToString(CultureInfo.InvariantCulture) + "]";
                        }*/
                        sequenceRules += "\") THEN " +  ruleName + "_" + (clastersCount - 1 + grams) + "GRAMS!";
                    }
                }
                String configuration = "";
                configuration += "//Date and time of an analysis: " + DateTime.Now + "\r\n"
                              + "//Done by: Tomasz Hachaj\r\n"
                              + "//Clustering method: K-Means clustering\r\n"
                              + "//Clusters count: " + clastersCount + "\r\n"
                              + "//N-grams range: [" + (clastersCount - 1) + "," + (2 * clastersCount - 1) + "]\r\n"
                              + "//Minimal time distance between rules in sequence: " + minimalTimeDistance.ToString(CultureInfo.InvariantCulture) + " seconds";

                Random random = new Random();
                String sentencjaDnia = SentencjaDnia[random.Next(SentencjaDnia.Length)];
                newGDLFileContent += "//-------------R-GDLv1.0 RULES---------------------------------------\r\n"
                                  + rules;
                newGDLFileContent += "\r\n\r\n";
                newGDLFileContent += "//-------------N-gram based analysis of sequences--------------------\r\n";
                newGDLFileContent += sequenceRules;
                newGDLFileContent += "\r\n\r\n";
                newGDLFileContent += "//-------------R-GDLv1.0 configuration details----------------------\r\n";
                newGDLFileContent += configuration;
                newGDLFileContent += "\r\n\r\n";
                newGDLFileContent += "//Citation of the day: " + sentencjaDnia;
                //String rules = Generate1_0Rules(cr, features);
            }
            if (GDLVersion == "1.1")
            {
                for (int a = 0; a < features.Length; a++)
                {
                    if (a > 0)
                        newFeatures += "\r\n";
                    newFeatures += "FEATURE " + festureEPS.ToString(CultureInfo.InvariantCulture) + " AS " + features[a].Conclusion + "_EPS";
                }
                newFeatures += "\r\n";
                newFeatures += "\r\n";
                for (int a = 0; a < cr.means.Length; a++)
                {
                    if (a > 0)
                        newFeatures += "\r\n";
                    for (int b = 0; b < cr.means[0].Length; b++)
                    {
                        newFeatures += "FEATURE " + cr.means[a][b].ToString(CultureInfo.InvariantCulture) + " AS " + features[b].Conclusion + "_MEAN_" + a + "\r\n";
                        newFeatures += "FEATURE " + cr.standardDeviations[a][b].ToString(CultureInfo.InvariantCulture) + " AS " + features[b].Conclusion + "_DEV_" + a + "\r\n";
                    }
                }
                String rules = "";
                for (int a = 0; a < cr.means.Length; a++)
                {
                    if (a > 0)
                        rules += "\r\n";
                    rules += "RULE ";
                    for (int b = 0; b < cr.means[0].Length; b++)
                    {
                        if (b > 0)
                            rules += "& ";
                        rules += "abs(" + features[b].Conclusion + " -" + features[b].Conclusion + "_MEAN_" + a
                            + ") <= " + features[b].Conclusion + "_DEV_" + a + " + " + features[b].Conclusion + "_EPS ";
                    }
                    rules += "THEN " + ruleName + "" + a;
                }

                String sequenceRules = "";
                double[] probability;
                /*double[][][] seqHelp = NGramAnalyser(hla, time, clastersCount, minimalTimeDistance, out probability);
                for (int grams = 0; grams < seqHelp.Length; grams++)
                {
                    if (grams > 0)
                        sequenceRules += "\r\n";
                    if (seqHelp[grams] != null)
                    {
                        sequenceRules += "//Frequency of appearance: " + probability[grams].ToString(CultureInfo.InvariantCulture) + "\r\n";
                        //odwrócić kolejność wypisywania - od ostatniego do pierwszego
                        sequenceRules += "RULE " + ruleName + "" + (int)seqHelp[grams][0][0];
                        if (seqHelp[grams].Length > 1)
                        {
                            sequenceRules += " & sequenceexists(\"";
                            for (int a = 1; a < seqHelp[grams].Length; a++)
                            {
                                sequenceRules += "[" + ruleName + "" + (int)seqHelp[grams][a][0] + "," + (seqHelp[grams][a][1] / 1000).ToString(CultureInfo.InvariantCulture) + "]";
                            }
                            sequenceRules += "\")";
                        }
                        sequenceRules += " THEN GESTURE_" + (clastersCount - 1 + grams) + "GRAMS!";
                    }
                }*/

                String configuration = "";
                configuration += "//Date and time of an analysis: " + DateTime.Now + "\r\n"
                              + "//Done by: Tomasz Hachaj\r\n"
                              + "//Clustering method: K-Means clustering\r\n"
                              + "//Clusters count: " + clastersCount + "\r\n"
                              + "//N-grams range: [" + (clastersCount - 1) + "," + (2 * clastersCount - 1) + "]\r\n"
                              + "//Minimal time distance between rules in sequence: " + minimalTimeDistance.ToString(CultureInfo.InvariantCulture) + " seconds";

                Random random = new Random();
                String sentencjaDnia = SentencjaDnia[random.Next(SentencjaDnia.Length)];

                //string newGDLFileContent = System.IO.File.ReadAllText(oldFileName);
                newGDLFileContent = oldFileContent;
                newGDLFileContent = "//-------------Original FEATURES-------------------------------------\r\n"
                                  + newGDLFileContent + "\r\n";
                newGDLFileContent += "//-------------R-GDLv1.0 FEATURES------------------------------------\r\n"
                                  + newFeatures + "\r\n";
                newGDLFileContent += "//-------------R-GDLv1.0 RULES---------------------------------------\r\n"
                                  + rules;
                /*
                newGDLFileContent += "\r\n\r\n";
                newGDLFileContent += "//-------------N-gram based analysis of sequences--------------------\r\n";
                newGDLFileContent += sequenceRules;
                newGDLFileContent += "\r\n\r\n";
                newGDLFileContent += "//-------------R-GDLv1.0 configuration details----------------------\r\n";
                newGDLFileContent += configuration;
                newGDLFileContent += "\r\n\r\n";
                newGDLFileContent += "//Citation of the day: " + sentencjaDnia;*/
            }
            if (GDLVersion == "1.2")
            {
                for (int a = 0; a < features.Length; a++)
                {
                    if (a > 0)
                        newFeatures += "\r\n";
                    newFeatures += "FEATURE " + festureEPS.ToString(CultureInfo.InvariantCulture) + " AS " + features[a].Conclusion + "_EPS";
                }
                newFeatures += "\r\n";
                newFeatures += "\r\n";
                for (int a = 0; a < cr.means.Length; a++)
                {
                    if (a > 0)
                        newFeatures += "\r\n";
                    for (int b = 0; b < cr.means[0].Length; b++)
                    {
                        newFeatures += "FEATURE " + cr.means[a][b].ToString(CultureInfo.InvariantCulture) + " AS " + features[b].Conclusion + "_MEAN_" + a + "\r\n";
                        newFeatures += "FEATURE " + cr.standardDeviations[a][b].ToString(CultureInfo.InvariantCulture) + " AS " + features[b].Conclusion + "_DEV_" + a + "\r\n";
                    }
                }
                //newFeatures += "\r\n";
                String clusterBordersFeatures = "";
                for (int a = 0; a < cr.means.Length; a++)
                {
                    for (int b = 0; b < cr.means[0].Length; b++)
                    {
                        clusterBordersFeatures += "FEATURE " + "abs(" + features[b].Conclusion + " -" + features[b].Conclusion + "_MEAN_" + a
                                + ") / (" + features[b].Conclusion + "_DEV_" + a + " + " + features[b].Conclusion + "_EPS ) AS " + features[b].Conclusion + "_RULE_" + a + "_FEATURE_" + b;
                        clusterBordersFeatures += "\r\n";
                    }
                }

                //newFeatures += "\r\n";
                String ClustersScoresFeatures = "";
                for (int a = 0; a < cr.means.Length; a++)
                {
                    ClustersScoresFeatures += "FEATURE (";
                    for (int b = 0; b < cr.means[0].Length; b++)
                    {
                        if (b > 0)
                            ClustersScoresFeatures += " + ";
                        ClustersScoresFeatures += features[b].Conclusion + "_RULE_" + a + "_FEATURE_" + b;
                    }
                    ClustersScoresFeatures += " ) / " + cr.means[0].Length + " AS " + ruleName + a + "_SCORE\r\n";
                }
                /*
                ClustersScoresFeatures += "FEATURE (";
                for (int a = 0; a < cr.means.Length; a++)
                {
                    if (a > 0)
                        ClustersScoresFeatures += "+ ";
                    ClustersScoresFeatures += ruleName + a + "_SCORE ";
                }
                ClustersScoresFeatures += ") / " + cr.means.Length + " AS " + ruleName + "_OVERALL_SCORE\r\n";
                */
                String rules = "";
                for (int a = 0; a < cr.means.Length; a++)
                {
                    if (a > 0)
                        rules += "\r\n";
                    rules += "RULE ";
                    for (int b = 0; b < cr.means[0].Length; b++)
                    {
                        if (b > 0)
                            rules += " & ";
                        rules += features[b].Conclusion + "_RULE_" + a + "_FEATURE_" + b + " <= 1";
                    }
                    rules += " THEN " + ruleName + "" + a;
                }


                String sequenceRules = "";
                /*double[] probability;
                double[][][] seqHelp = NGramAnalyser(hla, time, clastersCount, minimalTimeDistance, out probability);
                for (int grams = 0; grams < seqHelp.Length; grams++)
                {
                    if (grams > 0)
                        sequenceRules += "\r\n";
                    if (seqHelp[grams] != null)
                    {
                        sequenceRules += "//Frequency of appearance: " + probability[grams].ToString(CultureInfo.InvariantCulture) + "\r\n";
                        //odwrócić kolejność wypisywania - od ostatniego do pierwszego
                        sequenceRules += "RULE " + ruleName + "" + (int)seqHelp[grams][0][0];
                        if (seqHelp[grams].Length > 1)
                        {
                            sequenceRules += " & sequenceexists(\"";
                            for (int a = 1; a < seqHelp[grams].Length; a++)
                            {
                                sequenceRules += "[" + ruleName + "" + (int)seqHelp[grams][a][0] + "," + (seqHelp[grams][a][1] / 1000).ToString(CultureInfo.InvariantCulture) + "]";
                            }
                            sequenceRules += "\")";
                        }

                        sequenceRules += " THEN GESTURE_" + (clastersCount - 1 + grams) + "GRAMS!";
                    }
                }*/

                String configuration = "";
                configuration += "//Date and time of an analysis: " + DateTime.Now + "\r\n"
                              + "//Done by: Tomasz Hachaj\r\n"
                              + "//Clustering method: K-Means clustering\r\n"
                              + "//Clusters count: " + clastersCount + "\r\n"
                              + "//N-grams range: [" + (clastersCount - 1) + "," + (2 * clastersCount - 1) + "]\r\n"
                              + "//Minimal time distance between rules in sequence: " + minimalTimeDistance.ToString(CultureInfo.InvariantCulture) + " seconds";

                Random random = new Random();
                String sentencjaDnia = SentencjaDnia[random.Next(SentencjaDnia.Length)];

                newGDLFileContent = oldFileContent;
                newGDLFileContent = "//-------------ORIGINAL FEATURES--------------------------------------\r\n"
                                  + newGDLFileContent + "\r\n";
                newGDLFileContent += "//-------------R-GDLv1.0 FEATURES------------------------------------\r\n";
                newGDLFileContent += "//-------------CLUSTER ANALYSIS--------------------------------------\r\n"
                                  + newFeatures + "\r\n";
                newGDLFileContent += "//-------------CLUSTER BORDERS---------------------------------------\r\n";
                newGDLFileContent += clusterBordersFeatures + "\r\n";
                newGDLFileContent += "//-------------FEATURE SCORES----------------------------------------\r\n";
                newGDLFileContent += ClustersScoresFeatures + "\r\n";

                newGDLFileContent += "//-------------R-GDLv1.0 RULES---------------------------------------\r\n"
                                  + rules;
                /*
                newGDLFileContent += "\r\n\r\n";
                newGDLFileContent += "//-------------N-gram based analysis of sequences--------------------\r\n";
                newGDLFileContent += sequenceRules;
                newGDLFileContent += "\r\n\r\n";
                newGDLFileContent += "//-------------R-GDLv1.0 configuration details----------------------\r\n";
                newGDLFileContent += configuration;
                newGDLFileContent += "\r\n\r\n";
                newGDLFileContent += "//Citation of the day: " + sentencjaDnia;*/

            }
            return newGDLFileContent;
            //System.IO.File.WriteAllText(newFileName, newGDLFileContent);
        }

        private static HowLong[] GenerateSequenceFromRawData(int[] clustering, long[] time)
        {
            ArrayList seqence = new ArrayList();
            HowLong hl = null;
            long firstTime = 0;
            for (int a = 0; a < time.Length; a++)
            {
                if (hl == null)
                {
                    if (clustering[a] != -1)
                    {
                        hl = new HowLong();
                        hl.claster = clustering[a];
                        //hl.time = time[a];
                        hl.time = firstTime;
                        seqence.Add(hl);
                    }
                    else
                        firstTime += time[a];
                }
                else
                {
                    if (hl.claster != clustering[a])
                    {
                        if (clustering[a] != -1)
                        {
                            hl = new HowLong();
                            hl.claster = clustering[a];
                            seqence.Add(hl);
                        }
                    }
                    hl.time += time[a];
                    /*if (hl.claster != clustering[a])
                    {
                        if (clustering[a] != -1)
                        {
                            hl = new HowLong();
                            hl.claster = clustering[a];
                            seqence.Add(hl);
                        }
                        hl.time += time[a];
                    }
                    else
                    {
                        hl.time += time[a];
                    }*/
                }
            }
            HowLong[] hla = new HowLong[seqence.Count];
            for (int a = 0; a < hla.Length; a++)
            {
                hla[a] = (HowLong)seqence[a];
            }
            return hla;
        }

        private static HowLong[] GenerateSequenceFromClusteredData(int[] clustering, long[] time)
        {
            ArrayList seqence = new ArrayList();
            HowLong hl = null;
            for (int a = 0; a < time.Length; a++)
            {
                if (hl == null)
                {
                    hl = new HowLong();
                    hl.claster = clustering[a];
                    //hl.time = time[a];
                    seqence.Add(hl);
                }
                if (hl.claster != clustering[a])
                {
                    hl = new HowLong();
                    hl.claster = clustering[a];
                    seqence.Add(hl);
                    hl.time += time[a];
                }
                else
                {
                    hl.time += time[a];
                }
            }
            HowLong[] hla = new HowLong[seqence.Count];
            for (int a = 0; a < hla.Length; a++)
            {
                hla[a] = (HowLong)seqence[a];
            }
            return hla;
        }

        private static String Generate1_0Rules(ClasteringResults cr, ParserToken[] features)
        {
            String rules = "";
            String[] featuresStrings = new String[features.Length];
            for (int a = 0; a < featuresStrings.Length; a++)
            {
                featuresStrings[a] = "";
                featuresStrings[a] = ReturnRecursiveFeatureValue(features[a]);
            }
            return rules;
        }

        private static String ReturnRecursiveFeatureValue(ParserToken features)
        {
            String returnString = "";
            /*
            //returnString += features.TokenString;
            if (features.Children != null)
            {
                for (int a = 0; a < features.Children.Count; a++)
                {
                    returnString += ReturnRecursiveFeatureValue((ParserToken)features.Children[a]);
                    //dodajmey przecinek, ale nie po ostatnim argumencie
                    if (features.TokenTypeMemory == ParserToken.TokenTypeNumericFunction3D && a < features.Children.Count - 1)
                        returnString += ",";
                    if (features.TokenTypeMemory == ParserToken.TokenTypeNumericOperator)
                        returnString += features.TokenString;
                }
                if (features.TokenTypeMemory == ParserToken.TokenTypeBodyPart3D)
                    returnString += "]";
                

            }
            else returnString += features.TokenString;
            */
            return returnString;
        }

        class HowLong
        {
            public int claster = 0;
            public long time = 0;
        }

        //private static double[][][] NGramAnalyser(int[] clustering, long[] time, int clustersCount, double minimalTimeDinstance, out double[] probability)
        private static double[][][] NGramAnalyser(HowLong[] hla, long[] time, int clustersCount, double minimalTimeDinstance, out double[] probability)
        {
            minimalTimeDinstance *= 1000;
            //HowLong[] hla = GenerateSequenceFromClusteredData(clustering, time);

            //generate n-grams
            double[][][] sequences = new double[clustersCount + 1][][];
            probability = new double[clustersCount + 1];
            int index = 0;
            
            for (int nGramsLength = clustersCount - 1; nGramsLength < 2 * clustersCount; nGramsLength++)
            {
                Dictionary<string, double[]> nGramHelp = GenerateNGrams(hla, time, nGramsLength);
                if (nGramHelp != null)
                {
                    sequences[index] = MostProbableNGram(nGramHelp, out probability[index]);
                    for (int a = 0; a < sequences[index].Length; a++)
                    {
                        double timeSpanHelp = 0;
                        while (sequences[index][a][1] > timeSpanHelp)
                            timeSpanHelp += minimalTimeDinstance;
                        sequences[index][a][1] = timeSpanHelp;
                    }
                }
                else
                    sequences[index] = null;
                index++;
            }
            return sequences;
        }

        private static double[][] MostProbableNGram(Dictionary<string, double[]>nGrams, out double probability)
        {
            double[] most = null;
            String mostSequence = "";
            foreach (KeyValuePair<string, double[]> kvp in nGrams)
            {
                double[] d = kvp.Value;
                String k = kvp.Key;
                if (most == null)
                {
                    most = d;
                    mostSequence = kvp.Key;
                }
                if (most[0] < d[0])
                {
                    most = d;
                    mostSequence = kvp.Key;
                }
            }
            String[] seqHelp = mostSequence.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            double[][] seqInt = new double[seqHelp.Length][];
            for (int a = 0; a < seqInt.Length; a++)
            {
                seqInt[a] = new double[2];
                //id klasy
                seqInt[a][0] = double.Parse(seqHelp[a]);
                //max. czas trwania klasy
                seqInt[a][1] = most[a + 1];
            }
            if (most != null)
                probability = most[0];
            else
                probability = 0;
            return seqInt;
        }

        private static Dictionary<string, double[]> GenerateNGrams(HowLong[] hla, long[] time, int n)
        {
            if (n == 0)
                return null;
            String nGramName = "";
            int totatlCount = 0;
            //name of n-gram, count of elements
            Dictionary<string, double[]> nGramsCount =
            new Dictionary<string, double[]>();

            //TU SPRAWADZIć!! TUTUTUTU
            //dlaczego < length, a nie <=
            for (int a = 0; a <= hla.Length - n; a++)
            {
                nGramName = "";
                for (int b = 0; b < n; b++)
                {
                    nGramName += hla[a + b].claster + " ";
                }
                if (!nGramsCount.ContainsKey(nGramName))
                {
                    //0 - ilość wystąpień
                    //kolejne indeksy - czas trwania poszczególnych
                    nGramsCount[nGramName] = new double[n + 1];
                    for (int b = 0; b < n + 1; b++)
                    {
                        nGramsCount[nGramName][b] = 0;
                    }
                }
                nGramsCount[nGramName][0]++;
                for (int b = 0; b < n; b++)
                {
                    if (nGramsCount[nGramName][b + 1] < hla[a + b].time)
                        nGramsCount[nGramName][b + 1] = hla[a + b].time;
                }
                totatlCount++;
            }
            Dictionary<string, double[]>.KeyCollection keyColl = nGramsCount.Keys;
            if (n == 1)
            {
                nGramsCount["0"] = new double[n + 1];
                nGramsCount["0"][0] = hla.Length;
                nGramsCount["0"][1] = hla[0].time;
                totatlCount = 1;
            }
            // The elements of the KeyCollection are strongly typed 
            // with the type that was specified for dictionary keys.
            String[] allKeys = new String[keyColl.Count];
            int indexHelp = 0;
            foreach (string s in keyColl)
            {
                allKeys[indexHelp] = s;
                indexHelp++;
            }
            for (int a = 0; a < allKeys.Length; a++)
            {
                nGramsCount[allKeys[a]][0] = nGramsCount[allKeys[a]][0] / totatlCount;
            }
            return nGramsCount;
        }


        //KSIĘGA CZWARTA - DYPLOMATYKA I ŁOWY
        private static String[] SentencjaDnia = {"Rówienniki litewskich wielkich kniaziów, drzewa",
                                    "Białowieży, Świtezi, Ponar, Kuszelewa!",
                                    "Których cień spadał niegdyś na koronne głowy",
                                    "Groźnego Witenesa, wielkiego Mindowy",
                                    "I Giedymina, kiedy na Ponarskiej górze,",
                                    "Przy ognisku myśliwskiem, na niedźwiedziej skórze,",
                                    "Leżał, słuchając pieśni mądrego Lizdejki,",
                                    "A Wiliji widokiem i szumem Wilejki,",
                                    "Ukołysany, marzył o wilku żelaznym;",
                                    "I zbudzony, za bogów rozkazem wyraźnym",
                                    "Zbudował miasto Wilno, które w lasach siedzi",
                                    "Jak wilk pośrodku żubrów, dzików i niedźwiedzi.",
                                    "Z tego to miasta Wilna, jak z rzymskiej wilczycy,",
                                    "Wyszedł Kiejstut i Olgierd, i Olgierdowicy,",
                                    "Równie myśliwi wielcy jak sławni rycerze,",
                                    "Czyli wroga ścigali, czyli dzikie źwierze.",
                                    "Sen myśliwski nam odkrył tajnie przyszłych czasów,",
                                    "Że Litwie trzeba zawsze żelaza i lasów.",
                                    "Knieje! do was ostatni przyjeżdżał na łowy,",
                                    "Ostatni król, co nosił kołpak Witoldowy,",
                                    "Ostatni z Jagiellonów wojownik szczęśliwy",
                                    "I ostatni na Litwie monarcha myśliwy.",
                                    "Drzewa moje ojczyste! jeśli Niebo zdarzy,",
                                    "Bym wrócił was oglądać, przyjaciele starzy,",
                                    "Czyli was znajdę jeszcze? czy dotąd żyjecie?",
                                    "Wy, koło których niegdyś pełzałem jak dziecię...",
                                    "Czy żyje wielki Baublis, w którego ogromie",
                                    "Wiekami wydrążonym, jakby w dobrym domie,",
                                    "Dwunastu ludzi mogło wieczerzać za stołem?",
                                    "Czy kwitnie gaj Mendoga pod farnym kościołem?",
                                    "I tam na Ukrainie, czy się dotąd wznosi",
                                    "Przed Hołowińskich domem, nad brzegami Rosi,",
                                    "Lipa tak rozrośniona, że pod jej cieniami",
                                    "Stu młodzieńców, sto panien szło w taniec parami?",
                                    "Pomniki nasze! ileż co rok was pożera",
                                    "Kupiecka lub rządowa, moskiewska siekiera!",
                                    "Nie zostawia przytułku ni leśnym śpiewakom,",
                                    "Ni wieszczom, którym cień wasz tak miły jak ptakom.",
                                    "Wszak lipa czarnolaska, na głos Jana czuła,",
                                    "Tyle rymów natchnęła! Wszak ów dąb gaduła",
                                    "Kozackiemu wieszczowi tyle cudów śpiewa!",
                                    "Ja ileż wam winienem, o domowe drzewa!",
                                    "Błahy strzelec, uchodząc szyderstw towarzyszy",
                                    "Za chybioną źwierzynę, ileż w waszej ciszy",
                                    "Upolowałem dumań, gdy w dzikim ostępie",
                                    "Zapomniawszy o łowach usiadłem na kępie,",
                                    "A koło mnie srebrzył się tu mech siwobrody,",
                                    "Zlany granatem czarnej zgniecionej jagody,",
                                    "A tam się czerwieniły wrzosiste pagórki,",
                                    "Strojne w brusznice jakby w koralów paciorki.",
                                    "Wokoło była ciemność; gałęzie u góry",
                                    "Wisiały jak zielone, gęste, niskie chmury;",
                                    "Wicher kędyś nad sklepem szalał nieruchomym,",
                                    "Jękiem, szumami, wyciem, łoskotami, gromem:",
                                    "Dziwny, odurzający hałas! Mnie się zdało,",
                                    "Że tam nad głową morze wiszące szalało.",
                                    "Na dole jak ruiny miast: tu wywrot dębu",
                                    "Wysterka z ziemi na kształt ogromnego zrębu;",
                                    "Na nim oparte, jak ścian i kolumn obłamy:",
                                    "Tam gałęziste kłody, tu wpół zgniłe tramy,",
                                    "Ogrodzone parkanem traw. W środek tarasu",
                                    "Zajrzeć straszno, tam siedzą gospodarze lasu:",
                                    "Dziki, niedźwiedzie, wilki; u wrót leżą kości",
                                    "Na pół zgryzione jakichś nieostrożnych gości.",
                                    "Czasem wymkną się w górę przez trawy zielenie,",
                                    "Jakby dwa wodotryski, dwa rogi jelenie",
                                    "I mignie między drzewa źwierz żółtawym pasem,",
                                    "Jak promień, kiedy wpadłszy gaśnie między lasem.",
                                    "I znowu cichość w dole. Dzięcioł na jedlinie",
                                    "Stuka z lekka i dalej odlatuje, ginie,",
                                    "Schował się, ale dziobem nie przestaje pukać,",
                                    "Jak dziecko, gdy schowane woła, by go szukać.",
                                    "Bliżej siedzi wiewiórka, orzech w łapkach trzyma,",
                                    "Gryzie go; zawiesiła kitkę nad oczyma,",
                                    "Jak pióro nad szyszakiem u kirasyjera;",
                                    "Chociaż tak osłoniona, dokoła spoziera;",
                                    "Dostrzegłszy gościa, skacze gajów tanecznica",
                                    "Z drzew na drzewa, miga się jako błyskawica;",
                                    "Na koniec w niewidzialny otwór pnia przepada,",
                                    "Jak wracająca w drzewo rodzime dryjada.",
                                    "Znowu cicho.",
                                    "Wtem gałąź wstrząsła się trącona",
                                    "I pomiędzy jarzębin rozsunione grona",
                                    "Kraśniejsze od jarzębin zajaśniały lica:",
                                    "To jagód lub orzechów zbieraczka, dziewica;",
                                    "W krobeczce z prostej kory podaje zebrane",
                                    "Bruśnice świeże jako jej usta rumiane;",
                                    "Obok młodzieniec idzie, leszczynę nagina,",
                                    "Chwyta w lot migające orzechy dziewczyna.",
                                    "Wtem usłyszeli odgłos rogów i psów granie:",
                                    "Zgadują, że się ku nim zbliża polowanie,",
                                    "I pomiędzy gałęzi gęstwę, pełni trwogi,",
                                    "Zniknęli nagle z oczu jako leśne bogi.",
                                    "W Soplicowie ruch wielki; lecz ni psów hałasy,",
                                    "Ani rżące rumaki, skrzypiące kolasy,",
                                    "Ni odgłos trąb dających hasło polowania",
                                    "Nie mogły Tadeusza wyciągnąć z posłania;",
                                    "Ubrany padłszy w łóżko, spał jak bobak w norze.",
                                    "Nikt z młodzieży nie myślił szukać go po dworze;",
                                    "Każdy sobą zajęty śpieszył, gdzie kazano;",
                                    "O towarzyszu sennym całkiem zapomniano."};
    }
}
