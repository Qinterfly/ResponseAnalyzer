using System;
using System.Collections.Generic;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.RootFinding;

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

        public PairDouble evaluateResonanceFrequency(ChartTypes type, SignalUnits units, double approximationResonanceFrequency = 0.0)
        {
            int kNumSteps = 4096;
            double[,] complexData = data[units];
            int nData = frequency.Length;
            double[] realPart = new double[nData];
            double[] imagPart = new double[nData];
            for (int i = 0; i != nData; ++i)
            {
                realPart[i] = complexData[i, 0];
                imagPart[i] = complexData[i, 1];
            }
            CubicSpline splineRealPart = CubicSpline.InterpolateNatural(frequency, realPart);
            CubicSpline splineImagPart = CubicSpline.InterpolateNatural(frequency, imagPart);
            List<double> resFrequencies = null;
            Func<double, double> fun = null;
            Func<double, double> diffFun = null;
            switch (type)
            {
                case ChartTypes.REAL_FREQUENCY:
                    fun = x => splineRealPart.Interpolate(x);
                    diffFun = x => splineRealPart.Differentiate(x);
                    break;
                case ChartTypes.IMAG_FREQUENCY:
                    fun = x => splineImagPart.Differentiate(x);
                    diffFun = x => splineRealPart.Differentiate2(x);
                    break;
            }
            if (fun == null || diffFun == null)
                return null;
            double startFrequency = frequency[0];
            double endFrequency = frequency[nData - 1];
            resFrequencies = findAllRootsBisection(fun, startFrequency, endFrequency, kNumSteps);
            if (resFrequencies == null || resFrequencies.Count == 0)
                return null;
            double distance;
            double minDistance = Double.MaxValue;
            int indClosestResonance = 0;
            int numFrequencies = resFrequencies.Count;
            for (int i = 0; i != numFrequencies; ++i)
            {
                resFrequencies[i] = NewtonRaphson.FindRootNearGuess(fun, diffFun, resFrequencies[i], startFrequency, endFrequency);
                distance = Math.Abs(resFrequencies[i] - approximationResonanceFrequency);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    indClosestResonance = i;
                }

            }
            double resonanceFrequency = resFrequencies[indClosestResonance];
            double realPeak = splineRealPart.Interpolate(resonanceFrequency);
            double imagPeak = splineImagPart.Interpolate(resonanceFrequency);
            double ampPeak = Math.Sqrt(Math.Pow(realPeak, 2.0) + Math.Pow(imagPeak, 2.0));
            return new PairDouble(resonanceFrequency, ampPeak);
        }

        private List<double> findAllRootsBisection(Func<double, double> fun, double leftBound, double rightBound, int numSteps)
        {
            List<double> roots = new List<double>();
            double step = (rightBound - leftBound) / (numSteps - 1);
            bool isFound;
            double previousX = leftBound;
            double previousY = fun(leftBound);
            double currentX, currentY;
            for (int i = 1; i != numSteps; ++i)
            {
                currentX = previousX + step;
                currentY = fun(currentX);
                isFound = currentY * previousY < 0.0;
                if (isFound)
                    roots.Add((currentX + previousX) / 2.0);
                previousX = currentX;
                previousY = currentY;
            }
            return roots;
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
