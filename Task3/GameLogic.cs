using System.Security.Cryptography;

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

            var messages = new List<string>()
            {
                "Let's determine who makes the first move.",
                "I selected a random value in the range 0..1.",
                $"(HMAC={Convert.ToHexString(serverHmac)}).",
                "Try to guess my selection.",
            };

            TableGeneration.DisplayMessages("START GAME", messages);

            var playerNumber = PlayerInput(() => TableGeneration.DisplayMessages("INFO", ["Deciding who pick dice first.", "50% chance to guess"]), 2, false);

            var playerHmac = Hmac.Calculate($"{playerNumber}", key);

            GameProcess(serverNumber, serverHmac, playerHmac, key);
        }

        //I dont kno whow to make this place better
        private void GameProcess(int serverNumber, byte[] serverHmac, byte[] playerHmac, byte[] key)
        {
            var messages = new List<string>() { $"My selection: {serverNumber} (KEY={Convert.ToHexString(key)})." };

            TableGeneration.DisplayMessages("FIRST TURN", messages);

            if (CryptographicOperations.FixedTimeEquals(serverHmac, playerHmac))
            {
                TableGeneration.DisplayMessages("YOU WON!!!", ["Please select dice."]);

                PlayerSelectedDice = SelectDice();

                ServerSelectedDice = RandomNumberGenerator.GetInt32(Dices.Count);
                while (PlayerSelectedDice == ServerSelectedDice)
                    ServerSelectedDice = RandomNumberGenerator.GetInt32(Dices.Count);

                TableGeneration.DisplayMessages("SERVER DICE CHOICE", [$"I make the second move and choose the [{TableGeneration.GetDiceInfo(Dices, ServerSelectedDice.Value)}] dice."]);
            }
            else
            {
                ServerSelectedDice = RandomNumberGenerator.GetInt32(Dices.Count);
                TableGeneration.DisplayMessages("SERVER DICE CHOICE", [$"I make the first move and choose the [{TableGeneration.GetDiceInfo(Dices, ServerSelectedDice.Value)}] dice."]);

                PlayerSelectedDice = SelectDice();
            }

            SelectDiceRoll("my", ServerSelectedDice.Value);
            SelectDiceRoll("your", PlayerSelectedDice);

            TableGeneration.DisplayMessages("THE END", [GameResultInfo()]);
        }

        private int SelectDice()
        {
            TableGeneration.DisplayMessages("SELECT DICE", ["Choose your dice:"]);

            return PlayerInput(() => TableGeneration.WinProbabilityExplanation(Dices), Dices.Count, true);
        }

        private void SelectDiceRoll(string whichRoll, int diceIndex)
        {
            var key = KeyGeneration.Generate();
            var serverNumber = RandomNumberGenerator.GetInt32(2);
            var serverHmac = Hmac.Calculate($"{serverNumber}", key);

            var messages = new List<string>()
            {
                $"Lets roll {whichRoll.ToUpper()} dice [{TableGeneration.GetDiceInfo(Dices, diceIndex)}]",
                $"I selected a random value in the range 0..5",
                $"(HMAC={Convert.ToHexString(serverHmac)}).",
                "Add your number (modulo 6)."
            };

            TableGeneration.DisplayMessages($"ROLLING {whichRoll.ToUpper()} DICE", messages);

            var playerNumber = PlayerInput(() => TableGeneration.WinProbabilityExplanation(Dices), 6, false);

            var rollResult = FairNumberGeneration.DiceRollResult([serverNumber, playerNumber]);

            messages = new()
            {
                $"My number is {serverNumber} (KEY={Convert.ToHexString(key)}).",
                $"The fair number generation result is {serverNumber} + {playerNumber} = {rollResult} (mod 6).",
                $"{whichRoll.ToUpper()} roll result is {Dices[ServerSelectedDice!.Value][rollResult]}."
            };

            TableGeneration.DisplayMessages($"{whichRoll.ToUpper()} DICE ROLL RESULT", messages);
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

        private int PlayerInput(Action helpGeneration, int inputMaxRange, bool isPrintingDicesInfo)
        {
            var isReadingConsole = true;
            var playerInput = string.Empty;

            while (isReadingConsole)
            {
                TableGeneration.SelectionDisplay(inputMaxRange, isPrintingDicesInfo, Dices, ServerSelectedDice);

                playerInput = Console.ReadLine().ToLower().Trim();

                if (int.TryParse(playerInput, out int selection) && selection >= 0 && selection < inputMaxRange)
                    isReadingConsole = false;
                else if (playerInput == "?")
                    helpGeneration();
                else if (playerInput == "x")
                    Environment.Exit(0);
                else
                    ValidationHandler.InputError("Wrong input, try again");
            }
            return Convert.ToInt32(playerInput);
        }
    }
}

