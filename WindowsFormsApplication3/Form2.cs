using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeaponDll;
using MissDll;
using System.IO;

namespace WindowsFormsApplication3
{
    public partial class MyForm : Form
    {
        public int[,] Map = new int[8, 8];
        private Timer time2 = new Timer();
        Bonus Reward;
        private int i, j, second = 0;
        Miss player;
        Weapon weapon;
        public Timer Frequency = new Timer();
        public Timer Clock = new Timer();
        Timer Check = new Timer();
        private Label ClockTimer = new Label();
        private Label Scoring = new Label();
        Timer InFrequency = new Timer(); // 增加發射頻率
        Timer InNum = new Timer(); // 增加子彈數目
        Timer Change_State = new Timer(); // 改變子彈種類
        int Speed;
        int NumOfStone;
        int SpaceTime;
        int State; // 子彈種類
        
        public MyForm()
        {
            Speed = 200;
            NumOfStone = 1;
            State = 0;
         //   int SpaceTime = 1;

            InitializeComponent();
            
            label1.Location = new Point(450, 163);
            label1.Size = new Size(125, 21);
            label3.Location = new Point(450, 265);
            label3.Size = new Size(125, 21);
            pictureBox1.Size = new Size(686, 462);
            pictureBox1.Location = new Point(0, 0);
            pictureBox2.Size = new Size(560, 190);
            pictureBox2.Location = new Point(53, 110);
            pictureBox2.Hide();
            button1.Location = new Point(454, 364);
            label3.Size = new Size(125, 34);
            button1.Hide();

            player = new Miss(this,this.Map);
            SpaceTime = 2000;
            this.Width = 700;
            this.Height = 500;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            //this.BackColor = Color.Wheat;

            for (i = 0; i <= 7; i++)
            {
                for (j = 0; j <= 7; j++)
                {
                    Map[i, j] = 0;
                }
            }
            Controls.Add(player.MissFortune);
            player.MissFortune.BringToFront();

            // timer of bonus
            time2.Tick += new EventHandler(time2_Tick);
            time2.Interval = 10000;
            time2.Start();
            // Timer of State
            Change_State.Interval = 30000;
            Change_State.Tick += new EventHandler(State_Tick);
            Change_State.Start();

            // Timer of InFrequency

            InFrequency.Interval = 30000;
            InFrequency.Tick += new EventHandler(Frequency_Increase);
            InFrequency.Start();

            // Timer of Num

            InNum.Interval = 15000;
            InNum.Tick += new EventHandler(Num_Increase);
            InNum.Start();

            // timre of bonus
            time2.Tick += new EventHandler(time2_Tick);
            time2.Interval = 10000;
            time2.Start();
            // timer of check && removing coin
            Check.Tick += new EventHandler(Check_Time_Tick);
            Check.Interval = 300;
            Check.Start();

            // about clock
            Clock.Interval = 1000;
            Clock.Tick += new EventHandler(this.Clock_Tick);
            Clock.Start();

            Check.Tick += new EventHandler(Check_Time_Tick);
            Check.Interval = 300;
            Check.Start();
            Frequency.Interval = 2000 + 6 * Speed + SpaceTime;
            Frequency.Tick += new EventHandler(this.Frequency_Tick);
            Frequency.Start();
//            weapon.RandomFire(NumOfStone);
        }
        
        // Weapon Function
        private void State_Tick(object sender, EventArgs e)
        {
            if (State < 2)
            {
                State += 1;
            }
            switch(State)
            {
                case 0: Speed = 300;break;
                case 1: Speed = 100; break;
                case 2: Speed = 500; break;
            }
        }

        private void Frequency_Tick(object sender, EventArgs e)
        {
            weapon = new Weapon(ref this.Map, Speed, this, this.pictureBox1, ref Frequency, State);
            weapon.RandomFire(NumOfStone);
            weapon = null;
        }

        private void Frequency_Increase(object sender, EventArgs e)
        {
            if (SpaceTime > 0)
            {
                SpaceTime -= 500;
            }
            else
            {
                InFrequency.Stop();
            }
        }

        private void Num_Increase(object sender, EventArgs e)
        {
            if (NumOfStone < 5)
            {
                NumOfStone += 1;
            }
            else
            {
                InNum.Stop();
            }
        }

        //      bonus timer     //
        private void time2_Tick(object sender, EventArgs e)     
        {
            if (player.bonusCheck == false)
            {
                Reward= new Bonus();
                if (Reward.X != player.x && Reward.Y != player.y)
                {
                    Map[Reward.X, Reward.Y] = Reward.bonusType;
                    player.bonusCheck = true;
                    Controls.Add(Reward.Coin);
                    Reward.Coin.BringToFront();
                    Reward.Coin.BackColor = Color.Transparent;
                    Reward.Coin.Parent = this.pictureBox1;
                }
            }
        }

        //      Clock timer     //
        private void Clock_Tick(object sender, EventArgs e)
        {
            second++;
            string Second = Convert.ToString(second);
            ClockTimer.Location = new Point(454, 205);
            ClockTimer.Size = new Size(100, 20);
            ClockTimer.Font = new Font("Times New Roman", 15, FontStyle.Bold);
            ClockTimer.Text = Second;
            Controls.Add(ClockTimer);
            ClockTimer.BringToFront();
        }

