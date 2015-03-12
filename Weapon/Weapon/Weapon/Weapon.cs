/* ************************************************************* 
 *  作者: 劉家瑄
 *  功能: 產生武器的Class, 裡面用了多個Timer來控制武器的移動.
 *             並且可以修改主程式裡面的array
 *                         
 *******************************************************************/
 


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


namespace WeaponDll
{
     public struct FireLocation   // 紀錄發射點座標用的struct
    {
        public int X; // X座標
        public int Y; // Y座標
        public int Index; // 發射編號
    } // struct FrieLocation 

     public class Bullet  // 紀錄子彈的座標, 生成子彈的圖片
     {
         public int X; // X座標
         public int Y; // Y座標
/*         private int RealX;
         private int RealY;   */
         public int Index; // 發射編號
         public PictureBox BulletPic;

         public Bullet() // 給List用的簡單重載
         {
         }

         public Bullet(string ImageName, int x, int y)
         {
             this.X = x;
             this.Y = y;


             // 產生圖片的PictureBox

             BulletPic = new PictureBox();
             BulletPic.Image = Image.FromFile(ImageName);
             BulletPic.Size = new Size(50, 50);
             BulletPic.Location = new Point(25 + (x * 50), 25 + (y * 50));
             BulletPic.SizeMode = PictureBoxSizeMode.StretchImage;
             BulletPic.Visible = true;

         }

         // 讓圖片產生移動的方法

         public void Move(int x, int y)
         {
             BulletPic.Location = new Point(25 + (x * 50), 25 + (y * 50));
         }


     } // class Bullet

    public class Weapon
    {
        Form MyForm;  // 取得主程式form的reference
        List<FireLocation> FireList; // 存外圍發射點的各個座標
        List<FireLocation> StoneList;  // 存子彈每次到達的座標
        List<Int32> AlarmNumList;  // 紀錄警告次數的list, 每兩次發射.
        List<Int32> FireNumList = new List<int>(); // 紀錄火焰發射的時間 2個cycle才消失
        ArrayList IndexList; // 紀錄random出來的發射位置
        List<PictureBox> AlarmPicList; // 控制每個Alarm圖片的picturebox
        List<bool> RewardList; // 紀錄每個子彈是否有碰到金幣
        Timer FireTime; // 控制子彈前進的Timer
        Timer AlarmTime; // 控制警告圖片的Timer
        List<Bullet> BulletList; // 實際控制每個子彈的List
        private int[,] Map; // 取得Form Map array的reference
        int Speed; // 儲存子彈移動速度
        bool FireEnd; // 判斷子彈是否結束發射
        bool AlarmVis = true; // 判斷警告圖片是否結束
        public PictureBox Mf_Pic;  // 取得主程式背景的Reference
        Timer Frequ; // 取得主程式的Timer, 遊戲結束時可停止
        String[] WeaponName; // 紀錄武器種類的array
        int State; // 紀錄武器的狀態, 速度
        List<Point> PointList = new List<Point>(); // 紀錄武器經過的地方的原始值

        public Weapon(ref int [,] map, int speed, Form mf, PictureBox Pic, ref Timer fre, int state)
        {

            // initial parameters
            int i;
            WeaponName = new String[]{"bullet", "bomb","fire"};
            State = state;
            MyForm = mf;
            Mf_Pic = Pic;
            Frequ = fre;
            FireList = new List<FireLocation>();
            StoneList = new List<FireLocation>();
            AlarmNumList = new List<Int32>();
            BulletList = new List<Bullet>();
            AlarmPicList = new List<PictureBox>();
            RewardList = new List<bool>();
           

            Int32 AlarmPtr;
            Map = map;
            FireTime = new Timer();
            AlarmTime = new Timer();
            Speed = speed;
            FireEnd = true;

            InitialFireList();

            for (i = 0; i < 24; i++)
            {
                AlarmPtr = AlarmNumList[i];
                AlarmPtr = 0;
            }

        } // public Weapon()

        // 初始化每一回合的武器預設值

