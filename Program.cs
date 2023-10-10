Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.CursorVisible = false;
Game game = new(new Settings());
game.Run();

//===========================================================//
//+++ GAME OBJECTS +++||-------------------------------------//
//===========================================================//

class Game
{
    private readonly Settings _settings;
    private bool _gameOver = false;
    private bool _paused = true;
    private int _score = 0;
    private Direction _direction;
    private Snake _snake;
    private Map _map;
    private Apple _apple;

    public Game(Settings settings)
    {
        _settings = settings;
        _snake = new(settings);
        _apple = new(settings);
        _map = new(settings);
    }

    ConsoleKey? GetKey() => Console.KeyAvailable ? Console.ReadKey(intercept: true).Key : null;

    Direction? GetDirection(ConsoleKey? key) =>
        key switch
        {
            ConsoleKey.A or ConsoleKey.LeftArrow => Direction.Left,
            ConsoleKey.D or ConsoleKey.RightArrow => Direction.Right,
            ConsoleKey.W or ConsoleKey.UpArrow => Direction.Up,
            ConsoleKey.S or ConsoleKey.DownArrow => Direction.Down,
            _ => null,
        };

    public void Run()
    {
        while (!_gameOver)
        {
            if (_snake.IsDead)
            {
                break;
            }

            Console.Clear();
            Helper.PrintAt(new Pos(0, _settings.Height + 3), $"SCORE: {_score}");

            var newDirection = GetDirection(GetKey()) ?? _direction;
            _direction = newDirection.IsOppositeDirection(_direction) ? _direction : newDirection;
            _snake.Move(_direction);

            if (_snake.Head == _apple.Position)
            {
                _score += 1;
                _snake.Grow();
                // Spawn apple until it finds a valid position (outside of snake)
                do
                {
                    _apple.Spawn();
                } while (_snake.Head == _apple.Position || _snake.Body.Contains(_apple.Position));
            }

            // Render
            _apple.Render();
            _snake.Render();
            _map.Render();

            if (_paused)
            {
                _direction = GetDirection(Console.ReadKey().Key) ?? _direction;
                _paused = false;
            }

            Thread.Sleep(_settings.TickRate);
        }
        Helper.PrintAt(
            new Pos((_settings.Width + 2) / 2, (_settings.Height + 1) / 2),
            "YOU DEAD",
            centered: true
        );
        Console.ReadLine();
        Console.SetCursorPosition(0, 0);
        Console.Clear();
    }
}

class Snake
{
    private readonly Settings _settings;

    public Snake(Settings settings)
    {
        _settings = settings;
        Head = new Pos(settings.Width / 2 + 2, settings.Height / 2 + 1);
    }

    public bool IsDead { get; private set; } = false;
    public Pos Head { get; private set; }
    public List<Pos> Body { get; private set; } = new();

    public void Move(Direction direction)
    {
        Body.Insert(0, Head);
        if (Body.Count > 0)
        {
            Body.RemoveAt(Body.Count() - 1);
        }

        var newHead = direction switch
        {
            Direction.Left => Head.MoveRelative(-2, 0),
            Direction.Right => Head.MoveRelative(2, 0),
            Direction.Up => Head.MoveRelative(0, -1),
            Direction.Down => Head.MoveRelative(0, 1),
            _ => Head.MoveRelative(0, 0),
        };

        Head = new Pos(
            WrapIfOutOfMap(newHead.X, 2, _settings.Width),
            WrapIfOutOfMap(newHead.Y, 1, _settings.Height)
        );

        if (Body.Contains(Head))
        {
            IsDead = true;
        }
    }

    private int WrapIfOutOfMap(int input, int stepSize, int max) =>
        input < stepSize
            ? max
            : input > max
                ? stepSize
                : input;

    public void Render()
    {
        Helper.PrintAt(Head, "🟨");
        foreach (var pos in Body)
        {
            Helper.PrintAt(pos, "🟩");
        }
    }

    public void Grow()
    {
        Body.Add(Head);
    }
}

class Apple
{
    private readonly Settings _settings;

    public Apple(Settings settings)
    {
        _settings = settings;
        Spawn();
    }

    public Pos Position { get; private set; }

    public void Spawn()
    {
        Position = new Pos(
            Random.Shared.Next(2, (_settings.Width + 2) / 2) * 2,
            Random.Shared.Next(1, (_settings.Height + 1) / 2) * 2
        );
    }

    public void Render()
    {
        Helper.PrintAt(Position, "🍎");
    }
}

class Map
{
    private readonly Settings _settings;

    public Map(Settings settings)
    {
        _settings = settings;
    }

    public void Render()
    {
        // Top & bottom walls
        Helper.PrintAt(new Pos(0, 0), "█".Repeat(_settings.Width + 2));
        Helper.PrintAt(new Pos(0, _settings.Height + 1), "█".Repeat(_settings.Width + 2));

        // Left & right walls
        Helper.PrintAt(new Pos(0, 0), "██\n".Repeat(_settings.Height + 1));
        for (var i = 0; i <= _settings.Height + 1; i++)
        {
            Helper.PrintAt(new Pos(_settings.Width + 2, i), "██");
        }
    }
}

//===========================================================//
//+++ GAME DATA +++||----------------------------------------//
//===========================================================//

public record Settings(int TickRate = 100, int Width = 32, int Height = 12);

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
}

public record struct Pos(int X, int Y)
{
    public Pos MoveRelative(int x, int y) => new Pos(X + x, Y + y);
}

//===========================================================//
//+++ HELPERS AND EXTENSIONS +++||---------------------------//
//===========================================================//

public static class Extensions
{
    public static bool IsOppositeDirection(this Direction thisDir, Direction otherDir)
    {
        return thisDir switch
        {
            Direction.Left when otherDir is Direction.Right => true,
            Direction.Right when otherDir is Direction.Left => true,
            Direction.Up when otherDir is Direction.Down => true,
            Direction.Down when otherDir is Direction.Up => true,
            _ => false,
        };
    }

    public static string Repeat(this string text, int n)
    {
        var textAsSpan = text.AsSpan();
        var span = new Span<char>(new char[textAsSpan.Length * n]);
        for (var i = 0; i < n; i++)
        {
            textAsSpan.CopyTo(span.Slice(i * textAsSpan.Length, textAsSpan.Length));
        }

        return span.ToString();
    }
}

public static class Helper
{
    public static void PrintAt(Pos pos, string symbol, bool centered = false)
    {
        if (centered)
        {
            Console.SetCursorPosition(pos.X - (symbol.Length / 2), pos.Y);
        }
        else
        {
            Console.SetCursorPosition(pos.X, pos.Y);
        }
        Console.Write(symbol);
    }
}
