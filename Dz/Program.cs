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
        public bool bot;
        public int wins;

        public Player(string[,] array, int playerX,int playerY, int numberOfLivingShipCells, bool bot, int wins)
        {
            this.array = array;
            this.playerX = playerX;
            this.playerY = playerY;
            this.numberOfLivingShipCells = numberOfLivingShipCells;
            this.bot = bot;
            this.wins = wins;
        }
    }
    public class Symbols
    {
        public const string ship = "█";
        public const string voidCell = " ";
        public const string miss = "#";
        public const string hit = "X";
    }
    public enum Direction
    {
        up,
        left,
        right,
        down,
        nothing
    }

    class Dz
    {
        private Direction direction;

        private readonly int arraySize = 10;

        private bool hints = true;

        private bool battle;
        private bool theEnd;
        private bool canSeeBotArea = false;
        private int playersAlive;
        private int roundsForWin = 0;

        private Player[] player = new Player[SetNumberOfPlayers()];
        
        private int[] limitForShips = new int[5];

        private int playerTurn;
        private int nextPlayer;

        private ConsoleKeyInfo key;

        static void Main()
        {
            var dz = new Dz();            
            dz.Lobby();
        }

        static int SetNumberOfPlayers()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter number of players");
                Console.WriteLine("Min - 2, Max - 4");
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        return 2;
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        return 3;
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        return 4;
                }
            }
        }

        private void Lobby()
        {
            while (roundsForWin <= 0)
            {
                Console.Clear();
                Console.WriteLine("Enter number of won rounds that needed for win in the game!");
                Console.WriteLine("You can enter number from 1 to 5");
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        roundsForWin = 1;
                        break;
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        roundsForWin = 2;
                        break;
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        roundsForWin = 3;
                        break;
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        roundsForWin = 4;
                        break;
                    case ConsoleKey.NumPad5:
                    case ConsoleKey.D5:
                        roundsForWin = 5;
                        break;
                }
            }
            bool start = false;
            while (!start)
            {
                int? num = null;
                Console.Clear();
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i].bot)
                        Console.WriteLine($"Player {i + 1} is a bot");
                    else
                        Console.WriteLine($"Player {i + 1} is a human");
                }
                Console.WriteLine();
                if (canSeeBotArea)
                    Console.WriteLine("Press S to don't see bot area");
                else
                    Console.WriteLine("Press S to see bot area");
                Console.WriteLine("Press Enter to start game");
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        num = 0;
                        break;
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        num = 1;
                        break;
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        num = 2;
                        break;
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        num = 3;
                        break;
                    case ConsoleKey.Enter:
                        start = true;
                        break;
                    case ConsoleKey.S:
                        canSeeBotArea = !canSeeBotArea;
                        break;
                }
                if (num != null && player.Length > num)
                    player[Convert.ToInt32(num)].bot = !player[Convert.ToInt32(num)].bot;
            }
            playersAlive = player.Length;
            for (int i = 0; i < player.Length; i++)
            {
                player[i].wins = 0;
            }
            Start();
        }

        private void Start()
        {
            Console.Clear();
            for (int i = 0; i < player.Length; i++)
            {
                player[i].array = new string[arraySize, arraySize];
                player[i].numberOfLivingShipCells = 0;
                player[i].playerX = 0;
                player[i].playerY = 0;
                GenerateArea(i);
            }
            playerTurn = 0;
            nextPlayer = playerTurn + 1;
            theEnd = false;
            battle = false;
            GenerateLimitForShipsNumber();
            DrawArea(playerTurn);
            Information();
            GameLoop();
        }

        private void GenerateArea(int playerNumber)
        {
            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    player[playerNumber].array[i, j] = Symbols.voidCell;
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
            if (!player[playerTurn].bot)
            {
                Console.WriteLine("Press arrow for choosing direction");
                key = Console.ReadKey();
                DirectionChange();
            }
            else
            {
                SetRandomDirection();
            }
            if (direction != Direction.nothing && CanYouSpawnShip(size))
            {
                limitForShips[size]--;
                player[playerTurn].numberOfLivingShipCells += size;
                GenerateShipCells(size);
                int j = 1;
                for (int i = 1; i < limitForShips.Length; i++)
                {
                    if (!DoYouHaveLimitForThisShip(i))
                    {
                        j++;
                    }
                }
                if (j >= limitForShips.Length)
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
            else if (!player[playerTurn].bot)
            {
                Thread.Sleep(1169);
            }
        }

        private void DirectionChange()
        {
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    direction = Direction.left;
                    break;
                case ConsoleKey.RightArrow:
                    direction = Direction.right;
                    break;
                case ConsoleKey.UpArrow:
                    direction = Direction.up;
                    break;
                case ConsoleKey.DownArrow:
                    direction = Direction.down;
                    break;
                default:
                    direction = Direction.nothing;
                    break;
            }
        }

        private void GenerateShipCells(int size)
        {
            int x = player[playerTurn].playerX, y = player[playerTurn].playerY;
            for (int l = 0; l < size; l++)
            {
                player[playerTurn].array[y, x] = Symbols.ship;
                x = ChangeX(x);
                y = ChangeY(y);
            }
        }

        private void GameLoop()
        {
            int size = 1;
            while (!theEnd)
            {
                if (!player[playerTurn].bot)
                {
                    key = Console.ReadKey();
                    size = EnterSize(size);
                    DirectionChange();
                    if (ThisCoordInArray(ChangeX(player[playerTurn].playerX)))
                        player[playerTurn].playerX = ChangeX(player[playerTurn].playerX);
                    if (ThisCoordInArray(ChangeY(player[playerTurn].playerY)))
                        player[playerTurn].playerY = ChangeY(player[playerTurn].playerY);
                    switch (key.Key)
                    {
                        case ConsoleKey.H:
                            hints = !hints;
                            break;
                        case ConsoleKey.R:
                            Start();
                            break;
                        case ConsoleKey.S:
                            Lobby();
                            break;
                        case ConsoleKey.Enter:
                            if (battle)
                                Shoot();
                            else
                                GenerationShips(size);
                            break;
                    }
                }
                else
                {                    
                    Random rand = new Random();
                    player[playerTurn].playerX = rand.Next(0, arraySize);
                    player[playerTurn].playerY = rand.Next(0, arraySize);
                    Console.Clear();
                    if (battle)
                        DrawArea(nextPlayer);
                    if (canSeeBotArea)
                    {
                        DrawArea(playerTurn);
                        Information();
                    }
                    if (battle)
                    {
                        Thread.Sleep(400);
                        Shoot();
                    }
                    else
                    {
                        GenerationShips(size);
                    }
                    size = rand.Next(1, 5);
                }                
                Console.Clear();
                if (battle)                
                    DrawArea(nextPlayer);                
                if (!player[playerTurn].bot || (player[playerTurn].bot && canSeeBotArea))
                {
                    DrawArea(playerTurn);
                    Information();
                }
                else if (!battle)
                {
                    Console.WriteLine("Wait...");
                }
                if (playersAlive <= 1)
                {
                    theEnd = true;
                }
            }
            TheEnd();
        }

        private void SetRandomDirection()
        {
            Random rand = new Random();
            int randDir = rand.Next(0, 100);
            if (randDir > 75)
                direction = Direction.right;
            else if (randDir > 50)
                direction = Direction.left;
            else if (randDir > 25)
                direction = Direction.up;
            else
                direction = Direction.down;
        }

        private bool ThisCoordInArray(int coord)
            => coord >= 0 && coord < arraySize;

        private void Shoot()
        {
            if (NextPlayer().array[CurrentPlayer().playerY, CurrentPlayer().playerX] == Symbols.ship)
            {
                Console.WriteLine("You hited an enemy!!!");
                Console.WriteLine("You can walk again!!!");
                NextPlayer().array[CurrentPlayer().playerY, CurrentPlayer().playerX] = Symbols.hit;
                player[nextPlayer].numberOfLivingShipCells--; // tyt bez etogo nikak
                if (NextPlayer().numberOfLivingShipCells <= 0)
                {
                    Console.WriteLine($"{nextPlayer + 1} player is died...");
                    playersAlive--;
                    nextPlayer = ChangeTurn(nextPlayer);
                }
            }
            else if (NextPlayer().array[CurrentPlayer().playerY, CurrentPlayer().playerX] == Symbols.voidCell)
            {
                Console.WriteLine("You missed :(");
                NextPlayer().array[CurrentPlayer().playerY, CurrentPlayer().playerX] = Symbols.miss;
                playerTurn = ChangeTurn(playerTurn);
                nextPlayer = ChangeTurn(playerTurn);
            }
            else
            {
                Console.WriteLine("You can't shoot in this place");
            }
            Thread.Sleep(900);
        }

        private Player NextPlayer()
            => player[nextPlayer];

        private Player CurrentPlayer()
            => player[playerTurn];

        private int ChangeTurn(int num)
        {
            for (int i = 0; i < player.Length; i++)
            {
                num++;
                if (num >= player.Length)
                {
                    num = 0;
                }
                if (player[num].numberOfLivingShipCells > 0 && battle || !battle)
                {
                    break;
                }
            }
            if (num == nextPlayer)
            {
                Thread.Sleep(1200);
                Console.Clear();
                Console.WriteLine("Press any button to change player");
                Console.WriteLine($"Next player: {nextPlayer + 1}");
                var key = Console.ReadKey();
            }
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
                    if (ArrayCoordsEqualsPlayerCoords(i, j) && ThisArrayNeededForDrawingPlayer(numOfPlayer))
                    {
                        Console.BackgroundColor = ConsoleColor.Red;                        
                    }
                    ChangeForegroundColor(player[numOfPlayer].array[i,j]);
                    if (numOfPlayer != playerTurn && player[numOfPlayer].array[i, j] == Symbols.ship)
                        Console.Write(Symbols.voidCell);
                    else
                        Console.Write(player[numOfPlayer].array[i, j]);
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private bool ArrayCoordsEqualsPlayerCoords(int i, int j)
            => i == player[playerTurn].playerY && j == player[playerTurn].playerX;

        private bool ThisArrayNeededForDrawingPlayer(int numOfPlayer)
            => (!battle && numOfPlayer == playerTurn) || (battle && numOfPlayer == nextPlayer);

        private ConsoleColor ChangeForegroundColor(string color)
            =>Console.ForegroundColor = color switch
            {
                Symbols.ship => ConsoleColor.Gray,
                Symbols.miss => ConsoleColor.DarkBlue,
                Symbols.hit => ConsoleColor.DarkRed,
                _ => ConsoleColor.White
            };

        private void TheEnd()
        {
            Console.Clear();
            if (player[playerTurn].wins >= roundsForWin)
            {
                Console.Clear();
                Console.WriteLine($"{playerTurn + 1} player won in this game!!!");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Lobby();
            }
            else
            {
                Console.WriteLine($"{playerTurn + 1} player won this round!!!");
                player[playerTurn].wins++;
                Console.WriteLine();
                Console.WriteLine("Wins of players:");
                for (int i = 0; i < player.Length; i++)
                {
                    Console.WriteLine($"Player {i + 1}: {player[i].wins} wins");
                }
                Console.WriteLine();
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
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
            int x = player[playerTurn].playerX;
            int y = player[playerTurn].playerY;            
            for (int l = 0; l < shipSize; l++)
            {
                if (!ThisCoordInArray(x) || !ThisCoordInArray(y))
                {
                    Console.WriteLine("You can't place ship in this place");
                    return false;
                }
                for (int i = y - 1; i <= y + 1; i++)
                {                    
                    for (int j = x - 1; j <= x + 1; j++)
                    {
                        if (ThisCoordInArray(i) && ThisCoordInArray(j) && player[playerTurn].array[i, j] == Symbols.ship)
                        {
                            Console.WriteLine("You can't place ship in this place");
                            return false;
                        }
                    }
                }
                x = ChangeX(x);
                y = ChangeY(y);
            }
            return true;
        }

        private int ChangeX(int num)
        {
            var change = new Dictionary<Direction, int>()
            {
                [Direction.right] = num + 1,
                [Direction.left] = num - 1
            };
            try
            {
                return change[direction];
            }
            catch
            { 
                return num;             
            }
            
        }

        private int ChangeY(int num)
        {
            var change = new Dictionary<Direction, int>()
            {
                [Direction.down] = num + 1,
                [Direction.up] = num - 1
            };
            try
            {
                return change[direction];
            }
            catch
            {
                return num;
            }
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
            Console.WriteLine($"Press R to restart with the same enemies");
            Console.WriteLine();
            Console.WriteLine($"Press S to restart the game with new enemies");
        }

        private void Hints()
        {
            Console.WriteLine("-------------------------------------------Hints--------------------------------------------");
            Console.WriteLine($"Red cell is you, you can move with arrows");
            if (!battle)
            {
                Console.WriteLine($"For placing ship press enter and arrow for direction");
                Console.WriteLine($"For changing size of ship press button with needed number(min - 1, max - 4)");
            }
            else
            {
                Console.WriteLine("Press Enter to shoot");
                Console.WriteLine("You can shoot only void cells");
            }
            Console.WriteLine("--------------------------------------------------------------------------------------------");
        }
    }
}