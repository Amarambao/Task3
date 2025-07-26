using ConsoleTables;
using System.Security.Cryptography;
using System.Text;

namespace Task3
{
    public class GameLogic
    {
        public List<List<int>> Dices { get; set; }

        public int PlayerSelectedDice { get; set; }
        public int? ServerSelectedDice { get; set; }

        public int PlayerDiceRollResult { get; set; }
        public int ServerDiceRollResult { get; set; }

        public GameLogic(List<List<int>> dices)
        {
            Dices = dices;
        }

        public void StartGame()
        {
            var serverNumber = RandomNumberGenerator.GetInt32(2);
            var key = KeyGeneration.Generate();
            var serverHmac = Hmac.Calculate($"{serverNumber}", key);

            Console.WriteLine(
                $"Let's determine who makes the first move." +
                $"\nI selected a random value in the range 0..1." +
                $"\n(HMAC={Convert.ToHexString(serverHmac)})." +
                $"\nTry to guess my selection.");

            var playerNumber = PlayerInput(() => Console.WriteLine("Deciding who pick dice first.\n50% chance to guess"), 2, false);

            var playerHmac = Hmac.Calculate($"{playerNumber}", key);

            GameProcess(serverNumber, serverHmac, playerHmac, key);
        }

        //I dont kno whow to make this place better
        private void GameProcess(int serverNumber, byte[] serverHmac, byte[] playerHmac, byte[] key)
        {
            Console.WriteLine($"My selection: {serverNumber} (KEY={Convert.ToHexString(key)}).");

            if (CryptographicOperations.FixedTimeEquals(serverHmac, playerHmac))
            {
                Console.WriteLine("YOU WON!!!\nPlease select dice.");

                PlayerSelectedDice = SelectDice();

                ServerSelectedDice = RandomNumberGenerator.GetInt32(Dices.Count);
                while (PlayerSelectedDice == ServerSelectedDice)
                    ServerSelectedDice = RandomNumberGenerator.GetInt32(Dices.Count);

                Console.WriteLine($"I make the second move and choose the [{GetDiceInfo(Dices, ServerSelectedDice.Value)}] dice.");
            }
            else
            {
                ServerSelectedDice = RandomNumberGenerator.GetInt32(Dices.Count);
                Console.WriteLine($"I make the first move and choose the [{GetDiceInfo(Dices, ServerSelectedDice.Value)}] dice.");

                PlayerSelectedDice = SelectDice();
            }

            SelectDiceRoll(false, ServerSelectedDice.Value);
            SelectDiceRoll(true, PlayerSelectedDice);

            Console.WriteLine($"THE END\n" + GameResultInfo());
        }

        private int SelectDice()
        {
            Console.WriteLine("Choose your dice:");

            return PlayerInput(() => TableGeneration.WinProbabilityExplanation(Dices), Dices.Count, true);
        }

        private void SelectDiceRoll(bool isPlayerRoll, int diceIndex)
        {
            var whichRoll = isPlayerRoll ? "your" : "my";

            var key = KeyGeneration.Generate();
            var serverNumber = RandomNumberGenerator.GetInt32(2);
            var serverHmac = Hmac.Calculate($"{serverNumber}", key);

            Console.WriteLine(
                $"Lets roll {whichRoll.ToUpper()} dice [{GetDiceInfo(Dices, diceIndex)}]\n" +
                "I selected a random value in the range 0..5\n" +
                $"(HMAC={Convert.ToHexString(serverHmac)}).\n" +
                "Add your number (modulo 6).");

            var playerNumber = PlayerInput(() => TableGeneration.WinProbabilityExplanation(Dices), 6, false);

            var rollResult = FairNumberGeneration.DiceRollResult([serverNumber, playerNumber]);

            Console.WriteLine(
                $"My number is {serverNumber} (KEY={Convert.ToHexString(key)})." +
                $"The fair number generation result is {serverNumber} + {playerNumber} = {rollResult} (mod 6).");

            if (isPlayerRoll)
            {
                PlayerDiceRollResult = Dices[PlayerSelectedDice][rollResult];
                Console.WriteLine($"{whichRoll.ToUpper()} roll result is {PlayerDiceRollResult}.");
            }
            else
            {
                ServerDiceRollResult = Dices[ServerSelectedDice!.Value][rollResult];
                Console.WriteLine($"{whichRoll.ToUpper()} roll result is {ServerDiceRollResult}.");
            }
        }

        private string GameResultInfo()
        {
            if (PlayerDiceRollResult > ServerDiceRollResult)
                return $"YOU WIN ({PlayerDiceRollResult} > {ServerDiceRollResult})!!!";
            else if (PlayerDiceRollResult < ServerDiceRollResult)
                return $"YOU LOST ({PlayerDiceRollResult} < {ServerDiceRollResult})!!!";
            else
                return $"Draw ({PlayerDiceRollResult} = {ServerDiceRollResult}).";
        }

        public static string GetDiceInfo(List<List<int>> dices, int diceIndex)
        {
            var sb = new StringBuilder();
            foreach (var roll in dices[diceIndex])
                sb.Append($"{roll}, ");

            return sb.ToString().TrimEnd([' ', ',']);
        }

        private int PlayerInput(Action helpGeneration, int inputMaxRange, bool isPrintingDicesInfo)
        {
            var isReadingConsole = true;
            var playerInput = string.Empty;

            while (isReadingConsole)
            {
                SelectionDisplay(inputMaxRange, isPrintingDicesInfo);

                playerInput = Console.ReadLine().ToLower().Trim();
                Console.WriteLine();

                if (int.TryParse(playerInput, out int selection) && selection >= 0 && selection < inputMaxRange)
                    isReadingConsole = false;
                else if (playerInput == "?")
                    helpGeneration();
                else if (playerInput == "x")
                    Environment.Exit(0);
                else
                    Console.WriteLine("Wrong input, try again\n");
            }
            return Convert.ToInt32(playerInput);
        }

        private void SelectionDisplay(int inputMaxRange, bool isPrintingDicesInfo)
        {
            for (var i = 0; i < inputMaxRange; i++)
            {
                if (isPrintingDicesInfo && i == ServerSelectedDice)
                    continue;

                var selectionOptions = $"{i} - ";
                var selectionValues = $"{i}";

                if (isPrintingDicesInfo)
                    selectionValues = GetDiceInfo(Dices, i);

                Console.WriteLine($"{selectionOptions}{selectionValues}");
            }
            Console.WriteLine("? - help");
            Console.WriteLine("X - exit"); 
            Console.Write("Your selection?: ");
        }
    }
}

