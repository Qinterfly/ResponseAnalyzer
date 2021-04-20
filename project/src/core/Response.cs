using System;
using System.Collections.Generic;
using MathNet.Numerics.Interpolation;

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
            reference = null;
        }

        public bool equals(Response another)
        {
            return path == another.path;
        }

        public PairDouble evaluateResonanceFrequency(ChartTypes type, SignalUnits units)
        {
            const int kNumSteps = 2048;
            double[,] complexData = data[units];
            int nData = frequency.Length;
            double[] realPart = new double[nData];
            double[] imaginaryPart = new double[nData];
            for (int i = 0; i != nData; ++i)
            {
                realPart[i] = complexData[i, 0];
                imaginaryPart[i] = complexData[i, 1];
            }
            CubicSpline splineRealPart = CubicSpline.InterpolateNatural(frequency, realPart);
            CubicSpline splineImaginaryPart = CubicSpline.InterpolateNatural(frequency, imaginaryPart);
            switch (type)
            {
                case ChartTypes.REAL_FREQUENCY:
                    return findRoot(splineRealPart, splineImaginaryPart, frequency[0], frequency[nData - 1], kNumSteps);
                case ChartTypes.IMAG_FREQUENCY:
                    return findPeak(splineRealPart, splineImaginaryPart, frequency[0], frequency[nData - 1], kNumSteps);
            }
            return null;
        }

        private PairDouble findPeak(in CubicSpline realPart, in CubicSpline imaginaryPart, double startX, double endX, int numSteps)
        {
            double maxValue = 0.0;
            double tempValue;
            double amplitude = 0.0;
            double root = -1.0;
            double X;
            double YReal, YImaginary;
            double step = (endX - startX) / (numSteps - 1);
            for (int i = 0; i != numSteps; ++i)
            {
                X = startX + i * step;
                YImaginary = imaginaryPart.Interpolate(X);
                tempValue = Math.Abs(YImaginary);
                if (tempValue > maxValue)
                {
                    YReal = realPart.Interpolate(X);
                    maxValue = tempValue;
                    amplitude = Math.Sqrt(Math.Pow(YReal, 2.0) + Math.Pow(YImaginary, 2.0));
                    root = X;
                }
            }
            return new PairDouble(root, amplitude);
        }

        private PairDouble findRoot(in CubicSpline realPart, in CubicSpline imaginaryPart, double startX, double endX, int numSteps)
        {
            double root = -1.0;
            double amplitude = 0.0;
            bool isFound = false;
            double previousX, previousYReal;
            double currentX, currentYReal;
            previousX = startX;
            previousYReal = realPart.Interpolate(startX);
            double step = (endX - startX) / (numSteps - 1);
            for (int i = 1; i != numSteps; ++i)
            {
                currentX = previousX + step;
                currentYReal = realPart.Interpolate(currentX);
                isFound = currentYReal * previousYReal < 0.0;
                if (isFound)
                {
                    root = (currentX + previousX) / 2.0;
                    // Calculating amplitude
                    double previousYImaginary = imaginaryPart.Interpolate(previousX);
                    double currentYImaginary = imaginaryPart.Interpolate(currentX);
                    amplitude = Math.Sqrt(Math.Pow(previousYReal, 2.0) + Math.Pow(previousYImaginary, 2.0)); 
                    amplitude += Math.Sqrt(Math.Pow(currentYReal, 2.0) + Math.Pow(currentYImaginary, 2.0));        
                    amplitude /= 2.0;                                                               
                    break;
                }
                previousX = currentX;
                previousYReal = currentYReal;
            }
            return isFound ? new PairDouble(root, amplitude) : null;
        }

        // Data
        public Dictionary<SignalUnits, double[,]> data { get; set; }
        public double[] frequency { get; set; }
        // Info
        public string signalName { get; set; }
        public string path { get; set; }
        public string originalRun { get; set; }
        public ResponseReference reference { get; set; }
    }

    public class ResponseReference
    {
        public bool isResolved = false;
        public string nodeFullName;
        public ChartDirection direction;
        public double directionSign;
    }
}
