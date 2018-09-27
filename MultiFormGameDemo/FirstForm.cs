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
    public partial class FirstForm : Form
    {
        private GameHelper game;

        List<PictureBox> trees = new List<PictureBox>();

        Random rand = new Random();

        // GAME STATE
        int goodguySpeed = 8;
        bool hasgun = false;

        // GHOST STUFF
        int ghostSpeed = 5;
        int ghostDirection = 0;
        int ghostStepsSinceLastDirectionChange = 0;

        // BULLET STUFF
        int bulletSpeed = 10;
        int bulletDelay = 0;
        List<PictureBox> bulletList = new List<PictureBox>();
        public FirstForm()
        {
            InitializeComponent();

            trees.Add(pictureBoxTree1);
            trees.Add(pictureBoxTree2);
            trees.Add(pictureBoxTree3);
            trees.Add(pictureBoxTree4);
            trees.Add(pictureBoxTree5);
            trees.Add(pictureBoxTree6);
            trees.Add(pictureBoxTree7);
            trees.Add(pictureBoxTree8);
            trees.Add(pictureBoxTree9);
            trees.Add(pictureBoxTree10);
            trees.Add(pictureBoxTree11);
            trees.Add(pictureBoxTree12);
            trees.Add(pictureBoxTree13);


            game = new GameHelper(this);
            game.Update += game_Update;
            game.Start();
        }

        private void game_Update(object sender, EventArgs e)
        {
            MovePlayer();
            MoveGhost();
            CheckDoors();
            CheckGun();
            ShootGun();
            TrackGun();



        }

        private void MovePlayer()
        {
            if (game.IsPressed(Keys.W))
            {
                Goodguy.Top -= goodguySpeed;
                //if (Goodguy.IsInsideOfForm() == false || Goodguy.IsTouching(pictureBoxTree1) == true) 
                if (CanMove() == false)
                {
                    Goodguy.Top += goodguySpeed;
                }
            }
            if (game.IsPressed(Keys.A))
            {
                Goodguy.Left -= goodguySpeed;
                if (CanMove() == false)
                {
                    Goodguy.Left += goodguySpeed;
                }
            }
            if (game.IsPressed(Keys.S))
            {
                Goodguy.Top += goodguySpeed;
                if (CanMove() == false)
                {
                    Goodguy.Top -= goodguySpeed;
                }
            }
            if (game.IsPressed(Keys.D))
            {
                Goodguy.Left += goodguySpeed;
                if (CanMove() == false)
                {
                    Goodguy.Left -= goodguySpeed;
                }
            }
        }

        private bool CanMove()
        {
            if (Goodguy.IsInsideOfForm() == false)
            {
                return false;
            }

            foreach (var tree in trees)
            {
                if (Goodguy.IsTouching(tree) == true)
                {
                    return false;
                }

            }

            return true;
        }


        public void MoveGhost()
        {
           

                    if (ghostStepsSinceLastDirectionChange == 4)
                    {
                        ghostDirection = rand.Next(3); // number between 0 and 3
                        ghostStepsSinceLastDirectionChange = 0;
                    }
                    ghostStepsSinceLastDirectionChange++;

                    // if direction is 0 == up, 1 == right, 2 == down, 3 == left
                    if (ghostDirection == 0) // UP
                    {
                        pictureBoxGhost.Top -= ghostSpeed;
                        //if (pictureBoxPlayer.IsInsideOfForm() == false || pictureBoxPlayer.IsTouching(pictureBoxBush1) == true)
                        if (CanMove(pictureBoxGhost) == false)
                        {
                            pictureBoxGhost.Top += ghostSpeed;
                        }
                    }
                    if (ghostDirection == 3) // LEFT
                    {
                        pictureBoxGhost.Left -= ghostSpeed;
                        if (CanMove(pictureBoxGhost) == false)
                        {
                            pictureBoxGhost.Left += ghostSpeed;
                        }
                    }
                    if (ghostDirection == 2) // DOWN
                    {
                        pictureBoxGhost.Top += ghostSpeed;
                        if (CanMove(pictureBoxGhost) == false)
                        {
                            pictureBoxGhost.Top -= ghostSpeed;
                        }
                    }
                    if (ghostDirection == 1) // RIGHT
                    {
                        pictureBoxGhost.Left += ghostSpeed;
                        if (CanMove(pictureBoxGhost) == false)
                        {
                            pictureBoxGhost.Left -= ghostSpeed;
                        }
                    }
                }

            
        

        private bool CanMove(PictureBox pb)
        {
            if (pb.IsInsideOfForm() == false)
            {
                return false;
            }
            foreach (var trees in trees)
            {
                if (pb.IsTouching(trees) == true)
                {
                    return false;
                }
            }
            return true;
        }

        private void CheckDoors()
        {
            //if (pictureBoxPlayer.IsTouching(labelDeath))
            //{
            //    MainForm.NextForm = "GameOver";
            //    Close();
            //}
            if (Goodguy.IsTouching(pictureBoxCave))
            {
                MainForm.NextForm = "SecondForm";
                Close();
            }
        }

        public void CheckGun()
        {
            if (hasgun == false)
            {
                if (Goodguy.IsTouching(pictureBoxGun))
                {
                    hasgun = true;
                    pictureBoxGun.Visible = false;
                }
            }
        }

        public void ShootGun()
        {
            pictureBoxBullet.Visible = false;
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
                    bullet.Top = Goodguy.Top + Goodguy.Height / 2;
                    bullet.Left = Goodguy.Left + Goodguy.Width;
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

                // private void CheckDoors()
                // {
                //if (pictureBoxPlayer.IsTouching(labelDeath))
                // {
                //    MainForm.NextForm = "GameOver";
                //   Close();
                // }
                // if (pictureBoxPlayer.IsTouching(labelDoorNext))
                // {
                //   MainForm.NextForm = "SecondForm";
                //  Close();
                //}
                //  }


            }
        }

        private void FirstForm_Load(object sender, EventArgs e)
        {

        }
    }
}

      