using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonitorBreak.Bebug
{
    public class Graph
    {
        /**
            We should create a visual graph system
            Not sure how that would be done tho
            Bar chart seems easy but maybe less useful (I actually don't think this is true)
            (Main issue is the rotation of the lines drawn between the points) 

            There should be a constructor (is it not based on an instance then*, seems like the best approach)
            The constructor should define a screen position to place the graph (displacement from the screen origin**), and a size for the window
            It should also take a y axis range (if set to 0 (default value) the y axis should dynamically adjust) and
            a number of segments (default of like 20?)
            There should also be a SetNewValue function which sets the current last bar in the graph, all other previous bars should adjust
            Fixed array for storing values (length = number of segments)

            *We are gonna need a instance anyawy to run the OnGUI() code, having multiple graphs seems vital tho
            **Maybe we should have an option to make the graph relative to the gui screen origin or the screen center
        **/

        private Texture2D outputTexture;

        private Color color;
        private Vector2 screenPosition;
        private Vector2 size;
        private List<Vector2> points = new List<Vector2>();
        //private float scaleFactor = 1.0f; //Scale factor is conversion from points in axis-space to cells in the texture pixel grid 

        public Graph(Vector2 screenPos, Vector2 windowSize, Color graphColor, float graphScaleFactor = 1.0f)
        {
            color = graphColor;

            screenPosition = screenPos;
            size = windowSize;
            //scaleFactor = graphScaleFactor;

            outputTexture = new Texture2D((int)size.x, (int)size.y);
            UpdateGraph();

            BebugManagment.AddGraph(this);
        }

        public void ClearGraph()
        {
            points = new List<Vector2>();
        }

        public void AddNewPoint(float x, float y)
        {
            points.Add(new Vector2(x, y));
        }

        public void UpdateGraph()
        {
            Vector2Int dimensions = new Vector2Int((int)size.x, (int)size.y);

            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    outputTexture.SetPixel(x, y, Color.black);
                }
            }

            AddNewPoint(0, 0);

            //Compute all positions we need to draw a pixel
            List<Vector2Int> pixelPositions = new List<Vector2Int>();

            //Find the minimum and maximum positions in each axis
            float xMax = Mathf.NegativeInfinity;
            float xMin = Mathf.Infinity;

            float yMax = Mathf.NegativeInfinity;
            float yMin = Mathf.Infinity;

            foreach (Vector2 point in points)
            {
                if (point.x > xMax)
                {
                    xMax = point.x;
                }
                if (point.x < xMin)
                {
                    xMin = point.x;
                }

                if (point.y > yMax)
                {
                    yMax = point.y;
                }
                if (point.y < yMin)
                {
                    yMin = point.y;
                }
            }

            //Get the scale of the graph
            float xDifference = xMax - xMin;
            float yDifference = yMax - yMin;

            //Get the conversion in scale from graph to output texture in both axis and get the minimum one
            float minScaleFactor = Mathf.Min(dimensions.x / (xDifference), dimensions.y / (yDifference));

            //Get the absolute value of both minimums
            float yMinAbs = Mathf.Abs(yMin);
            float xMinAbs = Mathf.Abs(xMin);

            //Apply all points on graph, first getting them in texture space (relative to bottom left) then scale them by the scale factor
            for (int i = 0; i < points.Count; i++)
            {
                Vector2Int newPixPos = new Vector2Int((int)((points[i].x + xMinAbs) * minScaleFactor), (int)((points[i].y + yMinAbs) * minScaleFactor));
                pixelPositions.Add(newPixPos);
            }

            //Set all pixels in output texture 
            foreach (Vector2Int pos in pixelPositions)
            {
                outputTexture.SetPixel(pos.x, pos.y, Color.white);
            }

            //If we have any negative y values we need to draw the x-axis
            if (yMin <= 0)
            {
                //Draw X-axis
                int xAxisLevel = (int)(yMinAbs * minScaleFactor);
                for (int x = 0; x < dimensions.x; x++)
                {
                    outputTexture.SetPixel(x, xAxisLevel, Color.green);
                }


            }

            //If we have any negative x values we need to draw the y-axis
            if (xMin <= 0)
            {
                //Draw Y-axis
                int yAxisColumn = (int)(xMinAbs * minScaleFactor);
                for (int y = 0; y < dimensions.y; y++)
                {
                    outputTexture.SetPixel(yAxisColumn, y, Color.green);
                }
            }

            //Apply to texture
            outputTexture.Apply();
        }

        public void DrawGraph()
        {
            Rect graphRect = new Rect(screenPosition.x, screenPosition.y, size.x, size.y);

            GUI.Box(graphRect, outputTexture);
        }
    }
}