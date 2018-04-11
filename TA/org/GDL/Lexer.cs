using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;

namespace org.GDL
{
    /// <summary>
    /// Bardzo wolny lexer.... Ale działa ;-) 
    /// Athor: Tomasz Hachaj
    /// 2013.10.12
    /// </summary>
    public class Lexer
    {
        private static int FindPosition(String text, int startPosition, String[] array, ref int arrayIndex)
        {
            int position = int.MaxValue;
            //int posHelp = -1;
            for (int a = 0; a < array.Length; a++)
            {
                /*posHelp = text.IndexOf(array[a], startPosition);
                if (posHelp >= 0 && posHelp <= position)
                {
                    position = posHelp;
                    if (position == startPosition)
                    {
                        arrayIndex = a;
                        return position;
                    }
                }*/
                //if (text.StartsWith(array[a], startPosition)
                bool found = true;
                int b = 0;
                for (b = 0; b < array[a].Length && startPosition + b < text.Length && found; b++)
                    if (text[startPosition + b] != array[a][b])
                        found = false;
                if (found) b--;
                if (!(startPosition + b >= text.Length) && found)
                {
                    arrayIndex = a;
                    return startPosition;
                }
            }
            return position;
        }
        /////////////////////
        private static int IsRuleChar(String text, int startPosition, String[] array, ref int arrayIndex)
        {
            if ((text[startPosition] >= 'a' && text[startPosition] <= 'z') || (text[startPosition] >= '0' && text[startPosition] <= '9')
                    || text[startPosition] == '!' || text[startPosition] == '_')
                return startPosition;
            return int.MaxValue;
        }

        private static String FindCharString2(String text, int position, String[] array)
        {
            String ret = "";
            int len = 0;
            bool koniec = false;
            int foo = 0;
            while (!koniec)
            {
                int posHelp = IsRuleChar(text, position + len, array, ref foo);
                if (position + len != posHelp)
                    koniec = true;
                else
                {
                    len++;
                }
                if (position + len >= text.Length)
                {
                    koniec = true;
                }
            }
            ret = text.Substring(position, len);
            return ret;
        }

        private static int FindFirstChar(String text, int startPosition, String[] array, ref int arrayIndex)
        {
            bool koniec = false;
            int posHelp = startPosition;

            while (!koniec)
            {
                if ((text[posHelp] >= 'a' && text[posHelp] <= 'z') || (text[posHelp] >= '0' && text[posHelp] <= '9')
                    || text[posHelp] == '!' || text[posHelp] == '_')
                {
                    koniec = true;
                }
                if (!koniec)
                    posHelp++;
                if (posHelp >= text.Length)
                    koniec = true;
            }
            if (posHelp >= text.Length)
                return int.MaxValue;
            return posHelp;
        }



        private static String FindCharString(String text, int position, String[] array)
        {
            String ret = "";
            int len = 0;
            bool koniec = false;
            int foo = 0;
            while (!koniec)
            {
                int posHelp = FindFirstChar(text, position + len, array, ref foo);
                if (position + len != posHelp)
                    koniec = true;
                else
                {
                    len++;
                }
                if (position + len >= text.Length)
                {
                    koniec = true;
                }
            }
            ret = text.Substring(position, len);
            return ret;
        }

        private static int IsNumberNumber(String text, int startPosition, String[] array, ref int arrayIndex)
        {
            if ((text[startPosition] >= '0' && text[startPosition] <= '9') || text[startPosition] == '.')
                return startPosition;
            return int.MaxValue;
        }

        private static String FindNumberString2(String text, int position, String[] array)
        {
            String ret = "";
            int len = 0;
            bool koniec = false;
            int foo = 0;
            while (!koniec)
            {
                int posHelp = IsNumberNumber(text, position + len, array, ref foo);
                if (position + len != posHelp)
                    koniec = true;
                else
                {
                    len++;
                }
                if (position + len >= text.Length)
                {
                    koniec = true;
                }
            }
            ret = text.Substring(position, len);
            return ret;
        }

