using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DigitReco
{
    class NeuralNetwork
    {
        public List<Perceptron> perceptrones = new List<Perceptron>();

        public NeuralNetwork(int perceptronesCount, int perceptroneInputs)
        {
            for (int i = 0; i < perceptronesCount; i++)
            {
                Perceptron pct = new Perceptron(perceptroneInputs, i);
                perceptrones.Add(pct);
            }
        }

        public int GiveAnswer(int[] inputs)
        {
            double maxVal = 0;
            int maxIndex = 0;
            for (int i = 0; i < perceptrones.Count; i++)
            {
                var guess = perceptrones[i].guess(inputs);

                if (guess > maxVal)
                {
                    maxVal = guess;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        public void train(int[] inputs, byte label)
        {
            foreach (var perceptron in perceptrones)
            {
                perceptron.train(inputs, label);
            }
        }
        public void SaveWeights()
        {
            if (!Directory.Exists("weights")) Directory.CreateDirectory("weights");
            foreach(var perc in perceptrones)
            {
                perc.WriteWeightsToFile();
            }
        }
        public void ReadWeights()
        {
            foreach (var perc in perceptrones)
            {
                if (!perc.ReadWeightsFile()) break;
            }
        }
    }

    class Perceptron
    {
        public int perceptroneIndex;
        public int inputsCount;
        const double learningRate = 0.05;
        public List<double> weights = new List<double>();
        static Random random = new Random();

        public Perceptron(int input_count, int _perceptroneIndex)
        {
            perceptroneIndex = _perceptroneIndex;
            inputsCount = input_count;
            for (int i = 0; i < input_count; i++)
            {
                weights.Add(GetRandomNumber(0, 1));
            }
        }

        public double guess(int[] inputs)
        {
            double sum = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                sum += inputs[i] * weights[i];
            }
            return sum / inputsCount;
        }

        public void train(int[] inputs, byte correctOutput)
        {
            if (correctOutput == perceptroneIndex)
            {
                correctOutput = 1;
            }
            else
            {
                correctOutput = 0;
            }
            double answer = guess(inputs);
            double error = correctOutput - answer;

            for (int i = 0; i < weights.Count; i++)
            {
                weights[i] += error * inputs[i] * learningRate;
            }
        }

        public bool ReadWeightsFile()
        {
            string path = @"weights\p" + perceptroneIndex + ".w";
            if (File.Exists(path))
            {
                var wagi = File.ReadAllLines(path);
                weights.Clear();

                for (int i = 0; i < wagi.Length; i++)
                {
                    weights.Add(double.Parse(wagi[i]));
                }
                return true;
            }
            else
            {
                MessageBox.Show("Cannot read weights");
                return false;
            }
        }
        public void WriteWeightsToFile()
        {
            string path = @"weights\p" + perceptroneIndex + ".w";
            File.WriteAllText(path, "");
            foreach (var w in weights)
            {
                    File.AppendAllText(path, w.ToString() + Environment.NewLine);         
            }
        }

        public double GetRandomNumber(double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
        public double Sigmoid(double x)
        {
            return 2 / (1 + Math.Exp(-2 * x)) - 1;
        }
        public int singn(double val)
        {
            if (val < 0) return -1;
            else return 1;
        }
      
    }

}
