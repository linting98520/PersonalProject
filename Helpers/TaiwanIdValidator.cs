namespace PersonalInfoApi.Helpers
{
    public static class TaiwanIdValidator
    {
        public static bool IsValid(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 10)
                return false;

            id = id.ToUpper();
            if (!char.IsLetter(id[0]))
                return false;
            
            for(int i = 1; i < id.Length; i++)
            {
                if (!char.IsDigit(id[i]))
                    return false;
            }

            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int[] letterValues = {
                10,11,12,13,14,15,16,17,18,19,20,21,22,
                23,24,25,26,27,28,29,32,30,31,32,33,34
            };

            int letterIndex = letters.IndexOf(id[0]);
            if (letterIndex == -1) return false;

            int total = letterValues[letterIndex] / 10 + (letterValues[letterIndex] % 10) * 9;

            int[] weights = { 8, 7, 6, 5, 4, 3, 2, 1 };
            for (int i = 0; i < 8; i++)
            {
                total += (id[i + 1] - '0') * weights[i];
            }
            total += (id[9] - '0');

            return total % 10 == 0;
        }
    }
}