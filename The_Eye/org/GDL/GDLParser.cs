using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;

namespace org.GDL
{
    public class GDLParser
    {
        private static String RemoveAllWhitespaceChar(String text)
        {
            text = text.Replace(" ", "");
            text = text.Replace("\r\n", "");
            text = text.Replace("\n", "");
            text = text.Replace("\t", "");
            return text;
        }

        public static String[] GenerateSkeletonJoints()
        {
            /*
            String[] sjHelp = {"HipCenter", "Spine", "ShoulderCenter","Head","ShoulderLeft","ElbowLeft","WristLeft","HandLeft","ShoulderRight",
                                "ElbowRight","WristRight","HandRight","HipLeft","KneeLeft","AnkleLeft","FootLeft","HipRight","KneeRight","AnkleRight",
                                "FootRight"};
            */
            String[] sjHelp = {"SpineBase","SpineMid", "Neck", "Head", "ShoulderLeft", "ElbowLeft", "WristLeft", "HandLeft",
                                   "ShoulderRight", "ElbowRight", "WristRight",
        "HandRight", "HipLeft", "KneeLeft", "AnkleLeft","FootLeft","HipRight","KneeRight","AnkleRight","FootRight",
        "SpineShoulder", "HandTipLeft", "ThumbLeft", "HandTipRight", "ThumbRight"};

            String[] returnStringArray = new String[sjHelp.Length * 4];
            for (int a = 0; a < sjHelp.Length; a++)
            {
                returnStringArray[4 * a] = sjHelp[a].ToLower() + ".x[";
                returnStringArray[(4 * a) + 1] = sjHelp[a].ToLower() + ".y[";
                returnStringArray[(4 * a) + 2] = sjHelp[a].ToLower() + ".z[";
                returnStringArray[(4 * a) + 3] = sjHelp[a].ToLower() + ".a[";
            }
            return returnStringArray;
        }

        public static String[] GenerateSkeletonJoints3D()
        {
            /*String[] sjHelp = {"HipCenter", "Spine", "ShoulderCenter","Head","ShoulderLeft","ElbowLeft","WristLeft","HandLeft","ShoulderRight",
                                "ElbowRight","WristRight","HandRight","HipLeft","KneeLeft","AnkleLeft","FootLeft","HipRight","KneeRight","AnkleRight",
                                "FootRight"};*/
            String[] sjHelp = {"SpineBase","SpineMid", "Neck", "Head", "ShoulderLeft", "ElbowLeft", "WristLeft", "HandLeft",
                                   "ShoulderRight", "ElbowRight", "WristRight",
        "HandRight", "HipLeft", "KneeLeft", "AnkleLeft","FootLeft","HipRight","KneeRight","AnkleRight","FootRight",
        "SpineShoulder", "HandTipLeft", "ThumbLeft", "HandTipRight", "ThumbRight"};

            String[] returnStringArray = new String[sjHelp.Length];
            for (int a = 0; a < sjHelp.Length; a++)
            {
                returnStringArray[a] = sjHelp[a].ToLower() + ".xyz[";
            }
            return returnStringArray;
        }

        /*public static String[] GenerateConclusionsFeaturesNames()
        {
            String[] array = {"q","w","e","r","t","y","u","i","o","p","a","s","d","f","g","h","j","k","l","z","x","c","v","b","n","m","!","-"};
            return array;
        }*/


        private static String FindRule(ref String text)
        {
            int index1 = text.IndexOf("rule");
            int index2;
            String ruleText = null;
            if (index1 >= 0)
            {
                index2 = text.IndexOf("rule ", index1 + 1);
                if (index2 < 0)
                    index2 = text.Length;
                ruleText = text.Substring(index1, index2 - index1);
                text = text.Remove(index1, index2 - index1);
            }
            return ruleText;
        }

        private static int OpenBracketPosition(String text, int tokenPosition)
        {
            return text.IndexOf("(", tokenPosition);
        }

        private static int CloseBracketPosition(String text, int tokenPosition)
        {
            return text.IndexOf(")", tokenPosition);
        }

        private static int CloseSquareBracketPosition(String text, int tokenPosition)
        {
            return text.IndexOf("]", tokenPosition);
        }

        private static int OpenSquareBracketPosition(String text, int tokenPosition)
        {
            return text.IndexOf("[", tokenPosition);
        }

        private static int CommaPosition(String text, int tokenPosition)
        {
            return text.IndexOf(",", tokenPosition);
        }

        private static int QuotationMarkPosition(String text, int tokenPosition)
        {
            return text.IndexOf("\"", tokenPosition);
        }

        private static int FindOperatorPosition(String text, String[] operatorTable, int tokenPosition)
        {
            int index1 = -1;
            int index2 = -1;
            for (int b = 0; b < operatorTable.Length; b++)
            {
                index2 = text.IndexOf(operatorTable[b], tokenPosition);
                if (index2 >= 0)
                {
                    if (index1 >= 0 && index2 < index1)
                    {
                        index1 = index2;
                    }
                    if (index1 < 0)
                        index1 = index2;
                }
            }
            return index1;
        }

        private static bool isValidAndOr(String text, int position, int operatorLength)
        {

            if (position <= 0) return false;
            //<0 and >z
            if ((text[position - 1] < 48 || text[position - 1] > 122)
                && (text[position + operatorLength] < 48 || text[position + operatorLength] > 122)) return true;
            return false;
        }

        private static int FindOperatorPositionLogical(String text, String[] operatorTable, int tokenPosition)
        {
            int index1 = -1;
            int index2 = -1;

            int helpPosition = -1;
            for (int b = 0; b < operatorTable.Length; b++)
            {
                index2 = text.IndexOf(operatorTable[b], tokenPosition);
                if (index2 >= 0)
                {
                    if (index1 >= 0 && index2 < index1)
                    {
                        index1 = index2;
                        helpPosition = b;
                    }
                    if (index1 < 0)
                    {
                        index1 = index2;
                        helpPosition = b;
                    }
                }
            }
            if (helpPosition >= 0)
                if (!isValidAndOr(text,index1,operatorTable[helpPosition].Length)) return -1;
            return index1;
        }
        

        private static int FindOperatorType(String text, String[] operatorTable, int tokenPosition, int index)
        {
            int index1 = -1;
            int index2 = -1;
            int type = -1;
            int b = -1;
            for (b = 0; b < operatorTable.Length; b++)
            {
                index2 = text.IndexOf(operatorTable[b], index);
                if (index2 >= 0)
                {
                    if (index1 >= 0 && index2 <= index1)
                    {
                        index1 = index2;
                        type = b;
                    }
                    if (index1 < 0)
                    {
                        index1 = index2;
                        type = b;
                    }
                }
            }
            return type;
        }

        //poniewa¿ przed funkcj¹ musi byæ inny operator (albo to pierwszy symbol w regule), który zostanie wczeœniej sparsowany
        //to index1 musi = 0
        private static int FindFunctionPosition(String text, String[] operatorTable, int tokenPosition)
        {
            int index1 = -1;
            int index2 = -1;
            for (int b = 0; b < operatorTable.Length; b++)
            {
                //index2 = text.IndexOf(operatorTable[b] + "(", tokenPosition);
                index2 = text.IndexOf(operatorTable[b], tokenPosition);
                if (index2 >= 0)
                {
                    if (index1 >= 0 && index2 < index1)
                    {
                        index1 = index2;
                    }
                    if (index1 < 0)
                        index1 = index2;
                }
            }
            if (index1 - tokenPosition > 0)
                index1 = -1;
            return index1;
        }

