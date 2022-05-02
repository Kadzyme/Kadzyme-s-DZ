using System;
using System.Collections;

namespace Dz
{
    public struct PlayerCoords
    {
        public int playerX;
        public int playerY;
         
        public PlayerCoords(int playerX, int playerY)
        {
            this.playerX = playerX;
            this.playerY = playerY;
        }
    }
    public class Symbols
    {
        public string ship = "█";
        public string voidCell = " ";
        public string miss = "#";
        public string hit = "*";
    }
    public enum Direction
    {
        right,
        left,
        up,
        down
    }

    class Dz
    {
        private Symbols symbols = new Symbols();
        private PlayerCoords playerCoords;
        private Direction direction;

        private int arraySize = 11;
        private int numberOfLivingShipCells;
        private int numberOfEnemyLivingShipCells;

        private string[,] array = new string[arraySize, arraySize];
        private string[,] enemyArray = new string[arraySize, arraySize];

        private bool hints = true;
        private bool battle;
        private bool theEnd;
        private bool firstPlayerTurn;

        static int[] limitForShips = new int[5];

        static void Main()
        {
            var dz = new Dz();
            dz.Start();
        }

        public void Start()
        {
            numberOfEnemyLivingShipCells = 0;
            numberOfLivingShipCells = 0;
            theEnd = false;
            playerCoords.playerX = 0;
            playerCoords.playerY = 0;
            battle = false;
            firstPlayerTurn = true;
            GenerateLimitForShipsNumber();
            GenerateArea();
            DrawArea();
            PlayerMoving();
        }

        private void GenerateArea()
        {
            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    array[i, j] = symbols.voidCell;
                    enemyArray[i, j] = symbols.voidCell;
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
            int numberOfShip = 0;
            Console.WriteLine("Press arrow for choosing direction");
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.RightArrow:
                    direction = Direction.right;
                    break;
                case ConsoleKey.LeftArrow:
                    direction = Direction.left;
                    break;
                case ConsoleKey.UpArrow:
                    direction = Direction.up;
                    break;
                case ConsoleKey.DownArrow:
                    direction = Direction.down;
                    break;
                default:
                    direction = 0;
                    break;
            }
            if (CanYouSpawnShip(size))
            {
                limitForShips[size]--;
                numberOfLivingShipCells += size;
                if (direction == Direction.right)
                {
                    for (int l = playerCoords.playerX; l < playerCoords.playerX + size; l++)
                    {
                        array[playerCoords.playerY, l] = symbols.ship;
                    }
                }
                else if (direction == Direction.left)
                {
                    for (int l = playerCoords.playerX; l > playerCoords.playerX - size; l--)
                    {
                        array[playerCoords.playerY, l] = symbols.ship;
                    }
                }
                else if (direction == Direction.up)
                {
                    for (int l = playerCoords.playerY; l > playerCoords.playerY - size; l--)
                    {
                        array[l, playerCoords.playerX] = symbols.ship;
                    }
                }
                else if (direction == Direction.down)
                {
                    for (int l = playerCoords.playerY; l < playerCoords.playerY + size; l++)
                    {
                        array[l, playerCoords.playerX] = symbols.ship;
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

        private void PlayerMoving()
        {
            int size = 3;
            while (!theEnd)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                size = EnterSize(key, size);
                if (key.Key == ConsoleKey.RightArrow && CanYouMovePlayer(playerCoords.playerX + 1))
                {
                    playerCoords.playerX++;
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    playerCoords.playerX--;
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    playerCoords.playerY--;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    playerCoords.playerY++;
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
                    int j = 0;
                    for (int i = 1; i < limitForShips.Length; i++)
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

        private bool CanYouMovePlayer(int coord)
            => coord >= 0 && coord < arraySize;

        private void ChangeTurn()
        {
            string[,] arrayForChanging = new string[arraySize, arraySize];
            int num = numberOfEnemyLivingShipCells;
            numberOfEnemyLivingShipCells = numberOfLivingShipCells;
            numberOfLivingShipCells = num;
            Random rand = new Random();
            playerCoords.playerX = rand.Next(0, arraySize - 1);
            playerCoords.playerY = rand.Next(0, arraySize - 1);
            arrayForChanging = enemyArray;
            enemyArray = array;
            array = arrayForChanging;
            firstPlayerTurn = !firstPlayerTurn;
        }

        private void Shoot()
        {
            if (enemyArray[playerCoords.playerY, playerCoords.playerX] == symbols.ship)
            {
                Console.WriteLine("You hited an enemy!!!");
                Console.WriteLine("You can walk again!!!");
                enemyArray[playerCoords.playerY, playerCoords.playerX] = symbols.hit;
                numberOfEnemyLivingShipCells--;
                if (numberOfEnemyLivingShipCells <= 0)
                {
                    theEnd = true;
                }
            }
            else if (enemyArray[playerCoords.playerY, playerCoords.playerX] != symbols.miss && enemyArray[playerCoords.playerY, playerCoords.playerX] != symbols.hit)
            {
                Console.WriteLine("You missed :(");
                enemyArray[playerCoords.playerY, playerCoords.playerX] = symbols.miss;
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
        private int EnterSize(ConsoleKeyInfo key, int size)
        {
            var changeSize = new Dictionary<ConsoleKey, int>()
            {
                [ConsoleKey.NumPad1] = 1,
                [ConsoleKey.NumPad2] = 2,
                [ConsoleKey.NumPad3] = 3,
                [ConsoleKey.NumPad4] = 4
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

        private void DrawArea()
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
                    if (i == playerCoords.playerY && j == playerCoords.playerX && !battle)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                    if (array[i, j] == symbols.ship)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else if (array[i, j] == symbols.miss)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    else if (array[i, j] == symbols.hit)
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

        private void DrawEnemyArea()
        {
            Console.WriteLine("Enemy area:");
            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    if (i == playerCoords.playerY && j == playerCoords.playerX)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                    if (enemyArray[i, j] == symbols.miss)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    else if (enemyArray[i, j] == symbols.hit)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (enemyArray[i, j] == symbols.ship)
                    {
                        Console.Write(symbols.voidCell);
                    }
                    else
                    {
                        Console.Write(enemyArray[i, j]);
                    }
                }
                Console.WriteLine();
            }
        }

        private void The_End()
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
                for (int i = playerCoords.playerY + y + 1; i >= playerCoords.playerY + y - 1; i--)
                {
                    for (int j = playerCoords.playerX + x - 1; j <= playerCoords.playerX + x + 1; j++)
                    {
                        if (i >= 0 && j >= 0 && i < arraySize && j < arraySize && array[i, j] == symbols.ship)
                        {
                            Console.WriteLine("You can't place ship in this place");
                            return false;
                        }
                    }
                }
                //int p = dictionary[direction];
                //Vector<int> fr = new Vector<int>(arr,y);
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