using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CAD
{
    public class PeripheralCurves
    {
        public List<PeripheralCurve> Items { get; set; }

        public List<Point2d> ViewCorners { get; set; }

        private double _x1, _x2, _y1, _y2;

        public PeripheralCurves(LineSegment2d topSegment, LineSegment2d bottomSegment)
        {
            Items = new List<PeripheralCurve>();
            ViewCorners = new List<Point2d>
            {
                topSegment.StartPoint,
                topSegment.EndPoint,
                bottomSegment.StartPoint,
                bottomSegment.EndPoint
            };
            _x1 = topSegment.StartPoint.X;
            _x2 = topSegment.EndPoint.X;
            _y1 = topSegment.StartPoint.Y;
            _y2 = topSegment.EndPoint.Y;
        }

        public void Add(Dictionary<Point2d, DrawingCurveSegment> curveSegments)
        {
            //Nearest to the rectangular lines
            AddSegments(curveSegments.OrderBy(a => a.Key.X).ToDictionary(a => a.Key, a => a.Value), _x1);
            AddSegments(curveSegments.OrderByDescending(a => a.Key.X).ToDictionary(a => a.Key, a => a.Value), _x2);
            AddSegments(curveSegments.OrderBy(a => a.Key.Y).ToDictionary(a => a.Key, a => a.Value), _y1,false);
            AddSegments(curveSegments.OrderByDescending(a => a.Key.Y).ToDictionary(a => a.Key, a => a.Value), _y2,false);

            //Nearest to Corner points
            foreach (var item in ViewCorners)
            {
                AddCorner(curveSegments, item);
            }
        }

        private void AddSegments(Dictionary<Point2d, DrawingCurveSegment> curveSegments, double target, bool xDirection = true)
        {
            PeripheralCurve peripheralCurve = new PeripheralCurve
            {
                AttachPoint = curveSegments.First().Key,
                Segment = curveSegments.First().Value,
                Distance = xDirection
                    ? Math.Abs(target - curveSegments.First().Key.X)
                    : Math.Abs(target - curveSegments.First().Key.Y)
            };
            Items.Add(peripheralCurve);
        }

        private void AddCorner(Dictionary<Point2d, DrawingCurveSegment> curveSegments, Point2d cornerPt)
        {
            double minDistance = 0;
            DrawingCurveSegment drawingCurve = null;
            Point2d attachPoint = null;

            foreach (var item in curveSegments)
            {
                double mindist = cornerPt.DistanceTo(item.Key);
                if (minDistance == 0 || mindist < minDistance)
                {
                    minDistance = mindist;
                    drawingCurve = item.Value;
                    attachPoint = item.Key;
                }
            }

            PeripheralCurve peripheralCurve = new PeripheralCurve
            {
                AttachPoint = attachPoint,
                Segment = drawingCurve,
                Distance = minDistance
            };
            Items.Add(peripheralCurve);
        }
    }
    public class PeripheralCurve
    {
        public double Distance { get; set; }
        public Point2d AttachPoint { get; set; }
        public DrawingCurveSegment Segment { get; set; }
    }
}
