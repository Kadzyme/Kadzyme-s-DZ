using SFML.Window;
using SFML.Graphics;
using SFML.System;

namespace Game
{
    class Program
    {
        private RenderWindow window = new RenderWindow(new VideoMode(1600, 900), "Game");
        private CircleShape ball = new CircleShape(25);

        private RectangleShape rightPlayer = new RectangleShape(new Vector2f(25, 120));
        private RectangleShape leftPlayer = new RectangleShape(new Vector2f(25, 120));

        private const float normalBallSpeed = 4f;
        private float ballSpeedX;
        private float ballSpeedY;
        private float ballSpeed;

        static void Main(string[] args)
        {
            Program start = new Program();
            start.Game();
        }

        private void Init()
        {
            //window
            window.SetFramerateLimit(60);
            window.Closed += WindowClosed;
            //left player
            leftPlayer.FillColor = Color.Blue;
            leftPlayer.Origin = new Vector2f(leftPlayer.Size.X / 2, leftPlayer.Size.Y / 2);
            leftPlayer.Position = new Vector2f(leftPlayer.Size.X / 2, leftPlayer.Size.Y / 2);
            //right player
            rightPlayer.FillColor = Color.Red;
            rightPlayer.Origin = new Vector2f(rightPlayer.Size.X / 2, rightPlayer.Size.Y / 2);
            rightPlayer.Position = new Vector2f(window.Size.X - rightPlayer.Size.X / 2, rightPlayer.Size.Y / 2);
            //ball
            ball.FillColor = Color.Green;
            ball.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2);
            ball.Origin = new Vector2f(ball.Radius, ball.Radius);
            ballSpeedX = normalBallSpeed;
            ballSpeedY = normalBallSpeed;
            ballSpeed = normalBallSpeed;
        }

        private void Game()
        {
            Init();
            while (window.IsOpen)
            {
                PlayerMove(Keyboard.Key.W, Keyboard.Key.S, leftPlayer);
                PlayerMove(Keyboard.Key.Up, Keyboard.Key.Down, rightPlayer);
                BallMove();
                Draw();
            }
        }

        private void PlayerMove(Keyboard.Key UpKey, Keyboard.Key DownKey, RectangleShape player)
        {
            float playerSpeed = 7;
            float movePlayer = 0;
            if (Keyboard.IsKeyPressed(DownKey) && player.Position.Y + player.Size.Y / 2 < window.Size.Y)
            {
                movePlayer += playerSpeed;
            }
            else if (Keyboard.IsKeyPressed(UpKey) && player.Position.Y - player.Size.Y / 2 > 0)
            {
                movePlayer -= playerSpeed;
            }
            player.Position += new Vector2f(0, movePlayer);
        }

        private void BallMove()
        {
            if (ball.Position.X + ball.Radius >= window.Size.X || ball.Position.X - ball.Radius <= 0)
            {
                if (ballSpeedX > 0)
                    ballSpeedX = normalBallSpeed;
                else
                    ballSpeedX = -normalBallSpeed;
                if (ballSpeedY > 0)
                    ballSpeedY = normalBallSpeed;
                else
                    ballSpeedY = -normalBallSpeed;
                ballSpeed = normalBallSpeed;
                ball.Position = new Vector2f(window.Size.X / 2, window.Size.Y / 2);
            }
            if (ball.Position.X + ball.Radius >= rightPlayer.Position.X - rightPlayer.Size.X / 2 && BallTouchPlayer(rightPlayer))
            {
                ballSpeedX = -ballSpeed;
            }
            else if (ball.Position.X - ball.Radius <= leftPlayer.Position.X + leftPlayer.Size.X / 2 && BallTouchPlayer(leftPlayer))
            {
                ballSpeedX = ballSpeed;
            }
            if (ball.Position.Y - ball.Radius <= 0)
            {
                ballSpeedY = ballSpeed;
            }
            else if (ball.Position.Y + ball.Radius >= window.Size.Y)
            {
                ballSpeedY = -ballSpeed;
            }
            ball.Position += new Vector2f(ballSpeedX, ballSpeedY);
            ballSpeed = BallSpeedIncrease(ballSpeed);
            ballSpeedX = BallSpeedIncrease(ballSpeedX);
            ballSpeedY = BallSpeedIncrease(ballSpeedY);
        }

        private float BallSpeedIncrease(float ballSpeed)
        {
            float speedIncrease = 0.005f;
            Random rand = new Random();
            if (ballSpeed == 0)
                ballSpeed += speedIncrease * rand.Next(-1, 2);
            else if (ballSpeed > 0)
                ballSpeed += speedIncrease;
            else
                ballSpeed -= speedIncrease;
            return ballSpeed;
        }

        private void Draw()
        {
            window.DispatchEvents();
            window.Clear();
            window.Draw(ball);
            window.Draw(leftPlayer);
            window.Draw(rightPlayer);
            window.Display();
        }

        private bool BallTouchPlayer(RectangleShape player)
            => ball.Position.Y <= player.Position.Y + player.Size.Y / 2 
            && ball.Position.Y >= player.Position.Y - player.Size.Y / 2;

        private void WindowClosed(object sender, EventArgs e)
        {
            RenderWindow w = (RenderWindow)sender;
            w.Close();
        }
    }
}
