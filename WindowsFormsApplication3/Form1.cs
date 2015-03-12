using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1.Location = new Point(368, 196);
            button1.Size = new Size(188, 43);
            button2.Location = new Point(368, 260);
            button2.Size = new Size(188, 43);
            button3.Location = new Point(368, 323);
            button3.Size = new Size(188, 43);
            pictureBox1.Size = new Size(686, 462);
            pictureBox1.Location = new Point(0, 0);
            this.Width = 700;
            this.Height = 500;  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            MyForm game = new MyForm();
            if (game.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            { 
            }
            game = null;
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(Environment.ExitCode);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int[] top10 = new int[10];
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
            MessageBox.Show("TOP 10:\n\n1.\t" + top10[0] +
                            "\n2.\t" + top10[1] + "\n3.\t" + top10[2] +
                            "\n4.\t" + top10[3] + "\n5.\t" + top10[4] +
                            "\n6.\t" + top10[5] + "\n7.\t" + top10[6] +
                            "\n8.\t" + top10[7] + "\n9.\t" + top10[8] +
                            "\n10.\t" + top10[9]);
        }


    }
}
