using ConsoleTables;
using System.Collections.Generic;
using System.Text;

namespace Task3
{
    public static class TableGeneration
    {
        public static void DisplayMessages(string columnName, List<string> messages)
        {
            var table = new ConsoleTable([$"{columnName}"]);
            table.Options.EnableCount = false;
            table.MaxWidth = messages.MaxBy(x => x.Length)!.Length;

            foreach (var message in messages)
                table.AddRow(message);

            Console.WriteLine(table.ToString().TrimEnd());
        }

        public static void SelectionDisplay(int inputMaxRange, bool isPrintingDicesInfo, List<List<int>> dices, int? serverSelectedDice)
        {
            var table = new ConsoleTable(["SELECTION", "OPTIONS"]);
            table.Options.EnableCount = false;

            for (var i = 0; i < inputMaxRange; i++)
            {
                if (isPrintingDicesInfo && i == serverSelectedDice)
                    continue;

                var selectionOptions = $"{i} ";
                var selectionValues = $"{i}";

                if (isPrintingDicesInfo)
                    selectionValues = GetDiceInfo(dices, i);

                table.AddRow(selectionOptions, selectionValues);
            }
            table.AddRow("X", "exit");
            table.AddRow("?", "help");
            table.AddRow("Your selection", "?");

            Console.WriteLine(table.ToString().TrimEnd());
        }

        public static void WinProbabilityExplanation(List<List<int>> dices)
        {
            var table = new ConsoleTable();
            table.Options.EnableCount = false;

            var columns = new List<string>() { "Dice win probability" };
            
            for (int i = 0; i < dices.Count; i++)
                columns.Add(GetDiceInfo(dices, i));
            
            table.AddColumn(columns);

            for (int i = 0; i < dices.Count; i++)
            {
                var currentRow = new List<string>() { $"{GetDiceInfo(dices, i)}" };

                foreach (var dice in dices)
                    currentRow.Add($"{CountWins(dices[i], dice)} %");

                table.AddRow(currentRow.ToArray());
            }

            Console.WriteLine(table.ToString().TrimEnd());
        }

        public static string GetDiceInfo(List<List<int>> dices, int diceIndex)
        {
            var sb = new StringBuilder();
            foreach (var roll in dices[diceIndex])
            {
                sb.Append($"{roll}, ");
            }

            return sb.ToString().TrimEnd([' ', ',']);
        }

        private static int CountWins(List<int> a, List<int> b)
            => a.SelectMany(_ => b, (x, y) => x > y).Count(x => x);
    }
}