        private static int findMinimal(int p1, int p2, int p3, int p4, int p5, int p6, 
            int p7, int p8, int p9, int p10, int p11, int p12, int p13, int p14, int p15, int p16)
        {
            if (p1 < 0) p1 = int.MaxValue;
            if (p2 < 0) p2 = int.MaxValue;
            if (p3 < 0) p3 = int.MaxValue;
            if (p4 < 0) p4 = int.MaxValue;
            if (p5 < 0) p5 = int.MaxValue;
            if (p6 < 0) p6 = int.MaxValue;
            if (p7 < 0) p7 = int.MaxValue;
            if (p8 < 0) p8 = int.MaxValue;
            if (p9 < 0) p9 = int.MaxValue;
            if (p10 < 0) p10 = int.MaxValue;
            if (p11 < 0) p11 = int.MaxValue;
            if (p12 < 0) p12 = int.MaxValue;
            if (p13 < 0) p13 = int.MaxValue;
            if (p14 < 0) p14 = int.MaxValue;
            if (p15 < 0) p15 = int.MaxValue;
            if (p16 < 0) p16 = int.MaxValue;
            int returnValue = Math.Min(p1, p2);
            returnValue = Math.Min(returnValue, p3);
            returnValue = Math.Min(returnValue, p4);
            returnValue = Math.Min(returnValue, p5);
            returnValue = Math.Min(returnValue, p6);
            returnValue = Math.Min(returnValue, p7);
            returnValue = Math.Min(returnValue, p8);
            returnValue = Math.Min(returnValue, p9);
            returnValue = Math.Min(returnValue, p10);
            returnValue = Math.Min(returnValue, p11);
            returnValue = Math.Min(returnValue, p12);
            returnValue = Math.Min(returnValue, p13);
            returnValue = Math.Min(returnValue, p14);
            returnValue = Math.Min(returnValue, p15);
            returnValue = Math.Min(returnValue, p16);
            if (returnValue == int.MaxValue)
                returnValue = -1;
            return returnValue;
        }
        /*
        private static ArrayList ParseSequence(String text, int tokenPosition, int col, int ln)
        {
            ArrayList al = new ArrayList();
            int actualPosition = text.IndexOf("\"");
            int helpIndex = -1;
            int bracketStart = -1;
            int bracketStop = -1;
            SequenceToken st = null;
            String doubleHelp = null;
            char[] commaSeparators = { ',' };
            String textPart = null;
            //ArrayList ConclusionsHelp = new ArrayList();
            //ArrayList PrefixesHelp = new ArrayList();
            do
            {
                bracketStart = text.IndexOf("[", actualPosition);
                bracketStop = text.IndexOf("]", actualPosition);
                if (bracketStart >= 0)
                {
                    if (bracketStop < 0)
                        throw new ParserException("Line: " + tokenPosition + ", sequence expression lack of terminating ]", col, ln);
                    textPart = text.Substring(bracketStart + 1, bracketStop - 1 - bracketStart);
                    actualPosition = bracketStart + 1;
                    st = new SequenceToken();
                    String[] splittingResult = textPart.Split(commaSeparators);
                    if (splittingResult.Length < 0)
                        throw new ParserException("Line: " + tokenPosition + ", sequence expression lack of ,", col, ln);

                    st.Prefixes = new String[splittingResult.Length - 1];
                    st.Conclusions = new String[splittingResult.Length - 1];

                    for (int a = 0; a < splittingResult.Length - 1; a++)
                    {
                        if (splittingResult[a][0] == '!')
                        {
                            st.Prefixes[a] = "!";
                            st.Conclusions[a] = splittingResult[a].Substring(1);
                        }
                        else
                        {
                            st.Prefixes[a] = "";
                            st.Conclusions[a] = splittingResult[a];
                        }
                        
                    }
                    doubleHelp = splittingResult[splittingResult.Length - 1].Substring(0);
                    try
                    {
                        st.TimeConstraintSeconds = Double.Parse(doubleHelp,CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new ParserException("Line: " + tokenPosition + ", sequence expression lack of time constraint value: " + doubleHelp, col, ln);
                    }
                    al.Add(st);
                    actualPosition = bracketStop + 1;
                }
            }
            while (bracketStart >= 0);
            if (al.Count == 0)
                throw new ParserException("Line: " + tokenPosition + ", sequence expression lack of proper sequences: " + text, col, ln);
            helpIndex = text.IndexOf("\"", actualPosition);
            if (helpIndex == -1)
                throw new ParserException("Line: " + tokenPosition + ", sequence expression lack of terminating \"", col, ln);
            return al;
            
        }*/

        private static ParserToken GenerateBasicToken(String text, int tokenPosition, int index)
        {
            ParserToken pt2 = new ParserToken();
            pt2.Position = tokenPosition;
            //String subString = text.Substring(tokenPosition, (index - 1) - tokenPosition);
            String subString = text.Substring(tokenPosition, index - tokenPosition);
            subString = subString.Trim();
            pt2.TokenString = subString;
            /*bool parseFound = false;
            try
            {
                double dd = double.Parse(subString,CultureInfo.InvariantCulture);
                //pt2.tokenString = dd.ToString();
                pt2.TokenType = ParserToken.TokenTypeNumeric;
                parseFound = true;
            }
            catch (Exception e) { }*/
            /*if (parseFound == false)
            {
                if (subString[0] == '"')
                {
                    //pt2.tokenString = subString;
                    pt2.TokenType = ParserToken.TokenTypeSequence;
                    pt2.Sequence = ParseSequence(text, tokenPosition);
                    parseFound = true;
                }
            }*/
            /*if (parseFound == false)
            {
                for (int a = 0; a < ParserToken.BodyParts.Length; a++)
                    if (subString == ParserToken.BodyParts[a])
                    {
                        //pt2.tokenType = subString;
                        pt2.TokenType = ParserToken.TokenTypeBodyPart;
                        parseFound = true;
                    }
            }*/
            //if (parseFound == false)
            //{
                pt2.TokenType = ParserToken.TokenTypeConclusion;
                //parseFound = true;
            //}
            return pt2;
        }

        private static int FindDigitPosition(String text, int tokenPosition)
        {
            if (text[tokenPosition] == '.' || (text[tokenPosition] >= '0' && text[tokenPosition] <= '9'))
                return tokenPosition;
            return -1;
        }

