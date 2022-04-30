using System;
using System.Collections;

class Dz
{
    static string ship = "█";
    static string voidCell = " ";
    static string miss = "#";
    static string hit = "*";
    
    static int arraySize = 11;
    static int direction = -1;// -1 - null, 0 - right, 1 - left, 2 - up, 3 - down
    static int numberOfLivingShipCells;
    static int numberOfEnemyLivingShipCells;

    static int playerX;
    static int playerY;

    static string[,] array = new string[arraySize, arraySize];
    static string[,] enemyArray = new string[arraySize, arraySize];

    static bool hints = true;
    static bool battle;
    static bool the_end;
    static bool firstPlayerTurn;

    static int[] limitForShips = new int[5];

    static void Main()
    {
        numberOfEnemyLivingShipCells = 0;
        numberOfLivingShipCells = 0;
        the_end = false;
        playerX = 0;
        playerY = 0;
        battle = false;
        firstPlayerTurn = true;
        GenerateLimitForShipsNumber();
        GenerateArea();
        DrawArea();
        PlayerMoving();        
    }

    static void GenerateArea()
    {
        for (int i = 0; i < arraySize; i++)
        {
            for (int j = 0; j < arraySize; j++)
            {
                array[i, j] = voidCell;
                enemyArray[i, j] = voidCell;
            }
        }
    }

    static void GenerateLimitForShipsNumber()
    {
        int l = limitForShips.Length;
        for (int i = 1; i < limitForShips.Length; i++)
        {
            l--;
            limitForShips[i] = l;
        }
    }

    static void GenerationShips(int size)
    {
        int numberOfShip = 0;
        Console.WriteLine("Press arrow for choosing direction");
        var key = Console.ReadKey();
        switch (key.Key)
        {
            case ConsoleKey.RightArrow:
                direction = 0;
                break;
            case ConsoleKey.LeftArrow:
                direction = 1;
                break;
            case ConsoleKey.UpArrow:
                direction = 2;
                break;
            case ConsoleKey.DownArrow:
                direction = 3;
                break;
        }
        if (CanYouSpawnShip(size))
        {
            limitForShips[size]--;
            numberOfLivingShipCells += size;
            if (direction == 0)
            {
                for (int l = playerX; l < playerX + size; l++)
                {
                    array[playerY, l] = ship;
                }
            }
            else if (direction == 1)
            {
                for (int l = playerX; l > playerX - size; l--)
                {
                    array[playerY, l] = ship;
                }
            }
            else if (direction == 2)
            {
                for (int l = playerY; l > playerY - size; l--)
                {
                    array[l, playerX] = ship;
                }
            }
            else if (direction == 3)
            {
                for (int l = playerY; l < playerY + size; l++)
                {
                    array[l, playerX] = ship;
                }
            }
            numberOfShip++;
            DrawArea();
        }
        else
        {
            Thread.Sleep(1169);
        }
    }

    static void PlayerMoving()
    {
        int size = 3;
        while (!the_end)
        {
            ConsoleKeyInfo key = Console.ReadKey();
            size = EnterSize(key, size);
            if (key.Key == ConsoleKey.RightArrow && CanYouMovePlayer(playerX + 1, playerY))
            {
                playerX++;
            }
            else if (key.Key == ConsoleKey.LeftArrow && CanYouMovePlayer(playerX - 1, playerY))
            {
                playerX--;
            }
            else if (key.Key == ConsoleKey.UpArrow && CanYouMovePlayer(playerX, playerY - 1))
            {
                playerY--;
            }
            else if (key.Key == ConsoleKey.DownArrow && CanYouMovePlayer(playerX, playerY + 1))
            {
                playerY++;
            }
            else if (key.Key == ConsoleKey.H)
            {
                hints = !hints;
            }
            else if (key.Key == ConsoleKey.R)
            {
                Main();
            }
            else if (key.Key == ConsoleKey.Enter && !battle)
            {
                GenerationShips(size);
            }
            else if (key.Key == ConsoleKey.Enter && battle)
            {
                Shoot();
            }
            if (!battle)
            {
                int j = 0;
                for(int i = 1; i < limitForShips.Length; i++)
                {
                    if (!DoYouHaveLimitForThisShip(i))
                    {
                        j++;
                    }
                }
                if (j == limitForShips.Length - 1)
                {
                    if (!firstPlayerTurn)                
                    {
                        battle = true;             
                    }
                    else
                    {
                        GenerateLimitForShipsNumber();
                    }
                    ChangeTurn();
                }
                
            }
            DrawArea();
        }
        The_End();
    }

    static void ChangeTurn()
    {
        string[,] arrayForChanging = new string[arraySize,arraySize];
        int num = numberOfEnemyLivingShipCells;
        numberOfEnemyLivingShipCells = numberOfLivingShipCells;
        numberOfLivingShipCells = num;
        Random rand = new Random();
        playerX = rand.Next(0, arraySize - 1);
        playerY = rand.Next(0, arraySize - 1);
        arrayForChanging = enemyArray;
        enemyArray = array;
        array = arrayForChanging;
        firstPlayerTurn = !firstPlayerTurn;
    }

