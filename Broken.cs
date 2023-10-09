// // Create game settings record
// Settings settings = new(width: 48, height: 16, bgChar: ' ', wallChar: '#', headChar: 'O', bodyChar: '*', appleChar: '@');

// // Create new game instance
// Game game = new(settings);

// var snake = new Snake(new Point(8, 8));

// while (true)
// {
//     var grid = game.CreateFrame();
//     snake.Move(Direction.Left, grid);
//     game.DrawFrame(grid);
//     Console.ReadLine();
//     Console.Clear();
// }

// //===========================================================//
// //+++ GAME OBJECTS +++||-------------------------------------//
// //===========================================================//

// record Settings(
//     int width,
//     int height,
//     char bgChar,
//     char wallChar,
//     char headChar,
//     char bodyChar,
//     char appleChar);

// enum Tile
// {
//     Empty,
//     Wall,
//     Head,
//     Body,
//     Apple,
// }

// enum Direction
// {
//     Left,
//     Right,
//     Up,
//     Down,
// }

// struct Point
// {
//     public int row;
//     public int col;

//     public Point(int row, int col)
//     {
//         this.row = row;
//         this.col = col;
//     }
// }

// //===========================================================//
// //+++ GAME CLASS +++||---------------------------------------//
// //===========================================================//

// class Game
// {
//     private readonly Settings _settings;

//     public Game(Settings settings) { _settings = settings; }

//     /// <summary>
//     /// Creates the game map as a 2d array and adds walls
//     /// </summary>
//     public List<List<Tile>> CreateFrame()
//     {
//         List<List<Tile>> grid = new();
//         for (int row = -1; row < _settings.height + 1; row++)
//         {
//             List<Tile> rowList = new();
//             for (int col = -1; col < _settings.width + 1; col++)
//             {
//                 // Draw top/bottom walls
//                 if (row < 0 || row >= _settings.height)
//                 {
//                     rowList.Add(Tile.Wall);
//                     continue;
//                 }
//                 // Draw side walls
//                 if (col < 0 || col >= _settings.width)
//                 {
//                     rowList.Add(Tile.Wall);
//                     continue;
//                 }
//                 rowList.Add(Tile.Empty);
//             }
//             grid.Add(rowList);
//         }
//         return grid;
//     }

//     /// <summary>
//     /// Draws the supplied game frame to the console.
//     /// </summary>
//     /// <param name="frame"></param>
//     public void DrawFrame(List<List<Tile>> grid)
//     {
//         Console.Clear();

//         foreach (var row in grid)
//         {
//             foreach (var col in row)
//             {
//                 Console.Write(col switch
//                 {
//                     Tile.Wall => _settings.wallChar,
//                     Tile.Head => _settings.headChar,
//                     Tile.Body => _settings.bodyChar,
//                     Tile.Apple => _settings.appleChar,
//                     _ => _settings.bgChar,
//                 });
//             }
//             Console.WriteLine();
//         }
//     }
// }


// //===========================================================//
// //+++ SNAKE CLASS +++||--------------------------------------//
// //===========================================================//

// class Snake
// {
//     private Point _head;
//     private Point[] _body = { };

//     public Snake(Point spawnPoint) { _head = spawnPoint; }

//     public void Move(Direction? direction, List<List<Tile>> grid)
//     {
//         _head = direction switch
//         {
//             Direction.Left => new Point(_head.row, _head.col - 1),
//             Direction.Right => new Point(_head.row, _head.col + 1),
//             Direction.Up => new Point(_head.row - 1, _head.col),
//             Direction.Down => new Point(_head.row + 1, _head.col),
//             _ => _head
//         };

//         grid[_head.row][_head.col] = Tile.Head;
//     }
// }