        private static int FindFirstNumber(String text, int startPosition, String[] array, ref int arrayIndex)
        {
            bool koniec = false;
            int posHelp = startPosition;

            while (!koniec)
            {
                if ((text[posHelp] >= '0' && text[posHelp] <= '9') || text[posHelp] == '.')
                {
                    koniec = true;
                }
                if (!koniec)
                    posHelp++;
                if (posHelp >= text.Length)
                    koniec = true;
            }
            if (posHelp >= text.Length)
                return int.MaxValue;
            return posHelp;
        }

        private static String FindNumberString(String text, int position, String[] array)
        {
            String ret = "";
            int len = 0;
            bool koniec = false;
            int foo = 0;
            while (!koniec)
            {
                int posHelp = FindFirstNumber(text, position + len, array, ref foo);
                if (position + len != posHelp)
                    koniec = true;
                else
                {
                    len++;
                }
                if (position + len >= text.Length)
                {
                    koniec = true;
                }
            }
            ret = text.Substring(position, len);
            return ret;
        }
        ///////////////////////////////////////

        private static int FindRulesPosition(String text, int startPosition, String[] array, ref int arrayIndex)
        {
            int position = int.MaxValue;
            int posHelp = -1;
            int foo = 0;
            for (int a = 0; a < array.Length; a++)
            {
                posHelp = text.IndexOf(array[a], startPosition);
                if (posHelp >= 0 && posHelp <= position)
                {
                    //sprawdza, czy nazwa nie jest częścią innej nazwy, np. ruledupa
                    if (array[a].Length + startPosition < text.Length)
                    {
                        if (FindPosition(text, array[a].Length + startPosition, ParserToken.EscapeChar, ref foo) == array[a].Length + startPosition)
                        {
                            position = posHelp;
                            arrayIndex = a;
                        }
                    }
                    else
                    {
                        position = posHelp;
                        arrayIndex = a;
                    }
                }
            }
            return position;
        }

