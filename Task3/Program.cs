using Task3;

var dices = DiceParser.ParseArgs(args);

var game = new GameLogic(dices);

game.StartGame();