        private void InitialFireList()
        {
            int i;
            FireLocation FirePtr;

            for (i = 0; i < 24; i++)
            {
                PointList.Add(new Point(0,0));
                FireList.Add(new FireLocation());
                StoneList.Add(new FireLocation());
                AlarmNumList.Add(new Int32());
                BulletList.Add(new Bullet());
                AlarmPicList.Add(new PictureBox());
                FireNumList.Add(0);
                RewardList.Add(false);
            }


            // 上6個發射點

            for (i = 0; i < 6; i++)
            {
                FirePtr = FireList[i];
                FirePtr.X = i + 1;
                FirePtr.Y = 0;
                FireList[i] = FirePtr;
            }

            // 下 6個發射點

            for (i = 6; i < 12; i++)
            {
                FirePtr = FireList[i];
                FirePtr.X = i - 5;
                FirePtr.Y = 7;
                FireList[i] = FirePtr;
            }


            // 左 6個發射點

            for (i = 12; i < 18; i++)
            {
                FirePtr = FireList[i];
                FirePtr.X = 0;
                FirePtr.Y = i - 11;
                FireList[i] = FirePtr;
                //               MessageBox.Show("FireList" + Convert.ToString(i) + " , " + Convert.ToString(FirePtr.X) + " , " + Convert.ToString(FirePtr.Y));
            }

            // 右 6個發射點
            for (i = 18; i < 24; i++)
            {
                FirePtr = FireList[i];
                FirePtr.X = 7;
                FirePtr.Y = i - 17;
                FireList[i] = FirePtr;
            }


        } // private void InitialFireList()
        
        public void RandomFire(int Num)// 隨機決定子彈發射的位置, 並啟動一次射擊
        {
            Random Rad = new Random();
            IndexList = new ArrayList(Num);
            int Index;
            while (Num != 0)
            {
                Index = Rad.Next(0,23);
                if (!(IndexList.Contains(Index)))
                {
                    IndexList.Add(Index);  // 紀錄隨機決定的位置
                    Num -= 1;
                }
                //        MessageBox.Show(Convert.ToString(Index));
            }

            AlarmTime.Tick += new EventHandler(Alarm_Time_Tick); // 依序啟動多個Timer
            AlarmTime.Interval = 500;
            AlarmTime.Start();

        } // public void RandFire(ref int[,] Map, int Speed, int Freq, int Num)

        private void Alarm_Time_Tick(object sender, EventArgs e)   // 依照隨機決定的位置index, 啟動警告的Function, 經過兩個週期後, 啟動子彈射擊
        {
            int i;
            for (i = 0; i < IndexList.Count; i++)
            {
                int NowIndex = (int)IndexList[i];
                int AlarmPtr = AlarmNumList[NowIndex];
                if (AlarmNumList[NowIndex] <= 2)
                {
                    FireAlarm(NowIndex);
                    //          MessageBox.Show("Alarm Stop!");
                }
                else
                {
                    Mf_Pic.Controls.Remove(AlarmPicList[NowIndex]);
                    if (i == (IndexList.Count - 1))
                    {
                        AlarmTime.Stop();
                        FireTime.Interval = Speed;
                        FireTime.Tick += Fire_Time_Tick;
                   //     MoveTime.Start();
                        FireTime.Start();

                    }
                }

            }
        } //private void Alarm_Time_Tick(object sender, EventArgs e)

        private void Fire_Time_Tick(object sender, EventArgs e) // FireTimer 觸發的事件
        {
            int i;
            for (i = 0; i < IndexList.Count; i++)
            {
                int NowIndex = (int)IndexList[i]; // 取得現在在控制的子彈編號
                if (State != 2) Fire(NowIndex); // 子彈與大砲
                else Fire_Light(NowIndex); // 武器換火焰
                if (i == (IndexList.Count - 1))
                {
                    if (FireEnd)
                    {
                        //MessageBox.Show("Fire Stop!");
                        FireTime.Stop(); // 子彈完成發射
                    }
                }
            }
        } // private void Fire_Time_Tick(object sender, EventArgs e)

        private void Flash_Tick(object sender, EventArgs e) // 原先想製作警告閃爍的功能, 但沒完成
        {
            int i;
            for (i = 0; i < 24; i++)
            {
                if (AlarmVis)
                {
                    AlarmPicList[i].Visible = false;
                    AlarmVis = false;
                }
                else
                {
                    AlarmPicList[i].Visible = true;
                    AlarmVis = true; ;
                }
            }
        }

