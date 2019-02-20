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
        List<double> allPoints = new List<double>();
        int maxTop = 200; //bottom end of map
        int maxLeft = 200; //right end of map
        DispatcherTimer timer = new DispatcherTimer();
        // keys to control snake, wsad or arrows.
        Key up;
        Key down;
        Key left;
        Key right;
        snake snake = new snake();
        // snake.segment.dir nextDir;
        NeuralNetwork neuralNetwork = new NeuralNetwork(5, 1024, 3);
        //Evolution evo = new Evolution(populationCount, 7, 128, 3);

        List<Matrix> inputsList = new List<Matrix>();
        List<Matrix> targetsList = new List<Matrix>();
        double prevFoodDist = 9999;
        int foodExpire = 0;
        int generationCounter = 0;
        double maxOfGeneration = 0;
        Random rand = new Random();

        // bot evo
        botEvolution botEvo = new botEvolution(populationCount);
        int botIndex = 0;
        const int populationCount = 40;

        int pointsTreshold1 = 50;
        int pointsTreshold2 = 60;
        int fedGoodDelta1 = 20;
        int fedGoodDelta2 = 5;
        int fedGoodDelta3 = 30;//snake.segments.Count;
        int fedGoodDelta4 = 20;
        int foodExpiration = 120;
        int howDeep1 = 60;
        int howDeep2 = 40;
        int howDeep3 = 30;
        int howDeep4 = 15;
        int howDeep5 = 5;
        int howDeep6 = 3;
        double fedGood = 0;
        double angleNegative = -0.18;
        double anglePositive = 0.18;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O)
            {
                throwFood();
            }
            if (e.Key == up && snake.segments[0].Direction != snake.segment.dir.down)// && top >= step)
            {
                snake.nextDir = snake.segment.dir.up;
            }
            else if (e.Key == down && snake.segments[0].Direction != snake.segment.dir.up)// && top <= maxTop)
            {
                snake.nextDir = snake.segment.dir.down;
            }
            else if (e.Key == left && snake.segments[0].Direction != snake.segment.dir.right)// && top <= maxTop)
            {
                snake.nextDir = snake.segment.dir.left;
            }
            else if (e.Key == right && snake.segments[0].Direction != snake.segment.dir.left)// && top <= maxTop)
            {
                snake.nextDir = snake.segment.dir.right;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < evo.population.Count; i++)
            //{
            //    var dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            //        + @"\weights" + i + @"\";
            //    if (!evo.population[i].readWeights(dir)) break;
            //}
            //neuralNetwork.readWeights();
            botEvo.readPopulation();
            wsad.IsChecked = true;
            mediumspeed.IsChecked = true;
            timer.Tick += Timer_Tick;
            canv.Children.Add(snake[0].rec);
            canv.Children.Add(snake[1].rec);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //  if (c++ == 4) //when snakes takes 2pixel steps instead of 10 (better visual ef.)
            //   {
            // lblScore.Content = "Score: " + evolutionPoints;
            if (selfCollision())
            {
                gameover();
            }
            var sntop = double.Parse(snake.segments[0].rec.GetValue(Canvas.TopProperty).ToString());
            var snleft = double.Parse(snake.segments[0].rec.GetValue(Canvas.LeftProperty).ToString());
            var foodtop = double.Parse(food.GetValue(Canvas.TopProperty).ToString());
            var foodleft = double.Parse(food.GetValue(Canvas.LeftProperty).ToString());
            var foodDist = GetDistance(snleft, sntop, foodleft, foodtop);
            if (foodDist < prevFoodDist)
            {
                evolutionPoints++;
            }
            else
            {
                evolutionPoints -= 2;
                if (evolutionPoints < -10)
                {
                    // evolutionPoints -= 100;
                    gameover();
                }
            }
            prevFoodDist = foodDist;


            if (foodCollision(snake[0].rec))
            {
                points++;

                // lblScore.Content = "Score: " + points;
                if (points < pointsTreshold1)
                    fedGood += fedGoodDelta1;
                else fedGood += fedGoodDelta2;
                // else fedGood += 5;
                evolutionPoints += 100;
                throwFood();
                var top = double.Parse(snake[snake.segments.Count - 1].rec.GetValue(Canvas.TopProperty).ToString());
                var left = double.Parse(snake[snake.segments.Count - 1].rec.GetValue(Canvas.LeftProperty).ToString());

                switch (snake[snake.segments.Count - 1].Direction)
                {
                    case snake.segment.dir.up: top += 10; break;
                    case snake.segment.dir.down: top -= 10; break;
                    case snake.segment.dir.left: left += 10; break;
                    case snake.segment.dir.right: left -= 10; break;
                }
                snake.segment sg = new snake.segment(top, left, snake[snake.segments.Count - 1].Direction);
                snake.segments.Add(sg);
                canv.Children.Add(snake[snake.segments.Count - 1].rec);
            }
            int way = botAnswer(snake, food);
            int next = deepExplore(way);
            if (next == 0)
            {
                if (lookahead(snake) == 1)
                {
                    if (lookleft(snake) == 0)
                    {
                        next = -1;
                    }
                    else next = 1;
                }
            }
            else if (next == -1)
            {
                if (lookleft(snake) == 1)
                {
                    if (lookahead(snake) == 0)
                    {
                        next = 0;
                    }
                    else next = 1;
                }
            }
            else if (next == 1)
            {
                if (lookright(snake) == 1)
                {
                    if (lookahead(snake) == 0)
                    {
                        next = 0;
                    }
                    else next = -1;
                }
            }
            botDrives(snake, next);
            // lblspeed.Content = lookahead(1) + " " + lookleft(1) + " " + lookright(1);
            //  var networkInput = makeInput();
            //  var target = makeTarget(way);

            //   neuralNetwork.train(networkInput, target);
            //  evo.population[networkIndex].train(networkInput, target);
            //  var networkAnswer = evo.population[networkIndex].get_answer(networkInput);

            //  var networkAnswer = neuralNetwork.get_answer(networkInput);
            //networkDrives(networkAnswer);

            if (foodExpire++ > foodExpiration)
            {
                //food was not taken in 80 steps, relocate
                throwFood();
            }
            //  }
            // make step for every segment of snake
            makeMove(snake);
            fedGood--;
        }
        int deepExplore(int way)
        {
            int next = explore(way, howDeep1);
            if (next == 5)
            {
                next = explore(way, howDeep2);
                if (next == 5)
                {
                    next = explore(way, howDeep3);
                    if (next == 5)
                    {
                        next = explore(way, howDeep4);
                        if (next == 5)
                        {
                            next = explore(way, howDeep5);
                            if (next == 5)
                            {
                                next = explore(way, howDeep6);
                                if (next == 5)
                                {
                                    next = way;
                                }

                            }
                        }
                    }
                }
            }
            return next;
        }
        int explore(int way, int howDeep)
        {
            if (!botImagination(way, howDeep))
            {
                if (way == 0)
                {
                    way = 1;
                    if (!botImagination(way, howDeep))
                    {
                        way = -1;
                        if (!botImagination(way, howDeep))
                        {
                            return 5;
                        }
                    }
                }
                else if (way == 1)
                {
                    way = -1;
                    if (!botImagination(way, howDeep))
                    {
                        way = 0;
                        if (!botImagination(way, howDeep))
                        {
                            return 5;
                        }
                    }
                }
                else if (way == -1)
                {
                    way = 1;
                    if (!botImagination(way, howDeep))
                    {
                        way = 0;
                        if (!botImagination(way, howDeep))
                        {
                            return 5;
                        }
                    }
                }
            }
            return way;
        }
        void makeMove(snake snake)
        {
            // nex dir for each segment
            for (int i = snake.segments.Count - 1; i > 0; i--)
            {
                snake.segments[i].Direction = snake.segments[i - 1].Direction;
            }
            // assign new dir for head 
            snake[0].Direction = snake.nextDir;
            //make move
            for (int i = 0; i < snake.segments.Count; i++)
            {
                var top = double.Parse(snake[i].rec.GetValue(Canvas.TopProperty).ToString());
                var left = double.Parse(snake[i].rec.GetValue(Canvas.LeftProperty).ToString());
                double step = 10;

                if (snake[i].Direction == snake.segment.dir.up)
                {
                    if (i == 0 && top <= 0) gameover();
                    else snake[i].rec.SetValue(Canvas.TopProperty, top - step);
                }
                else if (snake.segments[i].Direction == snake.segment.dir.down)
                {
                    if (i == 0 && (top > maxTop - snake[0].size - step)) gameover();
                    else snake[i].rec.SetValue(Canvas.TopProperty, top + step);
                }
                else if (snake[i].Direction == snake.segment.dir.left)
                {
                    if (i == 0 && left < step) gameover();
                    else snake[i].rec.SetValue(Canvas.LeftProperty, left - step);
                }
                else if (snake[i].Direction == snake.segment.dir.right)
                {
                    if (i == 0 && (left > maxLeft - snake[0].size - step)) gameover();
                    else snake[i].rec.SetValue(Canvas.LeftProperty, left + step);
                }
            }
        }

        int botAnswer(snake snake, Rectangle food)
        {
            // if angle is + food is on right, if - on left
            var angle = foodAngle(snake, food);
            int way = 0;
            if (lookahead(snake) == 1) // there is obstacle
            {
                if (lookleft(snake) == 1)
                {
                    way = 1; // for going right;
                }
                else way = -1; // for going left
            }
            else if (fedGood <= 0)
            {
                if (angle < angleNegative)
                {
                    if (lookleft(snake) == 0)
                        way = -1;
                }
                else if (angle > anglePositive)
                {
                    if (lookright(snake) == 0)
                        way = 1;
                }
                else way = 0;
            }
            return way;
        }
        //  List<int> snakePath = new List<int>();
        bool botImagination(int firstWay, int howDeep)
        {
            // copy snake & food
            snake imaginarySnake = new snake();// = snake;    
            imaginarySnake.nextDir = snake.nextDir;
            imaginarySnake.segments.Clear();
            foreach (var segment in snake.segments)
            {
                var top = Canvas.GetTop(segment.rec);
                var left = Canvas.GetLeft(segment.rec);
                imaginarySnake.segments.Add(new snake.segment(top, left, segment.Direction));
            }
            Rectangle imaginaryFood = new Rectangle();//food;
            Canvas.SetTop(imaginaryFood, Canvas.GetTop(food));
            Canvas.SetLeft(imaginaryFood, Canvas.GetLeft(food));
            //make map
            Canvas canvo = new Canvas();
            canvo.Height = 200;
            canvo.Width = 200;

            canvo.Children.Add(imaginaryFood);
            foreach (var segment in imaginarySnake.segments)
            {
                canvo.Children.Add(segment.rec);
            }
            // first step        
            botDrives(imaginarySnake, firstWay);
            makeMove(imaginarySnake);
            for (int i = 0; i < howDeep; i++)
            {
                int way = botAnswer(imaginarySnake, imaginaryFood);
                botDrives(imaginarySnake, way);
                makeMove(imaginarySnake);
                if (selfCollision(imaginarySnake, Canvas.GetLeft(imaginarySnake[0].rec), Canvas.GetTop(imaginarySnake[0].rec)))
                {
                    if (points < pointsTreshold2) fedGood = fedGoodDelta3; //* 1.5;//60;
                    else fedGood = fedGoodDelta4; //* 1.5;//60;
                    return false;
                }
            }
            return true;
        }
        void botDrives(snake snake, int way)
        {
            if (way == -1) // go left
            {
                if (snake.segments[0].Direction == snake.segment.dir.up)
                {
                    snake.nextDir = snake.segment.dir.left;
                }
                else if (snake.segments[0].Direction == snake.segment.dir.down)
                {
                    snake.nextDir = snake.segment.dir.right;
                }
                else if (snake.segments[0].Direction == snake.segment.dir.left)
                {
                    snake.nextDir = snake.segment.dir.down;
                }
                else if (snake.segments[0].Direction == snake.segment.dir.right)
                {
                    snake.nextDir = snake.segment.dir.up;
                }
            }
            else if (way == 1) // go right
            {
                if (snake.segments[0].Direction == snake.segment.dir.up)
                {
                    snake.nextDir = snake.segment.dir.right;
                }
                else if (snake.segments[0].Direction == snake.segment.dir.down)
                {
                    snake.nextDir = snake.segment.dir.left;
                }
                else if (snake.segments[0].Direction == snake.segment.dir.left)
                {
                    snake.nextDir = snake.segment.dir.up;
                }
                else if (snake.segments[0].Direction == snake.segment.dir.right)
                {
                    snake.nextDir = snake.segment.dir.down;
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
                case 1: botDrives(snake, 1); break;  // go right
                case 2: botDrives(snake, -1); break;
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
            Matrix input = new Matrix(5, 1);
            input[0, 0] = lookahead(snake);
            input[1, 0] = lookright(snake);
            input[2, 0] = lookleft(snake);
            input[3, 0] = foodAngle(snake, food);
            input[4, 0] = fedGood / 30;
            return input;
        }

        bool selfCollision()
        {
            var rect1x = Canvas.GetLeft(snake[0].rec);
            var rect1y = Canvas.GetTop(snake[0].rec);
            double rect2x;
            double rect2y;
            for (int i = 1; i < snake.segments.Count; i++)
            {
                rect2x = Canvas.GetLeft(snake.segments[i].rec);
                rect2y = Canvas.GetTop(snake.segments[i].rec);
                if (rect1x == rect2x && rect1y == rect2y)
                {
                    return true;
                }
            }
            return false;
        }
        bool selfCollision(snake snake, double x, double y)
        {
            double rect2x;
            double rect2y;
            for (int i = 1; i < snake.segments.Count; i++)
            {
                rect2x = Canvas.GetLeft(snake.segments[i].rec);
                rect2y = Canvas.GetTop(snake.segments[i].rec);
                if (x == rect2x && y == rect2y)
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

        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        double foodAngle(snake snake, Rectangle food)
        {
            var sn0top = double.Parse(snake.segments[0].rec.GetValue(Canvas.TopProperty).ToString());
            var sn1top = double.Parse(snake.segments[1].rec.GetValue(Canvas.TopProperty).ToString());
            var sn0left = double.Parse(snake.segments[0].rec.GetValue(Canvas.LeftProperty).ToString());
            var sn1left = double.Parse(snake.segments[1].rec.GetValue(Canvas.LeftProperty).ToString());
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

        int lookahead(snake snake, int howFar = 1)
        {
            var sntop = double.Parse(snake[0].rec.GetValue(Canvas.TopProperty).ToString());
            var snleft = double.Parse(snake[0].rec.GetValue(Canvas.LeftProperty).ToString());

            if (snake[0].Direction == snake.segment.dir.up)
            {
                if (sntop - (howFar * 10) <= 0) return 1;
                if (selfCollision(snake, snleft, sntop - (howFar * 10))) return 1;
            }
            else if (snake[0].Direction == snake.segment.dir.down)
            {
                if (sntop + 10 + (howFar * 10) >= 200) return 1;
                if (selfCollision(snake, snleft, sntop + (howFar * 10))) return 1;
            }
            else if (snake[0].Direction == snake.segment.dir.left)
            {
                if (snleft - (howFar * 10) <= 0) return 1;
                if (selfCollision(snake, snleft - (howFar * 10), sntop)) return 1;
            }
            else if (snake[0].Direction == snake.segment.dir.right)
            {
                if (snleft + 10 + (howFar * 10) >= 200) return 1;
                if (selfCollision(snake, snleft + (howFar * 10), sntop)) return 1;
            }
            return 0;
        }

        int lookleft(snake snake, int howFar = 1)
        {
            var sntop = double.Parse(snake[0].rec.GetValue(Canvas.TopProperty).ToString());
            var snleft = double.Parse(snake[0].rec.GetValue(Canvas.LeftProperty).ToString());

            if (snake[0].Direction == snake.segment.dir.up)
            {
                if (snleft - (howFar * 10) <= 0) return 1;
                if (selfCollision(snake, snleft - (howFar * 10), sntop)) return 1;
            }
            else if (snake[0].Direction == snake.segment.dir.down)
            {
                if (snleft + 10 + (howFar * 10) >= 200) return 1;
                if (selfCollision(snake, snleft + (howFar * 10), sntop)) return 1;
            }
            else if (snake[0].Direction == snake.segment.dir.left)
            {
                if (sntop + 10 + (howFar * 10) >= 200) return 1;
                if (selfCollision(snake, snleft, sntop + (howFar * 10))) return 1;
            }
            else if (snake[0].Direction == snake.segment.dir.right)
            {
                if (sntop - (howFar * 10) <= 0) return 1;
                if (selfCollision(snake, snleft, sntop - (howFar * 10))) return 1;
            }
            return 0;
        }

        int lookright(snake snake, int howFar = 1)
        {
            var sntop = double.Parse(snake[0].rec.GetValue(Canvas.TopProperty).ToString());
            var snleft = double.Parse(snake[0].rec.GetValue(Canvas.LeftProperty).ToString());

            if (snake[0].Direction == snake.segment.dir.up)
            {
                if (snleft + 10 + (howFar * 10) >= 200) return 1;
                if (selfCollision(snake, snleft + (howFar * 10), sntop)) return 1;
            }
            else if (snake[0].Direction == snake.segment.dir.down)
            {
                if (snleft - (howFar * 10) <= 0) return 1;
                if (selfCollision(snake, snleft - (howFar * 10), sntop)) return 1;
            }
            else if (snake[0].Direction == snake.segment.dir.left)
            {
                if (sntop - (howFar * 10) <= 0) return 1;
                if (selfCollision(snake, snleft, sntop - (howFar * 10))) return 1;
            }
            else if (snake[0].Direction == snake.segment.dir.right)
            {
                if (sntop + 10 + (howFar * 10) >= 200) return 1;
                if (selfCollision(snake, snleft, sntop + (howFar * 10))) return 1;
            }
            return 0;
        }



        bool selfCollision2(double x, double y)
        {
            Rect rect1 = new Rect(x, y, 10.0, 10.0);
            for (int i = 1; i < snake.segments.Count; i++)
            {
                Rect rect2 = new Rect(Canvas.GetLeft(snake[i].rec), Canvas.GetTop(snake[i].rec), 10.0, 10.0);
                if (rect1.IntersectsWith(rect2))
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
                    if (inputsList[i][k, 0] == 0) // lookaround() snake results in network input.
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

        void findBetterWay()
        {
            // train with other direction than one that got us killed.
            var index = findDecision();
            Matrix newTarget = new Matrix(3, 1);
            for (int i = 0; i < 3; i++)
            {
                newTarget[i, 0] = inputsList[index][i, 0];
            }
            newTarget += targetsList[index];
            for (int k = 0; k < 3; k++)
            {
                if (newTarget[k, 0] == 0) newTarget[k, 0] = 1;
                else newTarget[k, 0] = 0;
            }
            //  for (int i = 0; i < 2; i++)
            // neuralNetwork.train(inputsList[index], newTarget);
            inputsList.Clear();
            targetsList.Clear();
        }

        void gameover()
        {
            gamesCounter++;
            timer.IsEnabled = false;
            if (points > maxOfGeneration)
            {
                maxOfGeneration = points;
                lblScore.Content = "Top of generation: " + maxOfGeneration;
            }
            //allPoints.Add(points);
            //allPoints.Sort((p, q) => q.CompareTo(p));
            //double sum = 0;
            //int n = (int)(allPoints.Count * 0.75);
            //for (int i = 0; i < n; i++)
            //{
            //    sum += allPoints[i];
            //}
            //sum /= n;
            //lblctrl.Content = "Average of top 3/4: " + Math.Round(sum, 2);
            //lblspeed.Content = "Mediane: " + allPoints[allPoints.Count / 2];
            if (points > hihgpoints)
            {
                hihgpoints = points;
                lblTopScore.Content = "Top Score: " + hihgpoints;
            }
              totalPoints += points;

            botEvo.population[botIndex].adjustment = evolutionPoints * 2;
            botIndex++;

            totalEvolutionPoints += evolutionPoints * 2;
            points = 0;

            lblaverage.Content = "Average: " + Math.Round((totalPoints) / botIndex, 2);

            // adjustment 
            //  evolutionPoints *= 2;
            //evo.population[networkIndex].adjustment = evolutionPoints;
            //  if (evo.population[networkIndex].adjustment > evo.maxAdj) evo.maxAdj = evo.population[networkIndex].adjustment;
            //   if (evo.population[networkIndex].adjustment < evo.minAdj) evo.minAdj = evo.population[networkIndex].adjustment;
            //networkIndex++;
            lblgames.Content = "Game : " + botIndex;
            evolutionPoints = 0;
            if (botIndex == populationCount) // end of generation.
            {
                totalPoints = 0;
                maxOfGeneration = 0;
                //adjustment normalize
                for (int i = 0; i < botEvo.population.Count; i++)
                {
                    botEvo.population[i].adjustment /= totalEvolutionPoints;
                    if (botEvo.population[i].adjustment > botEvo.maxAdj) botEvo.maxAdj = botEvo.population[i].adjustment;
                    if (botEvo.population[i].adjustment < botEvo.minAdj) botEvo.minAdj = botEvo.population[i].adjustment;
                }
                totalEvolutionPoints = 0;
                lblctrl.Content = "generation: " + (generationCounter++);
                //networkIndex = 0;
                botIndex = 0;
                // select subjects to reproduce
                if (!botEvo.reprSelector())
                {
                    MessageBox.Show("repr loop");
                }
                botEvo.reproduce();
                // death selector
                if (!botEvo.Death())
                {
                    MessageBox.Show("death loop");
                }
                botEvo.toReproduction.Clear();
            }

            if (true)//MessageBox.Show("Wanna try again?", "Game Over!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //remove snake
                foreach (var s in snake.segments)
                {
                    canv.Children.Remove(s.rec);
                }
                snake.segments.Clear();
                // make new snake
                snake.segment head = new snake.segment(50, 50);
                snake.segment head2 = new snake.segment(50, 40, false);
                snake.segments.Add(head);
                snake.segments.Add(head2);
                canv.Children.Add(snake.segments[0].rec);
                canv.Children.Add(snake.segments[1].rec);
                snake.nextDir = snake.segment.dir.down;
                // button start
                Button_Start_Click(null, null);
            }
            else
            {
                Environment.Exit(0);
            }
        }
        void throwFood()
        {
            foodExpire = 0;// infinite loop breaker;
            canv.Children.Remove(food);
            food.SetValue(Canvas.TopProperty, (double)rand.Next(10, maxTop - 40));
            food.SetValue(Canvas.LeftProperty, (double)rand.Next(10, maxLeft - 40));
            canv.Children.Add(food);
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            // load next bot parameters
            pointsTreshold1 = (int)botEvo.population[botIndex][0];
            pointsTreshold2 = (int)botEvo.population[botIndex][1];
            fedGoodDelta1 = (int)botEvo.population[botIndex][2];
            fedGoodDelta2 = (int)botEvo.population[botIndex][3];
            fedGoodDelta3 = (int)botEvo.population[botIndex][4];
            fedGoodDelta4 = (int)botEvo.population[botIndex][5];
            //foodExpiration = (int)botEvo.population[botIndex][6];
            howDeep1 = (int)botEvo.population[botIndex][7];
            howDeep2 = (int)botEvo.population[botIndex][8];
            howDeep3 = (int)botEvo.population[botIndex][9];
            howDeep4 = (int)botEvo.population[botIndex][10];
            howDeep5 = (int)botEvo.population[botIndex][11];
            howDeep6 = (int)botEvo.population[botIndex][12];
            fedGood = (int)botEvo.population[botIndex][13];
            angleNegative = botEvo.population[botIndex][14];
            anglePositive = botEvo.population[botIndex][15];

            timer.Interval = new TimeSpan(0, 0, 0, 0, speed);
            timer.IsEnabled = true;
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
            speed = 200;
            mediumspeed.IsChecked = false;
            fast.IsChecked = false;
            timer.Interval = new TimeSpan(0, 0, 0, 0, speed);
        }

        private void Mediumspeed_Checked(object sender, RoutedEventArgs e)
        {
            speed = 40;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //neuralNetwork.saveWeights();
            //for (int i = 0; i < evo.population.Count; i++)
            //{
            //    var dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            //        + @"\weights" + i + @"\";
            //    evo.population[i].saveWeights(dir);
            //}
            botEvo.savePopulation();
        }
    }
}
