using System;
using System.Collections.Generic;

namespace ResponseAnalyzer
{

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
