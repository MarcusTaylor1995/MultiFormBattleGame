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
    public partial class SecondForm : Form
    {
        private GameHelper game;

        Random rand = new Random();


        int goodGuySpeed = 5;
        bool hasgun = true;


        int enemySpeed = 5;
        int enemyDirection = 3; // 0 is up, 1 is right, 2 is down, 3 is left
        int enemyStepsRemaining = 0;

        int sentinelSpeed = 6;
        List<PictureBox> sentinelPBList = new List<PictureBox>();
        List<int> sentinelDirList = new List<int>(); // 0 is up and 1 is down
        List<Point> badguyStart = new List<Point>();

        int trackerSpeed = 11;

        int bulletSpeed = 9;
        int bulletDelay = 10;
        List<PictureBox> bulletList = new List<PictureBox>();

        List<PictureBox> allBadGuys = new List<PictureBox>();
        List<PictureBox> deadBadGuys = new List<PictureBox>();

        public SecondForm()
        {
            InitializeComponent();

            sentinelPBList.Add(pictureBoxEnemy1);
            sentinelPBList.Add(pictureBoxEnemy2);
            sentinelPBList.Add(pictureBoxEnemy3);

            foreach (PictureBox pb in allBadGuys)
            {
                badguyStart.Add(pb.Location);
            }


            sentinelDirList.Add(0);
            sentinelDirList.Add(0);
            sentinelDirList.Add(0);


            allBadGuys.AddRange(sentinelPBList);
            allBadGuys.Add(pictureBoxTracker);

            game = new GameHelper(this);
            game.Update += game_Update;
            game.Start();
        }

        private void game_Update(object sender, EventArgs e)
        {
            MovePlayer();
            CheckDoors();
            MoveSentinel();
            MoveZombie();
            MoveTracker();
            TrackBullets();
            TrackBullets();
            TrackGun();
            ShootGun();
            CheckGun();

            for (int i = 0; i < sentinelPBList.Count; i++)
            {

                PictureBox enemy = allBadGuys[i];

                pictureBoxTeleport.Visible = false;
                bool allDead = true;
                foreach (PictureBox pb in allBadGuys)
                {
                    if (pb.Visible == true)
                    {
                        allDead = false;
                        continue;
                    }
                }

                if (allDead == true)
                {

                    pictureBoxTeleport.Visible = true;
                    continue;


                }






            }
        }



        private void MovePlayer()
        {
            if (game.IsPressed(Keys.W))
            {
                pictureBoxGoodGuy.Top -= goodGuySpeed;
                if (pictureBoxGoodGuy.IsInsideOfForm() == false)
                {
                    pictureBoxGoodGuy.Top += goodGuySpeed;
                }
            }
            if (game.IsPressed(Keys.A))
            {
                pictureBoxGoodGuy.Left -= goodGuySpeed;
                if (pictureBoxGoodGuy.IsInsideOfForm() == false)
                {
                    pictureBoxGoodGuy.Left += goodGuySpeed;
                }
            }
            if (game.IsPressed(Keys.S))
            {
                pictureBoxGoodGuy.Top += goodGuySpeed;
                if (pictureBoxGoodGuy.IsInsideOfForm() == false)
                {
                    pictureBoxGoodGuy.Top -= goodGuySpeed;
                }
            }
            if (game.IsPressed(Keys.D))
            {
                pictureBoxGoodGuy.Left += goodGuySpeed;
                if (pictureBoxGoodGuy.IsInsideOfForm() == false)
                {
                    pictureBoxGoodGuy.Left -= goodGuySpeed;
                }
            }
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
                    PictureBox badGuy = allBadGuys[j];

                    // Skip if invisible (already dead)
                    if (badGuy.Visible == false)
                    {
                        continue;
                    }


                    if (badGuy.IsTouching(bullet))
                    {
                        // remove the bullet from both the List and from the 
                        // Window's Controls collection.
                        bulletList.Remove(bullet);
                        this.Controls.Remove(bullet);
                        this.Controls.Remove(badGuy);

                        // "Kill" the bad guy
                        badGuy.Visible = false;

                        // Add to list of dead bad guys and check for win
                        deadBadGuys.Add(badGuy);




                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Sentinels go up and down. We have lots of them, so we've stored
        /// them in a list to make it easy to track them. However, each goes
        /// in an independent direction (up or down), so we need to have a
        /// direction variable for each, as well. The easiest way to do this
        /// is to create a "correlated" list of directions such that each
        /// sentinel has an associated direction in the list with the same
        /// index. Flip it from 0 to 1 or vice versa when the picture box
        /// moves outside of the form.
        /// </summary>

        void MoveSentinel()
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








        /// <summary>
        /// Zombies move in a random direction. Since there's only one on
        /// the screen, we just track it using the PictureBox variable
        /// instead of a list. Obviously if you had multiple zombies, you'd
        /// probably want to use a list to make things MUCH easier.
        /// </summary>
        void MoveZombie()
        {
            for (int i = 0; i < sentinelPBList.Count; i++)
            {
                PictureBox enemy = sentinelPBList[i];
                // Skip if invisible (must be dead)
                if (enemy.Visible == false)
                {
                    return;
                }

                // Zombies change direction completely randomly and stay moving
                // in the new direction for 10 to 20 steps (frames).
                enemyStepsRemaining--;
                if (enemyStepsRemaining < 0)
                {
                    enemyDirection = rand.Next(0, 10);
                    enemyStepsRemaining = rand.Next(15, 39);
                }

                switch (enemyDirection)
                {
                    case 0: //up
                        enemy.Top -= enemySpeed;
                        if (enemy.IsInsideOfForm() == false)
                        {
                            enemy.Top += enemySpeed;
                        }
                        break;
                    case 1: // right
                        enemy.Left += enemySpeed;
                        if (enemy.IsInsideOfForm() == false)
                        {
                            enemy.Left -= enemySpeed;
                        }
                        break;
                    case 2: // down
                        enemy.Top += enemySpeed;
                        if (enemy.IsInsideOfForm() == false)
                        {
                            enemy.Top -= enemySpeed;
                        }
                        break;
                    case 3: // left
                        enemy.Left -= enemySpeed;
                        if (enemy.IsInsideOfForm() == false)
                        {
                            enemy.Left += enemySpeed;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Tracker's are semi-intelligent. They chase the good guy whereever
        /// he goes. Gotta use some math for this. Since there's only one on
        /// the screen, we just track it using the PictureBox variable
        /// instead of a list. Obviously if you had multiple trackers, you'd
        /// probably want to use a list to make things MUCH easier.
        /// </summary>


        void MoveTracker()
        {
            // Skip if invisible (must be dead)
            if (pictureBoxTracker.Visible == false)
            {
                return;
            }

            // Oh No!! Math!!!! (just Pythagoras and ratios)
            int dx = pictureBoxTracker.Left - pictureBoxGoodGuy.Left;
            int dy = pictureBoxTracker.Top - pictureBoxGoodGuy.Top;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance != 0)
            {
                double distanceRatio = trackerSpeed / distance;
                pictureBoxTracker.Left -= (int)(dx * distanceRatio);
                pictureBoxTracker.Top -= (int)(dy * distanceRatio);
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
                    bullet.Top = pictureBoxGoodGuy.Top + pictureBoxGoodGuy.Height / 2;
                    bullet.Left = pictureBoxGoodGuy.Left + pictureBoxGoodGuy.Width;
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




        private void CheckDoors()
        {
            bool deadguys = false;
            for (int i = 0; i < allBadGuys.Count; i++)
            {
                PictureBox badguys = allBadGuys[i];


                if (pictureBoxGoodGuy.IsTouching(pictureBoxTeleport))
                {
                    MainForm.NextForm = "ThirdForm";
                    Close();
                }

                if (deadguys == true)
                {
                    badguys.Visible = false;

                }


                if (badguys.IsTouching(pictureBoxGoodGuy) && badguys.Visible == true)
                {

                    Death();
                }

                  

                }
                if (pictureBoxGoodGuy.IsTouching(pictureBoxBack))
                {
                    MainForm.NextForm = "FirstForm";
                    Close();
                }
            }
        
        
    

        private void Death()
        {

            game.Stop();

            MessageBox.Show("You shall NOT PASS! Begone Scrub!");

            MainForm.NextForm = "GameOver";

            game.Start();


            Close();

            ResetGame();


            MainForm.NextForm = "FirstForm";

        }

        private void ResetGame()
        {
              
                foreach (PictureBox enemy in sentinelPBList)
                {
                    this.Controls.Remove(enemy);
                }

                foreach (PictureBox badguys in allBadGuys)
                {
                    this.Controls.Remove(badguys);
                }

                foreach (PictureBox allbadguy in deadBadGuys)
                {
                    this.Controls.Remove(allbadguy);
                }
                foreach (PictureBox bullet in bulletList)
                {
                    this.Controls.Remove(bullet);
                }

                bulletList.Clear();
                allBadGuys.Clear();
                deadBadGuys.Clear();
                sentinelPBList.Clear();
                




            }

        
    }


    }