        //dla opratorów relacyjnych
        private static int FindIndexPrefixes(String text, int startPosition, String[] array)
        {
            int index = -1;
            int posHelp = -1;
            int maxLength = 0;
            for (int a = 0; a < array.Length; a++)
            {
                posHelp = text.IndexOf(array[a], startPosition);
                //jeśli są dwa ciągi o podobnym początku np. < i <= znajduje dłuższy
                if (posHelp == startPosition && maxLength < array[a].Length)
                {
                    index = a;
                    maxLength = array[a].Length;
                }
            }
            return index;
        }
        /*
        private static int FindIndex(String text, int startPosition, String[] array)
        {
            int index = -1;
            int posHelp = -1;
            int maxLength = 0;
            for (int a = 0; a < array.Length; a++)
            {
                posHelp = text.IndexOf(array[a], startPosition);
                //jeśli są dwa ciągi o podobnym początku np. < i <= znajduje dłuższy
                if (posHelp == startPosition)
                {
                    return a;
                }
            }
            return index;
        }*/
        public static ArrayList GenerateTokens(String text)
        {
            ArrayList arrayList = new ArrayList();
            int position = 0;
            int ln = 1;
            int col = 1;
            String lowerText = text.ToLower();
            ParserToken pt = null;
            int[] arrayPos = new int[15];
            int[] innerArrayPos = new int[15];
            bool tokenFound = false;
            while (position < text.Length)
            {
                pt = null;
                
                tokenFound = false;
                for (int a = 0; a < arrayPos.Length; a++)
                    arrayPos[a] = int.MaxValue;
                arrayPos[0] = FindPosition(lowerText, position, ParserToken.EscapeChar, ref innerArrayPos[0]);
                if (arrayPos[0] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[1] = FindPosition(lowerText, position, ParserToken.Commentary, ref innerArrayPos[1]);//OK
                if (arrayPos[1] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[2] = FindPosition(lowerText, position, ParserToken.NumericValues, ref innerArrayPos[2]);//OK
                    //TEST
                    //arrayPos[2] = FindFirstNumber(lowerText, position, ParserToken.NumericValues, ref innerArrayPos[2]);//OK
                    //arrayPos[2] = IsNumberNumber(lowerText, position, ParserToken.NumericValues, ref innerArrayPos[2]);//OK
                    
                if (arrayPos[2] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[3] = FindPosition(lowerText, position, ParserToken.BracketCommaQuotation, ref innerArrayPos[3]);//OK
                if (arrayPos[3] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[4] = FindRulesPosition(lowerText, position, ParserToken.Rules, ref innerArrayPos[4]);//OK
                if (arrayPos[3] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[5] = FindPosition(lowerText, position, ParserToken.AritmeticOperators, ref innerArrayPos[5]);//OK
                if (arrayPos[5] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[6] = FindPosition(lowerText, position, ParserToken.RelationalOperators, ref innerArrayPos[6]);//OK
                /*if (arrayPos[6] != position)
                    arrayPos[7] = FindPosition(lowerText, position, ParserToken.BodyParts);//OK
                if (arrayPos[7] != position)
                    arrayPos[8] = FindPosition(lowerText, position, ParserToken.BodyParts3D);//OK*/
                if (arrayPos[6] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[7] = FindPosition(lowerText, position, ParserToken.LogicalOperators, ref innerArrayPos[7]);//OK
                if (arrayPos[7] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[8] = FindPosition(lowerText, position, ParserToken.NumberFunctions, ref innerArrayPos[8]);
                if (arrayPos[8] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[9] = FindPosition(lowerText, position, ParserToken.LogicalFunctions, ref innerArrayPos[9]);
                if (arrayPos[9] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[10] = FindPosition(lowerText, position, ParserToken.SequentialFunctions, ref innerArrayPos[10]);
                if (arrayPos[10] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[11] = FindPosition(lowerText, position, ParserToken.NumberFunctions3D, ref innerArrayPos[11]);
                if (arrayPos[11] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[12] = FindPosition(lowerText, position, ParserToken.BodyParts, ref innerArrayPos[12]);//OK
                if (arrayPos[12] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[13] = FindPosition(lowerText, position, ParserToken.BodyParts3D, ref innerArrayPos[13]);//OK
                if (arrayPos[13] == position) tokenFound = true;
                if (!tokenFound)
                    arrayPos[14] = FindPosition(lowerText, position, ParserToken.ConclusionsAndFeaturesNames, ref innerArrayPos[14]);
                    //TEST
                    //arrayPos[14] = FindFirstChar(lowerText, position, ParserToken.ConclusionsAndFeaturesNames, ref innerArrayPos[14]);
                    //arrayPos[14] = IsRuleChar(lowerText, position, ParserToken.ConclusionsAndFeaturesNames, ref innerArrayPos[14]);
                    
                int minimal = int.MaxValue;
                int arrayIndex = -1;
                //pomimo że ConclusionsAndFeaturesNames zawiera literki, które wchodzą w skład innych ciągów, 
                //ponieważ jest na końcu *nie powinno* psuć wykrywania ;-)
                for (int a = 0; a < arrayPos.Length; a++)
                    if (arrayPos[a] < minimal)
                    {
                        minimal = arrayPos[a];
                        arrayIndex = a;
                    }
                //nie znaleziono żadnego ciągu, który pasuje - coś jest nie tak ;-)
                if (arrayIndex == -1)
                    throw new ParserException("Unknow token " + text[position] + " in line: " + ln + ", column: " + col, col, ln);
                ////////////////////////////////////////////////////////////////////////////////
                //EscapeChar
                ////////////////////////////////////////////////////////////////////////////////
                if (arrayIndex == 0)
                {
                    //int index = FindIndex(lowerText, position, ParserToken.EscapeChar);
                    int index = innerArrayPos[0];
                    if (index < 0) throw new ParserException("Unknow token " + text[position] + " in line: " + ln + ", column: " + col + ". Contact THE BOSS ;-)", col, ln);
                    if (ParserToken.EscapeChar[index] == "\n")
                    {
                        ln++;
                        col = 1;
                    }
                    else
                    {
                        col++;
                    }
                    position += ParserToken.EscapeChar[index].Length;
                    
                }
                ////////////////////////////////////////////////////////////////////////////////
                //Commentary
                ////////////////////////////////////////////////////////////////////////////////
                else if (arrayIndex == 1)
                {
                    //int index = FindIndex(lowerText, position, ParserToken.Commentary);
                    int index = innerArrayPos[1];
                    //koniec bloku komentarza nie do pary
                    if (ParserToken.Commentary[index] == "*/")
                        throw new ParserException("Unmached token */ in line: " + ln + ", column: " + col, col, ln);
                    else if (ParserToken.Commentary[index] == "//")
                    {
                        bool end = false;
                        //"dojedź" do końca linii
                        while (!end)
                        {
                            //jeśli nie koniec linii przejdź dalej
                            if (lowerText[position] != '\n')
                            {
                                position++;
                                col++;
                            }
                            //jeśli koniec linii - znak +1, i koniec
                            else
                            {
                                position++;
                                col = 1;
                                ln++;
                                end = true;
                            }
                            if (position >= lowerText.Length) end = true;
                        }
                    }
                    else if (ParserToken.Commentary[index] == "/*")
                    {
                        int endingComm = lowerText.IndexOf("*/", position);
                        if (endingComm < 0) throw new ParserException("Unmached token /* in line: " + ln + ", column: " + col, col, ln);
                        //pętla się na pewno skończy, bo poprzednia linijka nie rzuciła wyjątku
                        while (endingComm != position)
                        {
                            position++;
                            col++;
                            if (lowerText[position] == '\n')
                            {
                                col = 1;
                                ln++;
                            }
                            endingComm = lowerText.IndexOf("*/", position);
                        }
                        position += 2;
                    }
                }
                ///////////////////////////////////////////////////////////////////
                //NumericValue
                ///////////////////////////////////////////////////////////////////
                else if (arrayIndex == 2)
                {
                    pt = new ParserToken();
                    pt.TokenString = FindSymbolsString(lowerText, position, ParserToken.NumericValues);
                    //TEST
                    //pt.TokenString = FindNumberString(lowerText, position, ParserToken.NumericValues);
                    //pt.TokenString = FindNumberString2(lowerText, position, ParserToken.NumericValues);
                    pt.TokenType = ParserToken.TokenTypeNumeric;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += pt.TokenString.Length;
                    position += pt.TokenString.Length;
                    try
                    {
                        double dd = double.Parse(pt.TokenString, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new ParserException("Line: " + ln + ", column: " + col + " error parsing numeric value " + pt.TokenString, col, ln);
                    }
                }
                /////////////////////////////////////////////////////
                //BracketCommaQuotation
                /////////////////////////////////////////////////////
                else if (arrayIndex == 3)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.BracketCommaQuotation);
                    int index = innerArrayPos[3];
                    if (ParserToken.BracketCommaQuotation[index] == "(")
                    {
                        pt.TokenString = "(";
                        pt.TokenType = ParserToken.TokenTypeOpenBracket;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        col++;
                        position++;
                    }
                    else if (ParserToken.BracketCommaQuotation[index] == ")")
                    {
                        pt.TokenString = ")";
                        pt.TokenType = ParserToken.TokenTypeClosedBracket;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        col++;
                        position++;
                    } 
                    else if (ParserToken.BracketCommaQuotation[index] == "[")
                    {
                        pt.TokenString = "[";
                        pt.TokenType = ParserToken.TokenTypeOpenSquareBracket;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        col++;
                        position++;
                    }
                    else if (ParserToken.BracketCommaQuotation[index] == "]")
                    {
                        pt.TokenString = "]";
                        pt.TokenType = ParserToken.TokenTypeClosedSquareBracket;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        col++;
                        position++;
                    }
                    else if (ParserToken.BracketCommaQuotation[index] == ",")
                    {
                        pt.TokenString = ",";
                        pt.TokenType = ParserToken.TokenTypeComma;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        col++;
                        position++;
                    }
                    else if (ParserToken.BracketCommaQuotation[index] == "\"")
                    {
                        int indexQM2 = text.IndexOf("\"", position + 1);
                        if (indexQM2 == -1)
                            throw new ParserException("Line: " + ln + " column: " + col + " unmached \" in gesture sequence", col, ln);
                        String substringQM = lowerText.Substring(position, indexQM2 - position + 1);
                        if (substringQM.Contains('\n'))
                            throw new ParserException("Line: " + ln + " column: " + col + " gesture sequence cannot be multiline", col, ln);
                        pt.Sequence = ParseSequence(substringQM, position, ln, col, out pt.TokenType);
                        pt.TokenString = substringQM;
                        //pt.TokenType = ParserToken.TokenTypeSequence;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        position = indexQM2 + 1;
                        col += indexQM2 + 1;
                    }
                    else
                    {
                        throw new ParserException("Line: " + ln + ", column: " + col + " error parsing BracketCommaQuotation - CONTACT THE MASTER ;-)", col, ln);
                    }
                }
                ///////////////////////////////////////////////////////////////////
                //Rules
                ///////////////////////////////////////////////////////////////////
                else if (arrayIndex == 4)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.Rules);
                    //int index = innerArrayPos[4];
                    int index = innerArrayPos[4];
                    if (ParserToken.Rules[index] == "rule")
                    {
                        pt.TokenString = ParserToken.Rules[index];
                        pt.TokenType = ParserToken.TokenTypeRuleKeyword;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        col += ParserToken.Rules[index].Length;
                        position += ParserToken.Rules[index].Length;
                    }
                    else if (ParserToken.Rules[index] == "then")
                    {
                        pt.TokenString = ParserToken.Rules[index];
                        pt.TokenType = ParserToken.TokenTypeThenKeyword;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        col += ParserToken.Rules[index].Length;
                        position += ParserToken.Rules[index].Length;
                    }
                    else if (ParserToken.Rules[index] == "feature")
                    {
                        pt.TokenString = ParserToken.Rules[index];
                        pt.TokenType = ParserToken.TokenTypeFeatureKeyword;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        col += ParserToken.Rules[index].Length;
                        position += ParserToken.Rules[index].Length;
                    }
                    else if (ParserToken.Rules[index] == "as")
                    {
                        pt.TokenString = ParserToken.Rules[index];
                        pt.TokenType = ParserToken.TokenTypeAsKeyword;
                        pt.PositionCol = col;
                        pt.PositionLn = ln;
                        col += ParserToken.Rules[index].Length;
                        position += ParserToken.Rules[index].Length;
                    }
                    else
                    {
                        throw new ParserException("Line: " + ln + ", column: " + col + " error parsing Rules - CONTACT THE MASTER ;-)", col, ln);
                    }
                }
                ////////////////////////////////////////////////////////
                //AritmeticOperators
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 5)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.AritmeticOperators);
                    int index = innerArrayPos[5];
                    
                    pt.TokenString = ParserToken.AritmeticOperators[index];
                    pt.TokenType = ParserToken.TokenTypeNumericOperator;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += ParserToken.AritmeticOperators[index].Length;
                    position += ParserToken.AritmeticOperators[index].Length;
                }
                ////////////////////////////////////////////////////////
                //RelationalOperators
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 6)
                {
                    pt = new ParserToken();
                    int index = FindIndexPrefixes(lowerText, position, ParserToken.RelationalOperators);

                    pt.TokenString = ParserToken.RelationalOperators[index];
                    pt.TokenType = ParserToken.TokenTypeRelationalOperator;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += ParserToken.RelationalOperators[index].Length;
                    position += ParserToken.RelationalOperators[index].Length;
                }
                ////////////////////////////////////////////////////////
                //BodyParts
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 12)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.BodyParts);
                    int index = innerArrayPos[12];

                    pt.TokenString = ParserToken.BodyParts[index];
                    pt.TokenType = ParserToken.TokenTypeBodyPart;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += ParserToken.BodyParts[index].Length;
                    position += ParserToken.BodyParts[index].Length;
                }
                ////////////////////////////////////////////////////////
                //BodyParts3D
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 13)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.BodyParts3D);
                    int index = innerArrayPos[13];

                    pt.TokenString = ParserToken.BodyParts3D[index];
                    pt.TokenType = ParserToken.TokenTypeBodyPart3D;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += ParserToken.BodyParts3D[index].Length;
                    position += ParserToken.BodyParts3D[index].Length;
                }
                ////////////////////////////////////////////////////////
                //LogicalOperators
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 7)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.LogicalOperators);
                    int index = innerArrayPos[7];

