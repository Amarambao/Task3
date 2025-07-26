namespace Task3
{
    public static class DiceParser
    {
        public static List<List<int>> ParseArgs(string[] args)
        {
            var allDices = new List<List<int>>();

            var errorMessages = new List<string>() { String.Empty, "Exiting..." };

            if (args.Length < 3)
                ValidationHandler.ParsingDicesError("Not enough dice sets.");

            for (int i = 0; i < args.Length; i++)
            {
                var diceRolls = args[i].Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (diceRolls.Length != 6)
                    ValidationHandler.ParsingDicesError("Wrong ammount of rolls on dice.");

                foreach (var roll in diceRolls)
                    if (!int.TryParse(roll, out var rollInt))
                        ValidationHandler.ParsingDicesError("Please use numbers only.");

                allDices.Add(diceRolls.Select(int.Parse).ToList());
            }

            return allDices;
        }
    }
}
