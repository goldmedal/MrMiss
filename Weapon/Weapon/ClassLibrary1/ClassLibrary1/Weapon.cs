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

namespace Weapon
{
    struct FireLocation
    {
        public int X; // X座標
        public int Y; // Y座標
        public int Index; // 發射編號
    } // struct FireLocation

    class Weapon
    {
        List<FireLocation> FireList;
        List<FireLocation> StoneList;
        List<Int32> AlarmNumList;
        ArrayList IndexList;
        Timer FireTime;
        Timer AlarmTime;
        private int[,] Map;
        int Speed;
        bool FireEnd;
        int Go = 0;


        public Weapon(int [,] map, int speed)
        {
            int i;

            FireList = new List<FireLocation>();
            StoneList = new List<FireLocation>();
            AlarmNumList = new List<Int32>();
            Int32 AlarmPtr;
            Map = map;
            FireTime = new Timer();
            AlarmTime = new Timer();
            Speed = speed; // Default
            FireEnd = true;

            InitialFireList();

            for (i = 0; i < 24; i++)
            {
                AlarmPtr = AlarmNumList[i];
                AlarmPtr = 0;
            }

        } // public Weapon()

        private void InitialFireList()
        {
            int i;
            FireLocation FirePtr;

            for (i = 0; i < 24; i++)
            {
                FireList.Add(new FireLocation());
                StoneList.Add(new FireLocation());
                AlarmNumList.Add(new Int32());
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

            /*            for (i = 0; i < 24; i++)
                        {
                            FirePtr = FireList[i];
                            MessageBox.Show("FireList" + Convert.ToString(i) + " , " + Convert.ToString(FirePtr.X) + " , " + Convert.ToString(FirePtr.Y));
                        }   */

        } // private void InitialFireList()

        public void RandomFire(int Num)// 速度, 頻率, 數量
        {
            Random Rad = new Random();
            IndexList = new ArrayList(Num);
            int Index;
            while (Num != 0)
            {
                Index = Rad.Next(0, 23);
                if (!(IndexList.Contains(Index)))
                {
                    IndexList.Add(Index);
                    Num -= 1;
                }
                //        MessageBox.Show(Convert.ToString(Index));
            }

            AlarmTime.Tick += new EventHandler(Alarm_Time_Tick);
            AlarmTime.Interval = 1000;
            AlarmTime.Start();

        } // public void RandFire(ref int[,] Map, int Speed, int Freq, int Num)

        private void Alarm_Time_Tick(object sender, EventArgs e)
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
                    if (i == (IndexList.Count - 1))
                    {
                        AlarmTime.Stop();
                        FireTime.Interval = Speed;
                        FireTime.Tick += Fire_Time_Tick;
                        FireTime.Start();
                    }
                }

            }



        } //private void Alarm_Time_Tick(object sender, EventArgs e)

        private void Fire_Time_Tick(object sender, EventArgs e)
        {
            int i;
            for (i = 0; i < IndexList.Count; i++)
            {
                int NowIndex = (int)IndexList[i];
                Fire(NowIndex);
                if (i == (IndexList.Count - 1))
                {
                    if (FireEnd)
                    {
                        //                 MessageBox.Show("Fire Stop!");
                        FireTime.Stop();
                    }
                }
            }
        } // private void Fire_Time_Tick(object sender, EventArgs e)

        public void FireAlarm(int Index)  // Alarm 數秒發射
        {
            FireLocation FirePtr = FireList[Index];

            if (AlarmNumList[Index] < 2)
            {
                AlarmNumList[Index] += 1;
            }
            else
            {
                //          MessageBox.Show( "Hi"+Convert.ToString(FirePtr.X)+" , "+Convert.ToString(FirePtr.Y));
                Map[FirePtr.X, FirePtr.Y] = 1;
                AlarmNumList[Index] += 1;
            }
        } // public void FireAlarm(ref int [,] Map,int Index) 

        public void Fire(int Index) //  發射點索引
        {                                                                        //  每次前進一格
            FireLocation FirePtr = FireList[Index];
            FireLocation StonePtr = StoneList[Index];
            //         MessageBox.Show(Convert.ToString(Index) + "+1");
            if (Index < 6) // 上往下射
            {

                if (Map[FirePtr.X, FirePtr.Y] == 1)
                {
                    FireEnd = false;
                    int Y = FirePtr.Y + 1;
                    StonePtr.X = FirePtr.X;
                    StonePtr.Y = Y;
                    Map[FirePtr.X, Y] = 2;
                    Map[FirePtr.X, FirePtr.Y] = 0;
                    StoneList[Index] = StonePtr;

                }
                else
                {
                    int Y = StonePtr.Y + 1;
                    if (Y < 7)
                    {
                        Map[StonePtr.X, Y] = 2;
                        Map[StonePtr.X, StonePtr.Y] = 0;
                        StonePtr.Y = Y;
                        StoneList[Index] = StonePtr;

                    }
                    else
                    {
                        Map[StonePtr.X, StonePtr.Y] = 0;
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        AlarmNumList[Index] = 0;
                        FireEnd = true;
                    }

                }
            } // if (Index < 6) 
            else if (Index < 12) // 由下往上射
            {
                if (Map[FirePtr.X, FirePtr.Y] == 1)
                {
                    FireEnd = false;
                    int Y = FirePtr.Y - 1;
                    StonePtr.X = FirePtr.X;
                    StonePtr.Y = Y;
                    Map[FirePtr.X, Y] = 2;
                    Map[FirePtr.X, FirePtr.Y] = 0;
                    StoneList[Index] = StonePtr;

                }
                else
                {
                    int Y = StonePtr.Y - 1;
                    if (Y > 0)
                    {
                        Map[StonePtr.X, Y] = 2;
                        Map[StonePtr.X, StonePtr.Y] = 0;
                        StonePtr.Y = Y;
                        StoneList[Index] = StonePtr;

                    }
                    else
                    {
                        Map[StonePtr.X, StonePtr.Y] = 0;
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        AlarmNumList[Index] = 0;
                        FireEnd = true;
                    }
                }
            } // else if (Index < 12)
            else if (Index < 18) // 由左往右
            {
                if (Map[FirePtr.X, FirePtr.Y] == 1)
                {
                    FireEnd = false;
                    int X = FirePtr.X + 1;
                    StonePtr.X = X;
                    StonePtr.Y = FirePtr.Y;
                    Map[X, FirePtr.Y] = 2;
                    Map[FirePtr.X, FirePtr.Y] = 0;
                    StoneList[Index] = StonePtr;

                }
                else
                {
                    int X = StonePtr.X + 1;
                    if (X < 7)
                    {
                        Map[X, StonePtr.Y] = 2;
                        Map[StonePtr.X, StonePtr.Y] = 0;
                        StonePtr.X = X;
                        StoneList[Index] = StonePtr;

                    }
                    else
                    {
                        Map[StonePtr.X, StonePtr.Y] = 0;
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        AlarmNumList[Index] = 0;
                        FireEnd = true;
                    }
                }
            } // else if (Index < 18)
            else if (Index < 24)
            {
                if (Map[FirePtr.X, FirePtr.Y] == 1)
                {
                    FireEnd = false;
                    int X = FirePtr.X - 1;
                    StonePtr.X = X;
                    StonePtr.Y = FirePtr.Y;
                    Map[X, FirePtr.Y] = 2;
                    Map[FirePtr.X, FirePtr.Y] = 0;
                    StoneList[Index] = StonePtr;

                }
                else
                {
                    int X = StonePtr.X - 1;
                    if (X > 0)
                    {
                        Map[X, StonePtr.Y] = 2;
                        Map[StonePtr.X, StonePtr.Y] = 0;
                        StonePtr.X = X;
                        StoneList[Index] = StonePtr;

                    }
                    else
                    {
                        Map[StonePtr.X, StonePtr.Y] = 0;
                        Map[FirePtr.X, FirePtr.Y] = 0;
                        AlarmNumList[Index] = 0;
                        FireEnd = true;
                    }
                }
            } //else if (Index < 24)


        } // public void Fire(ref int[,] Map, int Index)
    } // class Weapon
} // namespace WindowsFormsApplication1
