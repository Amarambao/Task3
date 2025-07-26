using System.Security.Cryptography;
using System.Text;

namespace Task3
{
    public class GameLogic
    {
        public List<List<int>> Dices { get; set; }

        public CharacterInfo Player { get; set; }
        public CharacterInfo Server { get; set; }

        public GameLogic(List<List<int>> dices)
        {
            Dices = dices;
            Player = new CharacterInfo("player");
            Server = new CharacterInfo("server");
        }

        public void StartGame()
        {
            var serverNumber = RandomNumberGenerator.GetInt32(2);
            var key = KeyGeneration.Generate();
            var serverHmac = Hmac.Calculate($"{serverNumber}", key);

            Console.WriteLine(
                $"Let's determine who makes the first move.\n" +
                $"I selected a random value in the range 0..1.\n" +
                $"(HMAC={Convert.ToHexString(serverHmac)}).\n" +
                $"Try to guess my selection.\n");

            var playerNumber = PlayerInput(() => Console.WriteLine("Deciding who pick dice first.\n50% chance to guess\n"), 2, false);

            var playerHmac = Hmac.Calculate($"{playerNumber}", key);

            GameProcess(serverNumber, serverHmac, playerHmac, key);
        }

        //I dont kno whow to make this place better
        private void GameProcess(int serverNumber, byte[] serverHmac, byte[] playerHmac, byte[] key)
        {
            Console.WriteLine($"My selection: {serverNumber} (KEY={Convert.ToHexString(key)}).");

            if (CryptographicOperations.FixedTimeEquals(serverHmac, playerHmac))
            {
                Console.WriteLine("YOU WON!!!\nPlease select dice.\n");

                Player.SelectedDice = SelectDice();

                while (Server.SelectedDice is null || Player.SelectedDice == Server.SelectedDice)
                    Server.SelectedDice = RandomNumberGenerator.GetInt32(Dices.Count);

                Console.WriteLine($"I make the second move and choose the [{GetDiceInfo(Dices, Server.SelectedDice!.Value)}] dice.");
            }
            else
            {
                Server.SelectedDice = RandomNumberGenerator.GetInt32(Dices.Count);
                Console.WriteLine($"I make the first move and choose the [{GetDiceInfo(Dices, Server.SelectedDice.Value)}] dice.");

                Player.SelectedDice = SelectDice();
            }

            SelectDiceRoll(Server);
            SelectDiceRoll(Player);

            GameResultInfo();
        }

        private int SelectDice()
        {
            Console.WriteLine("Choose your dice:");

            return PlayerInput(() => TableGeneration.WinProbabilityExplanation(Dices), Dices.Count, true);
        }

        private void SelectDiceRoll(CharacterInfo character)
        {
            var key = KeyGeneration.Generate();
            var serverNumber = RandomNumberGenerator.GetInt32(2);
            var serverHmac = Hmac.Calculate($"{serverNumber}", key);

            Console.WriteLine(
                $"Lets roll {character.Name.ToUpper()} dice [{GetDiceInfo(Dices, character.SelectedDice!.Value)}]\n" +
                "I selected a random value in the range 0..5\n" +
                $"(HMAC={Convert.ToHexString(serverHmac)}).\n" +
                "Add your number (modulo 6).\n");

            var playerNumber = PlayerInput(() => TableGeneration.WinProbabilityExplanation(Dices), 6, false);

            var rollResult = FairNumberGeneration.DiceRollResult([serverNumber, playerNumber]);

            character.DiceRollResult = Dices[character.SelectedDice.Value][rollResult];

            Console.WriteLine(
                $"My number is {serverNumber}\n(KEY={Convert.ToHexString(key)}).\n" +
                $"The fair number generation result is {serverNumber} + {playerNumber} = {rollResult} (mod 6).\n" +
                $"{character.Name} roll result is {character.DiceRollResult}.\n");
        }

        private void GameResultInfo()
        {
            Console.WriteLine($"THE END\n");
            var characters = new List<CharacterInfo>() { Player, Server };
            var maxRoll = characters.Max(z => z.DiceRollResult!.Value);

            if (Player.DiceRollResult == Server.DiceRollResult)
            {
                Console.WriteLine($"Draw ({Player.DiceRollResult} = {Server.DiceRollResult}).");
            }
            else
            {
                var winner = characters.FirstOrDefault(z => z.DiceRollResult == maxRoll)!;
                var loser = characters.FirstOrDefault(z => z.DiceRollResult != maxRoll)!;
                Console.WriteLine($"{winner.Name} WON!!! :)\n{loser.Name} lost :(\n({winner.DiceRollResult} > {loser.DiceRollResult})");
            }
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
                if (isPrintingDicesInfo && i == Server.SelectedDice)
                    continue;

                var selectionOptions = $"{i} - ";
                var selectionValues = $"{i}";

                if (isPrintingDicesInfo)
                    selectionValues = GetDiceInfo(Dices, i);

                Console.WriteLine($"{selectionOptions}{selectionValues}");
            }
            Console.Write(
                "? - help\n" +
                "X - exit\n" +
                "Your selection: ");
        }
    }
}

