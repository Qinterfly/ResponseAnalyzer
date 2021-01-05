using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using LMSTestLabAutomation;


namespace ResponseAnalyzer
{
    // Defining types
    using StringDictionary = Dictionary<string, Dictionary<string, uint>>;
    using ArrayDictionary = Dictionary<string, Array>;
    using ElementDictionary = Dictionary<ElementType, Dictionary<string, Array>>;
    using ColorDictionary = Dictionary<string, Color4>;
    public enum ElementType { QUADS, TRIAS, LINES}
    public enum ChartTypes { UNKNOWN, REALFRF, IMAGFRF, MODESET, FORCE}
    public enum SignalUnits { UNKNOWN, MILLIMETERS, METERS_PER_SECOND2}
    public enum ChartDirection { UNKNOWN, X, Y, Z}

    public partial class LMSModel
    {
        // Constructor
        public LMSModel()
        {
            elementTypes_ = Enum.GetValues(typeof(ElementType));
            // Binding the element types with the rendering ones
            mapElements_ = new Dictionary<ElementType, PrimitiveType>();
            mapElements_[ElementType.LINES] = PrimitiveType.Lines;
            mapElements_[ElementType.TRIAS] = PrimitiveType.Triangles;
            mapElements_[ElementType.QUADS] = PrimitiveType.Quads;
        }

        // Retreiving drawing objects from the user specified geometry
        public void setGeometry(IGeometry geometry)
        {
            if (geometry == null)
                return;
            Array X, Y, Z, rotXY, rotXZ, rotYZ;
            Array nodeNamesA, nodeNamesB, nodeNamesC, nodeNamesD;
            componentNames_ = new List<string>();
            componentSet_ = new ComponentGeometry(elementTypes_);
            componentBuffers_ = new ComponentBufferPointers();
            componentShowMask_ = new Dictionary<string, bool>();
            Array componentNames = geometry.ComponentNames;
            int nColors = availableColors_.Count;
            int indexCurrentColor = 0;
            Vector4d globalNodeLocation = Vector4d.UnitW;
            Matrix4d transform = Matrix4d.Identity;
            // Limits of the coordinates
            float[,] limits = new float[3, 2];
            float tempVal = float.MaxValue;
            for (int i = 0; i != 3; ++i) {
                limits[i, 0] =  tempVal;
                limits[i, 1] = -tempVal;
            }
            foreach (string component in componentNames)
            {
                componentNames_.Add(component);
                Array nodeNames = geometry.ComponentNodeNames[component];
                int nNodes = nodeNames.Length;
                // Component node names
                Dictionary<string, uint> mapNodes = new Dictionary<string, uint>();
                for (uint iNode = 0; iNode != nNodes; ++iNode)
                    mapNodes.Add((string)nodeNames.GetValue(iNode), iNode);
                componentSet_.mapNodeNames.Add(component, mapNodes);
                componentSet_.nodeNames.Add(component, nodeNames);
                // Component positions
                double tX, tY, tZ, tRotXY, tRotXZ, tRotYZ;
                geometry.ComponentValues(component, out tX, out tY, out tZ, out tRotXY, out tRotXZ, out tRotYZ);
                double[] componentPosition = new double[6] { tX, tY, tZ, tRotXY, -tRotXZ, tRotYZ }; // (!) -rotY
                componentSet_.positions_.Add(component, componentPosition);
                // Nodal positions and angles of the local coordinate systems
                geometry.ComponentNodesValues(component, nodeNames, out X, out Y, out Z, out rotXY, out rotXZ, out rotYZ);
                double[,] angles = new double[nNodes, 3];
                double[,] coordinates = new double[nNodes, 3];
                float[] vertices = new float[nNodes * 3];
                int insertInd = 0;
                double tempCoord;
                for (int iNode = 0; iNode != nNodes; ++iNode)
                {
                    globalNodeLocation[0] = (double)X.GetValue(iNode);
                    globalNodeLocation[1] = (double)Y.GetValue(iNode);
                    globalNodeLocation[2] = (double)Z.GetValue(iNode);
                    globalNodeLocation[3] = 1.0;
                    // Conversion from the component's local to the global coordinate system
                    transform = Matrix4d.CreateRotationX(componentPosition[5])
                              * Matrix4d.CreateRotationY(componentPosition[4])
                              * Matrix4d.CreateRotationZ(componentPosition[3]);
                    transform *= Matrix4d.CreateTranslation(componentPosition[0], componentPosition[1], componentPosition[2]);
                    globalNodeLocation = Vector4d.Transform(globalNodeLocation, transform);
                    for (int k = 0; k != 3; ++k)
                    {
                        tempCoord = globalNodeLocation[k];
                        vertices[insertInd + k] = (float) tempCoord;
                        coordinates[iNode, k] = tempCoord;
                    }
                    // Saving the nodal angles
                    angles[iNode, 0] =  (double)rotXY.GetValue(iNode);
                    angles[iNode, 1] = -(double)rotXZ.GetValue(iNode); // (!) -rotY
                    angles[iNode, 2] =  (double)rotYZ.GetValue(iNode);
                    for (int k = 0; k != 3; ++k)
                        angles[iNode, k] = MathHelper.DegreesToRadians(angles[iNode, k]);
                    insertInd = insertInd + 3;
                }
                // Finding the maximum and minimum of the nodal coordinates { (:, 1) -- minimum, (:, 2) -- maximum }
                int lenVertices = vertices.Length;
                for (int i = 0; i != lenVertices; i = i + 3)
                {
                    for (int j = 0; j != 3; ++j)
                    {
                        tempVal = vertices[i + j];
                        if (tempVal < limits[j, 0]) limits[j, 0] = tempVal;
                        if (tempVal > limits[j, 1]) limits[j, 1] = tempVal;
                    }
                }
                componentSet_.vertices.Add(component, vertices);
                componentSet_.nodeAngles.Add(component, angles);
                componentSet_.nodeCoordinates.Add(component, coordinates);
                // Lines 
                geometry.ComponentLines(component, out nodeNamesA, out nodeNamesB);
                int nLines = nodeNamesA.Length;
                uint[] linesInd = new uint[nLines * 2];
                insertInd = 0;
                for (int i = 0; i != nLines; ++i)
                {
                    linesInd[insertInd] = mapNodes[(string)nodeNamesA.GetValue(i)];
                    linesInd[insertInd + 1] = mapNodes[(string)nodeNamesB.GetValue(i)];
                    insertInd = insertInd + 2;
                }
                componentSet_.elementData[ElementType.LINES].Add(component, linesInd);
                // Triangles
                geometry.ComponentTrias(component, out nodeNamesA, out nodeNamesB, out nodeNamesC);
                int nTrias = nodeNamesA.Length;
                uint[] triasInd = new uint[nTrias * 3];
                insertInd = 0;
                for (int i = 0; i != nTrias; ++i)
                {
                    triasInd[insertInd] = mapNodes[(string)nodeNamesA.GetValue(i)];
                    triasInd[insertInd + 1] = mapNodes[(string)nodeNamesB.GetValue(i)];
                    triasInd[insertInd + 2] = mapNodes[(string)nodeNamesC.GetValue(i)];
                    insertInd = insertInd + 3;
                }
                componentSet_.elementData[ElementType.TRIAS].Add(component, triasInd);
                // Quads
                geometry.ComponentQuads(component, out nodeNamesA, out nodeNamesB, out nodeNamesC, out nodeNamesD);
                int nQuads = nodeNamesA.Length;
                uint[] quadsInd = new uint[nQuads * 4];
                insertInd = 0;
                for (int i = 0; i != nQuads; ++i)
                {
                    quadsInd[insertInd] = mapNodes[(string)nodeNamesA.GetValue(i)];
                    quadsInd[insertInd + 1] = mapNodes[(string)nodeNamesB.GetValue(i)];
                    quadsInd[insertInd + 2] = mapNodes[(string)nodeNamesC.GetValue(i)];
                    quadsInd[insertInd + 3] = mapNodes[(string)nodeNamesD.GetValue(i)];
                    insertInd = insertInd + 4;
                }
                componentSet_.elementData[ElementType.QUADS].Add(component, quadsInd);
                // Specifying colors
                indexCurrentColor = indexCurrentColor >= nColors - 1 ? 0 : indexCurrentColor + 1;
                componentSet_.colors.Add(component, availableColors_[indexCurrentColor]);
                // Show mask
                componentShowMask_.Add(component, true);
            }
            // Calculating the range of the coordinates { Min -- (:, 0), Delta -- (:, 1) }
            float[] shift = new float[3];
            float maxProj = 0.0f;
            for (int i = 0; i != 3; ++i)
            {
                limits[i, 1] -= limits[i, 0];
                shift[i] = -1.0f;
                if (Math.Abs(limits[i, 1]) > maxProj)
                    maxProj = Math.Abs(limits[i, 1]);
                if (Math.Abs(limits[i, 1]) <= float.Epsilon)
                    shift[i] = 0.0f;
            }
            // Normalizing the coordinates and generating the buffers to render
            foreach (string component in componentNames)
            {
                normalizeVertices((float[])componentSet_.vertices[component], limits, shift, maxProj);
                generateBuffers(component);
            }
        }

