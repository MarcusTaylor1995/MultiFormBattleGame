using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiFormGameDemo
{
    public partial class ThirdForm : Form
    {
        private GameHelper game;

        Random rand = new Random();


        int goodGuySpeed = 7;
        bool hasgun = true;


        int sentinelSpeed = 5;
        List<PictureBox> sentinelPBList = new List<PictureBox>();
        List<int> sentinelDirList = new List<int>(); // 0 is up and 1 is down
        List<Point> badguyStart = new List<Point>();

        int trackerSpeed = 9;

        int bulletSpeed = 9;
        int bulletDelay = 20;
        List<PictureBox> bulletList = new List<PictureBox>();

        List<PictureBox> allBadGuys = new List<PictureBox>();
        List<PictureBox> deadBadGuys = new List<PictureBox>();
        List<PictureBox> Tracker = new List<PictureBox>();



        int badBulletSpeed = 10;
        int badBulletDelay = 10;
        int maxBadBulletDelay = 25;
        List<PictureBox> badBulletList = new List<PictureBox>();

        public ThirdForm()
        {
            InitializeComponent();

            sentinelPBList.Add(pictureBoxVDragon);
            sentinelPBList.Add(pictureBoxDragon);

            foreach (PictureBox pb in allBadGuys)
            {
                badguyStart.Add(pb.Location);
            }

            Tracker.Add(pictureBoxBull);
            Tracker.Add(pictureBoxSpirit);

            sentinelDirList.Add(0);
            sentinelDirList.Add(0);
            sentinelDirList.Add(0);



            allBadGuys.AddRange(sentinelPBList);
            allBadGuys.AddRange(Tracker);




            game = new GameHelper(this);
            game.Update += game_Update;
            game.Start();
        }

        private void game_Update(object sender, EventArgs e)
        {
            MovePlayer();
            CheckDoors();
            TrackGun();
            CheckGun();
            CheckBadGun();
            ShootGun();
            HandleDragonMove();
            TrackBullets();
            TrackBadBullets();
            BadGuyAttacks();
            MoveTracker();


            bool allDead = true;
            HeavenLights.Visible = false;
            BacktoCaverns.Visible = false;
            for (int i = 0; i < allBadGuys.Count; i++)
            {
                PictureBox pb = allBadGuys[i];

                if (pb.Visible == true)
                {

                    allDead = false;
                    break; // slightly more efficient

                }

                if (allDead == true && pictureBoxDragon.Visible == false && pictureBoxVDragon.Visible == false)
                {
                    BacktoCaverns.Visible = true;
                    HeavenLights.Visible = true;
                    Win();
                    
                

                }

            }
        }












        




        private void MovePlayer()
        {
            if (game.IsPressed(Keys.W))
            {
                pictureBoxPlayer.Top -= goodGuySpeed;
                if (pictureBoxPlayer.IsInsideOfForm() == false)
                {
                    pictureBoxPlayer.Top += goodGuySpeed;
                }
            }
            if (game.IsPressed(Keys.A))
            {
                pictureBoxPlayer.Left -= goodGuySpeed;
                if (pictureBoxPlayer.IsInsideOfForm() == false)
                {
                    pictureBoxPlayer.Left += goodGuySpeed;
                }
            }
            if (game.IsPressed(Keys.S))
            {
                pictureBoxPlayer.Top += goodGuySpeed;
                if (pictureBoxPlayer.IsInsideOfForm() == false)
                {
                    pictureBoxPlayer.Top -= goodGuySpeed;
                }
            }
            if (game.IsPressed(Keys.D))
            {
                pictureBoxPlayer.Left += goodGuySpeed;
                if (pictureBoxPlayer.IsInsideOfForm() == false)
                {
                    pictureBoxPlayer.Left -= goodGuySpeed;
                }
            }
        }


        private bool CanMove(PictureBox pb)
        {
            if (pb.IsInsideOfForm() == false)
            {
                return false;
            }


            return true;
        }
        public void CheckGun()
        {
            hasgun = true;
            pictureBoxGun.Visible = false;
            pictureBoxBullet.Visible = false;
        }

        public void CheckBadGun()
        {
            hasgun = true;
            pictureBoxFlame.Visible = false;
            pictureBoxFlame2.Visible = false;
        }



        public void ShootGun()
        {
            if (hasgun == true)
            {
                //*********************************************************************
                // SHOOT BULLETS
                // This is a bit tricky because we have to add new controls (Picture
                // Boxes) to the Window, track them, and remove them when they hit a
                // bad guy/wall or go off screen. The bulletDelay is there simply to
                // prevent a bullet from firing EVERY frame (which basically makes it
                // a laser or gatling gun.
                //*********************************************************************
                if (bulletDelay > 0)
                {
                    bulletDelay--;
                }
                if (game.IsPressed(Keys.Space) && bulletDelay == 0)
                {
                    bulletDelay = 20; // Wait 10 frames before firing another bullet

                    // Create a new bullet (PictureBox);
                    PictureBox bullet = new PictureBox();
                    bullet.Width = pictureBoxBullet.Width;
                    bullet.Height = pictureBoxBullet.Height;
                    bullet.BackColor = Color.Transparent;
                    bullet.Image = pictureBoxBullet.Image;

                    // Position it at the good guy's position
                    bullet.Top = pictureBoxPlayer.Top + pictureBoxPlayer.Height / 2;
                    bullet.Left = pictureBoxPlayer.Left + pictureBoxPlayer.Width;
                    this.Controls.Add(bullet); // MAGIC
                    bulletList.Add(bullet); // MY OWN LIST
                }
            }
        }

        private void TrackGun()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                PictureBox bullet = bulletList[i];
                bullet.Left += bulletSpeed;

                // Remove the bullet if it leaves the screen
                // OR touch a bush
                if (CanMove(bullet) == false)
                {
                    bulletList.Remove(bullet); // MY LIST
                    this.Controls.Remove(bullet); // MAGIC
                }
            }
        }








        //*********************************************************************
        // SHOOT BAD BULLETS


        public void BadGuyAttacks()
        {

            if (badBulletDelay > 0)
            {
                badBulletDelay--;

            }
            if (badBulletDelay == 0)
            {
                badBulletDelay = maxBadBulletDelay;


                // Create a new bullet (PictureBox);
                PictureBox badbullet = new PictureBox();
                badbullet.Width = pictureBoxFlame.Width;
                badbullet.Height = pictureBoxFlame.Height;
                badbullet.BackColor = Color.Transparent;
                badbullet.Image = pictureBoxFlame.Image;

                // Position it at the good guy's position
                badbullet.Top = pictureBoxDragon.Top + pictureBoxDragon.Height / 2;
                badbullet.Left = pictureBoxDragon.Left + pictureBoxDragon.Width;
                this.Controls.Add(badbullet); // MAGIC
                badBulletList.Add(badbullet); // MY OWN LIST
            }



            if (badBulletDelay > 0)
            {
                badBulletDelay--;

            }
            if (badBulletDelay == 0)
            {
                badBulletDelay = maxBadBulletDelay;


                // Create a new bullet (PictureBox);
                PictureBox badbullet = new PictureBox();
                badbullet.Width = pictureBoxFlame2.Width;
                badbullet.Height = pictureBoxFlame2.Height;
                badbullet.BackColor = Color.Transparent;
                badbullet.Image = pictureBoxFlame2.Image;

                // Position it at the good guy's position
                badbullet.Top = pictureBoxVDragon.Top + pictureBoxVDragon.Height / 2;
                badbullet.Left = pictureBoxVDragon.Left + pictureBoxVDragon.Width;
                this.Controls.Add(badbullet); // MAGIC
                badBulletList.Add(badbullet); // MY OWN LIST
            }


        }











        private void HandleDragonMove()
        {

            for (int i = 0; i < sentinelPBList.Count; i++)
            {
                PictureBox sentinel = sentinelPBList[i];
                int direction = sentinelDirList[i];


                // Skip if invisible (must be dead)
                if (sentinel.Visible == false)
                {
                    continue;
                }


                if (direction == 0)
                {
                    // up
                    sentinel.Top -= sentinelSpeed;
                }
                else
                {
                    // down
                    sentinel.Top += sentinelSpeed;
                }


                if (sentinel.IsInsideOfForm() == false)
                {
                    if (direction == 1)
                    {
                        sentinelDirList[i] = 0;
                    }
                    else
                    {
                        sentinelDirList[i] = 1;
                    }
                }
            }
        }


        void MoveTracker()

        {
            for (int i = 0; i < Tracker.Count; i++)
            {
                PictureBox Trackers = Tracker[i];

                // Skip if invisible (must be dead)
                if (Trackers.Visible == false)
                {
                    continue;
                }

                // Oh No!! Math!!!! (just Pythagoras and ratios)
                int dx = Trackers.Left - pictureBoxPlayer.Left;
                int dy = Trackers.Top - pictureBoxPlayer.Top;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance != 0)
                {
                    double distanceRatio = trackerSpeed / distance;
                    Trackers.Left -= (int)(dx * distanceRatio);
                    Trackers.Top -= (int)(dy * distanceRatio);
                }
            }
        }

        







        

        private void ResetGame()
        {
            foreach (PictureBox bullet in bulletList)
            {
                this.Controls.Remove(bullet);
            }
            foreach (PictureBox bullet in badBulletList)
            {
                this.Controls.Remove(bullet);
            }
            foreach (PictureBox trackers in Tracker)
            {
                this.Controls.Remove(trackers);
            }

            foreach (PictureBox badguy in allBadGuys)
            {
                this.Controls.Remove(badguy);
            }

            foreach (PictureBox sentinel in sentinelPBList)
            {
                this.Controls.Remove(sentinel);
            }

            bulletList.Clear();
            badBulletList.Clear();
            Tracker.Clear();
            allBadGuys.Clear();
            sentinelPBList.Clear();
        }

        private void TrackBullets()
        {
            for (int i = 0; i < bulletList.Count; i++)
            {
                PictureBox bullet = bulletList[i];
                bullet.Left += bulletSpeed;

                // Remove the bullet if it leaves the screen
                if (bullet.IsInsideOfForm() == false)
                {
                    // remove it from both the List and from the Window's
                    // Controls collection.
                    bulletList.Remove(bullet);
                    this.Controls.Remove(bullet);
                }

                // Kill a bad guy if the bullet hits him
                for (int j = 0; j < allBadGuys.Count; j++)
                {
                    PictureBox badguys = allBadGuys[j];

                    // Skip if invisible (already dead)
                    if (badguys.Visible == false)
                    {
                        continue;
                    }

                    if (badguys.IsTouching(bullet) == true)
                    {
                        // remove the bullet from both the List and from the 
                        // Window's Controls collection.
                        bulletList.Remove(bullet);
                        this.Controls.Remove(bullet);

                        // "Kill" the bad guy
                        badguys.Visible = false;

                        // Go on to the next bullet
                        break;
                    }






                }
            }
        }


        private void TrackBadBullets()
        {
            for (int i = 0; i < badBulletList.Count; i++)
            {
                PictureBox badbullet = badBulletList[i];
                badbullet.Left -= badBulletSpeed;


                // Remove the bullet if it leaves the screen
                if (badbullet.IsInsideOfForm() == false)
                {
                    // remove it from both the List and from the Window's
                    // Controls collection.
                    badBulletList.Remove(badbullet);
                    this.Controls.Remove(badbullet);
                }

                // Kill THE good guy if the bullet hits him
                if (pictureBoxPlayer.IsTouching(badbullet))
                {
                    Death();
                    Close();

                }






            }
        }


        private void CheckDoors()
        {
            bool allDead = false;
            for (int i = 0; i < allBadGuys.Count; i++)
            {
                PictureBox badguys = allBadGuys[i];


                if (pictureBoxPlayer.IsTouching(HeavenLights))
                {
                    MainForm.NextForm = "GameOver";
                    Close();
                }

                if (allDead == true && badguys.Visible == false)
                {

                    Win();
                    Close();
                }

                    if (badguys.IsTouching(pictureBoxPlayer) && badguys.Visible == true)
                    {

                        Death();
                        Close();
                        
                    }




                    if (pictureBoxPlayer.IsTouching(BacktoCaverns))
                    {
                        MainForm.NextForm = "SecondForm";
                        Close();
                    }
                }
            }


        private void Death()
        {
            DialogResult r = MessageBox.Show("Will you continue or quit?", "What will you do next?", MessageBoxButtons.YesNo);
            if (r == DialogResult.Yes)
            {

                game.Stop();

                ResetGame();

                


                MainForm.NextForm = "FirstForm";

                game.Start();


                Close();


               

               
            }

            else if (r == DialogResult.No)
            {
                game.Stop();
                ResetGame();
                MainForm.NextForm = "GameOver";
                Close();
            }




            }
        

        private void Win()
        {
            game.Stop();

            MessageBox.Show("YOU WIN! YOU HAVE DEFEATED THE DARK ARMY!");

            game.Start();

            Close();

            ResetGame();

            MainForm.NextForm = "GameOver";

        }


    }
}

