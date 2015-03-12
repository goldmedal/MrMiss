// by 李松軒
// 學號: F74001200
// 功能: 關於人物移動與bonus生成的兩個class
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;

namespace MissDll
{
    public class Miss
    {
        public PictureBox MissFortune;
        private int count = 1;
        public int x = 3, y = 3, coinGot = 0;
        public Timer time = new Timer();
        public bool bonusCheck = false, die = false, withBonus = false, moving;
        public int[,] subMap = new int[8, 8];

        public Miss(Form mf, int[,] Map)
        {
            Map[x, y] = 1;
            subMap = Map;

            MissFortune = new PictureBox();
            MissFortune.Image = Image.FromFile("miss1.gif");
            MissFortune.Size = new Size(50, 50);
            MissFortune.Location = new Point(25 + (x * 50), 25 + (y * 50));
            MissFortune.SizeMode = PictureBoxSizeMode.StretchImage;
            // time
            time.Tick += new EventHandler(time_Tick);
            time.Interval = 100;
            time.Start();
            // Key
            mf.KeyDown += new KeyEventHandler(move);

            Map = subMap;
        }
        //      key function        //
        public void move(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    checking(x, y + 1);
                    if (y + 1 <= 6 && moving == true)
                    {
                        subMap[x, y] = 0;
                        y++;
                        subMap[x, y] = 1;
                    }
                    break;
                case Keys.Up:
                    checking(x, y - 1);
                    if (y - 1 >= 1 && moving == true)
                    {
                        subMap[x, y] = 0;
                        y--;
                        subMap[x, y] = 1;
                    }
                    break;
                case Keys.Left:
                    checking(x - 1, y);
                    if (x - 1 >= 1 && moving == true)
                    {
                        subMap[x, y] = 0;
                        x--;
                        subMap[x, y] = 1;
                    }
                    break;
                case Keys.Right:
                    checking(x + 1, y);
                    if (x + 1 <= 6 && moving == true)
                    {
                        subMap[x, y] = 0;
                        x++;
                        subMap[x, y] = 1;
                    }
                    break;
                case Keys.Space:
                    if (withBonus == true)
                    {
                        withBonus = false;
                        bonusCheck = false;
                    }
                    break;
                default: break;
            }
            MissFortune.Location = new Point(25 + (x * 50), 25 + (y * 50));
            /*      GAME OVER judement     */
            if (die == true)
            {
                subMap[x, y] = 3;

            }
        }
        //      movement checking       //
        private void checking(int a, int b)
        {
            switch (subMap[a, b])
            {
                case 0:
                    moving = true;
                    break;
                case 2:
                    moving = true;
                    die = true;
                    break;
                case 4:
                    moving = false;
                    break;
                case 5:
                    moving = true;
                    withBonus = true;
                    break;
                default: break;
            }
        }
        //      character shaking func      //
        private void time_Tick(object sender, EventArgs e)
        {
            if (count == 5)
            {
                MissFortune.Image = Image.FromFile("miss2.gif");
            }
            else if (count == 10)
            {
                MissFortune.Image = Image.FromFile("miss1.gif");
                count = 1;
            }
            count++;
        }
    }

    public class Bonus
    {
        Random random = new Random();
        public int X, Y, bonusType = 5;
        public PictureBox Coin;
        public Bonus()
        {
            X = random.Next(1, 7);
            Y = random.Next(1, 7);

            Coin = new PictureBox();
            Coin.Image = Image.FromFile("coin.gif");
            Coin.Size = new Size(50, 50);
            Coin.Location = new Point(25 + (X * 50), 25 + (Y * 50));
            Coin.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }
}
