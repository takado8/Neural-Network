using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOR
{
    class Program
    {
        static void Main(string[] args)
        {
            // XOR
            double[][] inps =
            {
                 new double[] { 0, 0 },  // 0
                 new double[] { 1, 1 },  // 0
                 new double[] { 1, 0 },  // 1
                 new double[] { 0, 1 },  // 1
            };
            Matrix[] inputs = new Matrix[4];
            for (int i = 0; i < 4; i++)
            {
                inputs[i] = Matrix.InputfromArray(inps[i]);
            }

            Matrix[] answers = new Matrix[4];
            answers[0] = new Matrix(1, 1);          //0
            answers[1] = new Matrix(1, 1);          //0
            answers[2] = new Matrix(1, 1,false);   //1
            answers[3] = new Matrix(1, 1,false);   //1
            

            NeuralNetwork nn = new NeuralNetwork(2, 5, 1);

            Random rand = new Random();

            for (int i = 0; i < 50000; i++)
            {
                int r = rand.Next(4);
                nn.train(inputs[r], answers[r]);
            }

            foreach (var x in inputs)
            {
                x.print();
                nn.get_answer(x).print();
            }
          

            //nn.train(mx, mx2);

            Console.WriteLine("end.... press any.");
            Console.ReadKey();
        }
    }
}
