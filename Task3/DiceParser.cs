namespace Task3
{
    public static class DiceParser
    {
        public static List<List<int>> ParseArgs(string[] args)
        {
            var allDices = new List<List<int>>();

            if (args.Length < 3)
                ParsingDicesError("Not enough dice sets.\nExiting");

            for (int i = 0; i < args.Length; i++)
            {
                var diceRolls = args[i].Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (diceRolls.Length != 6)
                    ParsingDicesError("Wrong ammount of rolls on dice.\nExiting");

                foreach (var roll in diceRolls)
                    if (!int.TryParse(roll, out var rollInt))
                        ParsingDicesError("Please use numbers only.\nExiting");

                allDices.Add(diceRolls.Select(int.Parse).ToList());
            }

            return allDices;
        }

        private static void ParsingDicesError(string errorMessage)
        {
            Console.WriteLine($"ERROR:\n{errorMessage}");
            Environment.Exit(0);
        }
    }
}
