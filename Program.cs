using System;
using Bible;
using Utils;

class Program
{
    static void Main()
    {

        Console.Clear(); // Add blank line if needed
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
        ConfigureLogging(true, false);
        // Right after ConfigureLogging

        // Book.GetBook(BibleBooks.Genesis);

        // string verse = "In the beginning God created the heavens and the earth,";
        // string userInput = "";



        // // Display the verse
        // Print("Verse from JSON:");


        // Print("Type the verse:\n");

        // while (true)
        // {
        //     Console.SetCursorPosition(0, 2); // Print at fixed line
        //     for (int i = 0; i < verse.Length; i++)
        //     {
        //         if (i < userInput.Length)
        //         {
        //             if (userInput[i] == verse[i])
        //             {
        //                 Console.ForegroundColor = ConsoleColor.Green;
        //                 Console.Write(verse[i]);
        //             }
        //             else
        //             {
        //                 Console.ForegroundColor = ConsoleColor.Red;
        //                 Console.Write(verse[i]);
        //             }
        //         }
        //         else
        //         {
        //             Console.ForegroundColor = ConsoleColor.DarkGray;
        //             Console.Write(verse[i]);
        //         }
        //     }

        //     Console.ResetColor();

        //     if (userInput == verse)
        //     {
        //         Console.WriteLine("\n\n✔ Completed!");
        //         break;
        //     }

        //     var key = Console.ReadKey(true);

        //     if (key.Key == ConsoleKey.Backspace && userInput.Length > 0)
        //     {
        //         userInput = userInput.Substring(0, userInput.Length - 1);
        //     }
        //     else if (!char.IsControl(key.KeyChar) && userInput.Length < verse.Length)
        //     {
        //         userInput += key.KeyChar;
        //     }
        // }
    }
}
