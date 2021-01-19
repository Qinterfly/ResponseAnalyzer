using System;
using System.Collections.Generic;

namespace ResponseAnalyzer
{
    using PairDouble = Tuple<double, double>;

    public class Response
    {
        public Response()
        {
            data = new Dictionary<SignalUnits, double[,]>();
            data.Add(SignalUnits.MILLIMETERS, null);
            data.Add(SignalUnits.METERS_PER_SECOND2, null);
        }

        public bool equals(Response another)
        {
            return path == another.path;
        }

        public PairDouble evaluateResonanceFrequency(ChartTypes type, SignalUnits units)
        {
            switch (type)
            {
                case ChartTypes.REAL_FREQUENCY:
                    return findRoot(frequency, data[units], 0);
                case ChartTypes.IMAG_FREQUENCY:
                    return findPeak(frequency, data[units], 1);
            }
            return null;
        }

        private PairDouble findPeak(double[] X, double[,] Y, int iColumn)
        {
            double maxValue = 0.0;
            double tempValue;
            double amplitude = 0.0;
            double root = -1.0;
            int nY = Y.GetLength(0);
            for (int i = 0; i != nY; ++i)
            {
                tempValue = Math.Abs(Y[i, iColumn]);
                if (tempValue > maxValue)
                {
                    maxValue = tempValue; 
                    amplitude = Math.Sqrt(Math.Pow(Y[i, 0], 2.0) + Math.Pow(Y[i, 1], 2.0));
                    root = X[i];
                }
            }
            return new PairDouble(root, amplitude);
        }

        private PairDouble findRoot(double[] X, double[,] Y, int iColumn)
        {
            double prevValue = Y[0, iColumn];
            double root = -1.0;
            double amplitude = 0.0;
            bool isFound = false;
            int nY = Y.GetLength(0);
            for (int i = 1; i != nY; ++i)
            {
                isFound = Y[i, iColumn] * prevValue < 0.0;
                if (isFound)
                {
                    root = (X[i] + X[i - 1]) / 2.0;
                    // Calculating amplitude
                    amplitude = Math.Sqrt(Math.Pow(Y[i - 1, 0], 2.0) + Math.Pow(Y[i - 1, 1], 2.0)); // First
                    amplitude += Math.Sqrt(Math.Pow(Y[i, 0], 2.0) + Math.Pow(Y[i, 1], 2.0));        // Second
                    amplitude /= 2.0;                                                               // Mean
                    break;
                }
                prevValue = Y[i, iColumn];
            }
            return isFound ? new PairDouble(root, amplitude) : null;
        }

        // Data
        public Dictionary<SignalUnits, double[,]> data { get; set; }
        public double[] frequency { get; set; }
        // Info
        public string signalName { get; set; }
        public string path { get; set; }
    }
}
