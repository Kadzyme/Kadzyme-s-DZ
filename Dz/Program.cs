using System;
using System.Collections;

class Dz
{
    static void Main()
    {
        int coinsRequire = 10;
        string wall = "#";
        string coin = "O";
        string player = "@";
        string lockedFinish = "L";
        string finish = "F";
        int arrayWidth = 50;
        int arrayHeight = 20;
        Random rand = new Random();
        int playerX = rand.Next(1, arrayWidth - 1);
        int playerY = rand.Next(1, arrayHeight - 1);
        string[,] array = new string[arrayHeight, arrayWidth];        
        GenerateArray(array, arrayWidth, arrayHeight, playerX, playerY, wall, coin, player, lockedFinish);
        DrawArea(wall, coin, player, array, arrayWidth, arrayHeight, 0, true, lockedFinish, finish, coinsRequire, playerX, playerY);
        PlayerMoving(playerX, playerY, array, arrayWidth, arrayHeight, wall, coin, player, lockedFinish, finish, coinsRequire);
    }

    static void GenerateArray(string[,] array, int arrayWidth, int arrayHeight, int playerX, int playerY, string wall, string coin, string player, string lockedFinish)
    {
        Random rand = new Random();
        for (int i = 0; i < arrayHeight; i++)
        {            
            for (int j = 0; j < arrayWidth; j++)
            {                
                int randomNumber = rand.Next(0,100);
                if (i == playerY && j == playerX)
                {
                    array[i, j] = player;
                }
                else if (i == 0 || j == 0 || i == arrayHeight - 1 || j == arrayWidth - 1)
                {
                    array[i, j] = wall;
                }
                else if (randomNumber > 81)
                {
                    array[i, j] = wall;
                }
                else if (randomNumber < 7)
                {
                    array[i, j] = coin;
                }
                else
                {
                    array[i, j] = " ";
                }
            }
        }
        bool haveFinish = false;
        while (!haveFinish)
        {
            int x = rand.Next(0,arrayWidth);        
            int y = rand.Next(0, arrayHeight);
            if (x != playerX || y != playerY)
            { 
                array[y,x] = lockedFinish;
                haveFinish = true;
            }
        }
    }

    static void DrawArea(string wall, string coin, string player, string[,] array, int arrayWidth, int arrayHeight, int coins, bool hints, string lockedFinish, string finish, int coinsRequire, int playerX, int playerY)
    {
        Console.Clear();
        string[,] arrayForDrawing = array;//problem
        arrayForDrawing[playerY, playerX] = player;
        for (int i = 0; i < arrayHeight; i++)
        {
            for (int j = 0; j < arrayWidth; j++)
            {                
                if (arrayForDrawing[i,j] == wall)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (arrayForDrawing[i, j] == coin)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else if (arrayForDrawing[i, j] == lockedFinish)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                else if (arrayForDrawing[i, j] == finish)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (arrayForDrawing[i, j] == player)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.Write(arrayForDrawing[i, j]);
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Coins: {coins}/{coinsRequire}");
        if(hints)
        {
            Hints(wall, coin, player, lockedFinish, finish);
            Console.WriteLine("Press H to turn off hints");
        }
        else
        {
            Console.WriteLine("Press H to turn on hints");
        }
    }

    static void PlayerMoving(int playerX, int playerY, string[,] array, int arrayWidth, int arrayHeight, string wall, string coin, string player, string lockedFinish, string finish, int coinsRequire)
    {
        bool the_end = false;
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
            if (array[playerY, playerX] == coin)
            {
                coins++;
            }
            if (array[playerY, playerX] == finish)
            {
                the_end = true;                
            }       
            if (key.Key == ConsoleKey.H)
            {                
                hints = !hints;
            }
            //if (coins >= coinsRequire && array[,] == lockedFinish)
            //{
            //    array[playerY, playerX] = finish;
            //}
            DrawArea(wall, coin, player, array, arrayWidth, arrayHeight, coins, hints, lockedFinish, finish, coinsRequire, playerX, playerY);
        }        
    }

    static bool CanPlayerMove(int playerY, int playerX, string[,] array, string wall, string lockedFinish)
        =>array[playerY, playerX] != wall && array[playerY, playerX] != lockedFinish;    

    static void Hints(string wall, string coin, string player, string lockedFinish, string finish)
    {
        Console.WriteLine("------------------------Hints------------------------");
        Console.WriteLine($"{player} - this is you, you can walk using arrows");
        Console.WriteLine($"{wall} - it is a wall, you can't go across them");
        Console.WriteLine($"{coin} - it is a coin, take them for unlocking finish");
        Console.WriteLine($"{lockedFinish} - it is a locked finish, for unlocking you need coins");
        Console.WriteLine($"{finish} - it is an unlocked finish, run in him to win!");
        Console.WriteLine("-----------------------------------------------------");
    }
}