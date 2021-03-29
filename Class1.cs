using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace WindowsFormsApp1
{

    public static class Settings
    {
        /*File containing scores*/
        public static String score_file_path = @".\save.csv";

        /*Windows Form Settings*/
        public static int window_width = 500;
        public static int window_height = 450;

        /*Box size*/
        public static int box_width = 20;
        public static int box_height = 20;

        /*Game board grid size*/
        public static int grid_width = 10;
        public static int grid_height = 20;

        /*Threshold for boxes in Snakeboard*/
        public static int box_threshold = 1;

        /*Game board size*/
        public static int board_height = ((box_height + box_threshold) * grid_height) + 1;
        public static int board_width = ((box_width + box_threshold) * grid_width) + 1;

        /*Game board Location*/
        public static int X = window_width - (board_width + 30);
        public static int Y = 20;
        public static Point board_position = new Point(X,Y); 

        /*Game settings*/
        public static int tickrate = 200; /*in miliseconds*/ 
    }

    class SnakeGame
    {
        public Snakeboard snake_board;
        Snake snake;
        Keys previous_move;
        Timer timer;
        ListView listview;
        bool Food = false;
        int Score = 0;

        public SnakeGame(ref ListView listview)
        {
            this.listview = listview;
            this.load_results(Settings.score_file_path);
            snake_board = new Snakeboard();      
        }

        public void start(ref Timer timer)
        {
            snake = new Snake(5, 10, ref snake_board.Boxes[10, 5]);
            Food = false;
            this.timer = timer;
            this.timer.Start();
        }

        private bool setFood()
        {
            Random rand = new Random();
            var temp = from Panel box in snake_board.Boxes
                       where box.BackColor == SystemColors.ActiveCaptionText
                       orderby rand.Next()
                       select box;

            if (temp.Count() == 0)
            {
                return false;
            }

            temp.First().BackColor = SystemColors.GrayText;

            return true;
        }

        public void step(Keys key)
        {
            bool result = true;

            if (!Food)
            {
                Food = setFood();
                if (!Food)
                {
                    this.timer.Stop();
                    this.Score = snake.score();
                    this.save_results(Settings.score_file_path, this.Score.ToString());
                    MessageBox.Show("Wygrales :o \r\n Twoj wynik: " + this.Score.ToString());
                }
            }

            switch (key)
            {
                case Keys.Left:
                    if (previous_move != Keys.Right)
                    {
                        result = snake.move(0, -1, ref snake_board, ref this.Food);
                        previous_move = key;
                    }
                    else
                    {
                        result = snake.move(0, 1, ref snake_board, ref this.Food);
                    }
                    break;
                case Keys.Right:
                    if (previous_move != Keys.Left)
                    {
                        result = snake.move(0, 1, ref snake_board, ref this.Food);
                        previous_move = key;
                    }
                    else
                    {
                        result = snake.move(0, -1, ref snake_board, ref this.Food);
                    }
                    break;
                case Keys.Up:
                    if (previous_move != Keys.Down)
                    {
                        result = snake.move(-1, 0, ref snake_board, ref this.Food);
                        previous_move = key;
                    }
                    else
                    {
                        result = snake.move(1, 0, ref snake_board, ref this.Food);
                    }
                    break;
                case Keys.Down:
                    if (previous_move != Keys.Up)
                    {
                        result = snake.move(1, 0, ref snake_board, ref this.Food);
                        previous_move = key;
                    }
                    else
                    {
                        result = snake.move(-1, 0, ref snake_board, ref this.Food);
                    }
                    break;
                default:
                    break;
            }

            if (!result)
            {
                this.timer.Stop();
                this.Score = snake.score();
                this.save_results(Settings.score_file_path, this.Score.ToString());
            }
        }

        private void save_results(String score_file, String score)
        {
            Form2 input = new Form2();
            input.ShowDialog();

            if (!File.Exists(Settings.score_file_path))
            {
                File.Create(score_file).Close();
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0};{1};\r\n", score, input.nick);
            File.AppendAllText(score_file, sb.ToString());

            load_results(score_file);
        }

        private void load_results(String score_file)
        {
            this.listview.Items.Clear();

            if (File.Exists(score_file))
            {
                using (var reader = new StreamReader(score_file))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        var temp = new ListViewItem(values[0]);
                        temp.SubItems.Add(values[1]);

                        this.listview.Items.Add(temp);
                    }
                }
            }
            else
            {
                File.Create(score_file).Close();
            }
        }
    }

    class Snake
    {
        Queue<Panel> snake = new Queue<Panel>();
        Point head;

        public Snake(int x, int y, ref Panel box)
        {
            head = new Point(x, y);
            this.add(ref box);
        }

        public bool add(ref Panel box)
        {
            if (this.snake.Contains(box)) 
            {
                return false;
            }
            box.BackColor = SystemColors.GrayText;
            this.snake.Enqueue(box);
            return true;
        }

        public void remove_last()
        {
            Panel temp = snake.Dequeue();
            temp.BackColor = SystemColors.ActiveCaptionText;
        }

        public bool move(int y, int x, ref Snakeboard board, ref bool food)
        {
            if (this.head.X + x < 0 || this.head.X + x > Settings.grid_width - 1 || this.head.Y + y < 0 || this.head.Y + y > Settings.grid_height - 1)
                return false;
            else
            {
                this.head = new Point(this.head.X + x, this.head.Y + y);
                if (! (board.Boxes[this.head.Y, this.head.X].BackColor == SystemColors.GrayText))
                {
                    this.remove_last();                   
                }
                else
                {
                    food = false;
                }

                return this.add(ref board.Boxes[this.head.Y, this.head.X]);
            }

        }

        public int score()
        {
            return (this.snake.Count - 1) * 100;
        }
    }

    public class Snakeboard
    {
        public Panel background;
        public Panel[,] Boxes = new Panel[Settings.grid_height, Settings.grid_width];

        public Snakeboard()
        {
            generateBoard();
            generateBoxes();
        }

        private void generateBoard()
        {
            this.background = new Panel();
            this.background.Location = Settings.board_position;
            this.background.BackColor = SystemColors.ActiveCaptionText;
            this.background.Size = new Size(Settings.board_width, Settings.board_height);
        }

        private void generateBoxes()
        {
            int x = background.Location.X;
            int y = background.Location.Y;

            for (int i=0; i < Settings.grid_height; i++)
            {
                for (int j = 0; j < Settings.grid_width; j++)
                {
                    Panel temp = new Panel();

                    temp.Location = new Point(x + (Settings.box_width + Settings.box_threshold) * j + 1, y + (Settings.box_height + Settings.box_threshold) * i + 1);
                    temp.Size = new Size(Settings.box_width, Settings.box_height);
                    temp.BackColor = SystemColors.GrayText;

                    Boxes[i, j] = temp;
                }
            }
        }
    }
}
