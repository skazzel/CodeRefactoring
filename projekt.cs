using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            SnakeGame game = new SnakeGame(32, 16);
            game.Start();
        }
    }

    class SnakeGame
    {
        private const int InitialScore = 5;
        private const int GameSpeed = 200;

        private int screenWidth;
        private int screenHeight;
        private int score;
        private bool gameOver;
        private Direction direction;
        private Pixel head;
        private List<Pixel> body;
        private Random random;
        private Pixel berry;

        public SnakeGame(int width, int height)
        {
            if (OperatingSystem.IsWindows())
            {
                Console.WindowHeight = height;
                Console.WindowWidth = width;
            }

            screenWidth = width;
            screenHeight = height;
            InitializeGame();
        }

        private void InitializeGame()
        {
            head = new Pixel(screenWidth / 2, screenHeight / 2, ConsoleColor.Red);
            body = new List<Pixel>();
            score = InitialScore;
            gameOver = false;
            direction = Direction.RIGHT;
            random = new Random();
            SpawnBerry();
        }

        public void Start()
        {
            while (!gameOver)
            {
                ClearScreen();
                DrawBorders();
                DrawBerry();
                DrawSnake();
                HandleInput();
                UpdateGame();
                Thread.Sleep(GameSpeed);
            }

            ShowGameOver();
        }

        private void ClearScreen()
        {
            Console.Clear();
        }

        private void DrawBorders()
        {
            for (int i = 0; i < screenWidth; i++)
            {
                DrawPixel(i, 0, "■");
                DrawPixel(i, screenHeight - 1, "■");
            }

            for (int i = 0; i < screenHeight; i++)
            {
                DrawPixel(0, i, "■");
                DrawPixel(screenWidth - 1, i, "■");
            }
        }

        private void DrawPixel(int x, int y, string symbol)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(symbol);
        }

        private void DrawBerry()
        {
            DrawPixel(berry.X, berry.Y, "●");

            if (head.X == berry.X && head.Y == berry.Y)
            {
                score++;
                SpawnBerry();
            }
        }

        private void SpawnBerry()
        {
            berry = new Pixel(random.Next(1, screenWidth - 1), random.Next(1, screenHeight - 1), ConsoleColor.Green);
        }

        private void DrawSnake()
        {
            DrawPixel(head.X, head.Y, "■", head.Color);

            foreach (var segment in body)
            {
                DrawPixel(segment.X, segment.Y, "■");
            }
        }

        private void DrawPixel(int x, int y, string symbol, ConsoleColor color)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            Console.Write(symbol);
            Console.ResetColor();
        }

        private void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (direction != Direction.DOWN) direction = Direction.UP;
                        break;
                    case ConsoleKey.DownArrow:
                        if (direction != Direction.UP) direction = Direction.DOWN;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (direction != Direction.RIGHT) direction = Direction.LEFT;
                        break;
                    case ConsoleKey.RightArrow:
                        if (direction != Direction.LEFT) direction = Direction.RIGHT;
                        break;
                }
            }
        }

        private void UpdateGame()
        {
            body.Add(new Pixel(head.X, head.Y, ConsoleColor.Yellow));

            switch (direction)
            {
                case Direction.UP:
                    head.Y--;
                    break;
                case Direction.DOWN:
                    head.Y++;
                    break;
                case Direction.LEFT:
                    head.X--;
                    break;
                case Direction.RIGHT:
                    head.X++;
                    break;
            }

            if (body.Count > score)
            {
                body.RemoveAt(0);
            }

            if (IsCollision())
            {
                gameOver = true;
            }
        }

        private bool IsCollision()
        {
            if (head.X <= 0 || head.X >= screenWidth - 1 || head.Y <= 0 || head.Y >= screenHeight - 1)
            {
                return true;
            }

            foreach (var segment in body)
            {
                if (head.X == segment.X && head.Y == segment.Y)
                {
                    return true;
                }
            }

            return false;
        }

        private void ShowGameOver()
        {
            Console.SetCursorPosition(screenWidth / 2 - 5, screenHeight / 2);
            Console.WriteLine($"Game Over! Score: {score}");
        }
    }

    class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ConsoleColor Color { get; set; }

        public Pixel(int x, int y, ConsoleColor color)
        {
            X = x;
            Y = y;
            Color = color;
        }
    }

    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}
