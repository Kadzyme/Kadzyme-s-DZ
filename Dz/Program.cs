using System;
using System.Collections;

class Dz
{
    static void Main()
    {
        string wall = "#";
        string voidCell = " ";
        string coin = "O";
        string jetpack = "J";
        string player = "@";
        string lockedFinish = "L";
        string finish = "F";
        int arrayWidth = 50;
        int arrayHeight = 20;
        Random rand = new Random();
        int playerX = rand.Next(1, arrayWidth - 1);
        int playerY = rand.Next(1, arrayHeight - 1);
        string[,] array = new string[arrayHeight, arrayWidth];        
        GenerateArray(array, arrayWidth, arrayHeight, playerX, playerY, wall, coin, player, lockedFinish, voidCell, jetpack);
        DrawArea(wall, coin, player, array, arrayWidth, arrayHeight, 0, true, lockedFinish, finish, playerX, playerY, jetpack, false);
        PlayerMoving(playerX, playerY, array, arrayWidth, arrayHeight, wall, coin, player, lockedFinish, finish, voidCell, jetpack);
    }

    static void GenerateArray(string[,] array, int arrayWidth, int arrayHeight, int playerX, int playerY, string wall, string coin, string player, string lockedFinish, string voidCell, string jetpack)
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

    static void DrawArea(string wall, string coin, string player, string[,] array, int arrayWidth, int arrayHeight, int coins, bool hints, string lockedFinish, string finish, int playerX, int playerY, string jetpack, bool jetpackEnabled)
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
            Hints(wall, coin, player, lockedFinish, finish, jetpack);
            Console.WriteLine("Press H to turn off hints");
        }
        else
        {
            Console.WriteLine("--------------------------------------------------------------------------------------------");
            Console.WriteLine("Press H to turn on hints");
        }
    }

    static void PlayerMoving(int playerX, int playerY, string[,] array, int arrayWidth, int arrayHeight, string wall, string coin, string player, string lockedFinish, string finish, string voidCell, string jetpack)
    {
        bool the_end = false;
        bool jetpackEnabled = false;
        int coins = 0;
        bool hints = true;
        while (!the_end)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow && CanPlayerMove(playerY - 1, playerX, array, wall, lockedFinish))
            {
                playerY--;
            }
            else if (key.Key == ConsoleKey.DownArrow && CanPlayerMove(playerY + 1, playerX, array, wall, lockedFinish))
            {
                playerY++;
            }
            else if (key.Key == ConsoleKey.LeftArrow && CanPlayerMove(playerY, playerX - 1, array, wall, lockedFinish))
            {
                playerX--;                
            }
            else if (key.Key == ConsoleKey.RightArrow && CanPlayerMove(playerY, playerX + 1, array, wall, lockedFinish))
            {
                playerX++;
            }
            else if (key.Key == ConsoleKey.UpArrow && CanPlayerMove(playerY - 2, playerX, array, wall, lockedFinish) && jetpackEnabled)
            {
                playerY -= 2;
                jetpackEnabled = false;
            }
            else if (key.Key == ConsoleKey.DownArrow && CanPlayerMove(playerY + 2, playerX, array, wall, lockedFinish) && jetpackEnabled)
            {
                playerY += 2;
                jetpackEnabled = false;
            }
            else if (key.Key == ConsoleKey.LeftArrow && CanPlayerMove(playerY, playerX - 2, array, wall, lockedFinish) && jetpackEnabled)
            {
                playerX -= 2;
                jetpackEnabled = false;
            }
            else if (key.Key == ConsoleKey.RightArrow && CanPlayerMove(playerY, playerX + 2, array, wall, lockedFinish) && jetpackEnabled)
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
            DrawArea(wall, coin, player, array, arrayWidth, arrayHeight, coins, hints, lockedFinish, finish, playerX, playerY, jetpack, jetpackEnabled);
        }        
    }

    static bool CanPlayerMove(int playerY, int playerX, string[,] array, string wall, string lockedFinish)
        =>array[playerY, playerX] != wall && array[playerY, playerX] != lockedFinish;

    static void Hints(string wall, string coin, string player, string lockedFinish, string finish, string jetpack)
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