        private void Check_Time_Tick(object sender, EventArgs e)
        {
            //      Remove Coin     //
            if (player.withBonus == true)
            {
                this.pictureBox1.Controls.Remove(Reward.Coin);
                player.withBonus = false;
                player.bonusCheck = false;
            }
            /*      GAME OVER judement     */
            if (player.die == true)
            {
                Map[player.x, player.y] = 3;

                
            }
            //      Score       //
            if (Map[player.x, player.y] == 3)
            {
                pictureBox2.Show();
                button1.Show();
                pictureBox2.BringToFront();
                ClockTimer.Hide();
                label1.Hide();
                label3.Hide();

                int[] top10 = new int[10];
                player.time.Stop();
                time2.Stop();
                Check.Stop();
                Frequency.Stop();
                Clock.Stop();
                for (j = 0; j < 8; j++)
                {
                    for (i = 0; i < 8; i++)
                    {
                        Map[j, i] = 0;
                    }
                }
                int score = second * 10 + player.coinGot * 50;
                Scoring.Location = new Point(300, 400);
                Scoring.Size = new Size(100, 20);
                Scoring.Font = new Font("Times New Roman", 15, FontStyle.Bold);
                Scoring.Text = "Score: " + score;
                Controls.Add(Scoring);
                Scoring.BringToFront();
                //      Rank top 10     //
                //      read        //
                try
                {
                    using (StreamReader sr = new StreamReader("rank.txt"))
                    {
                        String line;
                        int r = 0;
                        while ((line = sr.ReadLine()) != null)
                        {
                            top10[r] = Convert.ToInt32(line);
                            r++;
                        }
                    }
                }
                catch (Exception a)
                {

                }
                //      compare     //
                for (i = 0; i <= 9; i++)
                {
                    if (score > top10[i])
                    {
                        for (j = 9; j > i; j--)
                        {
                            top10[j] = top10[j - 1];
                        }
                        top10[i] = score;
                        break;
                    }
                }
                //      write       //
                using (StreamWriter sw = new StreamWriter("rank.txt"))
                {
                    for (i = 0; i <= 9; i++)
                    {
                        sw.WriteLine(top10[i]);
                    }
                }
            }



            /*int i, j;
            Console.Clear();
            string Result = "";
            for (j = 0; j < 8; j++)
            {
                for (i = 0; i < 8; i++)
                {
                    Result += Convert.ToString(Map[i, j]);
                }
                Result += Environment.NewLine;
            }
            Console.Write(Result);*/
        } 
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MyForm_Load(object sender, EventArgs e)
        {
            this.player.MissFortune.BackColor = Color.Transparent;
            this.player.MissFortune.Parent = this.pictureBox1;
            this.pictureBox2.BackColor = Color.Transparent;
            this.pictureBox2.Parent = this.pictureBox1;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();

        }

    }
}

/*    class Miss
    {
        public PictureBox MissFortune;
        private int count = 1;
        public int x = 3, y = 3, coinGot = 0;
        public Timer time = new Timer();
        public bool bonusCheck = false, die = false, withBonus = false, moving;
        public int[,] subMap = new int[8, 8];

        public Miss(MyForm mf)
        {
            mf.Map[x, y] = 1;
            subMap = mf.Map;

            MissFortune = new PictureBox();
            MissFortune.Image = Image.FromFile("miss1.gif");
            MissFortune.Size = new Size(50, 50);
            MissFortune.Location = new Point(25+(x * 50), 25+(y * 50));
            MissFortune.SizeMode = PictureBoxSizeMode.StretchImage;
            // time
            time.Tick += new EventHandler(time_Tick);
            time.Interval = 100;
            time.Start();
            // Key
            mf.KeyDown += new KeyEventHandler(move);

            mf.Map = subMap;
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
            MissFortune.Location = new Point(25+(x * 50), 25+(y * 50));
            /*      GAME OVER judement     */
        /*    if (die == true)
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

    class Bonus
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
            Coin.Location = new Point(25+(X*50), 25+(Y*50));
            Coin.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }    */
    /*class Miss
    {
        public PictureBox MissFortune;
        private int count = 1;
        public int x = 3, y = 3;
        private Timer time = new Timer();

        public Miss(MyForm mf)
        {   
            mf.Map[x, y] = 1;
            
            MissFortune = new PictureBox();
            MissFortune.Image = Image.FromFile("miss1.gif");
            MissFortune.Size = new Size(50, 50);
            MissFortune.Location = new Point(25+(x * 50), 25+(y * 50));
            MissFortune.SizeMode = PictureBoxSizeMode.StretchImage;
            
            //  shaking time
            time.Tick += new EventHandler(time_Tick);
            time.Interval = 100;
            time.Start();
            
        }
        //      character shaking func       //
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

    class Bonus
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
            Coin.Location = new Point(25+(X * 50), 25+(Y * 50));
            Coin.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }*/

