using System;
using System.Collections;

namespace Dz
{
    public struct Player
    {
        public string[,] array;
        public int playerX;
        public int playerY;
        public int numberOfLivingShipCells;

        public Player(string[,] array, int playerX,int playerY, int numberOfLivingShipCells)
        {
            this.array = array;
            this.playerX = playerX;
            this.playerY = playerY;
            this.numberOfLivingShipCells = numberOfLivingShipCells;
        }
    }
    public class Symbols
    {
        public string ship = "█";
        public string voidCell = " ";
        public string miss = "#";
        public string hit = "*";
    }

    class Dz
    {
        private readonly Symbols symbols = new Symbols();

        private Player[] player = new Player[2];

        private readonly int arraySize = 10;

        private bool hints = true;
        private bool battle;
        private bool theEnd;

        private int[] limitForShips = new int[5];

        private int playerTurn = 0;
        private int nextPlayer;

        private ConsoleKeyInfo key;

        static void Main()
        {
            var dz = new Dz();
            dz.Start();
        }

        public void Start()
        {
            for (int i = 0; i < player.Length; i++)
            {
                player[i].array = new string[arraySize, arraySize];
                player[i].numberOfLivingShipCells = 0;
                player[i].playerX = 0;
                player[i].playerY = 0;
                GenerateArea(i);
            }
            nextPlayer = playerTurn + 1;
            theEnd = false;
            battle = false;
            GenerateLimitForShipsNumber();
            DrawArea(playerTurn);
            Information();
            PlayerMoving();
        }

