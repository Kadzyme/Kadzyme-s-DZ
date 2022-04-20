using System;
using System.Collections;

class Dz
{
    static void Main()
    {
        string wall = "#";
        string coin = "O";
        string player = "@";
        string lockedFinish = "L";
        string finish = "F";
        int arrayWidth = 20;
        int arrayHeight = 20;
        Random rand = new Random();
        int playerX = rand.Next(1, arrayWidth - 1);
        int playerY = rand.Next(1, arrayHeight - 1);
        string[,] array = new string[arrayHeight, arrayWidth];        
        FillArray(array, arrayWidth, arrayHeight, playerX, playerY, wall, coin, player, lockedFinish);
        Fill(array, arrayWidth, arrayHeight, 0, true, lockedFinish, finish);
        PlayerMove(playerX, playerY, array, arrayWidth, arrayHeight, wall, coin, player, lockedFinish, finish);
    }
    static void FillArray(string[,] array, int arrayWidth, int arrayHeight, int playerX, int playerY, string wall, string coin, string player, string lockedFinish)
    {
        Random rand = new Random();
        bool haveFinish = false;
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
                else if (randomNumber == 98 && haveFinish != true)
                {
                    array[i, j] = lockedFinish;
                    haveFinish = true;
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
    }
    static void Fill(string[,] array, int arrayWidth, int arrayHeight, int coins, bool hints, string lockedFinish, string finish)
    {
        int coinsRequire = 10;
        Console.Clear();
        for (int i = 0; i < arrayHeight; i++)
        {
            for (int j = 0; j < arrayWidth; j++)
            {
                if(coins >= coinsRequire && array[i, j] == lockedFinish)
                {
                    array[i, j] = finish;
                }
                Console.Write(array[i, j]);
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Coins: {coins}/{coinsRequire}");
        if(hints)
        {
            Hints();
            Console.WriteLine("Press H to turn off hints");
        }
        else
            Console.WriteLine("Press H to turn on hints");
    }
    static void PlayerMove(int playerX, int playerY, string[,] array, int arrayWidth, int arrayHeight, string wall, string coin, string player, string lockedFinish, string finish)
    {
        bool the_end = false;
        int coins = 0;
        bool hints = true;
        while (!the_end)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow && array[playerY - 1, playerX] != wall && array[playerY - 1, playerX] != lockedFinish)
            {
                array[playerY, playerX] = " ";
                playerY--;
            }
            else if (key.Key == ConsoleKey.DownArrow && array[playerY + 1, playerX] != wall && array[playerY + 1, playerX] != lockedFinish)
            {
                array[playerY, playerX] = " ";
                playerY++;
            }
            else if (key.Key == ConsoleKey.LeftArrow && array[playerY, playerX - 1] != wall && array[playerY, playerX - 1] != lockedFinish)
            {
                array[playerY, playerX] = " ";
                playerX--;                
            }
            else if (key.Key == ConsoleKey.RightArrow && array[playerY, playerX + 1] != wall && array[playerY, playerX + 1] != lockedFinish)
            {
                array[playerY, playerX] = " ";
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
            array[playerY, playerX] = player;            
            if (key.Key == ConsoleKey.H)
            {
                if (hints == true)
                    hints = false;
                else
                    hints = true;
            }
            Fill(array, arrayWidth, arrayHeight, coins, hints, lockedFinish, finish);
        }        
    }
    static void Hints()
    {
        Console.WriteLine("------------------------Hints------------------------");
        Console.WriteLine("@ - this is you, you can walk using arrows");
        Console.WriteLine("# - it is a wall, you can't go on this cell");
        Console.WriteLine("O - it is a coin, take them for unlocking finish");
        Console.WriteLine("L - it is a locked finish, for unlocking you need coins");
        Console.WriteLine("F - it is an unlocked finish, run in him to win!");
        Console.WriteLine("-----------------------------------------------------");
    }
}