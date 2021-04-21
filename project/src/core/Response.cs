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

        public PairDouble evaluateResonanceFrequency(ChartTypes type, SignalUnits units, double approximationResonanceFrequency = 0.0)
        {
            const int kNumSteps = 4096;
            const double kAccuracy = 1e-4;
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
            List<PairDouble> resFrequencies = null;
            switch (type)
            {
                case ChartTypes.REAL_FREQUENCY:
                    return findPhaseRootNewton(splineRealPart, splineImagPart, approximationResonanceFrequency, kAccuracy, kNumSteps);
                case ChartTypes.IMAG_FREQUENCY:
                    resFrequencies = findImagRootBisection(splineRealPart, splineImagPart, frequency[0], frequency[nData - 1], kNumSteps);
                    break;
            }
            if (resFrequencies == null || resFrequencies.Count == 0)
                return null;
            double distance;
            double minDistance = Double.MaxValue;
            int indClosestResonance = 0;
            int numFrequencies = resFrequencies.Count;
            for (int i = 0; i != numFrequencies; ++i)
            {
                distance = Math.Abs(resFrequencies[i].Item1 - approximationResonanceFrequency);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    indClosestResonance = i;
                }

            }
            return resFrequencies[indClosestResonance];
        }

        private List<PairDouble> findImagRootBisection(in CubicSpline realPart, in CubicSpline imagPart, double startX, double endX, int numSteps)
        {
            double previousX;
            double currentX;
            double previousDiffYImag, currentDiffYImag;
            previousX = startX;
            previousDiffYImag = imagPart.Differentiate(startX);
            double step = (endX - startX) / (numSteps - 1);
            double root, amplitude;
            bool isFound;
            List<PairDouble> resFrequencies = new List<PairDouble>();
            for (int i = 1; i != numSteps; ++i)
            {
                currentX = previousX + step;
                currentDiffYImag = imagPart.Differentiate(currentX);
                isFound = currentDiffYImag * previousDiffYImag < 0.0;
                if (isFound)
                {
                    root = (currentX + previousX) / 2.0;
                    // Calculating amplitude
                    double previousYReal = realPart.Interpolate(previousX);
                    double currentYReal = realPart.Interpolate(currentX);
                    double previousYImag = imagPart.Interpolate(previousX);
                    double currentYImag = imagPart.Interpolate(currentX);
                    amplitude = Math.Sqrt(Math.Pow(previousYReal, 2.0) + Math.Pow(previousYImag, 2.0));
                    amplitude += Math.Sqrt(Math.Pow(currentYReal, 2.0) + Math.Pow(currentYImag, 2.0));
                    amplitude /= 2.0;
                    resFrequencies.Add(new PairDouble(root, amplitude));
                }
                previousX = currentX;
                previousDiffYImag = currentDiffYImag;
            }
            return resFrequencies;
        }

        private PairDouble findPhaseRootNewton(in CubicSpline realPart, in CubicSpline imagPart, double approximation, double targetAccuracy, int maxIterationNumber)
        {
            int iterationNumber = 0;
            bool isFound = true;
            double prevRoot = approximation;
            double root = approximation;
            double accuracy = 1.0;
            while (accuracy > targetAccuracy)
            {
                if (iterationNumber >= maxIterationNumber)
                {
                    isFound = false;
                    break;
                }
                root = prevRoot - realPart.Interpolate(prevRoot) / realPart.Differentiate(prevRoot);
                accuracy = Math.Abs(realPart.Interpolate(root));
                prevRoot = root;
                ++iterationNumber;

            }
            if (isFound)
            {
                double amplitude = Math.Sqrt(Math.Pow(realPart.Interpolate(root), 2.0) + Math.Pow(imagPart.Interpolate(root), 2.0));
                return new PairDouble(root, amplitude);
            }
            else
            {
                return null;
            }
        }

        private List<PairDouble> findPhaseRootBisection(in CubicSpline realPart, in CubicSpline imagPart, double startX, double endX, int numSteps)
        {
            double previousX, previousYReal;
            double currentX, currentYReal;
            previousX = startX;
            previousYReal = realPart.Interpolate(startX);
            double step = (endX - startX) / (numSteps - 1);
            List<PairDouble> resFrequencies = new List<PairDouble>();
            double root, amplitude;
            bool isFound;
            for (int i = 1; i != numSteps; ++i)
            {
                currentX = previousX + step;
                currentYReal = realPart.Interpolate(currentX);
                isFound = currentYReal * previousYReal < 0.0;
                if (isFound)
                {
                    root = (currentX + previousX) / 2.0;
                    // Calculating amplitude
                    double previousYImag = imagPart.Interpolate(previousX);
                    double currentYImag = imagPart.Interpolate(currentX);
                    amplitude = Math.Sqrt(Math.Pow(previousYReal, 2.0) + Math.Pow(previousYImag, 2.0));
                    amplitude += Math.Sqrt(Math.Pow(currentYReal, 2.0) + Math.Pow(currentYImag, 2.0));
                    amplitude /= 2.0;
                    resFrequencies.Add(new PairDouble(root, amplitude));
                }
                previousX = currentX;
                previousYReal = currentYReal;
            }
            return resFrequencies;
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
