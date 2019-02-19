using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake2
{
    class Evolution
    {
        public int populationCount;
        public double minAdj = Double.MaxValue;
        public double maxAdj = Double.MinValue;
        public List<NeuralNetwork> population = new List<NeuralNetwork>();
        public List<int> toReproduction = new List<int>();
        static Random rand = new Random();
        int netInp, netHid, netOut;
        public Evolution(int pop_count, int netInp, int netHid, int netOut)
        {
            this.netInp = netInp;
            this.netHid = netHid;
            this.netOut = netOut;
            populationCount = pop_count;
            generate_pop();
        }

        public void generate_pop()
        {
            for (int i = 0; i < populationCount; i++)
            {
                NeuralNetwork nn = new NeuralNetwork(netInp, netHid, netOut);
                population.Add(nn);
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

            while (toReproduction.Count < populationCount / 2)
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
                //else if (breakLoop++ > 10000)
                //{
                //    return false;
                //}
            }
            return true;
        }
        public void reproduce()
        {
            for (int i = 0; i < toReproduction.Count; i += 2)
            {
                var A_chromosomeW1 = population[i].weights_ih;
                var A_chromosomeW2 = population[i].weights_ho;
                var A_chromosomeBh = population[i].bias_h;
                var A_chromosomeBo = population[i].bias_o;

                var B_chromosomeW1 = population[i + 1].weights_ih;
                var B_chromosomeW2 = population[i + 1].weights_ho;
                var B_chromosomeBh = population[i + 1].bias_h;
                var B_chromosomeBo = population[i + 1].bias_o;

                NeuralNetwork child1 = new NeuralNetwork(netInp, netHid, netOut);
                NeuralNetwork child2 = new NeuralNetwork(netInp, netHid, netOut);

                var chrW1 = crossingOver(A_chromosomeW1, B_chromosomeW1);
                var chrW2 = crossingOver(A_chromosomeW2, B_chromosomeW2);
                var chrB1 = crossingOver(A_chromosomeBh, B_chromosomeBh);
                var chrB2 = crossingOver(A_chromosomeBo, B_chromosomeBo);

                child1.weights_ih = chrW1[0];
                child1.weights_ho = chrW2[0];
                child1.bias_h = chrB1[0];
                child1.bias_o = chrB2[0];
                child2.weights_ih = chrW1[1];
                child2.weights_ho = chrW2[1];
                child2.bias_h = chrB1[1];
                child2.bias_o = chrB2[1];
                population.Add(child1);
                population.Add(child2);
            }
        }

        public Matrix[] crossingOver(Matrix A, Matrix B)
        {
            int n = A.rows * A.columns;
            Matrix[] result = new Matrix[2];
            double[] packedA = new double[n];
            double[] packedB = new double[n];
            double[] packedChildA = new double[n];
            double[] packedChildB = new double[n];
            int c = 0;
            int breakpoint = rand.Next(1, n);
            for (int i = 0; i < A.rows; i++)
            {
                for (int k = 0; k < A.columns; k++)
                {
                    packedA[c] = A[i, k];
                    packedB[c] = B[i, k];
                    c++;
                }
            }
            for (int i = 0; i < breakpoint; i++)
            {
                packedChildA[i] = packedA[i];
            }
            for (int i = breakpoint; i < packedB.Length - 1; i++)
            {
                packedChildA[i] = packedB[i];
            }
            for (int i = 0; i < breakpoint; i++)
            {
                packedChildB[i] = packedB[i];
            }
            for (int i = breakpoint; i < packedA.Length - 1; i++)
            {
                packedChildB[i] = packedA[i];
            }
            result[0] = new Matrix(A.rows, A.columns);
            result[1] = new Matrix(A.rows, A.columns);
            c = 0;
            for (int i = 0; i < A.rows; i++)
            {
                for (int k = 0; k < A.columns; k++)
                {
                    result[0][i, k] = packedA[c];
                    result[1][i, k] = packedB[c];
                    c++;
                }
            }
            // mutation 0.5% chance
            if (rand.Next(501) == 7)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        for (int i = 0; i < 2; i++)
                        {
                            result[i][rand.Next(A.rows), rand.Next(A.columns)] =
                                result[i][rand.Next(A.rows), rand.Next(A.columns)];
                        }
                        break; // random swap
                    case 1:
                        for (int i = 0; i < 2; i++)
                        {
                            result[i][rand.Next(A.rows), rand.Next(A.columns)] /= 1.5;
                        }
                        break; // divide by 2
                    case 2:
                        for (int i = 0; i < 2; i++)
                        {
                            result[i][rand.Next(A.rows), rand.Next(A.columns)] *= 1.5;
                        }
                        break; // multiply by 2     
                    case 3:
                        for (int i = 0; i < 2; i++)
                        {
                            result[i][rand.Next(A.rows), rand.Next(A.columns)] *= -1;
                        }
                        break; // switch sign 
                }
            }
            return result;
        }

        public bool Death()
        {
            int i = 0;
            int breakLoop = 0;
            while (population.Count > populationCount)
            {
                i = rand.Next(5, populationCount); // top 5 immortal

                //if (population[i].adjustment <= rand.Next(((int)minAdj), (int)maxAdj))
                //{
                //    population.RemoveAt(i);
                //    breakLoop = 0;
                //}
                if (rand.Next(100) > (int)(population[i].adjustment * 100)) // szansa na rozmnazanie wg wsp. rozmnazania.
                {
                    population.RemoveAt(i);
                    breakLoop = 0;
                }
                //else if (breakLoop++ > 10000)
                //{
                //    return false;
                //}
            }
            return true;
        }

    }
}
