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

    class ShipBattle
    {
        private Direction direction;

        private readonly int arraySize = 10;

        private bool hints = true;

        private bool battle;
        private bool theEnd;
        private bool canSeeBotArea = false;

        private int playersAlive;
        private int botLevel = 1;
        private int misses;
        private int roundsForWin = 1;

        private Player[] players = new Player[2];

        private int[] limitForShips = new int[5];

        private int playerTurn;
        private int nextPlayer;

        private ConsoleKeyInfo key;

        static void Main()
        {
            var dz = new ShipBattle();
            dz.Lobby();
        }

        private void LobbyOutput(bool mainMenu)
        {
            Console.Clear();
            if(mainMenu)
            {
                Console.WriteLine("Press S to change number of players");
                Console.WriteLine($"(Current number of players = {players.Length})");
                Console.WriteLine();
                Console.WriteLine("Press C to change bot settings");
                Console.WriteLine();
                Console.WriteLine("Press R to change number of rounds to win");
                Console.WriteLine($"(Current rounds for win = {roundsForWin})");
                Console.WriteLine();
                Console.WriteLine("Press Enter to start game");
            }
            else
            {
                switch (key.Key)
                {
                    case ConsoleKey.S:
                        Console.WriteLine("Enter number of players");
                        Console.WriteLine("Min - 2, Max - 4");
                        Console.WriteLine($"(Current number of players = {players.Length})");
                        break;
                    case ConsoleKey.R:
                        Console.WriteLine("Enter number of won rounds that needed for win in the game!");
                        Console.WriteLine("You can enter number from 1 to 5");
                        Console.WriteLine($"(Current rounds for win = {roundsForWin})");
                        break;
                    case ConsoleKey.C:
                        for (int i = 0; i < players.Length; i++)
                        {
                            if (players[i].bot)
                                Console.WriteLine($"Player {i + 1} is a bot");
                            else
                                Console.WriteLine($"Player {i + 1} is a human");
                        }
                        Console.WriteLine();
                        if (canSeeBotArea)
                            Console.WriteLine("Press S to don't see bot area");
                        else
                            Console.WriteLine("Press S to see bot area");
                        Console.WriteLine("Press L to change bot level");
                        break;
                    case ConsoleKey.L:
                        Console.WriteLine($"Current level of bots = {botLevel}");
                        Console.WriteLine("Min = 1, Max = 3");
                        break;
                    default:
                        Console.WriteLine("Wrong command, try again");
                        Thread.Sleep(1200);
                        break;
                }
                Console.WriteLine();
                Console.WriteLine("Press Enter to exit");
            }        
        }

        private void Lobby()
        {
            bool lobby = true;
            bool mainMenu;
            while (lobby)
            {
                mainMenu = true;
                LobbyOutput(mainMenu);
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    lobby = false;
                }
                else
                {
                    mainMenu = false;
                }
                while (!mainMenu)
                {
                    LobbyOutput(mainMenu);
                    switch (key.Key)
                    {
                        case ConsoleKey.R:
                            SetNumberOfRoundsToWin();
                            break;
                        case ConsoleKey.C:
                            SetBots();
                            break;
                        case ConsoleKey.S:
                            SetNumberOfPlayers();
                            break;
                        case ConsoleKey.L:
                            BotSettings();
                            break;
                        case ConsoleKey.Enter:
                            mainMenu = true;
                            break;
                    }
                }                
            }
            for (int i = 0; i < players.Length; i++)
            {
                players[i].wins = 0;
            }
            Start();
        }

        private void SetNumberOfPlayers()
        {
            int num = 0;
            num = EnterNum(num);
            if (num > 1 && num < 5)
            {
                Array.Resize(ref players, Convert.ToInt32(num));
            }
        }

        private void SetNumberOfRoundsToWin()
        {
            roundsForWin = EnterNum(roundsForWin);
        }

        private void SetBots()
        {
            int numberOfPlayer = 0;
            numberOfPlayer = EnterNum(numberOfPlayer) - 1;
            if (players.Length > numberOfPlayer && numberOfPlayer >= 0)
            {
                int num = Convert.ToInt32(numberOfPlayer);
                players[num].bot = !players[num].bot;
            }
            switch (key.Key)
            {
                case ConsoleKey.S:
                    canSeeBotArea = !canSeeBotArea;
                    break;
                case ConsoleKey.L:
                    BotSettings();
                    break;
            }
        }

        private void BotSettings()
        {
            int num = 0;
            bool exit = false;
            while (!exit)
            {
                num = EnterNum(num);
                if (num > 0 && num <= 3)
                {
                    botLevel = Convert.ToInt32(num);
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    exit = true;
                }
            }
        }

        private int EnterNum(int num)
        {
            key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.NumPad1:
                case ConsoleKey.D1:
                    return 1;
                case ConsoleKey.NumPad2:
                case ConsoleKey.D2:
                    return 2;
                case ConsoleKey.NumPad3:
                case ConsoleKey.D3:
                    return 3;
                case ConsoleKey.NumPad4:
                case ConsoleKey.D4:
                    return 4;
                case ConsoleKey.NumPad5:
                case ConsoleKey.D5:
                    return 5;
                default:
                    return num;
            }
        }

        private void Start()
        {
            Console.Clear();
            for (int i = 0; i < players.Length; i++)
            {
                players[i].array = new string[arraySize, arraySize];
                players[i].numberOfLivingShipCells = 0;
                players[i].playerX = 0;
                players[i].playerY = 0;
                GenerateArea(i);
            }
            playersAlive = players.Length;
            playerTurn = 0;
            nextPlayer = playerTurn + 1;
            theEnd = false;
            battle = false;
            misses = 0;
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
                    players[playerNumber].array[i, j] = Symbols.voidCell;
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
            if (!players[playerTurn].bot)
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
                players[playerTurn].numberOfLivingShipCells += size;
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
                    if (playerTurn == players.Length - 1)
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
            else if (!players[playerTurn].bot)
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
            int x = players[playerTurn].playerX, y = players[playerTurn].playerY;
            for (int l = 0; l < size; l++)
            {
                players[playerTurn].array[y, x] = Symbols.ship;
                x = ChangeX(x);
                y = ChangeY(y);
            }
        }

        private void GameLoop()
        {
            int size = 1;
            while (!theEnd)
            {
                if (!players[playerTurn].bot)
                {
                    key = Console.ReadKey();
                    size = EnterSize(size);
                    DirectionChange();
                    if (ThisCoordInArray(ChangeX(players[playerTurn].playerX)))
                        players[playerTurn].playerX = ChangeX(players[playerTurn].playerX);
                    if (ThisCoordInArray(ChangeY(players[playerTurn].playerY)))
                        players[playerTurn].playerY = ChangeY(players[playerTurn].playerY);
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
                    players[playerTurn].playerX = rand.Next(0, arraySize);
                    players[playerTurn].playerY = rand.Next(0, arraySize);
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
                if (!players[playerTurn].bot || (players[playerTurn].bot && canSeeBotArea))
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
                players[nextPlayer].numberOfLivingShipCells--; // tyt bez etogo nikak
                if (NextPlayer().numberOfLivingShipCells <= 0)
                {
                    Console.WriteLine($"{nextPlayer + 1} player is died...");
                    playersAlive--;
                    nextPlayer = ChangeTurn(nextPlayer);
                }
            }
            else if (NextPlayer().array[CurrentPlayer().playerY, CurrentPlayer().playerX] == Symbols.voidCell)
            {
                misses++;
                Console.WriteLine("You missed :(");
                NextPlayer().array[CurrentPlayer().playerY, CurrentPlayer().playerX] = Symbols.miss;
                if ((CurrentPlayer().bot && misses >= botLevel) || !CurrentPlayer().bot)
                {
                    playerTurn = ChangeTurn(playerTurn);
                    nextPlayer = ChangeTurn(playerTurn);
                }
            }
            else
            {
                Console.WriteLine("You can't shoot in this place");
            }
            Thread.Sleep(900);
        }

        private Player NextPlayer()
            => players[nextPlayer];

        private Player CurrentPlayer()
            => players[playerTurn];

        private int ChangeTurn(int num)
        {
            for (int i = 0; i < players.Length; i++)
            {
                num++;
                if (num >= players.Length)
                {
                    num = 0;
                }
                if (players[num].numberOfLivingShipCells > 0 && battle || !battle)
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
            misses = 0;
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
                    ChangeForegroundColor(players[numOfPlayer].array[i,j]);
                    if (numOfPlayer != playerTurn && players[numOfPlayer].array[i, j] == Symbols.ship)
                        Console.Write(Symbols.voidCell);
                    else
                        Console.Write(players[numOfPlayer].array[i, j]);
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private bool ArrayCoordsEqualsPlayerCoords(int i, int j)
            => i == players[playerTurn].playerY && j == players[playerTurn].playerX;

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
            players[playerTurn].wins++;
            if (players[playerTurn].wins >= roundsForWin)
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
                Console.WriteLine();
                Console.WriteLine("Wins of players:");
                for (int i = 0; i < players.Length; i++)
                {
                    Console.WriteLine($"Player {i + 1}: {players[i].wins} wins");
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
            int x = players[playerTurn].playerX;
            int y = players[playerTurn].playerY;            
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
                        if (ThisCoordInArray(i) && ThisCoordInArray(j) && players[playerTurn].array[i, j] == Symbols.ship)
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