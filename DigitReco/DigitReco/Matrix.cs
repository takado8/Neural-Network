using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitReco
{
    class Matrix
    {
        double[,] array;
        public int columns;
        public int rows;
        /// <summary>
        /// Init new matrix filled with 0.
        /// </summary>
        public Matrix(int _rows, int _columns)
        {
            columns = _columns;
            rows = _rows;
            array = new double[rows, columns];
        }
        /// <summary>
        /// Init new matrix filled with random numbers (-1;1) or filled with 1.
        /// </summary>
        /// <param name="random">if true returns random matrix, if false fill matrix with 1</param>
        public Matrix(int _rows, int _columns, bool random)
        {
            columns = _columns;
            rows = _rows;
            array = new double[rows, columns];
            if (random)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int k = 0; k < columns; k++)
                    {
                        array[i, k] = GetRandomNumber(-1, 1);
                    }
                }
            }
            else
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int k = 0; k < columns; k++)
                    {
                        array[i, k] = 1;
                    }
                }
            }
        }

        public Matrix(int _rows, int _columns, double[,] _array)
        {
            columns = _columns;
            rows = _rows;
            array = _array;
        }

        public void map(Func<double, double> func)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int k = 0; k < columns; k++)
                {
                    array[i, k] = func(array[i, k]);
                }
            }
        }

        public static Matrix map(Matrix mx, Func<double, double> func)
        {
            double[,] newArr = new double[mx.rows, mx.columns];
            for (int i = 0; i < mx.rows; i++)
            {
                for (int k = 0; k < mx.columns; k++)
                {
                    newArr[i, k] = func(mx.array[i, k]);
                }
            }
            return new Matrix(mx.rows, mx.columns, newArr);
        }

        public void print()
        {
            Console.WriteLine();
            for (int i = 0; i < rows; i++)
            {
                for (int k = 0; k < columns; k++)
                {
                    Console.Write(array[i, k]);// + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix temp = new Matrix(a.rows, a.columns);
            if (a.rows != b.rows || a.columns != b.columns)
            {
                throw new ArgumentException("Matrix can't be added.");
            }
            for (int i = 0; i < a.rows; i++)
            {
                for (int k = 0; k < a.columns; k++)
                {
                    temp[i, k] = a[i, k] + b[i, k];
                }
            }
            return temp;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.columns != b.columns)
            {
                throw new ArgumentException("Matrix can't be substract.");
            }
            Matrix temp = new Matrix(a.rows, a.columns);

            for (int i = 0; i < a.rows; i++)
            {
                for (int k = 0; k < a.columns; k++)
                {
                    temp[i, k] = a[i, k] - b[i, k];
                }
            }
            return temp;
        }
        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.columns != b.columns)
            {
                throw new ArgumentException("Matrix can't be added.");
            }
            else
            {
                for (int i = 0; i < a.rows; i++)
                {
                    for (int k = 0; k < a.columns; k++)
                    {
                        if (a[i, k] != b[i, k]) return false;
                    }
                }
                return true;
            }
        }
        public static bool operator !=(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.columns != b.columns)
            {
                throw new ArgumentException("Matrix can't be added.");
            }
            else
            {
                for (int i = 0; i < a.rows; i++)
                {
                    for (int k = 0; k < a.columns; k++)
                    {
                        if (a[i, k] != b[i, k]) return true;
                    }
                }
                return false;
            }
        }
        public static Matrix operator *(Matrix matrix, double a)
        {
            Matrix newMx = new Matrix(matrix.rows, matrix.columns);
            if (a == 1) return newMx = matrix;
            if (a == 0) return new Matrix(matrix.rows, matrix.columns);
            for (int i = 0; i < matrix.rows; i++)
            {
                for (int k = 0; k < matrix.columns; k++)
                {
                    newMx[i, k] = matrix[i, k] * a;
                }
            }
            return newMx;
        }

        public double this[int y, int x]
        {
            get
            {
                return array[y, x];
            }
            set
            {
                array[y, x] = value;
            }
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix mx = new Matrix(a.rows, b.columns, MultiplyMatrix_Dot(a.array, b.array));
            return mx;
        }

        private static double[,] MultiplyMatrix_Dot(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);
            double[,] result = new double[rA, cB];
            if (cA != rB)
            {
                throw new ArgumentException("matrix can't be multiplayed.");
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        for (int k = 0; k < cA; k++)
                        {
                            result[i, j] += A[i, k] * B[k, j];
                        }
                    }
                }
                return result;
            }
        }

        public void multiplyMatrix_Hadamart(Matrix b)
        {
            if (rows != b.rows || columns != b.columns)
            {
                throw new ArgumentException("Matix can't be multiplyed.");
            }
            for (int i = 0; i < rows; i++)
            {
                for (int k = 0; k < columns; k++)
                {
                    array[i, k] *= b[i, k];
                }
            }
        }

        public static Matrix Transpose(Matrix mx)
        {
            int w = mx.array.GetLength(0);
            int h = mx.array.GetLength(1);
            double[,] result = new double[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = mx[i, j];
                }
            }
            return new Matrix(mx.columns, mx.rows, result);
        }
        static Random random = new Random();
        public static double GetRandomNumber(double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        public static Matrix InputfromArray(double[] arr)
        {
            Matrix mx = new Matrix(arr.Length, 1);
            for (int i = 0; i < mx.rows; i++)
            {
                mx[i, 0] = arr[i];
            }
            return mx;
        }

        public static Matrix TargetFromLabel(byte label)
        {
            Matrix mx = new Matrix(10, 1);
            mx[label, 0] = 1;
            return mx;
        }

        public void writeMatrixToFile(string path)
        {
            using (var sw = new StreamWriter(path))
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        sw.Write(array[i, j] + Environment.NewLine);
                    }
                }
                sw.Flush();
                sw.Close();
            }
        }
        public void readMatrixFromFile(string path)
        {
            var lines = File.ReadAllLines(path);
            int c = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    array[i, j] = double.Parse(lines[c++]);
                }
            }
        }
    }


}