        private void normalizeVertices(float[] vertices, float[,] limits, float[] shift, float maxProj)
        {
            int lenVertices = vertices.Length;
            for (int i = 0; i != lenVertices; i = i + 3)
            {
                for (int j = 0; j != 3; ++j)
                    vertices[i + j] = 2.0f * (vertices[i + j] - limits[j, 0]) / maxProj + shift[j];
            }
        }
            
        // Components data
        private List<string> componentNames_;
        public ComponentGeometry componentSet_ { get; set; }
        private Shader shader_;
        private ComponentBufferPointers componentBuffers_;
        private Array elementTypes_;
        private Dictionary<ElementType, PrimitiveType> mapElements_;
    }

    public class ComponentGeometry
    {
        public ComponentGeometry(Array elementTypes)
        {
            positions_ = new ArrayDictionary();
            mapNodeNames = new StringDictionary();
            nodeNames = new ArrayDictionary();
            nodeAngles = new ArrayDictionary();
            nodeCoordinates = new ArrayDictionary();
            elementData = new ElementDictionary();
            vertices = new ArrayDictionary();
            foreach (ElementType type in elementTypes)
                elementData.Add(type, new Dictionary<string, Array>());
            colors = new ColorDictionary();
        }
        public ArrayDictionary positions_;
        public StringDictionary mapNodeNames;
        public ArrayDictionary nodeNames;
        public ArrayDictionary nodeAngles;
        public ArrayDictionary nodeCoordinates;
        public ElementDictionary elementData;
        public ArrayDictionary vertices;
        public ColorDictionary colors;
    }

    public class ComponentBufferPointers
    {
        public ComponentBufferPointers()
        {
            vertexBufferObject = new Dictionary<string, int>();
            elements = new Dictionary<string, int[]>();
            selection = new Dictionary<string, int>();
        }
        public Dictionary<string, int> vertexBufferObject;
        public Dictionary<string, int[]> elements;
        public Dictionary<string, int> selection;
    }
}