        private static String FindNumericValue(String text, int tokenPosition)
        {
            int lenghtHelp = 0;
            while (FindDigitPosition(text, tokenPosition + lenghtHelp) >= 0)
                lenghtHelp++;
            String parseString = text.Substring(tokenPosition, lenghtHelp);
            return parseString;
        }
        /*
        private static ArrayList GenerateTokens(String text)
        {
            text = text.ToLower();
            ArrayList tokenArray = new ArrayList();
            int tokenPosition = 0;


            bool whitespaceSkipLoopEnd = false;
            while (!whitespaceSkipLoopEnd)
            {
                if (tokenPosition < text.Length)
                    if (text[tokenPosition] == ' ' || text[tokenPosition] == '\t' || text[tokenPosition] == '\r' || text[tokenPosition] == '\n')
                        tokenPosition++;
                    else
                        whitespaceSkipLoopEnd = true;
                else
                    whitespaceSkipLoopEnd = true;
            }
            //bool ParsingFound = false;
            do
            {
                //nawiasy
                int indexOfDigit = FindDigitPosition(text, tokenPosition);
                int indexOfOpenBracket = OpenBracketPosition(text, tokenPosition);
                int indexOfCloseBracket = CloseBracketPosition(text, tokenPosition);
                int indexOfOpenSquareBracket = OpenSquareBracketPosition(text, tokenPosition);//!!
                int indexOfCloseSquareBracket = CloseSquareBracketPosition(text, tokenPosition);
                
                //operatory arytmetyczne, relacyjne i logiczne
                int indexOfAritmeticOperator = FindOperatorPosition(text, ParserToken.AritmeticOperators, tokenPosition);
                int indexOfLogicalOperator = FindOperatorPosition(text, ParserToken.LogicalOperators, tokenPosition);
                //int indexOfLogicalOperator = FindOperatorPositionLogical(text, ParserToken.LogicalOperators, tokenPosition);

                int indexOfRelationalOperator = FindOperatorPosition(text, ParserToken.RelationalOperators, tokenPosition);
                //funkcje
                int indexOfNumberFunction = FindFunctionPosition(text, ParserToken.NumberFunctions, tokenPosition);

                int indexOfNumberFunction3D = FindFunctionPosition(text, ParserToken.NumberFunctions3D, tokenPosition);//!!

                int indexOfLogicalFunction = FindFunctionPosition(text, ParserToken.LogicalFunctions, tokenPosition);
                int indexOfSequentialFunction = FindFunctionPosition(text, ParserToken.SequentialFunctions, tokenPosition);
                int indexOfBodyParts = FindFunctionPosition(text, ParserToken.BodyParts, tokenPosition);
                int indexOfBodyParts3D = FindFunctionPosition(text, ParserToken.BodyParts3D, tokenPosition);//!!


                int indexOfQuotationMark = QuotationMarkPosition(text, tokenPosition);
                int indexOfComma = CommaPosition(text, tokenPosition);//!!

                int minIndex = findMinimal(
                    indexOfOpenBracket, indexOfCloseBracket, indexOfAritmeticOperator, indexOfLogicalOperator,
                    indexOfRelationalOperator, indexOfNumberFunction, indexOfLogicalFunction, indexOfSequentialFunction,
                    indexOfBodyParts, indexOfCloseSquareBracket, indexOfQuotationMark,
                    indexOfDigit,
                indexOfOpenSquareBracket, indexOfNumberFunction3D, indexOfBodyParts3D, indexOfComma);

                if (minIndex >= 0)
                {
                    ParserToken pt = new ParserToken();
                    pt.Position = tokenPosition;
                    if (minIndex > tokenPosition)
                    {
                        pt.TokenString = text.Substring(tokenPosition, minIndex - tokenPosition);
                        pt.TokenType = ParserToken.TokenTypeConclusion;
                        tokenPosition = tokenPosition + pt.TokenString.Length;
                        pt.TokenString = pt.TokenString.Trim();
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfDigit)
                    {
                        pt.TokenString = FindNumericValue(text, tokenPosition);
                        pt.TokenType = ParserToken.TokenTypeNumeric;
                        try
                        {
                            double dd = double.Parse(pt.TokenString, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            throw new ParserException("Line: " + tokenPosition + ", " + "error parsing numeric value.");
                        }
                        tokenPosition = minIndex + pt.TokenString.Length;
                        pt.TokenString = pt.TokenString.Trim();
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfOpenBracket)
                    {
                        pt.TokenString = "(";
                        pt.TokenType = ParserToken.TokenTypeOpenBracket;
                        //tokenPosition+= minIndex + 1;
                        tokenPosition = minIndex + 1;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfCloseBracket)
                    {
                        pt.TokenString = ")";
                        pt.TokenType = ParserToken.TokenTypeClosedBracket;
                        //tokenPosition+= minIndex + 1;
                        tokenPosition = minIndex + 1;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfOpenSquareBracket)
                    {
                        pt.TokenString = "[";
                        pt.TokenType = ParserToken.TokenTypeOpenSquareBracket;
                        //tokenPosition+= minIndex + 1;
                        tokenPosition = minIndex + 1;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfCloseSquareBracket)
                    {
                        pt.TokenString = "]";
                        pt.TokenType = ParserToken.TokenTypeClosedSquareBracket;
                        //tokenPosition+= minIndex + 1;
                        tokenPosition = minIndex + 1;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfComma)
                    {
                        pt.TokenString = ",";
                        pt.TokenType = ParserToken.TokenTypeComma;
                        //tokenPosition+= minIndex + 1;
                        tokenPosition = minIndex + 1;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfAritmeticOperator)
                    {
                        pt.TokenString = text[minIndex].ToString();
                        pt.TokenType = ParserToken.TokenTypeNumericOperator;
                        //tokenPosition+= minIndex + 1;
                        tokenPosition = minIndex + 1;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfLogicalOperator)
                    {
                        int type = FindOperatorType(text, ParserToken.LogicalOperators, tokenPosition, minIndex);
                        pt.TokenString = ParserToken.LogicalOperators[type];
                        pt.TokenType = ParserToken.TokenTypeLogicalOperator;
                        //tokenPosition += minIndex + RuleTypes.RelationalOperators[type].Length;
                        tokenPosition = minIndex + ParserToken.LogicalOperators[type].Length;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfRelationalOperator)
                    {
                        int type = FindOperatorType(text, ParserToken.RelationalOperators, tokenPosition, minIndex);
                        pt.TokenString = ParserToken.RelationalOperators[type];
                        pt.TokenType = ParserToken.TokenTypeRelationalOperator;
                        //tokenPosition += minIndex + RuleTypes.RelationalOperators[type].Length;
                        tokenPosition = minIndex + ParserToken.RelationalOperators[type].Length;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfNumberFunction)
                    {
                        int type = FindOperatorType(text, ParserToken.NumberFunctions, tokenPosition, minIndex);
                        pt.TokenString = ParserToken.NumberFunctions[type];
                        pt.TokenType = ParserToken.TokenTypeNumericFunction;
                        //tokenPosition += minIndex + RuleTypes.NumberFunctions[type].Length;
                        tokenPosition = minIndex + ParserToken.NumberFunctions[type].Length;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfNumberFunction3D)
                    {
                        int type = FindOperatorType(text, ParserToken.NumberFunctions3D, tokenPosition, minIndex);
                        pt.TokenString = ParserToken.NumberFunctions3D[type];
                        pt.TokenType = ParserToken.TokenTypeNumericFunction3D;
                        //tokenPosition += minIndex + RuleTypes.NumberFunctions[type].Length;
                        tokenPosition = minIndex + ParserToken.NumberFunctions3D[type].Length;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfLogicalFunction)
                    {
                        int type = FindOperatorType(text, ParserToken.LogicalFunctions, tokenPosition, minIndex);
                        pt.TokenString = ParserToken.LogicalFunctions[type];
                        pt.TokenType = ParserToken.TokenTypeLogicalFunction;
                        //tokenPosition += minIndex + RuleTypes.LogicalFunctions[type].Length;
                        tokenPosition = minIndex + ParserToken.LogicalFunctions[type].Length;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfSequentialFunction)
                    {
                        int type = FindOperatorType(text, ParserToken.SequentialFunctions, tokenPosition, minIndex);
                        pt.TokenString = ParserToken.SequentialFunctions[type];
                        pt.TokenType = ParserToken.TokenTypeSequentialFunction;
                        //tokenPosition += minIndex + RuleTypes.LogicalFunctions[type].Length;
                        tokenPosition = minIndex + ParserToken.SequentialFunctions[type].Length;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfBodyParts)
                    {
                        int type = FindOperatorType(text, ParserToken.BodyParts, tokenPosition, minIndex);
                        pt.TokenString = ParserToken.BodyParts[type];
                        pt.TokenType = ParserToken.TokenTypeBodyPart;
                        //tokenPosition += minIndex + RuleTypes.LogicalFunctions[type].Length;
                        tokenPosition = minIndex + ParserToken.BodyParts[type].Length;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfBodyParts3D)
                    {
                        int type = FindOperatorType(text, ParserToken.BodyParts3D, tokenPosition, minIndex);
                        pt.TokenString = ParserToken.BodyParts3D[type];
                        pt.TokenType = ParserToken.TokenTypeBodyPart3D;
                        //tokenPosition += minIndex + RuleTypes.LogicalFunctions[type].Length;
                        tokenPosition = minIndex + ParserToken.BodyParts3D[type].Length;
                        tokenArray.Add(pt);
                    }
                    else if (minIndex == indexOfQuotationMark)
                    {
                        int indexQM2 = text.IndexOf("\"", minIndex + 1);
                        if (indexQM2 == -1)
                            throw new ParserException("Line: " + minIndex + " unmached \" in gesture sequence");
                        String substringQM = text.Substring(minIndex, indexQM2 - minIndex + 1);
                        //pt.Sequence = ParseSequence(text, tokenPosition);
                        pt.Sequence = ParseSequence(substringQM, tokenPosition);
                        pt.TokenString = substringQM;
                        pt.TokenType = ParserToken.TokenTypeSequence;
                        tokenPosition = indexQM2 + 1;
                        //int type = FindOperatorType(text, ParserToken.BodyParts, tokenPosition, minIndex);
                        //pt.TokenString = ParserToken.BodyParts[type];
                        //pt.TokenType = ParserToken.TokenTypeBodyPart;
                        //tokenPosition += minIndex + RuleTypes.LogicalFunctions[type].Length;
                        //tokenPosition = minIndex + ParserToken.BodyParts[type].Length;
                        tokenArray.Add(pt);
                    }
                    
                }
                //ostatni symbol
                else 
                    {
                        ParserToken pt2 = GenerateBasicToken(text, tokenPosition, text.Length);
                        tokenArray.Add(pt2);
                        tokenPosition = text.Length;
                    }
                    //skip spaces etc.
                    //bool whitespaceSkipLoopEnd = false;
                    whitespaceSkipLoopEnd = false;
                    while (!whitespaceSkipLoopEnd)
                    {
                        if (tokenPosition < text.Length)
                            if (text[tokenPosition] == ' ' || text[tokenPosition] == '\t' || text[tokenPosition] == '\r' || text[tokenPosition] == '\n')
                                tokenPosition++;
                            else
                                whitespaceSkipLoopEnd = true;
                        else
                            whitespaceSkipLoopEnd = true;
                    }
                    //while (text[tokenPosition] == ' ' || text[tokenPosition] == '\t' || text[tokenPosition] == '\r' || text[tokenPosition] == '\n')
                        //tokenPosition++;
                //}

                //
            }
            while (tokenPosition < text.Length);
            for (int a = 0; a < tokenArray.Count; a++)
            {
                ((ParserToken)tokenArray[a]).TokenTypeMemory = ((ParserToken)tokenArray[a]).TokenType;
            }
            return tokenArray;
        }

        */
        private static void DoBasicRuleTypes(ArrayList stack)
        {
            int stackSize = stack.Count;
            for (int a = 0; a < stack.Count; a++)
            {
                ParserToken pt = (ParserToken)stack[a];
                if (pt.RuleType == ParserToken.RuleTypeUndefined)
                {
                    if (pt.TokenType == ParserToken.TokenTypeNumeric)
                    {
                        pt.RuleType = ParserToken.RuleTypeNumeric;
                    }
                    /*if (pt.TokenType == ParserToken.TokenTypeBodyPart)
                    {
                        pt.RuleType = ParserToken.RuleTypeNumeric;
                    }*/
                    if (pt.TokenType == ParserToken.TokenTypeConclusion)
                    {
                        pt.RuleType = ParserToken.RuleTypeLogical;
                    }
                }
            }
        }

