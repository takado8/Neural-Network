using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOR
{
    class NeuralNetwork
    {
        double learning_rate = 0.1;

        int input_nodes;
        int hidden_nodes;
        int output_nodes;

        //Matrix inputs;
        Matrix weights_ih;
        Matrix bias_h;
        // Matrix hidden;
        Matrix weights_ho;
        Matrix bias_o;
        //  Matrix output;


        public NeuralNetwork(int _inputs_count, int _hidden_count, int _outputs_count)
        {
            input_nodes = _inputs_count;
            hidden_nodes = _hidden_count;
            output_nodes = _outputs_count;

            weights_ih = new Matrix(hidden_nodes, input_nodes, true);
            weights_ho = new Matrix(output_nodes, hidden_nodes, true);

            bias_h = new Matrix(hidden_nodes, 1, true);
            bias_o = new Matrix(output_nodes, 1, true);
        }

        public Matrix get_answer(Matrix inputs)
        {
            // multiply by weights
            var hidden = weights_ih * inputs;
            // add bias
            hidden += bias_h;
            // activation func
            hidden.map(Sigmoid);

            // next layer
            var output = weights_ho * hidden;
            output += bias_o;

            output.map(Sigmoid);
            return output;
        }

        public void train(Matrix inputs, Matrix target)
        {

            // multiply by weights
            var hidden = weights_ih * inputs;
            // add bias
            hidden += bias_h;
            // activation func
            hidden.map(Sigmoid);

            // next layer
            // 
            var output = weights_ho * hidden;
            output += bias_o;

            output.map(Sigmoid);

            // error....
            var output_error = target - output;

            var gradients = Matrix.map(output, dSigmoid);
            gradients.multiplyMatrix_Hadamart(output_error);
            gradients *= learning_rate;

            //calc deltas
            var hidden_t = Matrix.Transpose(hidden);
            var weight_ho_deltas = gradients * hidden_t;
            // Adjust the weights by deltas
            weights_ho += weight_ho_deltas;
            bias_o += gradients;

            // Calculate the hidden layer errors
            var who_t = Matrix.Transpose(weights_ho);
            var hidden_errors = who_t * output_error;
            // Calculate hidden gradient
            var hidden_gradient = Matrix.map(hidden, dSigmoid);
            hidden_gradient.multiplyMatrix_Hadamart(hidden_errors);
            hidden_gradient *= learning_rate;
            // Calcuate input->hidden deltas
            var inputs_T = Matrix.Transpose(inputs);
            var weight_ih_deltas = hidden_gradient * inputs_T;
            weights_ih += weight_ih_deltas;
            bias_h += hidden_gradient;
        }

        public double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }
        //juz bedzie sigmoida
        public double dSigmoid(double s)
        {
           // double s = Sigmoid(x);
            return s * (1 - s);
        }
    }
}