        public void FireAlarm(int Index)  // 控制Alarm圖片和發射子彈的第一步
        {
            FireLocation FirePtr = FireList[Index];

            if (AlarmNumList[Index] < 2)
            {
                if (Index < 6) AlarmPicList[Index].Image = Image.FromFile("pointer_down.png");
                else if (Index < 12) AlarmPicList[Index].Image = Image.FromFile("pointer_up.png");
                else if (Index < 18) AlarmPicList[Index].Image = Image.FromFile("pointer_right.png");
                else AlarmPicList[Index].Image = Image.FromFile("pointer_left.png");
                AlarmPicList[Index].Size = new Size(50, 50);
                AlarmPicList[Index].Location = new Point(25 + (FirePtr.X * 50), 25 + (FirePtr.Y * 50));
                AlarmPicList[Index].SizeMode = PictureBoxSizeMode.StretchImage;
                AlarmPicList[Index].Visible = true;
                AlarmPicList[Index].BackColor = Color.Transparent;
                AlarmPicList[Index].BringToFront();
                Mf_Pic.Controls.Add(AlarmPicList[Index]);
                AlarmNumList[Index] += 1;
            }
            else
            {
                Map[FirePtr.X, FirePtr.Y] = 1;
                AlarmNumList[Index] += 1;
            }
        } // public void FireAlarm(ref int [,] Map,int Index) 

