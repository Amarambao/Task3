using ConsoleTables;
using System.Text;

namespace Task3
{
    public static class TableGeneration
    {
        public static void WinProbabilityExplanation(List<List<int>> dices)
        {
            var table = new ConsoleTable();
            table.Options.EnableCount = false;

            var columns = new List<string>() { "Dice win probability" };
            
            for (int i = 0; i < dices.Count; i++)
                columns.Add(GameLogic.GetDiceInfo(dices, i));
            
            table.AddColumn(columns);

            for (int i = 0; i < dices.Count; i++)
            {
                var currentRow = new List<string>() { $"{GameLogic.GetDiceInfo(dices, i)}" };

                foreach (var dice in dices)
                    currentRow.Add($"{CountWins(dices[i], dice)} %");

                table.AddRow(currentRow.ToArray());
            }

            Console.WriteLine(table.ToString().TrimEnd());
        }

        private static int CountWins(List<int> a, List<int> b)
            => a.SelectMany(_ => b, (x, y) => x > y).Count(x => x);
    }
}
