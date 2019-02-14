using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Numerics;

namespace Snake2
{
    // snake master branch

    public partial class MainWindow : Window
    {
        int networkIndex = 0;
        int speed = 40;
        double evolutionPoints = 0;
        double totalEvolutionPoints = 0;
        double points = 0;
        double hihgpoints = 0;
        double totalPoints = 0;
        double gamesCounter = 0;
        int maxTop = 200; //bottom end of map
        int maxLeft = 200; //right end of map
        DispatcherTimer timer = new DispatcherTimer();
        // keys to control snake, wsad or arrows.
        Key up;
        Key down;
        Key left;
        Key right;

        NeuralNetwork neuralNetwork = new NeuralNetwork(4, 128, 3);
        const int populationCount = 60;
        int generations = 1000;
        Evolution evo = new Evolution(populationCount);

        List<Matrix> inputsList = new List<Matrix>();
        List<Matrix> targetsList = new List<Matrix>();

        public MainWindow()
        {
            InitializeComponent();
        }
        snake sn = new snake();

        snake.segment.dir nextDir;// snake.segment.dir.down;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O)
            {
                throwFood();
            }
            if (e.Key == up && sn.segments[0].Direction != snake.segment.dir.down)// && top >= step)
            {
                // sn.segments[0].Direction
                nextDir = snake.segment.dir.up;
            }
            else if (e.Key == down && sn.segments[0].Direction != snake.segment.dir.up)// && top <= maxTop)
            {
                //sn.segments[0].Direction;
                nextDir = snake.segment.dir.down;
            }
            else if (e.Key == left && sn.segments[0].Direction != snake.segment.dir.right)// && top <= maxTop)
            {
                //sn.segments[0].Direction;
                nextDir = snake.segment.dir.left;
            }
            else if (e.Key == right && sn.segments[0].Direction != snake.segment.dir.left)// && top <= maxTop)
            {
                //sn.segments[0].Direction;
                nextDir = snake.segment.dir.right;
            }
        }
        bool selfColision()
        {
            var rect1x = Canvas.GetLeft(sn.segments[0].rec);
            var rect1y = Canvas.GetTop(sn.segments[0].rec);
            double rect2x;
            double rect2y;
            for (int i = 1; i < sn.segments.Count; i++)
            {
                rect2x = Canvas.GetLeft(sn.segments[i].rec);
                rect2y = Canvas.GetTop(sn.segments[i].rec);
                if (rect1x == rect2x && rect1y == rect2y)
                {
                    return true;
                }
            }
            return false;
        }
        bool foodCollision(Rectangle Rectangle1)
        {
            Rect rect1 = new Rect(Canvas.GetLeft(Rectangle1), Canvas.GetTop(Rectangle1), Rectangle1.Width, Rectangle1.Height);
            Rect rect2 = new Rect(Canvas.GetLeft(food), Canvas.GetTop(food), food.Width, food.Height);
            if (rect1.IntersectsWith(rect2))
            {
                return true;
            }
            else return false;
        }
        private void Loaded_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < evo.population.Count; i++)
            {
                var dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                    + @"\weights" + i + @"\";
                if (!evo.population[i].readWeights(dir)) break;
            }
            // neuralNetwork.readWeights();
            wsad.IsChecked = true;
            mediumspeed.IsChecked = true;
            timer.Tick += Timer_Tick;

            canv.Children.Add(sn.segments[0].rec);
            canv.Children.Add(sn.segments[1].rec);
        }
        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
        int c = 3;
        double decisionCount = 0;
        double prevFoodDist = 9999;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (c++ == 4)
            {
                lblScore.Content = "Score: " + evolutionPoints;

                if (selfColision())
                {
                    gameover(true);
                }
                var sntop = double.Parse(sn.segments[0].rec.GetValue(Canvas.TopProperty).ToString());
                var snleft = double.Parse(sn.segments[0].rec.GetValue(Canvas.LeftProperty).ToString());
                var foodtop = double.Parse(food.GetValue(Canvas.TopProperty).ToString());
                var foodleft = double.Parse(food.GetValue(Canvas.LeftProperty).ToString());
                var foodDist = GetDistance(snleft, sntop, foodleft, foodtop);
                if (foodDist < prevFoodDist)
                {
                    evolutionPoints++;
                }
                else
                {
                    evolutionPoints -= 3;
                    if (evolutionPoints < -30)
                    {
                       // evolutionPoints -= 50;
                        gameover(true);
                    }
                }
                prevFoodDist = foodDist;
                if (foodCollision(sn.segments[0].rec))
                {
                    points++;
                    evolutionPoints += 1000;
                    lblScore.Content = "Score: " + evolutionPoints;
                    if (points > hihgpoints)
                    {
                        hihgpoints = points;
                        lblTopScore.Content = "Top Score: " + hihgpoints;
                    }
                    throwFood();
                    var top = double.Parse(sn.segments[sn.segments.Count - 1].rec.GetValue(Canvas.TopProperty).ToString());
                    var left = double.Parse(sn.segments[sn.segments.Count - 1].rec.GetValue(Canvas.LeftProperty).ToString());

                    switch (sn.segments[sn.segments.Count - 1].Direction)
                    {
                        case snake.segment.dir.up: top += 10; break;
                        case snake.segment.dir.down: top -= 10; break;
                        case snake.segment.dir.left: left += 10; break;
                        case snake.segment.dir.right: left -= 10; break;
                    }
                    snake.segment sg = new snake.segment(top, left, sn.segments[sn.segments.Count - 1].Direction);
                    sn.segments.Add(sg);
                    canv.Children.Add(sn.segments[sn.segments.Count - 1].rec);
                }
                 // int way = BotDrives();
                 //assignDirection(way);

                var networkInput = makeInput();
               //  var target = makeTarget(way);

                decisionCount++;

                //  inputsList.Add(networkInput);
                //  targetsList.Add(target);
                 // neuralNetwork.train(networkInput, target);
                //  evo.population[networkIndex].train(networkInput, target);
                var networkAnswer = evo.population[networkIndex].get_answer(networkInput);
               // var networkAnswer = neuralNetwork.get_answer(networkInput);
                // targetsList.Add(networkAnswer);
                networkDrives(networkAnswer);
                c = 0;
                // nex dir for each segment
                for (int i = sn.segments.Count - 1; i > 0; i--)
                {
                    sn.segments[i].Direction = sn.segments[i - 1].Direction;
                }
                // assign new dir to head (keybord input)
                sn[0].Direction = nextDir;
                if (c4++ > 100)
                {
                    //food was not taken in 100 steps, infinite loop probably
                    throwFood();
                }
            }
            for (int i = 0; i < sn.segments.Count; i++)
            {
                var top = double.Parse(sn.segments[i].rec.GetValue(Canvas.TopProperty).ToString());
                var left = double.Parse(sn.segments[i].rec.GetValue(Canvas.LeftProperty).ToString());
                double step = 2;

                if (sn.segments[i].Direction == snake.segment.dir.up)
                {
                    if (i == 0 && top <= 0) gameover();
                    else sn.segments[i].rec.SetValue(Canvas.TopProperty, top - step);
                }
                else if (sn.segments[i].Direction == snake.segment.dir.down)
                {
                    if (i == 0 && (top > maxTop - sn.segments[0].size - step)) gameover();
                    else sn.segments[i].rec.SetValue(Canvas.TopProperty, top + step);
                }
                else if (sn.segments[i].Direction == snake.segment.dir.left)
                {
                    if (i == 0 && left < step) gameover();
                    else sn.segments[i].rec.SetValue(Canvas.LeftProperty, left - step);
                }
                else if (sn.segments[i].Direction == snake.segment.dir.right)
                {
                    if (i == 0 && (left > maxLeft - sn.segments[0].size - step)) gameover();
                    else sn.segments[i].rec.SetValue(Canvas.LeftProperty, left + step);
                }
            }

        }
        int c4 = 0;
        int counter = 0;
        int index = 0;
        int c2 = 0;
        int c3 = 0;
        //  int c5 = 0;
        int BotDrives()
        {
            // if angle is + food is on right, if - on left
            var angle = foodAngle();
            int way = 0;
            if (lookahead() == 1)
            {
                if (lookleft() == 1)
                {
                    way = 1; // for going right;
                }
                else way = -1; // for going left
            }
            else
            {
                if (angle < -0.18)
                {
                    if (lookleft() == 0)
                        way = -1;
                }
                else if (angle > 0.18)
                {
                    if (lookright() == 0)
                        way = 1;
                }
                else way = 0;
            }
            return way;
            #region commented
            /*
            if (sn[0].Direction == snake.segment.dir.down)
            {
                if (lookahead() == 1)
                {
                    if (lookleft() == 1)
                    {
                        nextDir = snake.segment.dir.left;
                    }
                    else
                    {
                        nextDir = snake.segment.dir.right;
                    }
                }
                else
                {
                    if (angle < -0.18)
                    {
                        if (lookleft() == 0)
                            nextDir = snake.segment.dir.right;
                    }
                    else if (angle > 0.18)
                    {
                        if (lookright() == 0)
                            nextDir = snake.segment.dir.left;
                    }
                    else nextDir = snake.segment.dir.down;
                }

            }
            else if (sn[0].Direction == snake.segment.dir.up)
            {

                if (lookahead() == 1)
                {
                    if (lookleft() == 1)
                    {
                        nextDir = snake.segment.dir.right;
                    }
                    else nextDir = snake.segment.dir.left;
                }
                else
                {
                    if (angle < -0.18)
                    {
                        if (lookleft() == 0)
                            nextDir = snake.segment.dir.left;
                    }
                    else if (angle > 0.18)
                    {
                        if (lookright() == 0)
                            nextDir = snake.segment.dir.right;
                    }
                    else nextDir = snake.segment.dir.up;
                }
            }
            else if (sn[0].Direction == snake.segment.dir.left)
            {
                if (lookahead() == 1)
                {
                    if (lookleft() == 1)
                    {
                        nextDir = snake.segment.dir.up;
                    }
                    else nextDir = snake.segment.dir.down;
                }
                else
                {
                    if (angle < -0.18)
                    {
                        if (lookleft() == 0)
                            nextDir = snake.segment.dir.down;
                    }
                    else if (angle > 0.18)
                    {
                        if (lookright() == 0)
                            nextDir = snake.segment.dir.up;
                    }
                    else nextDir = snake.segment.dir.left;
                }
            }
            else if (sn[0].Direction == snake.segment.dir.right)
            {
                if (lookahead() == 1)
                {
                    if (lookleft() == 1)
                    {
                        nextDir = snake.segment.dir.down;
                    }
                    else nextDir = snake.segment.dir.up;
                }
                else
                {
                    if (angle < -0.18)
                    {
                        if (lookleft() == 0)
                            nextDir = snake.segment.dir.up;
                    }
                    else if (angle > 0.18)
                    {
                        if (lookright() == 0)
                            nextDir = snake.segment.dir.down;
                    }
                    else nextDir = snake.segment.dir.right;
                }
            }
            */
            // c5++;
            #endregion
        }
        void assignDirection(int way)
        {
            if (way == -1) // go left
            {
                if (sn.segments[0].Direction == snake.segment.dir.up)
                {
                    nextDir = snake.segment.dir.left;
                }
                else if (sn.segments[0].Direction == snake.segment.dir.down)
                {
                    nextDir = snake.segment.dir.right;
                }
                else if (sn.segments[0].Direction == snake.segment.dir.left)
                {
                    nextDir = snake.segment.dir.down;
                }
                else if (sn.segments[0].Direction == snake.segment.dir.right)
                {
                    nextDir = snake.segment.dir.up;
                }
            }
            else if (way == 1) // go right
            {
                if (sn.segments[0].Direction == snake.segment.dir.up)
                {
                    nextDir = snake.segment.dir.right;
                }
                else if (sn.segments[0].Direction == snake.segment.dir.down)
                {
                    nextDir = snake.segment.dir.left;
                }
                else if (sn.segments[0].Direction == snake.segment.dir.left)
                {
                    nextDir = snake.segment.dir.up;
                }
                else if (sn.segments[0].Direction == snake.segment.dir.right)
                {
                    nextDir = snake.segment.dir.down;
                }
            }
            //else don't change direction.
        }
        void networkDrives(Matrix Networkanswer)
        {
            // find max 
            var max = Networkanswer[0, 0];
            int maxIndex = 0;
            for (int i = 0; i < Networkanswer.rows; i++)
            {
                if (Networkanswer[i, 0] > max)
                {
                    max = Networkanswer[i, 0];
                    maxIndex = i;
                }
            }
            switch (maxIndex)
            {
                case 0: break; //don't change direction 
                case 1: assignDirection(1); break;  // go right
                case 2: assignDirection(-1); break;
                    //case 3: nextDir = snake.segment.dir.right; break;
            }
        }
        Matrix makeTarget(int way)
        {
            Matrix mx = new Matrix(3, 1);
            switch (way)
            {
                case 0: mx[0, 0] = 1; break; // don't change direction
                case 1: mx[1, 0] = 1; break; // go right
                case -1: mx[2, 0] = 1; break; // go left              
            }
            return mx;
        }
        Matrix makeInput()
        {
            var sntop = double.Parse(sn.segments[0].rec.GetValue(Canvas.TopProperty).ToString());
            var snleft = double.Parse(sn.segments[0].rec.GetValue(Canvas.LeftProperty).ToString());
            var foodtop = double.Parse(food.GetValue(Canvas.TopProperty).ToString());
            var foodleft = double.Parse(food.GetValue(Canvas.LeftProperty).ToString());
            Matrix input = new Matrix(4, 1);
            input[0, 0] = lookahead();
            input[1, 0] = lookright();
            input[2, 0] = lookleft();
            //   input[3, 0] = snleft / 200;
            //  input[4, 0] = sntop / 200;
            //switch (sn[0].Direction)
            //{
            //    case snake.segment.dir.up: input[2, 0] = 1; break;
            //    case snake.segment.dir.down: input[3, 0] = 1; break;
            //    case snake.segment.dir.left: input[4, 0] = 1; break;
            //    case snake.segment.dir.right: input[5, 0] = 1; break;
            //}
            //   input[5, 0] = foodleft / 200;
            //  input[6, 0] = foodtop / 200;
            input[3, 0] = foodAngle();


            return input;
        }
        double foodAngle()
        {
            var sn0top = double.Parse(sn.segments[0].rec.GetValue(Canvas.TopProperty).ToString());
            var sn1top = double.Parse(sn.segments[1].rec.GetValue(Canvas.TopProperty).ToString());
            var sn0left = double.Parse(sn.segments[0].rec.GetValue(Canvas.LeftProperty).ToString());
            var sn1left = double.Parse(sn.segments[1].rec.GetValue(Canvas.LeftProperty).ToString());
            var foodtop = double.Parse(food.GetValue(Canvas.TopProperty).ToString());
            var foodleft = double.Parse(food.GetValue(Canvas.LeftProperty).ToString());
            double VsnakeX = sn0left - sn1left;
            double VsnakeY = sn0top - sn1top;
            Vector Vsnake = new Vector(VsnakeX, VsnakeY);
            double VfoodX = foodleft - sn0left;
            double VfoodY = foodtop - sn0top;
            Vector Vfood = new Vector(VfoodX, VfoodY);
            var angle = Vector.AngleBetween(Vsnake, Vfood);
            return angle / 180;
        }

        int lookahead()
        {
            var sntop = double.Parse(sn.segments[0].rec.GetValue(Canvas.TopProperty).ToString());
            var snleft = double.Parse(sn.segments[0].rec.GetValue(Canvas.LeftProperty).ToString());

            if (sn[0].Direction == snake.segment.dir.up)
            {
                if (sntop - 10 <= 0) return 1;
                if (selfColision(snleft, sntop - 10)) return 1;
            }
            else if (sn[0].Direction == snake.segment.dir.down)
            {
                if (sntop + 20 >= 200) return 1;
                if (selfColision(snleft, sntop + 10)) return 1;
            }
            else if (sn[0].Direction == snake.segment.dir.left)
            {
                if (snleft - 10 <= 0) return 1;
                if (selfColision(snleft - 10, sntop)) return 1;
            }
            else if (sn[0].Direction == snake.segment.dir.right)
            {
                if (snleft + 20 >= 200) return 1;
                if (selfColision(snleft + 10, sntop)) return 1;
            }
            return 0;
        }
        int lookleft()
        {
            var sntop = double.Parse(sn.segments[0].rec.GetValue(Canvas.TopProperty).ToString());
            var snleft = double.Parse(sn.segments[0].rec.GetValue(Canvas.LeftProperty).ToString());

            if (sn[0].Direction == snake.segment.dir.up)
            {
                if (snleft - 10 <= 0) return 1;
                if (selfColision(snleft - 10, sntop)) return 1;
            }
            else if (sn[0].Direction == snake.segment.dir.down)
            {
                if (snleft + 20 >= 200) return 1;
                if (selfColision(snleft + 10, sntop)) return 1;
            }
            else if (sn[0].Direction == snake.segment.dir.left)
            {
                if (sntop + 20 >= 200) return 1;
                if (selfColision(snleft, sntop + 10)) return 1;
            }
            else if (sn[0].Direction == snake.segment.dir.right)
            {
                if (sntop - 10 <= 0) return 1;
                if (selfColision(snleft, sntop - 10)) return 1;
            }
            return 0;
        }
        int lookright()
        {
            var sntop = double.Parse(sn.segments[0].rec.GetValue(Canvas.TopProperty).ToString());
            var snleft = double.Parse(sn.segments[0].rec.GetValue(Canvas.LeftProperty).ToString());

            if (sn[0].Direction == snake.segment.dir.up)
            {
                if (snleft + 20 >= 200) return 1;
                if (selfColision(snleft + 10, sntop)) return 1;
            }
            else if (sn[0].Direction == snake.segment.dir.down)
            {
                if (snleft - 10 <= 0) return 1;
                if (selfColision(snleft - 10, sntop)) return 1;
            }
            else if (sn[0].Direction == snake.segment.dir.left)
            {
                if (sntop - 10 <= 0) return 1;
                if (selfColision(snleft, sntop - 10)) return 1;
            }
            else if (sn[0].Direction == snake.segment.dir.right)
            {
                if (sntop + 20 >= 200) return 1;
                if (selfColision(snleft, sntop + 10)) return 1;
            }
            return 0;
        }

        bool selfColision(double x, double y)
        {
            double rect2x;
            double rect2y;
            for (int i = 1; i < sn.segments.Count; i++)
            {
                rect2x = Canvas.GetLeft(sn.segments[i].rec);
                rect2y = Canvas.GetTop(sn.segments[i].rec);
                if (x == rect2x && y == rect2y)
                {
                    return true;
                }
            }
            return false;
        }

        int findDecision()
        {
            int i;
            for (i = 0; i < inputsList.Count; i++)
            {
                int count = 0;
                for (int k = 0; k < 3; k++)
                {
                    if (inputsList[i][k, 0] == 0) // look around snake results in network input.
                    {
                        count++;
                        if (count == 2) // there was other option
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
        void gameover(bool flag = false)
        {
          //  evolutionPoints *= 2;
           // totalEvolutionPoints += evolutionPoints;
            evo.population[networkIndex].adjustment = evolutionPoints;
            if (evo.population[networkIndex].adjustment > evo.maxAdj) evo.maxAdj = evo.population[networkIndex].adjustment;
            if (evo.population[networkIndex].adjustment < evo.minAdj) evo.minAdj = evo.population[networkIndex].adjustment;
            networkIndex++;
            lblgames.Content = networkIndex;
            evolutionPoints = 0;
            if (networkIndex == populationCount) // end of generation.
            {
                //adjustment normalize
                //for (int i = 0; i < evo.population.Count; i++)
                //{
                //    evo.population[i].adjustment /= totalEvolutionPoints;
                //    evo.population[i].adjustment *= 100;
                //    if (evo.population[i].adjustment > evo.maxAdj) evo.maxAdj = evo.population[i].adjustment;
                //    if (evo.population[i].adjustment < evo.minAdj) evo.minAdj = evo.population[i].adjustment;
                //}
                //totalEvolutionPoints = 0;
                if (generations-- == 0)
                {
                    generations = 100;
                    mediumspeed.IsChecked = true;
                }
                lblctrl.Content = "gen. left: " + generations;

                networkIndex = 0;

                if (!evo.reprSelector())
                {
                    MessageBox.Show("repr loop");
                }
                evo.reproduce();
                if (!evo.Death())
                {
                    MessageBox.Show("death loop");
                }
                evo.toReproduction.Clear();
            }

            // train with other direction than one that got us killed.

            //var index = findDecision();

            //Matrix newTarget = new Matrix(3, 1);
            //for (int i = 0; i < 3; i++)
            //{
            //    newTarget[i, 0] = inputsList[index][i, 0];
            //}
            //newTarget += targetsList[index];
            //for (int k = 0; k < 3; k++)
            //{
            //    if (newTarget[k, 0] == 0) newTarget[k, 0] = 1;
            //    else newTarget[k, 0] = 0;
            //}

            //for (int i = 0; i < 3; i++)
            //    neuralNetwork.train(inputsList[index], newTarget);
            //inputsList.Clear();
            // targetsList.Clear();

            timer.IsEnabled = false;
            if (true)//MessageBox.Show("Wanna try again?", "Game Over!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var s in sn.segments)
                {
                    canv.Children.Remove(s.rec);
                }
                sn.segments.Clear();
                snake.segment head = new snake.segment(50, 50);
                int y = 38;
                if (flag) y = 40;
                snake.segment head2 = new snake.segment(50, y, false);
                sn.segments.Add(head);
                sn.segments.Add(head2);
                canv.Children.Add(sn.segments[0].rec);
                canv.Children.Add(sn.segments[1].rec);
                nextDir = snake.segment.dir.down;
                Button_Click(null, null);
            }
            else
            {
                Environment.Exit(0);
            }
        }
        void throwFood()
        {
            c4 = 0;// infinite loop breaker;
            Random rand = new Random();

            canv.Children.Remove(food);
            food.SetValue(Canvas.TopProperty, (double)rand.Next(10, maxTop - 40));
            food.SetValue(Canvas.LeftProperty, (double)rand.Next(10, maxLeft - 40));
            canv.Children.Add(food);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //lblctrl.Content = decisionCount / gamesCounter;
            totalPoints += points;
            points = 0;
            // lblgames.Content = "Games: " + gamesCounter;
            lblaverage.Content = "Average: " + Math.Round(totalPoints / gamesCounter, 1);
            timer.Interval = new TimeSpan(0, 0, 0, 0, speed);
            timer.IsEnabled = true;
            gamesCounter++;
        }

        private void CheckBoxWSAD_Checked(object sender, RoutedEventArgs e)
        {
            arrows.IsChecked = false;
            up = Key.W;
            down = Key.S;
            left = Key.A;
            right = Key.D;
        }

        private void Arrows_Checked(object sender, RoutedEventArgs e)
        {
            wsad.IsChecked = false;
            up = Key.Up;
            down = Key.Down;
            left = Key.Left;
            right = Key.Right;
        }

        private void Slow_Checked(object sender, RoutedEventArgs e)
        {
            speed = 30;
            mediumspeed.IsChecked = false;
            fast.IsChecked = false;
            timer.Interval = new TimeSpan(0, 0, 0, 0, speed);

        }

        private void Mediumspeed_Checked(object sender, RoutedEventArgs e)
        {
            speed = 1;
            slow.IsChecked = false;
            fast.IsChecked = false;
            timer.Interval = new TimeSpan(0, 0, 0, 0, speed);

        }

        private void Fast_Checked(object sender, RoutedEventArgs e)
        {
            speed = 0;
            slow.IsChecked = false;
            mediumspeed.IsChecked = false;
            timer.Interval = new TimeSpan(0, 0, 0, 0, speed);
        }
        private double calcAngle(double x1, double y1, double x2, double y2)
        {
            double xDiff = x2 - x1;
            double yDiff = y2 - y1;
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        private void Loaded_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           // neuralNetwork.saveWeights();
            for (int i = 0; i < evo.population.Count; i++)
            {
                var dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                    + @"\weights" + i + @"\";
                evo.population[i].saveWeights(dir);
            }
        }
    }
}
