namespace PasswordManager.Utilities;

public static class SecureConsole
{
    public static string ReadHiddenInput(string prompt, bool showAsterisks = true)
    {
        Console.Write(prompt);

        var input = new List<char>();

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (input.Count > 0)
                {
                    input.RemoveAt(input.Count - 1);

                    if (showAsterisks)
                    {
                        Console.Write("\b \b");
                    }
                }

                continue;
            }

            if (!char.IsControl(keyInfo.KeyChar))
            {
                input.Add(keyInfo.KeyChar);

                if (showAsterisks)
                {
                    Console.Write("*");
                }
            }
        }

        return new string(input.ToArray());
    }
}