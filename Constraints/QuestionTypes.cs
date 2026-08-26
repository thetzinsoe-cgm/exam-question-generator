namespace ExamSystem.Constraints
{
    public static class QuestionTypes
    {
        public const short MultipleChoice = 1;
        public const short TrueFalse = 2;
        public const short ShortAnswer = 3;
        public const short Essay = 4;
        public const short MathExpression = 5;
        public const short BIO = 6;
        public const short ECO_Calculation = 7;
        public const short FillInTheBlank = 8;

        public const string MultipleChoiceName = "Multiple Choice";
        public const string TrueFalseName = "True / False";
        public const string ShortAnswerName = "Short Answer";
        public const string EssayName = "Essay";
        public const string MathExpressionName = "Math Expression (KaTeX)";
        public const string BIOName = "BIO (with Image)";
        public const string ECO_CalculationName = "ECO Calculation (Table)";
        public const string FillInTheBlankName = "Fill in the Blank";

        public static string GetTypeName(this short type)
        {
            return type switch
            {
                MultipleChoice => MultipleChoiceName,
                TrueFalse => TrueFalseName,
                ShortAnswer => ShortAnswerName,
                Essay => EssayName,
                MathExpression => MathExpressionName,
                BIO => BIOName,
                ECO_Calculation => ECO_CalculationName,
                FillInTheBlank => FillInTheBlankName,
                _ => MultipleChoiceName
            };
        }

        public static short[] AllTypes()
        {
            return new short[] { MultipleChoice, TrueFalse, ShortAnswer, Essay, MathExpression, BIO, ECO_Calculation, FillInTheBlank };
        }
    }
}
