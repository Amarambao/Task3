namespace Task3
{
    public static class ValidationHandler
    {
        private const string COLUMNNAME = "Error";

        public static void ParsingDicesError(string errorMessage)
        {
            TableGeneration.DisplayMessages(COLUMNNAME, [errorMessage]);
            Environment.Exit(0);
        }

        public static void InputError(string errorMessage)
        {
            TableGeneration.DisplayMessages(COLUMNNAME, [errorMessage]);
        }
    }
}
