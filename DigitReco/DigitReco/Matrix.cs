using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitReco
{
    class Matrix
    {
        double[,] array;
        int columns;
        int rows;

        public Matrix(int _rows, int _columns)
        {
            columns = _columns;
            rows = _rows;
            array = new double[rows, columns];
        }

        public Matrix(int _rows, int _columns, double[,] _array)
        {
            columns = _columns;
            rows = _rows;
            array = _array;
        }

        public Matrix()
        {
        }

        public void print()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int k = 0; k < columns; k++)
                {
                    Console.Write(array[i, k] + "\t");
                }
                Console.WriteLine();
            }
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix temp = new Matrix(a.rows, a.columns);

            for (int i = 0; i < a.rows; i++)
            {
                for (int k = 0; k < a.columns; k++)
                {
                    temp.array[i, k] = a.array[i, k] + b.array[i, k];
                }
            }
            return temp;
        }
        public static Matrix operator -(Matrix a, Matrix b)
        {
            Matrix temp = new Matrix(a.rows, a.columns);

            for (int i = 0; i < a.rows; i++)
            {
                for (int k = 0; k < a.columns; k++)
                {
                    temp.array[i, k] = a.array[i, k] - b.array[i, k];
                }
            }
            return temp;
        }
        public static Matrix operator *(double a, Matrix matrix)
        {
            for (int i = 0; i < matrix.rows; i++)
            {
                for (int k = 0; k < matrix.columns; k++)
                {
                    matrix.array[i, k] *= a;
                }
            }
            return matrix;
        }
        public static Matrix operator *(Matrix matrix, double a)
        {
            if (a == 1) return matrix;
            if (a == 0) return new Matrix(matrix.rows, matrix.columns);
            for (int i = 0; i < matrix.rows; i++)
            {
                for (int k = 0; k < matrix.columns; k++)
                {
                    matrix.array[i, k] *= a;
                }
            }
            return matrix;
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix mx = new Matrix(a.rows, b.columns,MultiplyMatrix(a.array,b.array));
            return mx;
        }
        public static double[,] MultiplyMatrix(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);
            double[,] result = new double[rA, cB];
            if (cA != rB)
            {
                throw new ArgumentException("matrix can't be multiplayed!");
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

        public static double[] MatrixProduct(double[,] matrixA, double[] vectorB)
        {
            int aRows = matrixA.GetLength(0); int aCols = matrixA.GetLength(1);
            int bRows = vectorB.Length;
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");
            double[] result = new double[aRows];
            for (int i = 0; i < aRows; ++i) // each row of A
                for (int k = 0; k < aCols; ++k)
                    result[i] += matrixA[i,k] * vectorB[k];
            return result;
        }
        public void Transpose()
        {
            int w = array.GetLength(0);
            int h = array.GetLength(1);

            double[,] result = new double[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = array[i, j];
                }
            }
            array = result;
        }
    }
}