        public void Fire(int Index) //  控制子彈的發射
        {                                                                        //  每次前進一格
            FireLocation FirePtr = FireList[Index];
            FireLocation StonePtr = StoneList[Index];

            //         MessageBox.Show(Convert.ToString(Index) + "+1");
            if (Index < 6) // 上往下射  射擊分了四種, 上到下, 下到上, 左到右, 右到左 原理一樣
            {

                // 人物的代號為1, 子彈為2, 死亡後改成3 ,空格為0, bonus為5


                if (Map[FirePtr.X, FirePtr.Y] == 1) // 子彈的第一步
                {
                    FireEnd = false;
                    int Y = FirePtr.Y + 1;
                    StonePtr.X = FirePtr.X; // 取得新的座標, 不動到警告區的座標
                    StonePtr.Y = Y;

                    if (!(Map[FirePtr.X, Y] == 1))
                    {
                        if (Map[FirePtr.X, Y] == 5) RewardList[Index] = true; // 如果碰到coin
                        Map[FirePtr.X, Y] = 2;  // 將子彈的位置改成 2
                    }
                    else
                    {
                        Map[FirePtr.X, Y] = 3; // 碰到人的話 結束遊戲
                        Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                        FireTime.Stop();
                        Frequ.Stop();
                        

                        BulletList[Index].BulletPic = null;
                        BulletList[Index] = new Bullet();
                        FireEnd = true;
                    }

                    Map[FirePtr.X, FirePtr.Y] = 0;
                    StoneList[Index] = StonePtr;

                    // PictureBox Handle
                    BulletList[Index] = new Bullet(WeaponName[State]+"_down.png", StonePtr.X, StonePtr.Y);
                    MyForm.Controls.Add(BulletList[Index].BulletPic);
                    BulletList[Index].BulletPic.BringToFront();
                    BulletList[Index].BulletPic.BackColor = Color.Transparent;
                    BulletList[Index].BulletPic.Parent = Mf_Pic;

                }
                else // 第二步之後的判斷
                {
                    int Y = StonePtr.Y + 1;
                    if (Y < 7) // 子彈尚未撞牆前
                    {
                        if (!(Map[FirePtr.X, Y] == 1))
                        {
                            if (!(RewardList[Index])) // 沒吃到子彈 就把走過得路改成0
                            {
                                Map[StonePtr.X, StonePtr.Y] = 0;
                            }
                            else
                            {
                                Map[StonePtr.X, StonePtr.Y] = 5;  // 吃coin後的子彈, 走過變回5
                                RewardList[Index] = false;
                            }
                            if (Map[FirePtr.X, Y] == 5) RewardList[Index] = true;

                            Map[StonePtr.X, Y] = 2;

                            StonePtr.Y = Y;
                            StoneList[Index] = StonePtr;
                            BulletList[Index].Move(StonePtr.X, Y);

                        }
                        else
                        {
                            Map[StonePtr.X, Y] = 3;  // 子彈碰到人, 改成3 結束遊戲.
                            Map[StonePtr.X, StonePtr.Y] = 0;
                            Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                            FireTime.Stop();
                            Frequ.Stop();
                            

                            BulletList[Index].BulletPic = null;
                            BulletList[Index] = new Bullet();
                            FireEnd = true;
                        }

                    } //  if (Y < 7) 
                    else  // 子彈撞牆前
                    {
                        if (!(RewardList[Index]))  // 牆壁旁不是bonus
                        {
                            Map[StonePtr.X, StonePtr.Y] = 0;
                        }
                        else
                        {
                            Map[StonePtr.X, StonePtr.Y] = 5;  // 若是bonus 要復原
                            RewardList[Index] = false;
                        }
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        AlarmNumList[Index] = 0;

                        // Remove PictureBox  
                        Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                        BulletList[Index].BulletPic = null;
                        BulletList[Index] = new Bullet();
                        FireEnd = true;
                    }

                }
            } // if (Index < 6) 
            else if (Index < 12) // 由下往上射  同Index<6的原理
            {
                if (Map[FirePtr.X, FirePtr.Y] == 1)
                {
                    FireEnd = false;
                    int Y = FirePtr.Y - 1;
                    StonePtr.X = FirePtr.X;
                    StonePtr.Y = Y;

                    if (!(Map[FirePtr.X, Y] == 1))
                    {
                        if (Map[FirePtr.X, Y] == 5) RewardList[Index] = true;
                        Map[FirePtr.X, Y] = 2;
                    }
                    else
                    {
                        Map[FirePtr.X, Y] = 3;
                        Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                        FireTime.Stop();
                        Frequ.Stop();
                    //    MessageBox.Show("Game over!");

                        BulletList[Index].BulletPic = null;
                        BulletList[Index] = new Bullet();
                        FireEnd = true;
                    }

                    Map[FirePtr.X, FirePtr.Y] = 0;
                    StoneList[Index] = StonePtr;

                 

                    // PictureBox Handle
                    BulletList[Index] = new Bullet(WeaponName[State]+"_Up.png", StonePtr.X, StonePtr.Y);
                    MyForm.Controls.Add(BulletList[Index].BulletPic);
                    BulletList[Index].BulletPic.BringToFront();
                    BulletList[Index].BulletPic.BackColor = Color.Transparent;
                    BulletList[Index].BulletPic.Parent = Mf_Pic;
                }
                else
                {
                    int Y = StonePtr.Y - 1;
                    if (Y > 0)
                    {
                        if (!(Map[FirePtr.X, Y] == 1))
                        {

                            Map[StonePtr.X, StonePtr.Y] = 0;
                            Map[StonePtr.X, Y] = 2;
                            StonePtr.Y = Y;
                            StoneList[Index] = StonePtr;
                            BulletList[Index].Move(StonePtr.X, Y);
                        }
                        else
                        {
                            Map[StonePtr.X, Y] = 3;
                            if (!(RewardList[Index]))
                            {
                                Map[StonePtr.X, StonePtr.Y] = 0;
                            }
                            else
                            {
                                Map[StonePtr.X, StonePtr.Y] = 5;
                                RewardList[Index] = false;
                            }
                            Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                            FireTime.Stop();
                            Frequ.Stop();
                //            MessageBox.Show("Game over!");

                            BulletList[Index].BulletPic = null;
                            BulletList[Index] = new Bullet();
                            FireEnd = true;
                        }
                    }
                    else
                    {
                        if (!(RewardList[Index]))
                        {
                            Map[StonePtr.X, StonePtr.Y] = 0;
                        }
                        else
                        {
                            Map[StonePtr.X, StonePtr.Y] = 5;
                            RewardList[Index] = false;
                        }
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        AlarmNumList[Index] = 0;
                        // Remove PictureBox
                        Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                        BulletList[Index].BulletPic = null;
                        BulletList[Index] = new Bullet();
                        FireEnd = true;
                    }
                }
            } // else if (Index < 12)
            else if (Index < 18) // 由左往右 同Index<6的原理
            {
                if (Map[FirePtr.X, FirePtr.Y] == 1)
                {
                    FireEnd = false;
                    int X = FirePtr.X + 1;
                    StonePtr.X = X;
                    StonePtr.Y = FirePtr.Y;
                    if (!(Map[X, FirePtr.Y] == 1))
                    {
                        if (Map[X, FirePtr.Y] == 5) RewardList[Index] = true;
                        Map[X, FirePtr.Y] = 2;
                    }
                    else
                    {
                        Map[X, FirePtr.Y] = 3;
                        Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                        FireTime.Stop();
                        Frequ.Stop();
                 //       MessageBox.Show("Game over!");

                        BulletList[Index].BulletPic = null;
                        BulletList[Index] = new Bullet();
                        FireEnd = true;
                    }

                    Map[FirePtr.X, FirePtr.Y] = 0;
                    StoneList[Index] = StonePtr;

                    // PictureBox Handle
                    BulletList[Index] = new Bullet(WeaponName[State]+"_right.png", StonePtr.X, StonePtr.Y);
                    MyForm.Controls.Add(BulletList[Index].BulletPic);
                    BulletList[Index].BulletPic.BringToFront();
                    BulletList[Index].BulletPic.BackColor = Color.Transparent;
                    BulletList[Index].BulletPic.Parent = Mf_Pic;
                }
                else
                {
                    int X = StonePtr.X + 1;
                    if (X < 7)
                    {
                        if (!(Map[X, StonePtr.Y] == 1))
                        {
                            if (!(RewardList[Index]))
                            {
                                Map[StonePtr.X, StonePtr.Y] = 0;
                            }
                            else
                            {
                                Map[StonePtr.X, StonePtr.Y] = 5;
                                RewardList[Index] = false;
                            }
                            if (Map[X, FirePtr.Y] == 5) RewardList[Index] = true;

                            Map[X, StonePtr.Y] = 2;
                            StonePtr.X = X;
                            StoneList[Index] = StonePtr;
                            BulletList[Index].Move(X, StonePtr.Y);
                        }
                        else
                        {
                            Map[X, StonePtr.Y] = 3;
                            Map[StonePtr.X, StonePtr.Y] = 0;
                            Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                            FireTime.Stop();
                            Frequ.Stop();
             //               MessageBox.Show("Game over!");

                            BulletList[Index].BulletPic = null;
                            BulletList[Index] = new Bullet();
                            FireEnd = true;
                        }
                    }
                    else
                    {
                        if (!(RewardList[Index]))
                        {
                            Map[StonePtr.X, StonePtr.Y] = 0;
                        }
                        else
                        {
                            Map[StonePtr.X, StonePtr.Y] = 5;
                            RewardList[Index] = false;
                        }
                        if (Map[X, FirePtr.Y] == 5) RewardList[Index] = true;
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        AlarmNumList[Index] = 0;
                        // Remove PictureBox
                        Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                        BulletList[Index].BulletPic = null;
                        BulletList[Index] = new Bullet();
                        FireEnd = true;
                    }
                }
            } // else if (Index < 18)
            else if (Index < 24) //同Index<6的原理
            {
                if (Map[FirePtr.X, FirePtr.Y] == 1)
                {

                    FireEnd = false;
                    int X = FirePtr.X - 1;
                    StonePtr.X = X;
                    StonePtr.Y = FirePtr.Y;

                    if (!(Map[X, FirePtr.Y] == 1))
                    {
                        if (Map[X, FirePtr.Y] == 5) RewardList[Index] = true;
                        Map[X, FirePtr.Y] = 2;
                    }
                    else
                    {
                        Map[X, FirePtr.Y] = 3;
                        Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                        FireTime.Stop();
                        Frequ.Stop();
            //            MessageBox.Show("Game over!");

                        BulletList[Index].BulletPic = null;
                        BulletList[Index] = new Bullet();
                        FireEnd = true;
                    }

                    Map[FirePtr.X, FirePtr.Y] = 0;
                    StoneList[Index] = StonePtr;

                    // PictureBox Handle
                    BulletList[Index] = new Bullet(WeaponName[State]+"_left.png", StonePtr.X, StonePtr.Y);
                    MyForm.Controls.Add(BulletList[Index].BulletPic);
                    BulletList[Index].BulletPic.BringToFront();
                    BulletList[Index].BulletPic.BackColor = Color.Transparent;
                    BulletList[Index].BulletPic.Parent = Mf_Pic;             
                }
                else
                {
                    int X = StonePtr.X - 1;
                    if (X > 0)
                    {
                        if (!(Map[X, StonePtr.Y] == 1))
                        {
                            
                            if (!(RewardList[Index]))
                            {
                                Map[StonePtr.X, StonePtr.Y] = 0;
                            }
                            else
                            {
                                Map[StonePtr.X, StonePtr.Y] = 5;
                                RewardList[Index] = false;
                            }
                            if (Map[X, FirePtr.Y] == 5) RewardList[Index] = true;

                            Map[X, StonePtr.Y] = 2;
                            StonePtr.X = X;
                            StoneList[Index] = StonePtr;
                            BulletList[Index].Move(X, StonePtr.Y);
                        }
                        else
                        {
                            Map[X, StonePtr.Y] = 3;
                            Map[StonePtr.X, StonePtr.Y] = 0;
                            Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                            FireTime.Stop();
                            Frequ.Stop();
              //              MessageBox.Show("Game over!");

                            BulletList[Index].BulletPic = null;
                            BulletList[Index] = new Bullet();
                            FireEnd = true;
                        }

                    }
                    else
                    {
                        if (!(RewardList[Index]))
                        {
                            Map[StonePtr.X, StonePtr.Y] = 0;
                        }
                        else
                        {
                            Map[StonePtr.X, StonePtr.Y] = 5;
                            RewardList[Index] = false;
                        }
                        if (Map[X, FirePtr.Y] == 5) RewardList[Index] = true;

                        Map[FirePtr.X, FirePtr.Y] = 0;
                        AlarmNumList[Index] = 0;
                        // Remove PictureBox
                        Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                        BulletList[Index].BulletPic = null;
                        BulletList[Index] = new Bullet();
                        FireEnd = true;
                    }
                }
            } //else if (Index < 24)


        } // public void Fire(ref int[,] Map, int Index)