                    pt.TokenString = ParserToken.LogicalOperators[index];
                    pt.TokenType = ParserToken.TokenTypeLogicalOperator;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += ParserToken.LogicalOperators[index].Length;
                    position += ParserToken.LogicalOperators[index].Length;
                }
                ////////////////////////////////////////////////////////
                //NumberFunctions
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 8)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.NumberFunctions);
                    int index = innerArrayPos[8];

                    pt.TokenString = ParserToken.NumberFunctions[index];
                    pt.TokenType = ParserToken.TokenTypeNumericFunction;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += ParserToken.NumberFunctions[index].Length;
                    position += ParserToken.NumberFunctions[index].Length;
                }
                ////////////////////////////////////////////////////////
                //LogicalFunctions
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 9)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.LogicalFunctions);
                    int index = innerArrayPos[9];

                    pt.TokenString = ParserToken.LogicalFunctions[index];
                    pt.TokenType = ParserToken.TokenTypeLogicalFunction;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += ParserToken.LogicalFunctions[index].Length;
                    position += ParserToken.LogicalFunctions[index].Length;
                }
                ////////////////////////////////////////////////////////
                //SequentialFunctions
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 10)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.SequentialFunctions);
                    int index = innerArrayPos[10];

                    pt.TokenString = ParserToken.SequentialFunctions[index];
                    pt.TokenType = ParserToken.TokenTypeSequentialFunction;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += ParserToken.SequentialFunctions[index].Length;
                    position += ParserToken.SequentialFunctions[index].Length;
                }
                ////////////////////////////////////////////////////////
                //NumberFunctions3D
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 11)
                {
                    pt = new ParserToken();
                    //int index = FindIndex(lowerText, position, ParserToken.NumberFunctions3D);
                    int index = innerArrayPos[11];

                    pt.TokenString = ParserToken.NumberFunctions3D[index];
                    pt.TokenType = ParserToken.TokenTypeNumericFunction3D;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += ParserToken.NumberFunctions3D[index].Length;
                    position += ParserToken.NumberFunctions3D[index].Length;
                }
                ////////////////////////////////////////////////////////
                //ConclusionsAndFeaturesNames
                ////////////////////////////////////////////////////////
                else if (arrayIndex == 14)
                {
                    pt = new ParserToken();
                    pt.TokenString = FindSymbolsString(lowerText, position, ParserToken.ConclusionsAndFeaturesNames);
                    //TEST
                    //pt.TokenString = FindCharString(lowerText, position, ParserToken.ConclusionsAndFeaturesNames);
                    //pt.TokenString = FindCharString2(lowerText, position, ParserToken.ConclusionsAndFeaturesNames);
                    pt.TokenType = ParserToken.TokenTypeConclusion;
                    pt.PositionCol = col;
                    pt.PositionLn = ln;
                    col += pt.TokenString.Length;
                    position += pt.TokenString.Length;
                }
                else
                {
                    throw new ParserException("Line: " + ln + ", column: " + col + " error parsing " + text[position] + " chracter not recognized.", col, ln);
                }
                if (pt != null)
                    arrayList.Add(pt);
            }
            for (int a = 0; a < arrayList.Count; a++)
            {
                ((ParserToken)arrayList[a]).TokenTypeMemory = ((ParserToken)arrayList[a]).TokenType;
            }
            return arrayList;
        }

        private static String FindSymbolsString(String text, int position, String[]array)
        {
            String ret = "";
            int len = 0;
            bool koniec = false;
            int foo = 0;
            while (!koniec)
            {
                int posHelp = FindPosition(text, position + len, array, ref foo);
                if (position + len != posHelp)
                    koniec = true;
                else
                {
                    len++;
                }
                if (position + len >= text.Length)
                {
                    koniec = true;
                }
            }
            ret = text.Substring(position, len);
            return ret;
        }

        //do poprawy!!
        private static ArrayList ParseSequence(String text, int tokenPosition, int ln, int col, out int tokenType)
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
            bool hasFeatures = false;
            //ArrayList ConclusionsHelp = new ArrayList();
            //ArrayList PrefixesHelp = new ArrayList();
            do
            {
                bracketStart = text.IndexOf("[", actualPosition);
                bracketStop = text.IndexOf("]", actualPosition);
                if (bracketStart > bracketStop)
                {
                    throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of [", col, ln);
                }
                st = null;
                if (bracketStart >= 0)
                {
                    if (bracketStop < 0)
                        throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of terminating ]", col, ln);
                    textPart = text.Substring(bracketStart + 1, bracketStop - 1 - bracketStart);
                    actualPosition = bracketStart + 1;
                    st = new SequenceToken();
                    String[] splittingResult = textPart.Split(commaSeparators);
                    if (splittingResult.Length < 0)
                        throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of ,",col, ln);

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
                        st.TimeConstraintSeconds = Double.Parse(doubleHelp, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of time constraint value: " + doubleHelp, col, ln);
                    }
                    al.Add(st);
                    actualPosition = bracketStop + 1;
                }
                //Features
                if (text.IndexOf("{", actualPosition) >= 0)
                {
                    bool skip = false;
                    //check, if it is a feature to a given conclusions, ie. [][]{}[]
                    if (text.IndexOf("[", actualPosition) > 0)
                    {
                        if (text.IndexOf("{", actualPosition) > text.IndexOf("[", actualPosition))
                            skip = true;
                    }
                    if (!skip)
                    {
                        hasFeatures = true;
                        bracketStart = text.IndexOf("{", actualPosition);
                        bracketStop = text.IndexOf("}", actualPosition);
                        if (bracketStart > bracketStop)
                        {
                            throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of {", col, ln);
                        }
                        if (bracketStart >= 0)
                        {
                            if (bracketStop < 0)
                                throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of terminating }", col, ln);
                            textPart = text.Substring(bracketStart + 1, bracketStop - 1 - bracketStart);
                            actualPosition = bracketStart + 1;
                            String[] splittingResult = textPart.Split(commaSeparators);

                            if (splittingResult.Length < 0)
                                throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of features,", col, ln);

                            if (splittingResult.Length != st.Conclusions.Length)
                                throw new ParserException("Line: " + ln + " column: " + col + " the number of features names has to agree with number of conclusions ,", col, ln);

                            st.Features = new String[splittingResult.Length];
                            for (int a = 0; a < splittingResult.Length; a++)
                            {
                                st.Features[a] = splittingResult[a];
                            }
                            actualPosition = bracketStop + 1;
                        }
                    }
                }
                //Features with values
                if (text.IndexOf("{", actualPosition) >= 0)
                {
                    bool skip = false;
                    //check, if it is a feature to a given conclusions, ie. [][]{}[]
                    if (text.IndexOf("[", actualPosition) > 0)
                    {
                        if (text.IndexOf("{", actualPosition) > text.IndexOf("[", actualPosition))
                            skip = true;
                    }
                    if (!skip)
                    {
                        hasFeatures = true;
                        bracketStart = text.IndexOf("{", actualPosition);
                        bracketStop = text.IndexOf("}", actualPosition);
                        if (bracketStart > bracketStop)
                        {
                            throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of {", col, ln);
                        }
                        if (bracketStart >= 0)
                        {
                            if (bracketStop < 0)
                                throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of terminating }", col, ln);
                            textPart = text.Substring(bracketStart + 1, bracketStop - 1 - bracketStart);
                            actualPosition = bracketStart + 1;
                            String[] splittingResult = textPart.Split(commaSeparators);

                            if (splittingResult.Length < 0)
                                throw new ParserException("Line: " + ln + " column: " + col + " sequence expression lack of features,", col, ln);

                            if (splittingResult.Length != st.Conclusions.Length)
                                throw new ParserException("Line: " + ln + " column: " + col + " the number of features names has to agree with number of conclusions ,", col, ln);

                            st.FeaturesToTakesValuesFrom = new String[splittingResult.Length];
                            for (int a = 0; a < splittingResult.Length; a++)
                            {
                                st.FeaturesToTakesValuesFrom[a] = splittingResult[a];
                            }
                            actualPosition = bracketStop + 1;
                        }
                    }
                }

            }
            while (bracketStart >= 0);
            if (al.Count == 0)
                throw new ParserException("Line: " + tokenPosition + ", sequence expression lack of proper sequences: " + text, col, ln);
            helpIndex = text.IndexOf("\"", actualPosition);
            if (helpIndex == -1)
                throw new ParserException("Line: " + tokenPosition + ", sequence expression lack of terminating \"", col, ln);
            if (hasFeatures)
                tokenType = ParserToken.TokenTypeSequenceWithFeatures;
            else
                tokenType = ParserToken.TokenTypeSequence;
            return al;

        }
    }
}