        private void GenerateArea(int playerNumber)
        {
            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    player[playerNumber].array[i, j] = symbols.voidCell;
                }
            }
        }

        private void GenerateLimitForShipsNumber()
        {
            for (int i = 1; i < limitForShips.Length; i++)
            {
                limitForShips[i] = limitForShips.Length - i;
            }
        }

        private void GenerationShips(int size)
        {            
            if (size > 0)
            {
                Console.WriteLine("Press arrow for choosing direction");
                key = Console.ReadKey();
            }
            if (CanYouSpawnShip(size))
            {
                limitForShips[size]--;
                player[playerTurn].numberOfLivingShipCells += size;
                bool RaiseX = false;
                bool RaiseY = false;
                bool LowX = false;
                if (key.Key == ConsoleKey.RightArrow)
                {
                    RaiseX = true;
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    LowX = true;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    RaiseY = true;
                }
                GenerateShipCells(RaiseX, LowX, RaiseY, size);
                DrawArea(playerTurn);
                Information();
            }
            else
            {
                Thread.Sleep(1169);
            }
        }

        private void GenerateShipCells(bool RaiseX, bool LowX, bool RaiseY, int size)
        {
            int x = 0, y = 0;
            for (int l = 0; l < size; l++)
            {
                player[playerTurn].array[player[playerTurn].playerY + y, player[playerTurn].playerX + x] = symbols.ship;
                if(RaiseX)
                {
                    x++;
                }
                else if (RaiseY)
                {
                    y++;
                }
                else if (LowX)
                {
                    x--;
                }
                else
                {
                    y--;
                }
            }
        }

        private void PlayerMoving()
        {
            int size = 1;
            while (!theEnd)
            {
                key = Console.ReadKey();
                size = EnterSize(size);
                if (key.Key == ConsoleKey.RightArrow && CanYouMovePlayer(player[playerTurn].playerX + 1))
                {
                    player[playerTurn].playerX++;
                }
                else if (key.Key == ConsoleKey.LeftArrow && CanYouMovePlayer(player[playerTurn].playerX - 1))
                {
                    player[playerTurn].playerX--;
                }
                else if (key.Key == ConsoleKey.UpArrow && CanYouMovePlayer(player[playerTurn].playerY - 1))
                {
                    player[playerTurn].playerY--;
                }
                else if (key.Key == ConsoleKey.DownArrow && CanYouMovePlayer(player[playerTurn].playerY + 1))
                {
                    player[playerTurn].playerY++;
                }
                if (key.Key == ConsoleKey.H)
                {
                    hints = !hints;
                }
                else if (key.Key == ConsoleKey.R)
                {
                    Start();
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
                    int j = 1;
                    for (int i = 1; i < limitForShips.Length; i++)
                    {
                        if (!DoYouHaveLimitForThisShip(i))
                        {
                            j++;
                        }
                    }
                    if (j == limitForShips.Length)
                    {
                        if (playerTurn == player.Length - 1)
                        {
                            battle = true;
                        }
                        else
                        {
                            GenerateLimitForShipsNumber();
                        }
                        playerTurn = ChangeTurn(playerTurn);
                        nextPlayer = ChangeTurn(nextPlayer);
                    }

                }
                Console.Clear();
                if (battle)
                {
                    DrawArea(nextPlayer);
                }
                DrawArea(playerTurn);
                Information();
            }
            The_End();
        }

        private bool CanYouMovePlayer(int coord)
            => coord >= 0 && coord < arraySize;

        private void Shoot()
        {
            if (player[nextPlayer].array[player[nextPlayer].playerY, player[nextPlayer].playerX] == symbols.ship)
            {
                Console.WriteLine("You hited an enemy!!!");
                Console.WriteLine("You can walk again!!!");
                player[nextPlayer].array[player[nextPlayer].playerY, player[nextPlayer].playerX] = symbols.hit;
                player[nextPlayer].numberOfLivingShipCells--;
                if (player[nextPlayer].numberOfLivingShipCells <= 0)
                {
                    theEnd = true;
                }
            }
            else if (player[nextPlayer].array[player[nextPlayer].playerY, player[nextPlayer].playerX] != symbols.miss && player[nextPlayer].array[player[nextPlayer].playerY, player[nextPlayer].playerX] != symbols.hit)
            {
                Console.WriteLine("You missed :(");
                player[nextPlayer].array[player[nextPlayer].playerY, player[nextPlayer].playerX] = symbols.miss;
                playerTurn = ChangeTurn(playerTurn);
                nextPlayer = ChangeTurn(nextPlayer);
            }
            else
            {
                Console.WriteLine("You can't shoot in this place");
            }
            Thread.Sleep(1200);
        }

        private int ChangeTurn(int num)
        {
            num++;
            if (num == player.Length)
            {
                num = 0;
            }
            Console.Clear();
            Console.WriteLine("Press any button to change player");
            var key = Console.ReadKey();
            return num;
        }

        private int EnterSize(int size)
        {
            var changeSize = new Dictionary<ConsoleKey, int>()
            {
                [ConsoleKey.NumPad1] = 1,
                [ConsoleKey.NumPad2] = 2,
                [ConsoleKey.NumPad3] = 3,
                [ConsoleKey.NumPad4] = 4,
                [ConsoleKey.D1] = 1,
                [ConsoleKey.D2] = 2,
                [ConsoleKey.D3] = 3,
                [ConsoleKey.D4] = 4
            };
            try
            {
                return changeSize[key.Key];
            }
            catch
            {
                return size;
            }
        }

        private void DrawArea(int numOfPlayer)
        {
            Console.WriteLine($"{numOfPlayer + 1} player's area:");
            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    if (i == player[numOfPlayer].playerY && j == player[numOfPlayer].playerX)
                    {
                        if (!battle && numOfPlayer == playerTurn || battle && numOfPlayer != playerTurn)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                        }
                    }
                    if (player[numOfPlayer].array[i, j] == symbols.ship)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (player[numOfPlayer].array[i, j] == symbols.miss)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    else if (player[numOfPlayer].array[i, j] == symbols.hit)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (numOfPlayer != playerTurn && player[numOfPlayer].array[i, j] == symbols.ship)
                        Console.Write(symbols.voidCell);
                    else
                        Console.Write(player[numOfPlayer].array[i, j]);
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
            }            
        }

        private void The_End()
        {
            Console.Clear();
            Console.WriteLine($"{playerTurn + 1} player won!!!");            
            Console.WriteLine("If you want to restart press R");
            var keyForRestart = Console.ReadKey();
            if (keyForRestart.Key == ConsoleKey.R)
            {
                Start();
            }
        }

        private bool CanYouSpawnShip(int shipSize)
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
                for (int i = player[playerTurn].playerY + y + 1; i >= player[playerTurn].playerY + y - 1; i--)
                {                    
                    for (int j = player[playerTurn].playerX + x - 1; j <= player[playerTurn].playerX + x + 1; j++)
                    {
                        if (i >= 0 && j >= 0 && i < arraySize && j < arraySize && player[playerTurn].array[i, j] == symbols.ship)
                        {
                            Console.WriteLine("You can't place ship in this place");
                            return false;
                        }
                    }
                    var changeX = new Dictionary<ConsoleKey, int>()
                    {
                        [ConsoleKey.RightArrow] = x++,
                        [ConsoleKey.LeftArrow] = x--
                    };
                    try
                    {
                        x = changeX[key.Key];
                    }
                    catch
                    { }
                    var changeY = new Dictionary<ConsoleKey, int>()
                    {
                        [ConsoleKey.DownArrow] = y++,
                        [ConsoleKey.UpArrow] = y--
                    };
                    try
                    {
                        y = changeY[key.Key];
                    }
                    catch
                    { }
                }
            }
            return true;
        }

        private bool DoYouHaveLimitForThisShip(int size)
        {
            for (int i = 1; i < limitForShips.Length; i++)
            {
                if (limitForShips[i] <= 0 && size == i)
                {
                    return false;
                }
            }
            return true;
        }        

        private void Information()
        {
            Console.WriteLine("--------------------------------------Current Information---------------------------------------");
            if (!battle)
            {
                for (int i = 1; i < limitForShips.Length; i++)
                {
                    if (limitForShips[i] > 0)
                        Console.WriteLine($"You can place {limitForShips[i]} ships with {i} deck(s)");
                }
            }
            else
            {
                Console.WriteLine($"{playerTurn + 1} player's turn");
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

        private void Hints()
        {
            Console.WriteLine("-------------------------------------------Hints--------------------------------------------");
            Console.WriteLine($"Red cell is you, you can move with arrows");
            Console.WriteLine($"For placing ship press enter and arrow for direction");
            Console.WriteLine($"For changing size of ship press button with needed number(min - 1, max - 4)");
            Console.WriteLine("--------------------------------------------------------------------------------------------");
        }
    }
}