    static void Shoot()
    {
        if(enemyArray[playerY,playerX] == ship)
        {
            Console.WriteLine("You hited an enemy!!!");
            Console.WriteLine("You can walk again!!!");
            enemyArray[playerY, playerX] = hit;
            numberOfEnemyLivingShipCells--;
            if (numberOfEnemyLivingShipCells <= 0)
            {
                the_end = true;
            }
        }
        else if (enemyArray[playerY, playerX] != miss && enemyArray[playerY, playerX] != hit)
        {
            Console.WriteLine("You missed :(");
            enemyArray[playerY, playerX] = miss;
            ChangeTurn();
        }
        else
        {
            Console.WriteLine("You can't shoot in this place");
        }
        Thread.Sleep(1200);
        Console.Clear();
        Console.WriteLine("Press any button to change player");
        var key = Console.ReadKey();
    }

    static int EnterSize(ConsoleKeyInfo key, int size)
    {
        if (key.Key == ConsoleKey.NumPad1 || key.Key == ConsoleKey.D1)
        {
            return 1;
        }
        else if (key.Key == ConsoleKey.NumPad2 || key.Key == ConsoleKey.D2)
        {
            return 2;
        }
        else if (key.Key == ConsoleKey.NumPad3 || key.Key == ConsoleKey.D3)
        {
            return 3;
        }
        else if (key.Key == ConsoleKey.NumPad4 || key.Key == ConsoleKey.D4)
        {
            return 4;
        }
        return size;
    }

    static void DrawArea()
    {
        Console.Clear();
        if (battle)
        {
            DrawEnemyArea();
        }
        Console.WriteLine("Your area:");
        for (int i = 0; i < arraySize; i++)
        {
            for (int j = 0; j < arraySize; j++)
            {
                if (i == playerY && j == playerX && !battle)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }
                if (array[i,j] == ship)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (array[i, j] == miss)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }
                else if (array[i, j] == hit)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write(array[i, j]);
            }
            Console.WriteLine();
        }        
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine("---------------------------------Your Current Information-----------------------------------");
        if (!battle)
        {
            for (int i = 1; i < limitForShips.Length; i++)
            {
                if (limitForShips[i] > 0)
                    Console.WriteLine($"You can place {limitForShips[i]} ships with {i} deck(s)");
            }
        }
        else if (firstPlayerTurn)
        {
            Console.WriteLine("First player's turn");
        }
        else
        {
            Console.WriteLine("Second player's turn");
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
        Console.WriteLine();
        Console.WriteLine($"Press R to restart");
    }
    
    static void DrawEnemyArea()
    {
        Console.WriteLine("Enemy area:");
        for (int i = 0; i < arraySize; i++)
        {
            for (int j = 0; j < arraySize; j++)
            {
                if (i == playerY && j == playerX)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }
                if (enemyArray[i, j] == miss)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }
                else if (enemyArray[i, j] == hit)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (enemyArray[i,j] == ship)
                {
                    Console.Write(voidCell);
                }
                else
                {
                    Console.Write(enemyArray[i, j]);
                }
            }
            Console.WriteLine();
        }
    }

    static void The_End()
    {
        Console.Clear();
        if (firstPlayerTurn)
        {
            Console.WriteLine("First player won!!!");
        }
        else
        {
            Console.WriteLine("Second player won!!!");
        }        
        Console.WriteLine("If you want to restart press R");
        var keyForRestart = Console.ReadKey();
        if (keyForRestart.Key == ConsoleKey.R)
        {
            Main();
        }
    }

    static bool CanYouSpawnShip(int shipSize)
    {
        if (!DoYouHaveLimitForThisShip(shipSize))
        {
            Console.WriteLine("You haven't limit for this ship");
            return false;            
        }
        int x = 0;
        int y = 0;
        for (int l = 0; l < shipSize; l++)
        {            
            for (int i = playerY + y + 1; i >= playerY + y - 1; i--)
            {
                for (int j = playerX + x - 1; j <= playerX + x + 1; j++)
                {
                    if (i >= 0 && j >= 0 && i < arraySize && j < arraySize && array[i, j] == ship)
                    {
                        Console.WriteLine("You can't place ship in this place");
                        return false;
                    }
                }
            }
            if (direction == 0)// -1 - null, 0 - right, 1 - left, 2 - up, 3 - down
            {
                x++;
            }
            else if (direction == 1)
            {
                x--;
            }
            else if (direction == 2)
            {
                y--;
            }
            else if (direction == 3)
            {
                y++;
            }
        }
        return true;
    }

    static bool DoYouHaveLimitForThisShip(int size)
    {
        for(int i = 1; i < limitForShips.Length; i++)
        {
            if(limitForShips[i] <= 0 && size == i)
            {
                return false;
            }
        }
        return true;
    }

    static bool CanYouMovePlayer(int playerCamX, int playerCamY)
        => playerCamX >= 0 && playerCamY >= 0 && playerCamX < arraySize && playerCamY < arraySize;

    static void Hints()
    {
        Console.WriteLine("-------------------------------------------Hints--------------------------------------------");
        Console.WriteLine($"Red cell is you, you can move with arrows");
        Console.WriteLine($"For placing ship press enter and arrow for direction");
        Console.WriteLine($"For changing size of ship press button with needed number(min - 1, max - 4)");
        Console.WriteLine("--------------------------------------------------------------------------------------------");
    }
}