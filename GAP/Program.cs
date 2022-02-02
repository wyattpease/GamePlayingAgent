using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MyNamespace
{
    class MyClassCS
    {
        public static int turn;
        public static char[][] gameboard = new char[8][];
        public static string Player;
        public static double white_time = 0;
        public static double black_time = 0;
        public static Dictionary<char, int> board_row = new Dictionary<char, int>() { { '8', 0 }, { '7', 1 }, { '6', 2 }, { '5', 3 }, { '4', 4 }, { '3', 5 }, { '2', 6 }, { '1', 7 } };
        public static Dictionary<char, int>board_col = new Dictionary<char, int>() { { 'a', 0 }, { 'b', 1 }, { 'c', 2 }, { 'd', 3 }, { 'e', 4 }, { 'f', 5 }, { 'g', 6 }, { 'h', 7 } };
        static void Main()
        {
            using var watcher = new FileSystemWatcher(@"C:\Users\wyatt\source\repos\homework11\Project1");

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*.txt";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            System.IO.StreamReader gamefile = new System.IO.StreamReader(@"C:\Users\wyatt\OneDrive\Desktop\Turns\origin.txt");
            gamefile.ReadLine();
            Player = gamefile.ReadLine();
            black_time = white_time = Convert.ToDouble(gamefile.ReadLine()) * 1000;
            turn = 1;

            for (int i = 0; i < 8; i++)
            {
                var row = gamefile.ReadLine().ToCharArray();
                gameboard[i] = row;
            }

            gamefile.Close();
            File.Copy(@"C:\Users\wyatt\OneDrive\Desktop\Turns\origin.txt", @"C:\Users\wyatt\OneDrive\Desktop\Turns\game.txt");
            File.Move(@"C:\Users\wyatt\OneDrive\Desktop\Turns\game.txt", @"C:\Users\wyatt\source\repos\homework11\Project1\input.txt");

            using (var p = new Process())
            {
                p.StartInfo.FileName = @"C:\Users\wyatt\source\repos\homework11\Project1\Project1.exe";
                p.StartInfo.WorkingDirectory = @"C:\Users\wyatt\source\repos\homework11\Project1";
                p.Start();
            }
            //Process.Start(@"C:\Users\wyatt\source\repos\homework11\Project1\Project1.exe");
                

            Console.ReadLine();
        }

        private async static void OnChanged(object sender, FileSystemEventArgs e)
        {
            
        }

        private async static void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.Name == "game.txt")
            {
                System.IO.StreamReader gamefile = new System.IO.StreamReader(e.FullPath);
                gamefile.ReadLine();
                Player = gamefile.ReadLine();
                black_time = white_time = Convert.ToDouble(gamefile.ReadLine());
                black_time=black_time * 1000; white_time = white_time * 1000;
                turn = 1;

                for (int i = 0; i < 8; i++)
                {
                    var row = gamefile.ReadLine().ToCharArray();
                    gameboard[i] = row;
                }

                gamefile.Close();
                File.Move(@"C:\Users\wyatt\source\repos\homework11\Project1\game.txt", @"C:\Users\wyatt\source\repos\homework11\Project1\input.txt");
            }


            if (e.Name == "output.txt")
            {                
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                bool free = false;
                while (!free)
                {
                    free = file_free(e.FullPath);
                }
                stopWatch.Stop();
                double elapsed = stopWatch.ElapsedMilliseconds;
                if (Player == "BLACK")
                    black_time -= elapsed;
                else
                    white_time -= elapsed;

                if (black_time < 0)
                {
                    Console.WriteLine("White has won");
                    Console.ReadLine();
                    return;
                }
                else if (white_time < 0)
                {
                    Console.WriteLine("Black has won");
                    Console.ReadLine();
                    return;
                }
                List<string> moves = new List<string>();
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(e.FullPath);


                while ((line = file.ReadLine()) != null)
                    moves.Add(line);

                if (moves[0] == "done")
                {
                    if (Player == "BLACK")
                    {
                        Console.WriteLine("White has won, Black can play no moves");
                        Console.ReadLine();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Black has won, white can play no moves");
                        Console.ReadLine();
                        return;
                    }
                }
                foreach (string m in moves)
                {
                    var move = m.Split(' ');
                    if (move[0] == "J")
                    {
                        var from = move[1].ToCharArray();
                        var temp = gameboard[board_row[from[1]]][board_col[from[0]]];
                        gameboard[board_row[from[1]]][board_col[from[0]]] = '.';
                        var to = move[2].ToCharArray();
                        var midrow = (board_row[to[1]] + board_row[from[1]]) / 2;
                        var midcol = (board_col[to[0]] + board_col[from[0]]) / 2;
                        gameboard[midrow][midcol] = '.';
                        if (Player == "BLACK")
                        {
                            if (board_row[to[1]] == 7)
                                temp = 'B';
                        }
                        else
                        {
                            if (board_row[to[1]] == 0)
                                temp = 'W';
                        }
                        gameboard[board_row[to[1]]][board_col[to[0]]] = temp;
                    }
                    else
                    {
                        var from = move[1].ToCharArray();
                        var temp = gameboard[board_row[from[1]]][board_col[from[0]]];
                        gameboard[board_row[from[1]]][board_col[from[0]]] = '.';
                        var to = move[2].ToCharArray();
                        if (Player == "BLACK")
                        {
                            if (board_row[to[1]] == 7)
                                temp = 'B';
                        }
                        else
                        {
                            if (board_row[to[1]] == 0)
                                temp = 'W';
                        }
                        gameboard[board_row[to[1]]][board_col[to[0]]] = temp;
                        
                    }                   
                }
                file.Close();
                System.IO.StreamWriter wfile = new System.IO.StreamWriter(@"C:\Users\wyatt\OneDrive\Desktop\Turns\Turns.txt", true);
                await wfile.WriteLineAsync();
                await wfile.WriteLineAsync("////////////////////////////////////////////////////////");
                if (Player == "BLACK")
                    await wfile.WriteLineAsync(Player + "-" + turn + " time elapsed: " + elapsed + " time remaining: " + black_time/1000);
                else
                    await wfile.WriteLineAsync(Player + "-" + turn + " time elapsed: " + elapsed + " time remaining: " + white_time/1000);
                foreach (var m in moves)
                    await wfile.WriteLineAsync(m);

                foreach (char[] row in gameboard)
                {
                    foreach (char s in row)
                        await wfile.WriteAsync(s);
                    await wfile.WriteLineAsync();
                }
                await wfile.WriteLineAsync("////////////////////////////////////////////////////////");
                await wfile.WriteLineAsync();
                wfile.Close();

                //File.Move(e.FullPath, @"C:\Users\wyatt\OneDrive\Desktop\Turns\" + Player + "-" + turn + ".txt");

                if (Player == "BLACK")
                    Player = "WHITE";
                else
                    Player = "BLACK";

                turn++;

                File.Delete(@"C:\Users\wyatt\source\repos\homework11\Project1\input.txt");
                File.Delete(e.FullPath);

                var fs = File.Create(@"C:\Users\wyatt\OneDrive\Desktop\Turns\input.txt");
                fs.Close();

                wfile = new StreamWriter(@"C:\Users\wyatt\OneDrive\Desktop\Turns\input.txt");
                await wfile.WriteLineAsync("GAME");
                await wfile.WriteLineAsync(Player);
                if (Player == "BLACK")
                    await wfile.WriteLineAsync((black_time/1000).ToString());
                else
                    await wfile.WriteLineAsync((white_time/1000).ToString());
                foreach (char[] row in gameboard)
                {
                    foreach (char s in row)
                        await wfile.WriteAsync(s);
                    await wfile.WriteLineAsync();
                }
                wfile.Close();

                File.Move(@"C:\Users\wyatt\OneDrive\Desktop\Turns\input.txt", @"C:\Users\wyatt\source\repos\homework11\Project1\input.txt");

                using (var p = new Process())
                {
                    p.StartInfo.FileName = @"C:\Users\wyatt\source\repos\homework11\Project1\Project1.exe";
                    p.StartInfo.WorkingDirectory = @"C:\Users\wyatt\source\repos\homework11\Project1";
                    p.Start();
                }
            }
        }

        private static bool file_free(string filename)
        {
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private async static void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (e.Name == "output.txt")
            {
                List<string> moves = new List<string>();
                string line;
                System.IO.StreamReader file = new System.IO.StreamReader(e.FullPath);
                

                while ((line = file.ReadLine()) != null)
                    moves.Add(line);

                foreach (string m in moves)
                {
                    var move = m.Split(' ');
                    if (move[0] == "J")
                    {
                        var from = move[1].ToCharArray();
                        var temp = gameboard[board_row[from[1]]][board_col[from[0]]];
                        gameboard[board_row[from[1]]][board_col[from[0]]] = '.';
                        var to = move[2].ToCharArray();
                        var midrow = (board_row[to[1]] + board_row[from[1]]) / 2;
                        var midcol = (board_col[to[0]] + board_col[from[0]]) / 2;
                        gameboard[midrow][midcol] = '.';
                        gameboard[board_row[to[1]]][board_col[to[0]]] = temp;
                    }
                    else
                    {
                        var from = move[1].ToCharArray();
                        var temp = gameboard[board_row[from[1]]][board_col[from[0]]];
                        gameboard[board_row[from[1]]][board_col[from[0]]] = '.';
                        var to = move[2].ToCharArray();
                        gameboard[board_row[to[1]]][board_col[to[0]]] = temp;
                    }
                }
                file.Close();
                System.IO.StreamWriter wfile = new System.IO.StreamWriter(e.FullPath);

                await wfile.WriteLineAsync(Player + "-" + turn);
                foreach (char[] row in gameboard)
                {
                    foreach (char s in row)
                        await wfile.WriteAsync(s);
                    await wfile.WriteLineAsync();
                }
                wfile.Close();

                File.Move(e.FullPath, @"C:\Users\wyatt\OneDrive\Desktop\Turns\" + Player + "-" + turn);

                
                if (Player == "BLACK")
                    Player = "WHITE";
                else
                    Player = "BLACK";

                turn++;
            }
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