        private static bool CheckTheNumericRules(ArrayList stack)
        {
            int stackSize = stack.Count;
            ParserToken p7 = null, p6 = null, p5 = null, p4 = null, p3 = null, p2 = null, p1 = null;
            for (int a = 0; a < stack.Count; a++)
            {
                p1 = (ParserToken)stack[a];
                if (stack.Count > 1 + a) p2 = (ParserToken)stack[a + 1];
                else return false;
                /////////////liczby ujemne, np. -15
                if (p1.TokenType == ParserToken.TokenTypeNumericOperator && p1.TokenString == "-" && p2.RuleType == ParserToken.RuleTypeNumeric)
                {
                    p2.TokenString = "-" + p2.TokenString;
                    stack.Remove(p1);
                    p2.RuleType = ParserToken.RuleTypeNumeric;
                    p1.TokenType = ParserToken.TokenTypeUndefined;
                    p2.TokenType = ParserToken.TokenTypeUndefined;
                    return true;
                }
                else
                {
                    if (stack.Count > 2 + a) p3 = (ParserToken)stack[a + 2];
                    else return false;
                    //44 + 33
                    if (p1.RuleType == ParserToken.RuleTypeNumeric && p2.TokenType == ParserToken.TokenTypeNumericOperator && p3.RuleType == ParserToken.RuleTypeNumeric)
                    {
                        p2.Children.Add(p1);
                        p2.Children.Add(p3);
                        stack.Remove(p1);
                        stack.Remove(p3);
                        p2.RuleType = ParserToken.RuleTypeNumeric;

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    //bodypart3D.xyz + bodypart3D.xyz
                    if (p1.RuleType == ParserToken.RuleTypeNumeric3D && p2.TokenType == ParserToken.TokenTypeNumericOperator && p3.RuleType == ParserToken.RuleTypeNumeric3D
                        && (p2.TokenString == "+" || p2.TokenString == "-"))
                    {
                        p2.Children.Add(p1);
                        p2.Children.Add(p3);
                        stack.Remove(p1);
                        stack.Remove(p3);
                        p2.TokenTypeMemory = ParserToken.TokenTypeNumericOperator3D;
                        p2.RuleType = ParserToken.RuleTypeNumeric3D;

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    else if (p2.TokenType == ParserToken.TokenTypeNumericOperator && p2.TokenString == "-" && p3.RuleType == ParserToken.RuleTypeNumeric)
                    {
                        p3.TokenString = "-" + p3.TokenString;
                        stack.Remove(p2);
                        p3.RuleType = ParserToken.RuleTypeNumeric;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    /////////////liczby ujemne3D?
                    else if (p1.TokenType == ParserToken.TokenTypeNumericOperator && p1.TokenString == "-" && p2.RuleType == ParserToken.RuleTypeNumeric3D)
                    {
                        p2.TokenString = "-" + p2.TokenString;
                        stack.Remove(p1);
                        p2.RuleType = ParserToken.RuleTypeNumeric3D;
                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    else if (p2.TokenType == ParserToken.TokenTypeNumericOperator && p2.TokenString == "-" && p3.RuleType == ParserToken.RuleTypeNumeric3D)
                    {
                        p3.TokenString = "-" + p3.TokenString;
                        stack.Remove(p2);
                        p3.RuleType = ParserToken.RuleTypeNumeric3D;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    //(44)
                    else if (p1.TokenType == ParserToken.TokenTypeOpenBracket && p2.RuleType == ParserToken.RuleTypeNumeric && p3.TokenType == ParserToken.TokenTypeClosedBracket)
                    {
                        /*p1.Children.Add(p2);
                        stack.Remove(p2);
                        stack.Remove(p3);
                        p1.RuleType = ParserToken.RuleTypeNumeric;
                        p1.TokenString = "()";

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;*/
                    
                    
                        p2.RuleType = ParserToken.RuleTypeNumeric;
                        stack.Remove(p1);
                        stack.Remove(p3);

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    //(bodypart3D.xyz)
                    else if (p1.TokenType == ParserToken.TokenTypeOpenBracket && p2.RuleType == ParserToken.RuleTypeNumeric3D && p3.TokenType == ParserToken.TokenTypeClosedBracket)
                    {
                        p2.RuleType = ParserToken.RuleTypeNumeric3D;
                        stack.Remove(p1);
                        stack.Remove(p3);

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    //ABS(44)
                    else if (p1.TokenType == ParserToken.TokenTypeNumericFunction && p2.RuleType == ParserToken.RuleTypeNumeric && p3.TokenType == ParserToken.TokenTypeClosedBracket
                        && (p1.TokenString == "sqrt(" || p1.TokenString == "abs(" || p1.TokenString == "sgn("))
                    {
                        p1.Children.Add(p2);
                        stack.Remove(p2);
                        stack.Remove(p3);
                        p1.RuleType = ParserToken.RuleTypeNumeric;

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    //bodypart.x[55]
                    else if (p1.TokenType == ParserToken.TokenTypeBodyPart && p2.RuleType == ParserToken.RuleTypeNumeric && p3.TokenType == ParserToken.TokenTypeClosedSquareBracket)
                    {
                        p1.Children.Add(p2);
                        stack.Remove(p2);
                        stack.Remove(p3);
                        p1.RuleType = ParserToken.RuleTypeNumeric;

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    //bodypart3D.xyz[55]
                    else if (p1.TokenType == ParserToken.TokenTypeBodyPart3D && p2.RuleType == ParserToken.RuleTypeNumeric && p3.TokenType == ParserToken.TokenTypeClosedSquareBracket)
                    {
                        p1.Children.Add(p2);
                        stack.Remove(p2);
                        stack.Remove(p3);
                        p1.RuleType = ParserToken.RuleTypeNumeric3D;

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    //sequenceexists("dupa")
                    else if (p1.TokenType == ParserToken.TokenTypeSequentialFunction && p2.TokenType == ParserToken.TokenTypeSequenceWithFeatures && p3.TokenType == ParserToken.TokenTypeClosedBracket
                        && p1.TokenString == "sequencescore(")
                    {
                        p1.Children.Add(p2);
                        stack.Remove(p2);
                        stack.Remove(p3);
                        p1.RuleType = ParserToken.RuleTypeNumeric;

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                    else
                    {
                        if (stack.Count > 3 + a)
                        {
                            p4 = (ParserToken)stack[a + 3];
                            if (stack.Count > 4 + a)
                            {
                                p5 = (ParserToken)stack[a + 4];
                                //sgnfuzzy(value, value)
                                if (p1.TokenType == ParserToken.TokenTypeNumericFunction && p2.RuleType == ParserToken.RuleTypeNumeric
                                    && p3.TokenType == ParserToken.TokenTypeComma && p4.RuleType == ParserToken.RuleTypeNumeric
                                    && p5.TokenType == ParserToken.TokenTypeClosedBracket
                                    && (p1.TokenString == "sgnfuzzy("))
                                {
                                    p1.Children.Add(p2);
                                    p1.Children.Add(p4);
                                    stack.Remove(p2);
                                    stack.Remove(p3);
                                    stack.Remove(p4);
                                    stack.Remove(p5);
                                    p1.TokenType = ParserToken.TokenTypeUndefined;
                                    p2.TokenType = ParserToken.TokenTypeUndefined;
                                    p3.TokenType = ParserToken.TokenTypeUndefined;
                                    p4.TokenType = ParserToken.TokenTypeUndefined;
                                    p5.TokenType = ParserToken.TokenTypeUndefined;
                                    p1.RuleType = ParserToken.RuleTypeNumeric;
                                    return true;
                                }
                                //distance( or angle
                                if (p1.TokenType == ParserToken.TokenTypeNumericFunction3D && p2.RuleType == ParserToken.RuleTypeNumeric3D
                                    && p3.TokenType == ParserToken.TokenTypeComma && p4.RuleType == ParserToken.RuleTypeNumeric3D
                                    && p5.TokenType == ParserToken.TokenTypeClosedBracket
                                    && (p1.TokenString == "distance(" || p1.TokenString == "angle("  || p1.TokenString == "cross("))
                                {
                                    p1.Children.Add(p2);
                                    p1.Children.Add(p4);
                                    stack.Remove(p2);
                                    stack.Remove(p3);
                                    stack.Remove(p4);
                                    stack.Remove(p5);
                                    p1.TokenType = ParserToken.TokenTypeUndefined;
                                    p2.TokenType = ParserToken.TokenTypeUndefined;
                                    p3.TokenType = ParserToken.TokenTypeUndefined;
                                    p4.TokenType = ParserToken.TokenTypeUndefined;
                                    p5.TokenType = ParserToken.TokenTypeUndefined;
                                    if (p1.TokenString == "cross(")
                                    {
                                        p1.RuleType = ParserToken.RuleTypeNumeric3D;
                                    }
                                    else
                                        p1.RuleType = ParserToken.RuleTypeNumeric;
                                    return true;
                                }
                                if (stack.Count > 6 + a)
                                {
                                    p6 = (ParserToken)stack[a + 5];
                                    p7 = (ParserToken)stack[a + 6];
                                    //CheckCollinearity( - PASKUDNY WYJ¥TEK, KTÓRY ZAMIAST ZWRACAÆ NUMERIC ZWRACA NUMERIC3D!!
                                    //WARTO BY POTEM PRZEROBIÆ
                                    if (p1.TokenType == ParserToken.TokenTypeNumericFunction3D
                                        //TU JEST KOLEJNA RÓ¯NICA - MUSIA£O TO BYÆ KIEDYŒ TokenTypeBodyPart3D
                                        && p2.TokenTypeMemory == ParserToken.TokenTypeBodyPart3D
                                        && p3.TokenType == ParserToken.TokenTypeComma && p4.RuleType == ParserToken.RuleTypeNumeric
                                        && p5.TokenType == ParserToken.TokenTypeComma && p6.RuleType == ParserToken.RuleTypeNumeric
                                        && p7.TokenType == ParserToken.TokenTypeClosedBracket
                                        && p1.TokenString == "checkcollinearity(")
                                    /*if (p1.TokenType == ParserToken.TokenTypeNumericFunction3D && p2.RuleType == ParserToken.RuleTypeNumeric3D
                                        && p3.TokenType == ParserToken.TokenTypeComma && p4.RuleType == ParserToken.RuleTypeNumeric
                                        && p5.TokenType == ParserToken.TokenTypeClosedBracket
                                        && p2.TokenString == "CheckCollinearity(")*/
                                    {
                                        p1.Children.Add(p2);
                                        p1.Children.Add(p4);
                                        p1.Children.Add(p6);
                                        stack.Remove(p2);
                                        stack.Remove(p3);
                                        stack.Remove(p4);
                                        stack.Remove(p5);
                                        stack.Remove(p6);
                                        stack.Remove(p7);
                                        p1.TokenType = ParserToken.TokenTypeUndefined;
                                        p2.TokenType = ParserToken.TokenTypeUndefined;
                                        p3.TokenType = ParserToken.TokenTypeUndefined;
                                        p4.TokenType = ParserToken.TokenTypeUndefined;
                                        p5.TokenType = ParserToken.TokenTypeUndefined;
                                        p6.TokenType = ParserToken.TokenTypeUndefined;
                                        p7.TokenType = ParserToken.TokenTypeUndefined;

                                        //tu jest ró¿nica!!!!
                                        p1.RuleType = ParserToken.RuleTypeNumeric3D;
                                        return true;
                                    }
                                }
                                if (stack.Count > 6 + a)
                                {
                                    p6 = (ParserToken)stack[a + 5];
                                    p7 = (ParserToken)stack[a + 6];
                                    //vector
                                    //[1,2,3]
                                    if (p1.TokenType == ParserToken.TokenTypeOpenSquareBracket && p2.RuleType == ParserToken.RuleTypeNumeric
                                        && p3.TokenType == ParserToken.TokenTypeComma && p4.RuleType == ParserToken.RuleTypeNumeric
                                        && p5.TokenType == ParserToken.TokenTypeComma && p6.RuleType == ParserToken.RuleTypeNumeric
                                        && p7.TokenType == ParserToken.TokenTypeClosedSquareBracket)
                                    {
                                        p1.RuleType = ParserToken.RuleTypeNumeric3D;
                                        p1.Children.Add(p2);
                                        p1.Children.Add(p4);
                                        p1.Children.Add(p6);
                                        stack.Remove(p2);
                                        stack.Remove(p3);
                                        stack.Remove(p4);
                                        stack.Remove(p5);
                                        stack.Remove(p6);
                                        stack.Remove(p7);
                                        p1.TokenType = ParserToken.TokenTypeUndefined;
                                        p2.TokenType = ParserToken.TokenTypeUndefined;
                                        p3.TokenType = ParserToken.TokenTypeUndefined;
                                        p4.TokenType = ParserToken.TokenTypeUndefined;
                                        p5.TokenType = ParserToken.TokenTypeUndefined;
                                        p6.TokenType = ParserToken.TokenTypeUndefined;
                                        p7.TokenType = ParserToken.TokenTypeUndefined;
                                        return true;
                                    }
                                }
                                if (stack.Count > 8 + a)
                                {
                                    p6 = (ParserToken)stack[a + 5];
                                    p7 = (ParserToken)stack[a + 6];
                                    ParserToken p8 = (ParserToken)stack[a + 7];
                                    ParserToken p9 = (ParserToken)stack[a + 8];
                                    //CheckCollinearity( - PASKUDNY WYJ¥TEK, KTÓRY ZAMIAST ZWRACAÆ NUMERIC ZWRACA NUMERIC3D!!
                                    //WARTO BY POTEM PRZEROBIÆ
                                    if (p1.TokenType == ParserToken.TokenTypeNumericFunction3D
                                        //TU JEST KOLEJNA RÓ¯NICA - MUSIA£O TO BYÆ KIEDYŒ TokenTypeBodyPart3D
                                        && p2.TokenTypeMemory == ParserToken.TokenTypeBodyPart3D
                                        && p3.TokenType == ParserToken.TokenTypeComma && p4.RuleType == ParserToken.RuleTypeNumeric
                                        && p5.TokenType == ParserToken.TokenTypeComma && p6.RuleType == ParserToken.RuleTypeNumeric
                                        && p7.TokenType == ParserToken.TokenTypeComma && p8.RuleType == ParserToken.RuleTypeNumeric
                                        && p9.TokenType == ParserToken.TokenTypeClosedBracket
                                        && p1.TokenString == "checkcircularity(")
                                    /*if (p1.TokenType == ParserToken.TokenTypeNumericFunction3D && p2.RuleType == ParserToken.RuleTypeNumeric3D
                                        && p3.TokenType == ParserToken.TokenTypeComma && p4.RuleType == ParserToken.RuleTypeNumeric
                                        && p5.TokenType == ParserToken.TokenTypeClosedBracket
                                        && p2.TokenString == "CheckCollinearity(")*/
                                    {
                                        p1.Children.Add(p2);
                                        p1.Children.Add(p4);
                                        p1.Children.Add(p6);
                                        p1.Children.Add(p8);
                                        stack.Remove(p2);
                                        stack.Remove(p3);
                                        stack.Remove(p4);
                                        stack.Remove(p5);
                                        stack.Remove(p6);
                                        stack.Remove(p7);
                                        stack.Remove(p7);
                                        stack.Remove(p8);
                                        stack.Remove(p9);
                                        p1.TokenType = ParserToken.TokenTypeUndefined;
                                        p2.TokenType = ParserToken.TokenTypeUndefined;
                                        p3.TokenType = ParserToken.TokenTypeUndefined;
                                        p4.TokenType = ParserToken.TokenTypeUndefined;
                                        p5.TokenType = ParserToken.TokenTypeUndefined;
                                        p6.TokenType = ParserToken.TokenTypeUndefined;
                                        p7.TokenType = ParserToken.TokenTypeUndefined;
                                        p8.TokenType = ParserToken.TokenTypeUndefined;
                                        p9.TokenType = ParserToken.TokenTypeUndefined;
                                        //tu jest ró¿nica!!!!
                                        p1.RuleType = ParserToken.RuleTypeNumeric3D;
                                        return true;
                                    }
                                }// || p1.TokenString == "checkcircularity(")
                            }

                        }
                    }
                }

            }
            return false;
        }

        private static bool CheckTheLogicalRules(ArrayList stack)
        {
            int stackSize = stack.Count;
            ParserToken p7 = null, p6 = null, p5 = null, p4 = null, p3 = null, p2 = null, p1 = null;
            for (int a = 0; a < stack.Count; a++)
            {
                p1 = (ParserToken)stack[a];
                if (stack.Count > 1 + a) p2 = (ParserToken)stack[a + 1];
                else return false;
                if (stack.Count > 2 + a) p3 = (ParserToken)stack[a + 2];
                else return false;
                //44 < 33
                if (p1.RuleType == ParserToken.RuleTypeNumeric && p2.TokenType == ParserToken.TokenTypeRelationalOperator && p3.RuleType == ParserToken.RuleTypeNumeric)
                {
                    p2.Children.Add(p1);
                    p2.Children.Add(p3);
                    stack.Remove(p1);
                    stack.Remove(p3);
                    p2.RuleType = ParserToken.RuleTypeLogical;

                    p1.TokenType = ParserToken.TokenTypeUndefined;
                    p2.TokenType = ParserToken.TokenTypeUndefined;
                    p3.TokenType = ParserToken.TokenTypeUndefined;

                    return true;
                }

                //TRUE AND FALSE
                if (p1.RuleType == ParserToken.RuleTypeLogical && p2.TokenType == ParserToken.TokenTypeLogicalOperator && p3.RuleType == ParserToken.RuleTypeLogical)
                {
                    p2.Children.Add(p1);
                    p2.Children.Add(p3);
                    stack.Remove(p1);
                    stack.Remove(p3);
                    p2.RuleType = ParserToken.RuleTypeLogical;

                    p1.TokenType = ParserToken.TokenTypeUndefined;
                    p2.TokenType = ParserToken.TokenTypeUndefined;
                    p3.TokenType = ParserToken.TokenTypeUndefined;

                    return true;
                }

                //(true)
                if (p1.TokenType == ParserToken.TokenTypeOpenBracket && p2.RuleType == ParserToken.RuleTypeLogical && p3.TokenType == ParserToken.TokenTypeClosedBracket)
                {
                    /*p1.Children.Add(p2);
                    stack.Remove(p2);
                    stack.Remove(p3);
                    p1.RuleType = ParserToken.RuleTypeLogical;
                    p1.TokenString = "()";

                    p1.TokenType = ParserToken.TokenTypeUndefined;
                    p2.TokenType = ParserToken.TokenTypeUndefined;
                    p3.TokenType = ParserToken.TokenTypeUndefined;*/

                    p2.RuleType = ParserToken.RuleTypeLogical;
                    stack.Remove(p1);
                    stack.Remove(p3);

                    p1.TokenType = ParserToken.TokenTypeUndefined;
                    p2.TokenType = ParserToken.TokenTypeUndefined;
                    p3.TokenType = ParserToken.TokenTypeUndefined;
                    return true;
                }
                //not(true)
                if (p1.TokenType == ParserToken.TokenTypeLogicalFunction && p2.RuleType == ParserToken.RuleTypeLogical && p3.TokenType == ParserToken.TokenTypeClosedBracket)
                {
                    p1.Children.Add(p2);
                    stack.Remove(p2);
                    stack.Remove(p3);
                    p1.RuleType = ParserToken.RuleTypeLogical;

                    p1.TokenType = ParserToken.TokenTypeUndefined;
                    p2.TokenType = ParserToken.TokenTypeUndefined;
                    p3.TokenType = ParserToken.TokenTypeUndefined;
                    return true;
                }
                //sequenceexists("dupa")
                if (p1.TokenType == ParserToken.TokenTypeSequentialFunction && p2.TokenType == ParserToken.TokenTypeSequence && p3.TokenType == ParserToken.TokenTypeClosedBracket
                    && p1.TokenString == "sequenceexists(")
                {
                    p1.Children.Add(p2);
                    stack.Remove(p2);
                    stack.Remove(p3);
                    p1.RuleType = ParserToken.RuleTypeLogical;

                    p1.TokenType = ParserToken.TokenTypeUndefined;
                    p2.TokenType = ParserToken.TokenTypeUndefined;
                    p3.TokenType = ParserToken.TokenTypeUndefined;
                    return true;
                }
                //rulepersists(dupa,1,2)
                if (stack.Count > 6 + a)
                {
                    p4 = (ParserToken)stack[a + 3];
                    p5 = (ParserToken)stack[a + 4];
                    p6 = (ParserToken)stack[a + 5];
                    p7 = (ParserToken)stack[a + 6];
                    //rulepersists(dupa,1,2)
                    if (p1.TokenType == ParserToken.TokenTypeSequentialFunction && p2.TokenType == ParserToken.TokenTypeConclusion
                        && p3.TokenType == ParserToken.TokenTypeComma
                        && p4.TokenType == ParserToken.TokenTypeNumeric
                        && p5.TokenType == ParserToken.TokenTypeComma
                        && p6.TokenType == ParserToken.TokenTypeNumeric
                        && p7.TokenType == ParserToken.TokenTypeClosedBracket
                        && p1.TokenString == "rulepersists(")
                    {
                        p1.Children.Add(p2);
                        p1.Children.Add(p4);
                        p1.Children.Add(p6);
                        stack.Remove(p2);
                        stack.Remove(p3);
                        stack.Remove(p4);
                        stack.Remove(p5);
                        stack.Remove(p6);
                        stack.Remove(p7);
                        p1.RuleType = ParserToken.RuleTypeLogical;

                        p1.TokenType = ParserToken.TokenTypeUndefined;
                        p2.TokenType = ParserToken.TokenTypeUndefined;
                        p3.TokenType = ParserToken.TokenTypeUndefined;
                        p4.TokenType = ParserToken.TokenTypeUndefined;
                        p5.TokenType = ParserToken.TokenTypeUndefined;
                        p6.TokenType = ParserToken.TokenTypeUndefined;
                        p7.TokenType = ParserToken.TokenTypeUndefined;
                        return true;
                    }
                }
                if (stack.Count > 3 + a) p4 = (ParserToken)stack[a + 3];
                else return false;
            }
            return false;
        }

        private static ParserToken GenerateTree(ArrayList tokens)
        {
            ArrayList stack = new ArrayList();
            int a = 0;
            bool reduce = false;

            String help = "";
            for (a = 0; a < tokens.Count; a++)
                help += ((ParserToken)tokens[a]).TokenString;

            for (a = 0; a < tokens.Count; a++)
                stack.Add(tokens[a]);

            DoBasicRuleTypes(stack);
            do
            {
               //reduce = CheckTheRules(stack);
                reduce = CheckTheNumericRules(stack);
            } while (reduce);

            do
            {
                //reduce = CheckTheRules(stack);
                reduce = CheckTheLogicalRules(stack);
            } while (reduce);

            if (stack.Count > 1)
            {
                String errorMessage = "Parser reduction error\r\n";
                for (a = 0; a < stack.Count; a++)
                {
                    errorMessage += "Position: " + ((ParserToken)stack[a]).Position + "; ";
                    errorMessage += "Text: " + ((ParserToken)stack[a]).TokenString + "; ";
                    errorMessage += "Token type: " + ((ParserToken)stack[a]).TokenType + "; ";
                    errorMessage += "Rule type" + ((ParserToken)stack[a]).RuleType + ";\r\n";
                }
                //throw new ParserException(errorMessage);
                ParserToken pt = (ParserToken)stack[1];
                throw new ParserException("Parser reduction error in line:" + pt.PositionLn + " column:" + pt.PositionCol + " on symbol: "
                + pt.TokenString, pt.PositionCol, pt.PositionLn);
            }
            return (ParserToken)stack[0];
        }

        /*
        private static ParserToken ParseRule(String text)
        {
            //Rule rule = new Rule();
            int index1 = text.IndexOf("rule");
            text = text.Remove(index1, "rule".Length);
            int index2 = text.IndexOf("then");
            //brakuje "then"
            if (index2 < 0)
                throw new ParserException("Line: " + index2 + ", for \"rule" + text + "\" matching \"then\" was not found.");
            String ruleText = text.Substring(0, index2);
            String conclusion = text.Substring(index2 + "then".Length, text.Length - index2 - "then".Length);
            conclusion = conclusion.Trim();
            //"then" ma wiêcej ni¿ jeden wyraz
            if (conclusion != RemoveAllWhitespaceChar(conclusion))
                throw new ParserException("Line: " + index2 + ", for \"rule" + text + "\" after \"then\" clause only one-ward phrase is allowed.");

            ArrayList tokens = GenerateTokens(ruleText);

            String controlString = "";
            for (int a = 0; a < tokens.Count; a++)
            {
                controlString += ((ParserToken)tokens[a]).TokenString;
            }
            ParserToken root = GenerateTree(tokens);
            root.Conclusion = conclusion;
            return root;
        }*/



        private static void CheckConclusionsRecursion(ParserToken rule, ParserToken[] rules, ParserToken[] features)
        {
            bool found = false;
            if (rule.TokenTypeMemory == ParserToken.TokenTypeConclusion)
            {
                //feature
                if (rule.RuleType == ParserToken.RuleTypeNumeric || rule.RuleType == ParserToken.RuleTypeNumeric3D)
                {
                    for (int a = 0; a < features.Length; a++)
                        if (rule.TokenString == features[a].Conclusion)
                        {
                            found = true;
                        }
                }
                //conslusion from rule
                else
                {
                    for (int a = 0; a < rules.Length; a++)
                        if (rule.TokenString == rules[a].Conclusion)
                        {
                            found = true;
                        }
                }
                if (!found) throw new ParserException("Position: " + rule.Position + ", \"" + rule.TokenString + "\" is not a conclusion or feature!", rule.PositionCol, rule.PositionLn);
            }
            if (rule.TokenTypeMemory == ParserToken.TokenTypeSequence || rule.TokenTypeMemory == ParserToken.TokenTypeSequenceWithFeatures)
            {
                for (int a = 0; a < rule.Sequence.Count; a++)
                {
                    SequenceToken st = (SequenceToken)rule.Sequence[a];
                    for (int b = 0; b < st.Conclusions.Length; b++)
                    {
                        found = false;
                        for (int c = 0; c < rules.Length; c++)
                            if (st.Conclusions[b] == rules[c].Conclusion)
                            {
                                found = true;
                            }
                        if (!found) throw new ParserException("Position: " + rule.Position + ", \"" + rule.TokenString + "\" is not a conclusion!", rule.PositionCol, rule.PositionLn);
                    }
                    if (rule.TokenTypeMemory == ParserToken.TokenTypeSequenceWithFeatures)
                    {
                        if (st.Features != null)
                        {
                            for (int b = 0; b < st.Features.Length; b++)
                            {
                                found = false;
                                for (int c = 0; c < features.Length; c++)
                                    if (st.Features[b] == features[c].Conclusion)
                                    {
                                        found = true;
                                    }
                                if (!found) throw new ParserException("Position: " + rule.Position + ", \"" + rule.TokenString + "\" is not a feature!", rule.PositionCol, rule.PositionLn);
                            }
                        }
                        if (st.FeaturesToTakesValuesFrom != null)
                        {
                            for (int b = 0; b < st.FeaturesToTakesValuesFrom.Length; b++)
                            {
                                found = false;
                                for (int c = 0; c < features.Length; c++)
                                    if (st.FeaturesToTakesValuesFrom[b] == features[c].Conclusion)
                                    {
                                        found = true;
                                    }
                                if (!found) throw new ParserException("Position: " + rule.Position + ", \"" + rule.TokenString + "\" is not a feature!", rule.PositionCol, rule.PositionLn);
                            }
                        }
                    }
                }
            }
            for (int a = 0; a < rule.Children.Count; a++)
            {
                CheckConclusionsRecursion((ParserToken)rule.Children[a], rules, features);
            }
        }

        private static void CheckConclusions(ParserToken[] rules, ParserToken[] features)
        {
            for (int a = 0; a < rules.Length; a++)
            {
                CheckConclusionsRecursion(rules[a], rules, features);
            }
            for (int a = 0; a < features.Length; a++)
            {
                CheckConclusionsRecursion(features[a], rules, features);
            }
        }

        private static ArrayList FindAndRemoveRuleTokens(ArrayList array)
        {
            if (array == null) return null;
            if (array.Count == 0) return null;
            int start = 0;
            bool end = false;
            while (!end)
            {
                if (((ParserToken)array[start]).TokenType == ParserToken.TokenTypeRuleKeyword)
                    end = true;
                else
                    start++;
                if (start >= array.Count)
                {
                    end = true;
                    return null;
                }
            }
            int stop = start;
            end = false;
            while (!end)
            {
                if (((ParserToken)array[stop]).TokenType == ParserToken.TokenTypeThenKeyword)
                    end = true;
                else
                    stop++;
                if (stop >= array.Count)
                {
                    end = true;
                    return null;
                }
            }
            //dodaæ +1 token, który jest konkluzj¹
            if (stop + 1 < array.Count)
                stop++;
            else
                return null;
            ArrayList returnA = new ArrayList();
            for (int a = start; a <= stop; a++)
                returnA.Add(array[a]);
            array.RemoveRange(start, stop - start + 1);
            return returnA;
        }

        private static ArrayList FindAndRemoveFeaturesTokens(ArrayList array)
        {
            if (array == null) return null;
            if (array.Count == 0) return null;
            int start = 0;
            bool end = false;
            while (!end)
            {
                if (((ParserToken)array[start]).TokenType == ParserToken.TokenTypeFeatureKeyword)
                    end = true;
                else
                    start++;
                if (start >= array.Count)
                {
                    end = true;
                    return null;
                }
            }
            int stop = start;
            end = false;
            while (!end)
            {
                if (((ParserToken)array[stop]).TokenType == ParserToken.TokenTypeAsKeyword)
                    end = true;
                else
                    stop++;
                if (stop >= array.Count)
                {
                    end = true;
                    return null;
                }
            }
            //dodaæ +1 token, który jest konkluzj¹
            if (stop + 1 < array.Count)
                stop++;
            else
                return null;
            ArrayList returnA = new ArrayList();
            for (int a = start; a <= stop; a++)
                returnA.Add(array[a]);
            array.RemoveRange(start, stop - start + 1);
            return returnA;
        }

        private static ParserToken ParseRule(ArrayList tokens)
        {
            
            ParserToken pt = (ParserToken)tokens[0];
            if (pt.TokenType != ParserToken.TokenTypeRuleKeyword)
                throw new ParserException("Line: " + pt.PositionLn + ", column" + pt.PositionCol + " error parsing " + pt.TokenString + " as RULE keyword.", pt.PositionCol, pt.PositionLn);
            if (tokens.Count < 3)
            {
                throw new ParserException("Line: " + pt.PositionLn + ", column" + pt.PositionCol + " rule without body.", pt.PositionCol, pt.PositionLn);
            }
            pt = (ParserToken)tokens[tokens.Count - 2];
            if (pt.TokenType != ParserToken.TokenTypeThenKeyword)
                throw new ParserException("Line: " + pt.PositionLn + ", column" + pt.PositionCol + " error parsing " + pt.TokenString + " as THEN keyword.", pt.PositionCol, pt.PositionLn);
            pt = (ParserToken)tokens[tokens.Count - 1];
            if (pt.TokenType != ParserToken.TokenTypeConclusion)
                throw new ParserException("Line: " + pt.PositionLn + ", column" + pt.PositionCol + " error parsing " + pt.TokenString + " as conclusion.", pt.PositionCol, pt.PositionLn);
            //remove conlcusion
            tokens.RemoveAt(tokens.Count - 1);
            //remove THEN keyword
            tokens.RemoveAt(tokens.Count - 1);
            //remove RULE keyword
            tokens.RemoveAt(0);

            String controlString = "";
            for (int a = 0; a < tokens.Count; a++)
            {
                controlString += ((ParserToken)tokens[a]).TokenString;
            }
            ParserToken root = GenerateTree(tokens);
            if (root.RuleType != ParserToken.RuleTypeLogical)
                throw new ParserException("Line: " + root.PositionLn + ", column" + root.PositionCol + " RULE content was not reduced to logical value", root.PositionCol, root.PositionLn);
            root.Conclusion = pt.TokenString;
            return root;
        }

        private static ParserToken ParseFeature(ArrayList tokens)
        {

            ParserToken pt = (ParserToken)tokens[0];
            if (pt.TokenType != ParserToken.TokenTypeFeatureKeyword)
                throw new ParserException("Line: " + pt.PositionLn + ", column" + pt.PositionCol + " error parsing " + pt.TokenString + " as RULE keyword.", pt.PositionCol, pt.PositionLn);
            if (tokens.Count < 3)
            {
                throw new ParserException("Line: " + pt.PositionLn + ", column" + pt.PositionCol + " rule without body.", pt.PositionCol, pt.PositionLn);
            }
            pt = (ParserToken)tokens[tokens.Count - 2];
            if (pt.TokenType != ParserToken.TokenTypeAsKeyword)
                throw new ParserException("Line: " + pt.PositionLn + ", column" + pt.PositionCol + " error parsing " + pt.TokenString + " as THEN keyword.", pt.PositionCol, pt.PositionLn);
            pt = (ParserToken)tokens[tokens.Count - 1];
            if (pt.TokenType != ParserToken.TokenTypeConclusion)
                throw new ParserException("Line: " + pt.PositionLn + ", column" + pt.PositionCol + " error parsing " + pt.TokenString + " as conclusion.", pt.PositionCol, pt.PositionLn);
            //remove conlcusion
            tokens.RemoveAt(tokens.Count - 1);
            //remove THEN keyword
            tokens.RemoveAt(tokens.Count - 1);
            //remove RULE keyword
            tokens.RemoveAt(0);

            String controlString = "";
            for (int a = 0; a < tokens.Count; a++)
            {
                controlString += ((ParserToken)tokens[a]).TokenString;
            }
            ParserToken root = GenerateTree(tokens);
            if (root.RuleType != ParserToken.RuleTypeNumeric && root.RuleType != ParserToken.RuleTypeNumeric3D)
                throw new ParserException("Line: " + root.PositionLn + ", column: " + root.PositionCol + " FEATURE content was not reduced to numeric value",root.PositionCol, root.PositionLn);
            root.Conclusion = pt.TokenString;
            return root;
        }

        public static void ParseFile(String fileName, ref ParserToken[] features, ref ParserToken[] rules)
        {
            string textWithoutComments = System.IO.File.ReadAllText(fileName);
            ParseString(textWithoutComments, ref features, ref rules);
        }

        private static void CheckDuplicatedNames(ParserToken[] table1, ParserToken[] table2)
        {
            for (int a = 0; a < table1.Length; a++)
                for (int b = 0; b < table2.Length; b++)
                    if (table1[a].Conclusion.ToLower() == table2[b].Conclusion.ToLower() && a != b)
                    {
                        throw new ParserException("Line: " + table1[a].PositionLn + ", column: " + table1[a].PositionCol + " and line: " + table2[b].PositionLn + ", column: " + table2[b].PositionCol + " duplicated name was detected. FEATURES' and RULES' names have to be unique in GDLs", table2[b].PositionCol, table2[b].PositionLn);
                    }
        }

        //public static void ParseFile(String fileName, ref ParserToken[]features, ref ParserToken[]rules)
        public static void ParseString(String text, ref ParserToken[] features, ref ParserToken[] rules)
        {
            //String ruleText = null;
            ParserToken myRule = null;
            ArrayList arrayRuleHelp = new ArrayList();
            ArrayList arrayFeaturesHelp = new ArrayList();

            ParserToken.BodyParts = GenerateSkeletonJoints();
            ParserToken.BodyParts3D = GenerateSkeletonJoints3D();
            //ParserToken.ConclusionsAndFeaturesNames = GenerateConclusionsFeaturesNames();

            //string textWithoutComments = System.IO.File.ReadAllText(fileName);
            string textWithoutComments = text;
            ArrayList allRulesTokens = Lexer.GenerateTokens(textWithoutComments);
            ArrayList ruleTokens = null;
            //PARES FEATURES
            do
            {
                ruleTokens = FindAndRemoveFeaturesTokens(allRulesTokens);
                if (ruleTokens != null)
                {
                    myRule = ParseFeature(ruleTokens);
                    arrayFeaturesHelp.Add(myRule);
                    //zmieñ we wszystkich pozosta³uch
                    for (int a = 0; a < allRulesTokens.Count; a++)
                    {
                        ParserToken pt = (ParserToken)allRulesTokens[a];
                        if (pt.TokenString == myRule.Conclusion)
                        {
                            if (myRule.RuleType == ParserToken.RuleTypeNumeric)
                                pt.RuleType = ParserToken.RuleTypeNumeric;
                            if (myRule.RuleType == ParserToken.RuleTypeNumeric3D)
                                pt.RuleType = ParserToken.RuleTypeNumeric3D;
                        }
                    }
                }
            } while (ruleTokens != null);
            features = new ParserToken[arrayFeaturesHelp.Count];
            for (int a = 0; a < features.Length; a++)
                features[a] = (ParserToken)arrayFeaturesHelp[a];
            //PARSE RULES
            do
            {
                ruleTokens = FindAndRemoveRuleTokens(allRulesTokens);
                if (ruleTokens != null)
                {
                    myRule = ParseRule(ruleTokens);
                    arrayRuleHelp.Add(myRule);
                }
            } while (ruleTokens != null);

            //czy wszytskie symbole zosta³y zredukowane?
            //jeœli nie - b³¹d!
            if (allRulesTokens.Count > 0)
            {
                ParserToken pt = (ParserToken)allRulesTokens[0];
                throw new ParserException("Line: " + pt.PositionLn + ", column" + pt.PositionCol + " unmached " + pt.TokenString, pt.PositionCol, pt.PositionLn);
            }
            rules = new ParserToken[arrayRuleHelp.Count];
            for (int a = 0; a < rules.Length; a++)
                rules[a] = (ParserToken)arrayRuleHelp[a];

            CheckConclusions(rules, features);
            CheckDuplicatedNames(rules, rules);
            CheckDuplicatedNames(rules, features);
            CheckDuplicatedNames(features, features);
            /*ParserToken[] returnTable = new ParserToken[arrayRuleHelp.Count];
            for (int a = 0; a < returnTable.Length; a++)
                returnTable[a] = (ParserToken)arrayRuleHelp[a];
            //sprawdza, czy napisy, które mia³y byæ konkluzjami, rzeczywiœcie wystêpuj¹ w bazie regu³
            CheckConclusions(returnTable);
            return returnTable;
            */
            /*
            textWithoutComments = RemoveComments(textWithoutComments);
            textWithoutComments = textWithoutComments.ToLower();

            
            //Parse all rules
            do
            {
                //find text of the rule
                ruleText = FindRule(ref textWithoutComments);
                if (ruleText != null)
                {
                    myRule = ParseRule(ruleText);
                    arrayRuleHelp.Add(myRule);
                }
            } while (ruleText != null);
            ParserToken[] returnTable = new ParserToken[arrayRuleHelp.Count];
            for (int a = 0; a < returnTable.Length; a++)
                returnTable[a] = (ParserToken)arrayRuleHelp[a];
            //sprawdza, czy napisy, które mia³y byæ konkluzjami, rzeczywiœcie wystêpuj¹ w bazie regu³
            CheckConclusions(returnTable);
            return returnTable;*/
        }
    }
}
