using System;
using System.Threading;

namespace RobotCleaner
{
    // Represents the map grid for the robot
    public class Map
    {
        private enum CellType { Empty, Dirt, Obstacle, Cleaned }
        private readonly CellType[,] _grid;

        public int Width { get; }
        public int Height { get; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            _grid = new CellType[width, height];

            // Initialize all cells as Empty
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _grid[x, y] = CellType.Empty;
                }
            }
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool IsDirt(int x, int y)
        {
            return IsInBounds(x, y) && _grid[x, y] == CellType.Dirt;
        }

        public bool IsObstacle(int x, int y)
        {
            return IsInBounds(x, y) && _grid[x, y] == CellType.Obstacle;
        }

        public void AddObstacle(int x, int y)
        {
            if (IsInBounds(x, y))
                _grid[x, y] = CellType.Obstacle;
        }

        public void AddDirt(int x, int y)
        {
            if (IsInBounds(x, y))
                _grid[x, y] = CellType.Dirt;
        }

        public void Clean(int x, int y)
        {
            if (IsInBounds(x, y))
                _grid[x, y] = CellType.Cleaned;
        }

        public void Display(int robotX, int robotY)
        {
            Console.Clear();
            Console.WriteLine("Vacuum Cleaner Robot Simulation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Legends: #=Obstacle, D=Dirt, .=Empty, R=Robot, C=Cleaned");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x == robotX && y == robotY)
                    {
                        Console.Write("R ");
                    }
                    else
                    {
                        switch (_grid[x, y])
                        {
                            case CellType.Empty: Console.Write(". "); break;
                            case CellType.Dirt: Console.Write("D "); break;
                            case CellType.Obstacle: Console.Write("# "); break;
                            case CellType.Cleaned: Console.Write("C "); break;
                        }
                    }
                }
                Console.WriteLine();
            }

            Thread.Sleep(200);
        }
    }

    // Strategy interface for cleaning algorithms
    public interface IStrategy
    {
        void Clean(Robot robot);
    }

    // Robot class representing the cleaner
    public class Robot
    {
        private readonly Map _map;
        private readonly IStrategy _strategy;

        public int X { get; private set; }
        public int Y { get; private set; }

        public Map Map => _map;

        public Robot(Map map, IStrategy strategy)
        {
            _map = map;
            _strategy = strategy;
            X = 0;
            Y = 0;
        }

        public bool Move(int newX, int newY)
        {
            if (_map.IsInBounds(newX, newY) && !_map.IsObstacle(newX, newY))
            {
                X = newX;
                Y = newY;
                _map.Display(X, Y);
                return true;
            }
            return false;
        }

        public void CleanCurrentSpot()
        {
            if (_map.IsDirt(X, Y))
            {
                _map.Clean(X, Y);
                _map.Display(X, Y);
            }
        }

        public void StartCleaning()
        {
            _strategy.Clean(this);
        }
    }

    // Example cleaning strategy: simple zigzag pattern
    public class SomeStrategy : IStrategy
    {
        public void Clean(Robot robot)
        {
            int direction = 1; // 1 = right, -1 = left

            for (int y = 0; y < robot.Map.Height; y++)
            {
                int startX = (direction == 1) ? 0 : robot.Map.Width - 1;
                int endX = (direction == 1) ? robot.Map.Width : -1;

                for (int x = startX; x != endX; x += direction)
                {
                    robot.Move(x, y);
                    robot.CleanCurrentSpot();
                }

                // Change direction after each row
                direction *= -1;
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Initialize robot");

            IStrategy strategy = new SomeStrategy();
            Map map = new Map(20, 10);

            map.AddDirt(5, 3);
            map.AddDirt(10, 8);
            map.AddObstacle(2, 5);
            map.AddObstacle(12, 1);

            map.Display(11, 8);

            Robot robot = new Robot(map, strategy);
            robot.StartCleaning();

            Console.WriteLine("Done.");
        }
    }
}