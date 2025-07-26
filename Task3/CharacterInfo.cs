namespace Task3
{
    public class CharacterInfo
    {
        public string Name { get; set; }
        public int? SelectedDice { get; set; }
        public int? DiceRollResult { get; set; }

        public CharacterInfo(string name)
        {
            Name = name.ToUpper();
        }
    }
}
