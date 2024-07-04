using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace chessgame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            panel1.Visible = true;
            foreach (Label item in this.Controls.OfType<Label>())
            {
                item.MouseDown += mouse_down;
                item.MouseMove += mouse_move;
                item.MouseUp += mouse_up;
            }
        }
        public Form1(string player1name, string player2name,bool mode, bool turn,string playersturn)
        {
            InitializeComponent();
            foreach (Label item in this.Controls.OfType<Label>())
            {
                item.MouseDown += mouse_down;
                item.MouseMove += mouse_move;
                item.MouseUp += mouse_up;
            }
            if (playersturn.Equals(player1name))
            {
                player1 = new player(player1name, "white", true);
                player2 = new player(player2name, "black", false);
            }
            else
            {
                player1 = new player(player1name, "black", false);
                player2 = new player(player2name, "white", true);
            }
            if (mode)
            {
                timed = true;
                button1.Enabled = false;
                this.BackgroundImage = Properties.Resources.chessboardt;
                this.MinimumSize = new Size(1079, 577);
                this.MaximumSize = new Size(1079, 577);
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }
            else
            {
                timed = false;
                button1.Enabled = true;
                this.BackgroundImage = Properties.Resources.chessboardt;
                this.MinimumSize = new Size(814, 580);
                this.MaximumSize = new Size(814, 580);
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }
        public class player
        {
            public string Name { get; set; }
            public string Pawns { get; set; }
            public bool Turn { get; set; }
            public player(string name, string pawns, bool turn)
            {
                Name = name;
                Pawns = pawns;
                Turn = turn;
            }
            public player() { }

        }
        public class game
        {
            public string Winner { get; set; }
            public string Time { get; set; }
            public string Winner_Pawns { get; set; }
            public game(string winner, string time, string winner_pawns)
            {
                Winner = winner;
                Time = time;
                Winner_Pawns = winner_pawns;
            }
        }
        string last;
        game game1;
        player player1;
        player player2;
        String connectionString = "Data Source=DB1.db;Version=3;";
        SQLiteConnection conn;
        bool startflag, flag, colorflag, found, timed, endflag;
        List<int> blacklist = new List<int>();
        List<int> whitelist = new List<int>();
        Random position = new Random();
        int timer_minutes1, timer_secs1, timer_minutes2, timer_secs2;
        DateTime starttime, endtime;
        private void mouse_down(object sender, MouseEventArgs e)
        {
            if (startflag)
            {
                flag = true;
                
            }
        }

        private void minutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            timer_minutes1 = 5;
            timer_secs1 = 0;
            timer_minutes2 = 5;
            timer_secs2 = 0;
            button2.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
            button3.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (timer_secs2 < 10)
            {
                button3.Text = "Time " + timer_minutes2.ToString() + ":0" + timer_secs2.ToString();
            }
            else
            {
                button3.Text = "Time " + timer_minutes2.ToString() + ":" + timer_secs2.ToString();
            }
            if (timer_minutes2 != 0 || timer_secs2 != 0)
            {
                if (timer_secs2 == 0)
                {
                    timer_minutes2--;
                    timer_secs2 = 59;
                }
                else
                {
                    timer_secs2--;
                }
            }
            else
            {
                endtime = DateTime.Now;
                startflag = false;
                endflag = true;
                MessageBox.Show("Game over");
                button1.Text = "Retry ⟲";
                button1.Visible = true;
                if (player2.Turn == false)
                {
                    last = endtime.Subtract(starttime).TotalMinutes.ToString();
                    game1 = new game(player2.Name, last, player2.Pawns);
                }
                timer2.Enabled = false;
                conn.Open();
                String insertQuery = "Insert into chess(Opponents,Duration,Start Time,End Time, Winner, Winning Pawns) values ('" + player1.Name + " " + player2.Name + "','" + last + "','" + starttime + "','" + endtime + "','" + game1.Winner + "','" + game1.Winner_Pawns + "')";
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                conn.Close();
            }
        }

        private void mouse_move(object sender, MouseEventArgs e)
        {
            Label thislabel = label1;
            if (flag)
            {
                if (e.Location.Y < 0)
                {
                    ((Label)sender).Location = new Point((((Label)sender).Location).X, (((Label)sender).Location).Y - 60);
                }
                else if (e.Location.Y > 60) {
                    ((Label)sender).Location = new Point((((Label)sender).Location).X, (((Label)sender).Location).Y + 60);
                } else if (e.Location.X < 0)
                {
                    ((Label)sender).Location = new Point((((Label)sender).Location).X - 60, (((Label)sender).Location).Y);
                }
                else if (e.Location.X > 60)
                {
                    ((Label)sender).Location = new Point((((Label)sender).Location).X + 60, (((Label)sender).Location).Y);
                }
                foreach (Label item in this.Controls.OfType<Label>())
                {
                    colorflag = false;
                    if (GetDistance(((Label)sender).Location.X, ((Label)sender).Location.Y, item.Location.X, item.Location.Y) < 50)
                    {
                        if ((item.Name != ((Label)sender).Name))
                        {
                            if ((item.Name.Contains("white") && ((Label)sender).Name.Contains("white")) || (item.Name.Contains("black") && ((Label)sender).Name.Contains("black")))
                            {
                                colorflag = true;
                            }
                            if (colorflag.Equals(false))
                            {
                                if (item.Name.Contains("black"))
                                {
                                    int randpos = position.Next(17, 33);
                                    foreach (int number in blacklist)
                                    {
                                        if (randpos.Equals(number))
                                        {
                                            blacklist.Remove(number);
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (found == true)
                                    {
                                        foreach (Label label in this.Controls.OfType<Label>())
                                        {
                                            if (label.Name.Equals("label" + randpos))
                                            {
                                                thislabel = label;
                                                break;
                                            }
                                        }
                                        item.Location = thislabel.Location;
                                        thislabel.Visible = false;
                                        found = false;
                                        if (item.Name.Contains("king")) {
                                            timer1.Enabled = false;
                                            timer2.Enabled = false;
                                            endtime = DateTime.Now;
                                            startflag = false;
                                            endflag = true;
                                            MessageBox.Show("Game over");
                                            last = endtime.Subtract(starttime).TotalMinutes.ToString();
                                            if (float.Parse(last) < 1)
                                            {
                                                last = ((endtime.Subtract(starttime).TotalMinutes)*60).ToString() + "secs";
                                            } else if (float.Parse(last)>60)
                                            {
                                                last = ((endtime.Subtract(starttime).TotalMinutes) / 60).ToString() + "hours";
                                            } else 
                                            {
                                                last = ((endtime.Subtract(starttime).TotalMinutes) / 60).ToString() + "mins";
                                            }
                                            if (player1.Pawns.Equals("white"))
                                            {
                                                game1 = new game(player1.Name, last, player1.Pawns);
                                            }
                                            else
                                            {
                                                game1 = new game(player2.Name, last, player2.Pawns);
                                            }
                                            button1.Text = "Retry ⟲";
                                            conn.Open();
                                            String insertQuery = "Insert into chess(Opponents,Duration,StartTime,EndTime, Winner, WinningPawns) values ('" + player1.Name + " " + player2.Name + "','" + last + "','" + starttime + "','" + endtime + "','" + game1.Winner + "','" + game1.Winner_Pawns + "')";
                                            SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn);
                                            SQLiteDataReader reader = cmd.ExecuteReader();
                                            conn.Close();
                                            button1.Visible = true;

                                        }
                                    }
                                }
                                else if (item.Name.Contains("white"))
                                {
                                    int randpos = position.Next(1, 17);
                                    foreach (int number in whitelist)
                                    {
                                        if (randpos.Equals(number))
                                        {
                                            whitelist.Remove(number);
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (found == true)
                                    {
                                        foreach (Label label in this.Controls.OfType<Label>())
                                        {
                                            if (label.Name.Equals("label" + randpos))
                                            {
                                                thislabel = label;
                                                break;

                                            }
                                        }
                                        item.Location = thislabel.Location;
                                        thislabel.Visible = false;
                                        found = false;
                                        if (item.Name.Contains("king"))
                                        {
                                            timer1.Enabled = false;
                                            timer2.Enabled = false;
                                            endtime = DateTime.Now;
                                            startflag = false;
                                            endflag = true;
                                            MessageBox.Show("Game over");
                                            last = endtime.Subtract(starttime).TotalMinutes.ToString();
                                            if (float.Parse(last) < 1)
                                            {
                                                last = ((endtime.Subtract(starttime).TotalMinutes) * 60).ToString() + "secs";
                                            }
                                            else if (float.Parse(last) > 60)
                                            {
                                                last = ((endtime.Subtract(starttime).TotalMinutes) / 60).ToString() + "hours";
                                            }
                                            else
                                            {
                                                last = ((endtime.Subtract(starttime).TotalMinutes) / 60).ToString() + "mins";
                                            }
                                            if (player1.Pawns.Equals("white"))
                                            {
                                                game1 = new game(player2.Name, last, player2.Pawns);
                                            }
                                            else
                                            {
                                                game1 = new game(player1.Name, last, player1.Pawns);
                                            }
                                            button1.Text = "Retry ⟲";
                                            conn.Open();
                                            String insertQuery = "Insert into chess(Opponents,Duration,StartTime,EndTime, Winner, WinningPawns) values ('" + player1.Name +" " + player2.Name + "','" + last + "','" + starttime + "','" + endtime + "','" + game1.Winner + "','" + game1.Winner_Pawns + "')";
                                            SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn);
                                            SQLiteDataReader reader = cmd.ExecuteReader();
                                            conn.Close();
                                            button1.Visible = true;

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void mouse_up(object sender, MouseEventArgs e)
        {
            flag = false;
            if (timed) {
                if (startflag)
                {
                    if (timer1.Enabled)
                    {
                        timer1.Enabled = false;
                        timer2.Enabled = true;
                    }
                    else if (timer2.Enabled == false)
                    {
                        timer1.Enabled = true;
                    }

                    else
                    {
                        if (timer2.Enabled)
                        {
                            timer2.Enabled = false;
                            timer1.Enabled = true;
                        }
                        else if (timer1.Enabled == false)
                        {
                            timer2.Enabled = true;

                        }
                       
                    }
                    if (player1.Turn)
                    {
                        player1.Turn = false;
                        player2.Turn = true;
                    }
                    else
                    {
                        player1.Turn = true;
                        player2.Turn = false;
                    }
                }
            }
            }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var answer = MessageBox.Show("Are you sure you want to exit game?", "Exiting game", MessageBoxButtons.YesNo);
                if (answer == DialogResult.No)
                {
                    /* Cancel the Closing event from closing the form. */
                    e.Cancel = true;
                }

                else if (answer == DialogResult.Yes)
                {
                    /* Closing the form. */
                    e.Cancel = false;
                    Application.Exit();
                }
            }
        }

        private void minutesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            timer_minutes1 = 10;
            timer_secs1 = 0;
            timer_minutes2 = 10;
            timer_secs2 = 0;
            button2.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
            button3.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
        }


        private void miutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            timer_minutes1 = 15;
            timer_secs1 = 0;
            timer_minutes2 = 15;
            timer_secs2 = 0;
            button2.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
            button3.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            label41.Text= DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if ((String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox1.Text)) || ((radioButton1.Checked && radioButton3.Checked) || (radioButton2.Checked && radioButton4.Checked)))
            {
                MessageBox.Show("Please enter valid game settings");
            }
            else
            {
                if (radioButton1.Checked)
                {
                    player1 = new player(textBox1.Text, "black", false);
                    player2 = new player(textBox2.Text, "white", true);
                }
                else if (radioButton2.Checked)
                {
                    player1 = new player(textBox1.Text, "white", true);
                    player2 = new player(textBox2.Text, "black", false);
                }
                if (radioButton5.Checked)
                {

                    timed = true;
                    button1.Enabled = false;
                    this.BackgroundImage = Properties.Resources.chessboardt;
                    this.MinimumSize = new Size(1079, 577);
                    this.MaximumSize = new Size(1079, 577);
                    if (this.WindowState == FormWindowState.Maximized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    panel1.Hide();
                }
                else if (radioButton6.Checked)
                {
                    timed = false;
                    button1.Enabled = true;
                    this.BackgroundImage = Properties.Resources.chessboardt;
                    this.MinimumSize = new Size(814, 580);
                    this.MaximumSize = new Size(814, 580);
                    if (this.WindowState == FormWindowState.Maximized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    panel1.Hide();
                }
            }

        }

        private void minuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            timer_minutes1 = 30;
            timer_secs1 = 0;
            timer_minutes2 = 30;
            timer_secs2 = 0;
            button2.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
            button3.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
        }

        private void minutesToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            timer_minutes1 = 40;
            timer_secs1 = 0;
            timer_minutes2 = 40;
            timer_secs2 = 0;
            button2.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
            button3.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
        }

        private void minutesToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            timer_minutes1 = 45;
            timer_secs1 = 0;
            timer_minutes2 = 45;
            timer_secs2 = 0;
            button2.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
            button3.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
        }
        private void minutesToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            timer_minutes1 = 60;
            timer_secs1 = 0;
            timer_minutes2 = 60;
            timer_secs2 = 0;
            button2.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
            button3.Text = "Time " + timer_minutes1 + ":0" + timer_secs1;
        }

        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            label41.Text= DateTime.Now.ToString("hh:mm:ss tt");
            timer3.Enabled = true;
            conn = new SQLiteConnection(connectionString);
            radioButton1.Checked = true;
            radioButton4.Checked = true;
            radioButton5.Checked = true;

            for (int i = 1; i < 17; i++)
            {
                whitelist.Add(i);
            }
            for (int i = 17; i < 33; i++)
            {
                blacklist.Add(i);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
           
            if (endflag)
            {
                Form1 form1;
                if (player1.Turn)
                {
                    form1 = new Form1(player1.Name, player2.Name, timed, player1.Turn,player1.Name);
                }
                else
                {
                    form1 = new Form1(player1.Name, player2.Name, timed, player2.Turn, player2.Name);
                }
                form1.Show();
                this.Hide();
                endflag = false;
            }
            if (timed)
            {
                timer1.Enabled = true;
                if (player1.Turn)
                {
                    player1.Turn = true;
                    player2.Turn = false;
                }
                else
                {
                    player1.Turn = false;
                    player2.Turn = true;
                }
            }
            starttime = DateTime.Now;
            startflag = true;
            button1.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer_secs1 < 10)
            {
                button2.Text = "Time " + timer_minutes1.ToString() + ":0" + timer_secs1.ToString();
            }
            else
            {
                button2.Text = "Time " + timer_minutes1.ToString() + ":" + timer_secs1.ToString();
            }
            if (timer_minutes1 != 0 || timer_secs1!=0)
            {
                if (timer_secs1 == 0)
                {
                    timer_minutes1 -- ;
                    timer_secs1 = 59;
                }
                else
                {
                    timer_secs1 -- ;
                }
            }
            else
            {
                if (player1.Turn == false) 
                {
                    last = endtime.Subtract(starttime).TotalMinutes.ToString();
                    game1 = new game(player2.Name , last, player2.Pawns);
                }
                
                if (float.Parse(last) < 1)
                {
                    last = ((endtime.Subtract(starttime).TotalMinutes) * 60).ToString() + "secs";
                }
                else if (float.Parse(last) > 60)
                {
                    last = ((endtime.Subtract(starttime).TotalMinutes) / 60).ToString() + "hours";
                }
                else
                {
                    last = ((endtime.Subtract(starttime).TotalMinutes) / 60).ToString() + "mins";
                }
                endtime = DateTime.Now;
                startflag = false;
                endflag = true;
                MessageBox.Show("Game over");
                button1.Text = "Retry ⟲";
                button1.Visible = true;
                timer1.Enabled = false;
                conn.Open();
                String insertQuery = "Insert into chess(Opponents,Duration,StartTime,EndTime, Winner, WinningPawns) values ('" +player1.Name + " " + player2.Name + "','" + last + "','" + starttime + "','" + endtime  + "','" + game1.Winner + "','" + game1.Winner_Pawns + "')";
                SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                conn.Close();
            }
        }  
    }
}