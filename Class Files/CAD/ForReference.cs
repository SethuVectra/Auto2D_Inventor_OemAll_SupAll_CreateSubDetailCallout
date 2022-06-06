using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CAD
{
    class ForReference
    {
        //private double GetCurveLength(Curve2dEvaluator Eval)
        //{
        //    var minParam = default(double);
        //    var maxParam = default(double);
        //    var curveLength = default(double);
        //    Eval.GetParamExtents(out minParam, out maxParam);
        //    Eval.GetLengthAtParam(minParam, maxParam, out curveLength);
        //    return curveLength;
        //}

        //private void ArrangeBalloonsOnView(DrawingView View)
        //{
        //    var leftBalloon = new List<Balloon>();
        //    var rightBalloon = new List<Balloon>();
        //    var topBalloon = new List<Balloon>();
        //    var btmBalloon = new List<Balloon>();
        //    foreach (Balloon balloon in View.Parent.Balloons)
        //    {
        //        if (object.ReferenceEquals(balloon.ParentView, View))
        //        {
        //            if (balloon.Position.X == View.Left - _blnViewMargin)
        //            {
        //                leftBalloon.Add(balloon);
        //            }
        //            else if (balloon.Position.X == View.Left + View.Width + _blnViewMargin)
        //            {
        //                rightBalloon.Add(balloon);
        //            }
        //            else if (balloon.Position.Y == View.Top + _blnViewMargin)
        //            {
        //                topBalloon.Add(balloon);
        //            }
        //            else if (balloon.Position.Y == View.Top - View.Height - _blnViewMargin)
        //            {
        //                btmBalloon.Add(balloon);
        //            }
        //        }
        //    }

        //    if (leftBalloon.Count > 1)
        //        ArrangeBalloonVerticaly(leftBalloon);
        //    if (rightBalloon.Count > 1)
        //        ArrangeBalloonVerticaly(rightBalloon);
        //    if (topBalloon.Count > 1)
        //        ArrangeBalloonHorizontaly(topBalloon);
        //    if (btmBalloon.Count > 1)
        //        ArrangeBalloonHorizontaly(btmBalloon);
        //}

        //private void ArrangeBalloonVerticaly(List<Balloon> ballons)
        //{
        //    ballons.Sort((x, y) => y.Position.Y.CompareTo(x.Position.Y));
        //    for (int index = 1, loopTo = ballons.Count - 1; index <= loopTo; index++)
        //    {
        //        if (ballons[index - 1].Position.Y - ballons[index].Position.Y < _blnVerticalOffset)
        //        {
        //            Point2d NewPos = ballons[index].Position.Copy();
        //            NewPos.Y = ballons[index - 1].Position.Y - _blnVerticalOffset;
        //            ballons[index].Position = NewPos;
        //        }
        //    }
        //}

        //private void ArrangeBalloonHorizontaly(List<Balloon> ballons)
        //{
        //    ballons.Sort((x, y) => y.Position.X.CompareTo(x.Position.X));
        //    for (int index = 1, loopTo = ballons.Count - 1; index <= loopTo; index++)
        //    {
        //        if (ballons[index - 1].Position.X - ballons[index].Position.X < _blnHorizontalOffset)
        //        {
        //            Point2d NewPos = ballons[index].Position.Copy();
        //            NewPos.X = ballons[index - 1].Position.X - _blnHorizontalOffset;
        //            ballons[index].Position = NewPos;
        //        }
        //    }
        //}

        //private DrawingCurveSegment GetBestSegmentFromOccurrence(List<DrawingCurve> OccurrenceCurves)
        //{
        //    double bestRating = 0d;
        //    DrawingCurveSegment bestSegment = default;
        //    foreach (DrawingCurve curve in OccurrenceCurves)
        //    {
        //        var rating = default(double);
        //        var bestInCurve = GetBestSegmentFromCurve(curve, ref rating);
        //        if (rating > bestRating)
        //        {
        //            bestSegment = bestInCurve;
        //            bestRating = rating;
        //        }
        //    }

        //    return bestSegment;
        //}

        //private DrawingCurveSegment GetBestSegmentFromCurve(DrawingCurve Curve, ref double segmentRating)
        //{
        //    DrawingCurveSegment bestSegment = default;
        //    segmentRating = 0d;
        //    foreach (DrawingCurveSegment segment in Curve.Segments)
        //    {
        //        double segLength = GetSegmentLength(segment);
        //        var segmentPoints = SplitSegment(segment, 10);
        //        double closestDist = double.PositiveInfinity;
        //        double distanceSum = 0d;
        //        foreach (Point2d point in segmentPoints)
        //        {
        //            double pointDist = DistanceToViewRangeBox(point, Curve.Parent);
        //            if (pointDist < closestDist)
        //                closestDist = pointDist;
        //            distanceSum += pointDist;
        //        }

        //        double avgDistance = distanceSum / segmentPoints.Count;
        //        double rating = segLength; // / (avgDistance * closestDist ^ 2)
        //        if (Curve.EdgeType == DrawingEdgeTypeEnum.kTangentEdge)
        //            rating /= 10d;
        //        if (rating > segmentRating)
        //        {
        //            bestSegment = segment;
        //            segmentRating = rating;
        //        }
        //    }

        //    return bestSegment;
        //}

        //private List<Point2d> SplitSegment(DrawingCurveSegment Segment, int SplitPrecision)
        //{
        //    var pointList = new List<Point2d>();
        //    double MinParam = default, MaxParam = default;
        //    TransientGeometry TransGeom;
        //    TransGeom = Application.TransientGeometry;
        //    Segment.Geometry.Evaluator.GetParamExtents(MinParam, MaxParam);
        //    for (int i = 0, loopTo = SplitPrecision; i <= loopTo; i++)
        //    {
        //        try
        //        {
        //            var pCoordinate = new double[2];
        //            Segment.Geometry.Evaluator.GetPointAtParam(new[] { MinParam + i * (MaxParam - MinParam) / SplitPrecision }, pCoordinate);
        //            pointList.Add(TransGeom.CreatePoint2d(pCoordinate[0], pCoordinate[1]));
        //        }
        //        catch
        //        {
        //            return new List<Point2d>();
        //        }
        //    }

        //    return pointList;
        //}

        //private double DistanceToViewRangeBox(Point2d TestPoint, DrawingView DView)
        //{
        //    double shortest = _topLine.DistanceTo(TestPoint);
        //    double dist = _btmLine.DistanceTo(TestPoint);
        //    if (dist < shortest)
        //        shortest = dist;
        //    dist = _leftLine.DistanceTo(TestPoint);
        //    if (dist < shortest)
        //        shortest = dist;
        //    dist = _leftLine.DistanceTo(TestPoint);
        //    if (dist < shortest)
        //        shortest = dist;
        //    return shortest;
        //}

        //private double GetSegmentLength(DrawingCurveSegment Segment)
        //{
        //    switch (Segment.GeometryType)
        //    {
        //        case var @case when @case == Curve2dTypeEnum.kLineCurve2d:
        //            {
        //                return Segment.EndPoint.DistanceTo(Segment.StartPoint);
        //            }

        //        default:
        //            {
        //                return GetCurveLength(Segment.Geometry.Evaluator);
        //            }
        //    }
        //}
    }
}
