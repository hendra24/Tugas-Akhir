using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace org.GDL
{
    public class ParserToken
    {
        public int Position = 0;
        public int PositionCol = 0;
        public int PositionLn = 0;
        public int TokenType = 0;
        public String TokenString = "";
        public String Conclusion = "";
        public ArrayList Sequence = null;

        public ArrayList Children = new ArrayList();
        public int RuleType = 0;
        public int TokenTypeMemory = 0;
        //predefined constants
        public static int RuleTypeUndefined = 0;
        public static int RuleTypeNumeric = 1;
        public static int RuleTypeLogical = 2;
        public static int RuleTypeNumeric3D = 3;

        public static int TokenTypeUndefined = 0;
        public static int TokenTypeNumeric = 1;
        public static int TokenTypeLogical = 2;
        public static int TokenTypeLogicalOperator = 3;
        public static int TokenTypeNumericOperator = 4;
        public static int TokenTypeRelationalOperator = 5;
        public static int TokenTypeOpenBracket = 6;
        public static int TokenTypeClosedBracket = 7;
        public static int TokenTypeLogicalFunction = 8;
        public static int TokenTypeNumericFunction = 9;
        public static int TokenTypeSequence = 10;
        public static int TokenTypeBodyPart = 11;
        public static int TokenTypeConclusion = 12;
        public static int TokenTypeSequentialFunction = 13;
        public static int TokenTypeClosedSquareBracket = 14;
        public static int TokenTypeOpenSquareBracket = 15;
        public static int TokenTypeComma = 16;
        public static int TokenTypeBodyPart3D = 17;
        public static int TokenTypeNumericFunction3D = 18;
        public static int TokenTypeNumericOperator3D = 19;

        public static int TokenTypeRuleKeyword = 20;
        public static int TokenTypeThenKeyword = 21;
        public static int TokenTypeFeatureKeyword = 22;
        public static int TokenTypeAsKeyword = 23;

        public static int TokenTypeSequenceWithFeatures = 24;

        public static String[] EscapeChar = { " ", "\r", "\n", "\t"};
        public static String[] Commentary = { "//", "/*", "*/" };
        public static String[] NumericValues = {".", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public static String[] BracketCommaQuotation = { "(", ")", "[", "]",",", "\"" };
        public static String[] Rules = {"rule", "then", "feature", "as"};

        public static String[] AritmeticOperators = { "+", "-", "*", "/", "%", "^" };
        public static String[] RelationalOperators = { "<", ">", "=", "<=", ">=", "!=" };
        
        /*
         Wartoœci z OpenNI.SkeletonJoint
                Invalid = 0,
                Head = 1,
                Neck = 2,
                Torso = 3,
                Waist = 4,
                LeftCollar = 5,
                LeftShoulder = 6,
                LeftElbow = 7,
                LeftWrist = 8,
                LeftHand = 9,
                LeftFingertip = 10,
                RightCollar = 11,
                RightShoulder = 12,
                RightElbow = 13,
                RightWrist = 14,
                RightHand = 15,
                RightFingertip = 16,
                LeftHip = 17,
                LeftKnee = 18,
                LeftAnkle = 19,
                LeftFoot = 20,
                RightHip = 21,
                RightKnee = 22,
                RightAnkle = 23,
                RightFoot = 24,
         */
        public static String[] BodyParts; //inicjowane w kodzie
        public static String[] BodyParts3D; //inicjowane w kodzie
        public static String[] ConclusionsAndFeaturesNames = 
        { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", 
            "f", "g", "h", "j", "k", "l", "z", "x", "c", "v", "b", "n", "m", "!", "_",
            "0" ,"1" ,"2" ,"3" ,"4" ,"5" ,"6" ,"7" , "8", "9"}; //inicjowane w kodzie

        public static String[] LogicalOperators = { "&", "|" };
        public static String[] NumberFunctions = { "sqrt(", "abs(", "sgn(", "sgnfuzzy(" };
        public static String[] LogicalFunctions = { "not(" };
        public static String[] SequentialFunctions = { "sequenceexists(", "rulepersists(", "sequencescore(" };
        public static String[] NumberFunctions3D = { "distance(", "angle(", "checkcollinearity(", "checkcircularity(", "cross(" };
        //NALE¯A£O BY WPORWADZIÆ TEN TYP FUNKCJI I WYWALIÆ PONI¯SZ¥ Z POWY¯SZEJ LISTY
        //public static String[] Number3DFunctions3D = {"CheckCollinearity(" };
    }
}
