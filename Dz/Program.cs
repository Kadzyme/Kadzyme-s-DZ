using System;
using System.Collections;

class Dz
{
    static string wall = "#";
    static string voidCell = " ";
    static string coin = "O";
    static string jetpack = "J";
    static string player = "@";
    static string lockedFinish = "L";
    static string finish = "F";
    
    static int arrayWidth = 50;
    static int arrayHeight = 20;

    static int playerX;
    static int playerY;

    static int coins = 0;

    static string[,] array = new string[arrayHeight, arrayWidth];

    static bool hints = true;

    static void Main()
    {        
        Random rand = new Random();
        playerX = rand.Next(1, arrayWidth - 1);
        playerY = rand.Next(1, arrayHeight - 1); 
        GenerateArray();
        DrawArea(false);
        PlayerMoving();
    }

    static void GenerateArray()
    {
        Random rand = new Random();
        for (int i = 0; i < arrayHeight; i++)
        {            
            for (int j = 0; j < arrayWidth; j++)
            {                
                int randomNumber = rand.Next(0,100);
                if (i == 0 || j == 0 || i == arrayHeight - 1 || j == arrayWidth - 1 || randomNumber > 81)
                {
                    array[i, j] = wall;
                }
                else if (randomNumber > 78)
                {
                    array[i, j] = jetpack;
                }
                else if (randomNumber < 7)
                {
                    array[i, j] = coin;
                }
                else
                {
                    array[i, j] = voidCell;
                }
            }
        }
        bool haveFinish = false;
        while (!haveFinish)
        {
            int x = rand.Next(1,arrayWidth - 2);        
            int y = rand.Next(1, arrayHeight - 2);
            if (x != playerX || y != playerY)
            { 
                array[y,x] = lockedFinish;
                haveFinish = true;
            }
        }
    }

    static void DrawArea(bool jetpackEnabled)
    {
        string oldChar;
        int coinsRequire = 10;
        Console.Clear();
        oldChar = array[playerY, playerX];
        array[playerY, playerX] = player;
        for (int i = 0; i < arrayHeight; i++)
        {
            for (int j = 0; j < arrayWidth; j++)
            {
                if (coins >= coinsRequire && array[i, j] == lockedFinish)
                {
                    array[i, j] = finish;
                }
                if (array[i,j] == wall)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                else if (array[i, j] == coin)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                else if (array[i, j] == lockedFinish)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                else if (array[i, j] == finish)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (array[i, j] == jetpack)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }
                else if (array[i, j] == player)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.Write(array[i, j]);
            }
            Console.WriteLine();
        }
        array[playerY, playerX] = oldChar;
        Console.WriteLine("----------------------------------Your Current Information----------------------------------");
        Console.WriteLine($"Coins: {coins}/{coinsRequire}");
        if (jetpackEnabled)
        {
            Console.WriteLine($"Jetpack is available");
        }
        else
        {
            Console.WriteLine($"Jetpack is not available now");
        }
        if (hints)
        {
            Hints();
            Console.WriteLine("Press H to turn off hints");
        }
        else
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------");
            Console.WriteLine("Press H to turn on hints");
        }
        Console.WriteLine("");
        Console.WriteLine($"Press R to restart");
    }

    static void PlayerMoving()
    {
        bool the_end = false;
        bool jetpackEnabled = false;
        int coins = 0;
        while (!the_end)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow && CanPlayerMove(playerY - 1, playerX))
            {
                playerY--;
            }
            else if (key.Key == ConsoleKey.DownArrow && CanPlayerMove(playerY + 1, playerX))
            {
                playerY++;
            }
            else if (key.Key == ConsoleKey.LeftArrow && CanPlayerMove(playerY, playerX - 1))
            {
                playerX--;                
            }
            else if (key.Key == ConsoleKey.RightArrow && CanPlayerMove(playerY, playerX + 1))
            {
                playerX++;
            }
            else if (key.Key == ConsoleKey.UpArrow && CanPlayerMove(playerY - 2, playerX) && jetpackEnabled)
            {
                playerY -= 2;
                jetpackEnabled = false;
            }
            else if (key.Key == ConsoleKey.DownArrow && CanPlayerMove(playerY + 2, playerX) && jetpackEnabled)
            {
                playerY += 2;
                jetpackEnabled = false;
            }
            else if (key.Key == ConsoleKey.LeftArrow && CanPlayerMove(playerY, playerX - 2) && jetpackEnabled)
            {
                playerX -= 2;
                jetpackEnabled = false;
            }
            else if (key.Key == ConsoleKey.RightArrow && CanPlayerMove(playerY, playerX + 2) && jetpackEnabled)
            {
                playerX += 2;
                jetpackEnabled = false;
            }
            if (array[playerY, playerX] == coin)
            {
                coins++;
                array[playerY, playerX] = voidCell;
            }
            else if (array[playerY, playerX] == jetpack && jetpackEnabled == false)
            {
                jetpackEnabled = true;
                array[playerY, playerX] = voidCell;
            }
            else if (array[playerY, playerX] == finish)
            {
                the_end = true;                
            }       
            if (key.Key == ConsoleKey.H)
            {                
                hints = !hints;
            }
            else if (key.Key == ConsoleKey.R)
            {
                Main();
            }
            DrawArea(jetpackEnabled);
        }
        The_End();
    }
    static void The_End()
    {
        Console.Clear();
        Console.WriteLine("If you want to restart press R");
        var keyForRestart = Console.ReadKey();
        if (keyForRestart.Key == ConsoleKey.R)
        {
            Main();
        }
    }
    static bool CanPlayerMove(int playerY, int playerX)
        =>array[playerY, playerX] != wall && array[playerY, playerX] != lockedFinish;

    static void Hints()
    {
        Console.WriteLine("-------------------------------------------Hints--------------------------------------------");
        Console.WriteLine($"{player} - this is you, you can walk using arrows");
        Console.WriteLine($"{wall} - it is a wall, you can't go across them");
        Console.WriteLine($"{coin} - it is a coin, take them for unlocking finish");
        Console.WriteLine($"{jetpack} - it is a jetpack, if you take them, you can walk across 1 wall (don't stacks)");
        Console.WriteLine($"{lockedFinish} - it is a locked finish, for unlocking you need coins");
        Console.WriteLine($"{finish} - it is an unlocked finish, run in him to win!");
        Console.WriteLine("--------------------------------------------------------------------------------------------");
    }
}