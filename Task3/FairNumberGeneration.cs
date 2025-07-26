namespace Task3
{
    public static class FairNumberGeneration
    {
        public static int DiceRollResult(int[] diceRolls)
            => Math.Abs(diceRolls[0] + diceRolls[1]) % 6;
    }
}
