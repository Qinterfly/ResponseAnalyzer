using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseAnalyzer
{
    public interface ISelection
    {
        object retrieveSelection();

    }

    [Serializable]
    class Lines : ISelection
    {
        // Get all the nodes binded to the line
        public object retrieveSelection()
        {
            return nodeNames_;
        }

        public string lineName_ { get; set; }
        public List<string> nodeNames_ { get; set; }
    }

    [Serializable]
    class Nodes : ISelection
    {
        // Get the selected node
        public object retrieveSelection()
        {
            return nodeName_;
        }

        public string nodeName_ { get; set; }
    }

}
