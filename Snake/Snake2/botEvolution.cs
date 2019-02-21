using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Snake2
{
    class botEvolution
    {
        public class subject
        {
            public List<double> chromosome = new List<double>();
            public double adjustment;

            public double this[int i]
            {
                get
                {
                    return chromosome[i];
                }
                set
                {
                    chromosome[i] = value;
                }
            }
        }

        public List<subject> population = new List<subject>();
        public List<int> toReproduction = new List<int>();

        public double minAdj = double.MaxValue;
        public double maxAdj = double.MinValue;

        public double mutationRate = 0.1;
        public int popCount;

        static Random rand = new Random();

        public botEvolution(int population_count)
        {
            popCount = population_count;
            generatePopulation();
        }

        void generatePopulation()
        {
            for (int i = 0; i < popCount; i++)
            {
                var subject = new subject();
                // first 14 ints (0 - 200)
                for (int k = 0; k < 14; k++)
                {
                    subject.chromosome.Add(rand.Next(101));
                }
                // 2 doubles 0-1
                for (int k = 0; k < 2; k++)
                {
                    subject.chromosome.Add(GetRandomNumber(-1, 1));
                }
                population.Add(subject);
            }
        }
        public bool reprSelector()
        {
            int i = 0;
            int breakLoop = 0;
            population.Sort((p, q) => q.adjustment.CompareTo(p.adjustment)); // sort in descending order
            toReproduction.Add(0); // best 5 to reproduction always
            toReproduction.Add(1);
            toReproduction.Add(2);
            toReproduction.Add(3);
            toReproduction.Add(4);

            while (toReproduction.Count < popCount / 2)
            {
                i = rand.Next(0, population.Count);
                //if (population[i].adjustment >= rand.Next((int)minAdj, (int)maxAdj))
                //{
                //    toReproduction.Add(i);
                //    breakLoop = 0;
                //}
                if (rand.Next(100) < (int)(population[i].adjustment * 100)) // szansa na rozmnazanie wg wsp. rozmnazania.
                {
                    toReproduction.Add(i);
                    breakLoop = 0;
                }
                else if (breakLoop++ > 10000)
                {
                    return false;
                }
            }
            return true;
        }
        public void reproduce()
        {
            for (int i = 0; i < toReproduction.Count; i += 2)
            {
                var sub1 = population[i];
                var sub2 = population[i + 1];
                var child1 = new subject();
                var child2 = new subject();
                int breakpoint = rand.Next(sub1.chromosome.Count);
                for (int k = 0; k < breakpoint; k++)
                {
                    child1.chromosome.Add(sub1[k]);
                    child2.chromosome.Add(sub2[k]);
                }
                for (int k = breakpoint; k < sub1.chromosome.Count; k++)
                {
                    child1.chromosome.Add(sub2[k]);
                    child2.chromosome.Add(sub1[k]);
                }
                //mutation 
                if (rand.Next(100) < (int)(mutationRate * 100)) // szansa na rozmnazanie wg wsp. rozmnazania.
                {
                    switch (rand.Next(4))
                    {
                        case 0: child1[rand.Next(child1.chromosome.Count - 2)] = rand.Next(201); break;
                        case 1: child1[rand.Next(child1.chromosome.Count - 2)] = child1[rand.Next(child1.chromosome.Count - 2)]; break;
                        case 2: child1[rand.Next(child1.chromosome.Count - 2)] = child2[rand.Next(child2.chromosome.Count - 2)]; break;
                        case 3: child1[rand.Next(child1.chromosome.Count - 3, child1.chromosome.Count)] = GetRandomNumber(-1, 1); break;
                    }
                }
                population.Add(child1);
                population.Add(child2);
            }
        }
        public bool Death()
        {
            int i = 0;
            //  int breakLoop = 0;
            while (population.Count > popCount)
            {
                i = rand.Next(5, popCount); // top 5 immortal
                //if (population[i].adjustment <= rand.Next(((int)minAdj), (int)maxAdj))
                //{
                //    population.RemoveAt(i);
                //    breakLoop = 0;
                //}
                if (rand.Next(100) >= (int)(population[i].adjustment * 100))
                {
                    population.RemoveAt(i);
                    //breakLoop = 0;
                }
                //else if (breakLoop++ > 10000)
                //{
                //    return false;
                //}
            }
            return true;
        }

        public void savePopulation()
        {
            if (!Directory.Exists("botEvo")) Directory.CreateDirectory("botEvo");
            for (int i = 0; i < population.Count; i++)
            {
                File.WriteAllLines(@"botEvo\" +population[i].adjustment+"_"+ i +".txt", population[i].chromosome.Select(x => x.ToString()).ToArray());
            }
        }

        public void readPopulation()
        {
            if (!Directory.Exists("botEvo"))
            {
                MessageBox.Show("cannot find botEvo dir");              
            }
            else
            {
                for(int i=0; i< popCount; i++)
                {
                    population[i].chromosome = File.ReadAllLines(@"botEvo\" + i + ".txt").Select(x => double.Parse(x)).ToList();
                }
            }
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            return rand.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
