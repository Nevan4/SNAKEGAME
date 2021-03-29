using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Keys last_key;
        SnakeGame game;
        public Form1()
        {
            MessageBox.Show("Autor: Kacper Dul - 227335\r\nGra Snake\r\nProwadzący: Dr inż. Krzysztof Dyrcz\r\nPrzedmiot: Programowanie Obiektowe");
            InitializeComponent();
            gameTimer.Interval = Settings.tickrate;
            gameTimer.Tick += step;
            game = new SnakeGame(ref listView1);
            update_form();
        }

        private void update_form()
        {
            foreach (Panel box in this.game.snake_board.Boxes)
            {
                this.Controls.Add(box);
            }
            this.Controls.Add(this.game.snake_board.background);
        }

        private void step(object sender, EventArgs e)
        {
            game.step(this.last_key);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Panel box in this.game.snake_board.Boxes)
            {
                box.BackColor = SystemColors.ActiveCaptionText;
            }

            game.start(ref gameTimer);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            this.last_key = e.KeyCode;
        }



    }

    
}