        public void Fire_Light(int Index)   // 發射火焰的 Function 一樣分成4個
        {
            FireLocation FirePtr = FireList[Index];
            FireLocation StonePtr = StoneList[Index];
            //MessageBox.Show(Convert.ToString(Index) + "+1");
            int i;

            if (Index < 6) // 上往下射
            {
                if (FireNumList[Index] == 0) // 第一次發射
                {
                    if (Map[FirePtr.X, FirePtr.Y] == 1) // 當FireAlarm將警告格改成1時, 就將之後的Array都改成2
                    {
                        FireEnd = false;
                        int Y = FirePtr.Y + 1;
                        StonePtr.X = FirePtr.X;
                        StonePtr.Y = Y;

                        for (i = 1; i < 7; i++)  // 一次修改整排
                        {
                            if (!(Map[FirePtr.X, i] == 1)) // 如果有任何一格是人, 結束遊戲
                            {
                                if (Map[FirePtr.X, i] == 5)  // 如果有任何一格是bonus 紀錄位置
                                {
                                    RewardList[Index] = true;
                                    PointList[Index] = new Point(FirePtr.X, i);
                                } 
                                Map[FirePtr.X, i] = 2; 
                                
                            }
                            else // 結束遊戲 全部重置
                            {
                                Map[FirePtr.X, i] = 3;
                                Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                                FireTime.Stop();
                                Frequ.Stop();
                        //        MessageBox.Show("Game over!");

                                BulletList[Index].BulletPic = null;
                                BulletList[Index] = new Bullet();
                                FireEnd = true;
                            }
                        }
                            Map[FirePtr.X, FirePtr.Y] = 0;
                            StoneList[Index] = StonePtr;

                            // PictureBox Handle
                            BulletList[Index] = new Bullet(WeaponName[State] + "_down.png", StonePtr.X, StonePtr.Y);
                            BulletList[Index].BulletPic.Size = new Size(50, 300);
                            MyForm.Controls.Add(BulletList[Index].BulletPic);
                            BulletList[Index].BulletPic.BringToFront();
                            BulletList[Index].BulletPic.BackColor = Color.Transparent;
                            BulletList[Index].BulletPic.Parent = Mf_Pic;
                        
                    }
                    FireNumList[Index] += 1;  // 計算火焰停留的時間
                }
                else if(FireNumList[Index] == 2)   // 兩個週期後消失
                {
                    for(i=1;i<7;i++)
                    {
                        Map[StonePtr.X, i] = 0;  // 初始整排的array
                    }
                    if (RewardList[Index]) // 重新寫上coin的位置
                    {
                        int x = PointList[Index].X;
                        int y = PointList[Index].Y;
                        Map[x,y] = 5;
                        RewardList[Index] = false;
                    }
                    Map[FirePtr.X, FirePtr.Y] = 0;
                    AlarmNumList[Index] = 0;
                    FireNumList[Index] = 0;
                    
                    // Remove PictureBox
                    
                    Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
               //     MessageBox.Show(Convert.ToString(BulletList[Index].BulletPic.Parent));
                    BulletList[Index].BulletPic = null;
                    BulletList[Index] = new Bullet();
                    FireEnd = true;
                }
                else
                {
                    FireNumList[Index] += 1; // 計算時間
                }
            }// if (Index < 6) 
            else if (Index < 12) // 下往上射 同Index <6的原理
            {
                if (FireNumList[Index] == 0)
                {
                    if (Map[FirePtr.X, FirePtr.Y] == 1)
                    {
                        FireEnd = false;
                        int Y = FirePtr.Y -1;
                        StonePtr.X = FirePtr.X;
                        StonePtr.Y = Y;

                        for (i = 1; i < 7; i++)
                        {
                            if (!(Map[FirePtr.X, i] == 1))
                            {
                                if (Map[FirePtr.X, i] == 5)
                                {
                                    RewardList[Index] = true;
                                    PointList[Index] = new Point(FirePtr.X, i);
                                } 
                                Map[FirePtr.X, i] = 2;
                            }
                            else
                            {
                                Map[FirePtr.X, i] = 3;
                                Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                                FireTime.Stop();
                                Frequ.Stop();
                       //         MessageBox.Show("Game over!");

                                BulletList[Index].BulletPic = null;
                                BulletList[Index] = new Bullet();
                                FireEnd = true;
                            }
                        }
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        StoneList[Index] = StonePtr;

                        // PictureBox Handle
                        BulletList[Index] = new Bullet(WeaponName[State] + "_up.png", StonePtr.X, 1);
                        BulletList[Index].BulletPic.Size = new Size(50, 300);
                        MyForm.Controls.Add(BulletList[Index].BulletPic);
                        BulletList[Index].BulletPic.BringToFront();
                        BulletList[Index].BulletPic.BackColor = Color.Transparent;
                        BulletList[Index].BulletPic.Parent = Mf_Pic;

                    }
                    FireNumList[Index] += 1;
                }
                else if (FireNumList[Index] == 2)
                {
                    for (i = 1; i < 7; i++)
                    {
                        Map[StonePtr.X, i] = 0;
                    }
                    if (RewardList[Index])
                    {
                        int x = PointList[Index].X;
                        int y = PointList[Index].Y;
                        Map[x, y] = 5;
                        RewardList[Index] = false;
                    }
                    Map[FirePtr.X, FirePtr.Y] = 0;
                    AlarmNumList[Index] = 0;
                    FireNumList[Index] = 0;

                    // Remove PictureBox

                    Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                    //     MessageBox.Show(Convert.ToString(BulletList[Index].BulletPic.Parent));
                    BulletList[Index].BulletPic = null;
                    BulletList[Index] = new Bullet();
                    FireEnd = true;
                }
                else
                {
                    FireNumList[Index] += 1;
                }
            }// if (Index < 12) 
            else if (Index < 18) // 左往右射 同Index <6的原理
            {
                if (FireNumList[Index] == 0)
                {
                    if (Map[FirePtr.X, FirePtr.Y] == 1)
                    {
                        FireEnd = false;
                        int X = FirePtr.X + 1;
                        StonePtr.X = X;
                        StonePtr.Y = FirePtr.Y;

                        for (i = 1; i < 7; i++)
                        {
                            if (!(Map[i,FirePtr.Y] == 1))
                            {
                                if (Map[i,FirePtr.Y] == 5)
                                {
                                    RewardList[Index] = true;
                                    PointList[Index] = new Point(i, FirePtr.Y);
                                } 
                                Map[i, FirePtr.Y] = 2;
                            }
                            else
                            {
                                Map[i, FirePtr.Y] = 3;
                                Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                                FireTime.Stop();
                                Frequ.Stop();
                       //         MessageBox.Show("Game over!");

                                BulletList[Index].BulletPic = null;
                                BulletList[Index] = new Bullet();
                                FireEnd = true;
                            }
                        }
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        StoneList[Index] = StonePtr;

                        // PictureBox Handle
                        BulletList[Index] = new Bullet(WeaponName[State] + "_right.png", StonePtr.X, StonePtr.Y);
                        BulletList[Index].BulletPic.Size = new Size(300, 50);
                        MyForm.Controls.Add(BulletList[Index].BulletPic);
                        BulletList[Index].BulletPic.BringToFront();
                        BulletList[Index].BulletPic.BackColor = Color.Transparent;
                        BulletList[Index].BulletPic.Parent = Mf_Pic;

                    }
                    FireNumList[Index] += 1;
                }
                else if (FireNumList[Index] == 2)
                {
                    for (i = 1; i < 7; i++)
                    {
                        Map[i, StonePtr.Y] = 0;
                    }
                    if (RewardList[Index])
                    {
                        int x = PointList[Index].X;
                        int y = PointList[Index].Y;
                        Map[x, y] = 5;
                        RewardList[Index] = false;
                    }
                    Map[FirePtr.X, FirePtr.Y] = 0;
                    AlarmNumList[Index] = 0;
                    FireNumList[Index] = 0;

                    // Remove PictureBox

                    Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                    //     MessageBox.Show(Convert.ToString(BulletList[Index].BulletPic.Parent));
                    BulletList[Index].BulletPic = null;
                    BulletList[Index] = new Bullet();
                    FireEnd = true;
                }
                else
                {
                    FireNumList[Index] += 1;
                }
            }// if (Index < 18) 
            else if (Index < 24) // 右往左射 同Index <6的原理
            {
                if (FireNumList[Index] == 0)
                {
                    if (Map[FirePtr.X, FirePtr.Y] == 1)
                    {
                        FireEnd = false;
                        int X = FirePtr.X - 1;
                        StonePtr.X = X;
                        StonePtr.Y = FirePtr.Y;

                        for (i = 1; i < 7; i++)
                        {
                            if (!(Map[i, FirePtr.Y] == 1))
                            {
                                if (Map[i, FirePtr.Y] == 5)
                                {
                                    RewardList[Index] = true;
                                    PointList[Index] = new Point(i, FirePtr.Y);
                                } 
                                Map[i, FirePtr.Y] = 2;
                            }
                            else
                            {
                                Map[i, FirePtr.Y] = 3;
                                Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                                FireTime.Stop();
                                Frequ.Stop();
                          //      MessageBox.Show("Game over!");

                                BulletList[Index].BulletPic = null;
                                BulletList[Index] = new Bullet();
                                FireEnd = true;
                            }
                        }
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        StoneList[Index] = StonePtr;

                        // PictureBox Handle
                        BulletList[Index] = new Bullet(WeaponName[State] + "_left.png", 1, StonePtr.Y);
                        BulletList[Index].BulletPic.Size = new Size(300, 50);
                        MyForm.Controls.Add(BulletList[Index].BulletPic);
                        BulletList[Index].BulletPic.BringToFront();
                        BulletList[Index].BulletPic.BackColor = Color.Transparent;
                        BulletList[Index].BulletPic.Parent = Mf_Pic;

                    }
                    FireNumList[Index] += 1;
                }
                else if (FireNumList[Index] == 2)
                {
                    for (i = 1; i < 7; i++)
                    {
                        Map[i, StonePtr.Y] = 0;
                    }
                    if (RewardList[Index])
                    {
                        int x = PointList[Index].X;
                        int y = PointList[Index].Y;
                        Map[x, y] = 5;
                        RewardList[Index] = false;
                    }
                    Map[FirePtr.X, FirePtr.Y] = 0;
                    AlarmNumList[Index] = 0;
                    FireNumList[Index] = 0;

                    // Remove PictureBox

                    Mf_Pic.Controls.Remove(BulletList[Index].BulletPic);
                    //     MessageBox.Show(Convert.ToString(BulletList[Index].BulletPic.Parent));
                    BulletList[Index].BulletPic = null;
                    BulletList[Index] = new Bullet();
                    FireEnd = true;
                }
                else
                {
                    FireNumList[Index] += 1;
                }
            }// if (Index < 12) 
          }

    } // class Weapon

   
} // namespace WindowsFormsApplication1

