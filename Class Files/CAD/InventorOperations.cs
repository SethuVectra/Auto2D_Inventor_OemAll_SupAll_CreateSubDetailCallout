using Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig;
using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CAD
{
    public class InventorOperations : ICADOperations
    {
        string ViewName = "B_LCS";
        private Inventor.Application Application;
        private PartsListLevelEnum PartsListLevelEnum;
        private TransientGeometry TransGeom;

        public bool DeletePartList()
        {
            try
            {
                foreach (PartsList item in ActiveSheet.PartsLists)
                    item.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Sheet ActiveSheet =>
             (Application.ActiveDocument as DrawingDocument).ActiveSheet;

        public List<string> GetDrawingFilePaths()
        {
            throw new NotImplementedException();
        }

        public object GetView()
        {
            if (Application == null) return null;
            foreach (DrawingView item in ActiveSheet.DrawingViews)
            {
                if (item.Name == ViewName) return item;
            }
            return null;
        }

        public bool PlaceBalloonCallouts(object PartList, VctBalloons VctBalloons)
        {
            StartApplication();
            DrawingView View = (DrawingView)GetView();
            AddBallonToView(View);
            foreach (VctBalloon VctBalloon in VctBalloons.BalloonCallouts)
            {
                ObjectCollection PointCollection = Application.TransientObjects.CreateObjectCollection();
                PointCollection.Add(VctBalloon.LeaderPoint);
                PointCollection.Add(ActiveSheet.CreateGeometryIntent(VctBalloon.DrawingCurve));
                Balloon balloon = ActiveSheet.Balloons.Add(PointCollection);
            }
            return true;
        }

        public object PlacePartList()
        {
            return ActiveSheet.PartsLists.Add(GetView(), Application.TransientGeometry.CreatePoint2d(ActiveSheet.Width - 1, ActiveSheet.Height - 1));
        }

        public bool StartApplication()
        {
            try
            {
                Application = System.Runtime.InteropServices.Marshal.GetActiveObject("Inventor.Application") as Inventor.Application;
            }
            catch
            {
                try
                {
                    Application = System.Activator.CreateInstance(System.Type.GetTypeFromProgID("Inventor.Application")) as Inventor.Application;
                }
                catch
                {
                    return false;
                }

            }
            if (Application != null)
                TransGeom = Application.TransientGeometry;
            return true;
        }



        public bool UpdateSettings(InputType CalloutDetails)
        {
            Inventor.Document oDocument = null;
            AssemblyDocument oAssemblyDocument = oDocument as AssemblyDocument;
            if (oAssemblyDocument == null) return false;

            TableSettings tableSettings = CalloutDetails.TableSettings;
            BOM oBOM = oAssemblyDocument.ComponentDefinition.BOM;

            if (tableSettings.BOMView == "Parts Only")
            {
                oBOM.PartsOnlyViewEnabled = true;
                oBOM.PartsOnlyViewNumberingScheme = tableSettings.Numbering == "Numeric" ? NumberingSchemeEnum.kNumericNumbering : NumberingSchemeEnum.kUppercaseAlphaNumbering;
                oBOM.PartsOnlyViewMinimumDigits = tableSettings.MinDigits;
                PartsListLevelEnum = PartsListLevelEnum.kPartsOnly;
            }
            else if (tableSettings.BOMView == "Structured")
            {
                oBOM.StructuredViewEnabled = true;
                oBOM.StructuredViewFirstLevelOnly = tableSettings.Levels == "First Level";
                oBOM.StructuredViewMinimumDigits = tableSettings.MinDigits;
                if (tableSettings.Levels == "First Level")
                    PartsListLevelEnum = PartsListLevelEnum.kStructured;
                else
                    PartsListLevelEnum = PartsListLevelEnum.kStructuredAllLevels;
            }

            DrawingDocument oDrawingDocument = oDocument as DrawingDocument;
            if (oDrawingDocument == null) return false;

            PartsList Partlist = oDrawingDocument.Sheets[0].PartsLists[0];

            //if (tableSettings.BOMView == "Parts Only")
            //{
            //    //Partlist.Level = PartsListLevelEnum.kPartsOnly;                
            //}

            bool IsAutoWrap = Convert.ToBoolean(tableSettings.AutomaticWrap);
            Partlist.WrapAutomatically = IsAutoWrap;
            Partlist.WrapLeft = tableSettings.TextWrapingDirection == "Left";
            if (IsAutoWrap)
            {
                Partlist.MaximumRows = tableSettings.MaximumRows;
                Partlist.NumberOfSections = tableSettings.NumberOfSections;
            }

            return true;
        }

        #region Auto Balloon
        private double _blnViewMargin = 2d;
        private double _blnVerticalOffset = 0.9d;
        private double _blnHorizontalOffset = 1d;
        private LineSegment2d _topLine { get; set; }
        private LineSegment2d _btmLine { get; set; }
        private LineSegment2d _rightLine { get; set; }
        private LineSegment2d _leftLine { get; set; }

        private Vector2d X_axis { get; set; }

        internal void AddBallonToView(DrawingView View)
        {
            DeletePartList();
            PlacePartList();
            PartsList PartsList = View.Parent.PartsLists[1];
            InitialiseViewBoundingBox(View);
            DeleteBalloons(View);
            foreach (PartsListRow row in PartsList.PartsListRows)
            {
                if (!row.Ballooned)
                    CreateRowItemBalloon(row, View);
            }

            ArrangeBalloonsOnView(View);
        }

        private void InitialiseViewBoundingBox(DrawingView DView)
        {
            double Offset = 1;
            Point2d TopLeft = TransGeom.CreatePoint2d(DView.Left - Offset, DView.Top + Offset);
            Point2d TopRight = TransGeom.CreatePoint2d(DView.Left + DView.Width + Offset, DView.Top + Offset);
            Point2d BtmLeft = TransGeom.CreatePoint2d(DView.Left - Offset, DView.Top - DView.Height - Offset);
            Point2d BtmRight = TransGeom.CreatePoint2d(DView.Left + DView.Width + Offset, DView.Top - DView.Height - Offset);

            _topLine = TransGeom.CreateLineSegment2d(TopLeft, TopRight);
            _btmLine = TransGeom.CreateLineSegment2d(BtmLeft, BtmRight);
            _leftLine = TransGeom.CreateLineSegment2d(TopLeft, BtmLeft);
            _rightLine = TransGeom.CreateLineSegment2d(TopRight, BtmRight);

            X_axis = DView.Center.VectorTo(TransGeom.CreatePoint2d(DView.Left + DView.Width + Offset - DView.Center.Y));
        }

        private void DeleteBalloons(DrawingView View)
        {
            foreach (Balloon Balloon in View.Parent.Balloons)
            {
                Balloon.Delete();
            }
        }

        private void CreateRowItemBalloon(PartsListRow Item, DrawingView View)
        {
            GeometryIntent CurveAttachPt = GetBalloonAttachGeometry(Item, View);
            if (CurveAttachPt is null)
                return;
            Point2d BalloonPositionPt = GetBalloonPosition(CurveAttachPt.PointOnSheet, View);
            ObjectCollection LeaderPoints = Application.TransientObjects.CreateObjectCollection();

            LeaderPoints.Add(BalloonPositionPt);
            LeaderPoints.Add(CurveAttachPt);

            Balloon balloon= View.Parent.Balloons.Add(LeaderPoints);
            UpdateBalloonItem(balloon);

        }

        int overridevalue = 1;
        public void UpdateBalloonItem(Balloon balloon)
        {
            BalloonValueSet balloonValueSet = balloon.BalloonValueSets[1];
            balloonValueSet.ReferencedRow.BOMRow.ItemNumber = overridevalue.ToString();
            //balloonValueSet.OverrideValue = overridevalue.ToString();
            overridevalue++;
        }

        private void ArrangeBalloonsOnView(DrawingView View)
        {
            var leftBalloon = new List<Balloon>();
            var rightBalloon = new List<Balloon>();
            var topBalloon = new List<Balloon>();
            var btmBalloon = new List<Balloon>();
            foreach (Balloon balloon in View.Parent.Balloons)
            {
                if (object.ReferenceEquals(balloon.ParentView, View))
                {
                    if (balloon.Position.X == View.Left - _blnViewMargin)
                    {
                        leftBalloon.Add(balloon);
                    }
                    else if (balloon.Position.X == View.Left + View.Width + _blnViewMargin)
                    {
                        rightBalloon.Add(balloon);
                    }
                    else if (balloon.Position.Y == View.Top + _blnViewMargin)
                    {
                        topBalloon.Add(balloon);
                    }
                    else if (balloon.Position.Y == View.Top - View.Height - _blnViewMargin)
                    {
                        btmBalloon.Add(balloon);
                    }
                }
            }

            if (leftBalloon.Count > 1)
                ArrangeBalloonVerticaly(leftBalloon);
            if (rightBalloon.Count > 1)
                ArrangeBalloonVerticaly(rightBalloon);
            if (topBalloon.Count > 1)
                ArrangeBalloonHorizontaly(topBalloon);
            if (btmBalloon.Count > 1)
                ArrangeBalloonHorizontaly(btmBalloon);
        }

        private void ArrangeBalloonVerticaly(List<Balloon> ballons)
        {
            ballons.Sort((x, y) => y.Position.Y.CompareTo(x.Position.Y));
            for (int index = 1, loopTo = ballons.Count - 1; index <= loopTo; index++)
            {
                if (ballons[index - 1].Position.Y - ballons[index].Position.Y < _blnVerticalOffset)
                {
                    Point2d NewPos = ballons[index].Position.Copy();
                    NewPos.Y = ballons[index - 1].Position.Y - _blnVerticalOffset;
                    ballons[index].Position = NewPos;
                }
            }
        }

        private void ArrangeBalloonHorizontaly(List<Balloon> ballons)
        {
            ballons.Sort((x, y) => y.Position.X.CompareTo(x.Position.X));
            for (int index = 1, loopTo = ballons.Count - 1; index <= loopTo; index++)
            {
                if (ballons[index - 1].Position.X - ballons[index].Position.X < _blnHorizontalOffset)
                {
                    Point2d NewPos = ballons[index].Position.Copy();
                    NewPos.X = ballons[index - 1].Position.X - _blnHorizontalOffset;
                    ballons[index].Position = NewPos;
                }
            }
        }

        private Point2d GetBalloonPosition(Point2d AttachPoint, DrawingView View)
        {
            Point2d leaderPoint = AttachPoint.Copy();
            double translationRatio;

            Vector2d SecondAxis = View.Center.VectorTo(leaderPoint);
            double Angle_Radial = X_axis.AngleTo(SecondAxis);
            double Angle = Angle_Radial * (180 / Math.PI);

            string Quadrant = GetQuadrant(leaderPoint, View);

            LineSegment2d BalloonPtLine = null;

            if (Quadrant == "Top")
            {
                BalloonPtLine = _topLine;
            }
            else if (Quadrant == "Left")
            {
                BalloonPtLine = _leftLine;
            }
            else if (Quadrant == "Bottom")
            {
                BalloonPtLine = _btmLine;
            }
            else if (Quadrant == "Right")
            {
                BalloonPtLine = _rightLine;
            }
            //if (Angle >= 45 && Angle < 135)
            //{
            //    BalloonPtLine = _topLine;
            //}
            //else if (Angle >= 135 && Angle < 225)
            //{
            //    BalloonPtLine = _leftLine;
            //}
            //else if (Angle >= 225 && Angle < 315)
            //{
            //    BalloonPtLine = _btmLine;
            //}
            //else
            //{
            //    BalloonPtLine = _rightLine;
            //}


            // Line AB represented as a1x + b1y = c1 
            double a1 = BalloonPtLine.EndPoint.Y - BalloonPtLine.StartPoint.Y;
            double b1 = BalloonPtLine.StartPoint.X - BalloonPtLine.EndPoint.X;
            double c1 = a1 * (BalloonPtLine.StartPoint.X) + b1 * (BalloonPtLine.StartPoint.Y);

            // Line CD represented as a2x + b2y = c2 
            double a2 = leaderPoint.Y - View.Center.Y;
            double b2 = View.Center.X - leaderPoint.X;
            double c2 = a2 * (View.Center.X) + b2 * (View.Center.Y);

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                // The lines are parallel. This is simplified 
                // by returning a pair of FLT_MAX 
                return null;
            }
            else
            {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                return TransGeom.CreatePoint2d(x, y);
            }


            switch (GetQuadrant(AttachPoint, View))
            {
                case "Top":
                    {
                        leaderPoint.Y = View.Top + _blnViewMargin;
                        translationRatio = (leaderPoint.Y - View.Center.Y) / (AttachPoint.Y - View.Center.Y);
                        leaderPoint.X = View.Center.X + (AttachPoint.X - View.Center.X) * translationRatio;
                        break;
                    }

                case "Bottom":
                    {
                        leaderPoint.Y = View.Top - View.Height - _blnViewMargin;
                        translationRatio = (leaderPoint.Y - View.Center.Y) / (AttachPoint.Y - View.Center.Y);
                        leaderPoint.X = View.Center.X + (AttachPoint.X - View.Center.X) * translationRatio;
                        break;
                    }

                case "Left":
                    {
                        leaderPoint.X = View.Left - _blnViewMargin;
                        translationRatio = (leaderPoint.X - View.Center.X) / (AttachPoint.X - View.Center.X);
                        leaderPoint.Y = View.Center.Y + (AttachPoint.Y - View.Center.Y) * translationRatio;
                        break;
                    }

                case "Right":
                    {
                        leaderPoint.X = View.Left + View.Width + _blnViewMargin;
                        translationRatio = (leaderPoint.X - View.Center.X) / (AttachPoint.X - View.Center.X);
                        leaderPoint.Y = View.Center.Y + (AttachPoint.Y - View.Center.Y) * translationRatio;
                        break;
                    }

                case "Quadrant not Found":
                    {
                        return default;
                    }
            }

            return leaderPoint;
        }

        private string GetQuadrant(Point2d AttachPoint, DrawingView View)
        {
            double CornerAngle = Math.Atan2(View.Height, View.Width);
            double PointAngle = Math.Atan2(AttachPoint.Y - View.Center.Y, AttachPoint.X - View.Center.X);
            if (PointAngle < CornerAngle && PointAngle > -CornerAngle)
                return "Right";
            if (PointAngle > CornerAngle && PointAngle < Math.PI - CornerAngle)
                return "Top";
            if (PointAngle > Math.PI - CornerAngle | PointAngle < -Math.PI + CornerAngle)
                return "Left";
            if (PointAngle > -Math.PI + CornerAngle && PointAngle < -CornerAngle)
                return "Bottom";
            return "Quadrant not Found";
        }

        private GeometryIntent GetBalloonAttachGeometry(PartsListRow Item, DrawingView View)
        {
            ComponentOccurrencesEnumerator itemOccurrences = View.ReferencedDocumentDescriptor.ReferencedDocument.ComponentDefinition.Occurrences.AllReferencedOccurrences(Item.ReferencedFiles[1].DocumentDescriptor);
            List<DrawingCurve> OccurrencesCurves = GetCurvesFromOcc(itemOccurrences, View);
            return GetAttachPoint(GetPheriperalCurves(OccurrencesCurves));
            //return GetAttachPoint(GetBestSegmentFromOccurrence(OccurrencesCurves));
            return null;
        }
        #endregion
        #region Curves analysis methods
        private List<DrawingCurve> GetCurvesFromOcc(ComponentOccurrencesEnumerator Occurrences, DrawingView View)
        {
            var Curves = new List<DrawingCurve>();
            foreach (ComponentOccurrence occ in Occurrences)
            {
                if (!occ.Suppressed)
                {
                    foreach (DrawingCurve curve in View.DrawingCurves[occ])
                    {
                        if (!Curves.Contains(curve))
                            Curves.Add(curve);
                    }
                }
            }

            return Curves;
        }

        private Dictionary<Point2d, DrawingCurveSegment> GetPheriperalSegments(List<DrawingCurve> occurrenceCurves, bool ConsiderAllSegments = false)
        {
            Dictionary<Point2d, DrawingCurveSegment> PheriperalSegments = new Dictionary<Point2d, DrawingCurveSegment>();

            foreach (DrawingCurve Curve in occurrenceCurves)
            {
                foreach (DrawingCurveSegment Segment in Curve.Segments)
                {
                    Sheet DrawingSheet = Segment.Parent.Parent.Parent;
                    if (ConsiderAllSegments)
                    {
                        if (Segment.GeometryType == Curve2dTypeEnum.kCircleCurve2d ||
                            Segment.GeometryType == Curve2dTypeEnum.kEllipseFullCurve2d)
                        {
                            PheriperalSegments.Add(
                                DrawingSheet
                                    .CreateGeometryIntent(Segment.Parent, PointIntentEnum.kCircularTopPointIntent)
                                    .PointOnSheet, Segment);
                            PheriperalSegments.Add(
                                DrawingSheet
                                    .CreateGeometryIntent(Segment.Parent, PointIntentEnum.kCircularBottomPointIntent)
                                    .PointOnSheet, Segment);
                            PheriperalSegments.Add(
                                DrawingSheet
                                    .CreateGeometryIntent(Segment.Parent, PointIntentEnum.kCircularLeftPointIntent)
                                    .PointOnSheet, Segment);
                            PheriperalSegments.Add(
                                DrawingSheet
                                    .CreateGeometryIntent(Segment.Parent, PointIntentEnum.kCircularRightPointIntent)
                                    .PointOnSheet, Segment);
                        }
                        else if (Segment.GeometryType == Curve2dTypeEnum.kEllipticalArcCurve2d)
                        {
                            PheriperalSegments.Add(GetSegmentMidPoint(Segment), Segment);
                        }
                        else
                        {
                            dynamic Geometry = Segment.Geometry;
                            PheriperalSegments.Add(Geometry.MidPoint, Segment);
                        }
                    }
                    else if (Segment.GeometryType == Curve2dTypeEnum.kLineSegmentCurve2d && DistancebtTwoPoints(Segment) > 0.5)
                    {
                        dynamic Geometry = Segment.Geometry;
                        PheriperalSegments.Add(Geometry.MidPoint, Segment);
                    }
                }
            }
            return PheriperalSegments;
        }

        private double DistancebtTwoPoints(DrawingCurveSegment drawingCurve)
        {
            if (drawingCurve.GeometryType == Curve2dTypeEnum.kLineSegmentCurve2d)
                return DistancebtTwoPoints(drawingCurve.Geometry.StartPoint, drawingCurve.Geometry.EndPoint);
            return 0;
        }
        private double DistancebtTwoPoints(Point2d Startpoint, Point2d Endpoint)
        {
            return Math.Sqrt(Math.Pow((Endpoint.X - Startpoint.X), 2) + Math.Pow((Endpoint.Y - Startpoint.Y), 2));
        }
        private DrawingCurveSegment GetPheriperalCurves(List<DrawingCurve> OccurrenceCurves)
        {
            DrawingCurveSegment result;
            Dictionary<Point2d, DrawingCurveSegment> AllSegments = new Dictionary<Point2d, DrawingCurveSegment>();
            AllSegments = GetPheriperalSegments(OccurrenceCurves);
            if (AllSegments.Count == 0)
                AllSegments = GetPheriperalSegments(OccurrenceCurves, true);

            double MinimumValue;
            var Tempresult = AllSegments.OrderBy(a => a.Key.X).FirstOrDefault();
            result = Tempresult.Value;
            MinimumValue = Tempresult.Key.X - _topLine.StartPoint.X;

            Tempresult = AllSegments.OrderBy(a => a.Key.Y).FirstOrDefault();
            if (_topLine.StartPoint.Y - Tempresult.Key.Y < MinimumValue)
            {
                result = Tempresult.Value;
                MinimumValue = _topLine.StartPoint.Y - Tempresult.Key.Y;
            }

            Tempresult = AllSegments.OrderByDescending(a => a.Key.X).FirstOrDefault();
            if (_btmLine.EndPoint.X - Tempresult.Key.X < MinimumValue)
            {
                result = Tempresult.Value;
                MinimumValue = _btmLine.EndPoint.X - Tempresult.Key.X;
            }

            Tempresult = AllSegments.OrderByDescending(a => a.Key.Y).FirstOrDefault();
            if (Tempresult.Key.Y - _btmLine.EndPoint.Y < MinimumValue)
                result = Tempresult.Value;
            
            return result;

        }

        private DrawingCurveSegment GetBestSegmentFromOccurrence(List<DrawingCurve> OccurrenceCurves)
        {
            double bestRating = 0d;
            DrawingCurveSegment bestSegment = default;
            foreach (DrawingCurve curve in OccurrenceCurves)
            {
                var rating = default(double);
                var bestInCurve = GetBestSegmentFromCurve(curve, ref rating);
                if (rating > bestRating)
                {
                    bestSegment = bestInCurve;
                    bestRating = rating;
                }
            }

            return bestSegment;
        }

        private DrawingCurveSegment GetBestSegmentFromCurve(DrawingCurve Curve, ref double segmentRating)
        {
            DrawingCurveSegment bestSegment = default;
            segmentRating = 0d;
            foreach (DrawingCurveSegment segment in Curve.Segments)
            {
                double segLength = GetSegmentLength(segment);
                var segmentPoints = SplitSegment(segment, 10);
                double closestDist = double.PositiveInfinity;
                double distanceSum = 0d;
                foreach (Point2d point in segmentPoints)
                {
                    double pointDist = DistanceToViewRangeBox(point, Curve.Parent);
                    if (pointDist < closestDist)
                        closestDist = pointDist;
                    distanceSum += pointDist;
                }

                double avgDistance = distanceSum / segmentPoints.Count;
                double rating = segLength; // / (avgDistance * closestDist ^ 2)
                if (Curve.EdgeType == DrawingEdgeTypeEnum.kTangentEdge)
                    rating /= 10d;
                if (rating > segmentRating)
                {
                    bestSegment = segment;
                    segmentRating = rating;
                }
            }

            return bestSegment;
        }

        private List<Point2d> SplitSegment(DrawingCurveSegment Segment, int SplitPrecision)
        {
            var pointList = new List<Point2d>();
            double MinParam = default, MaxParam = default;
            TransientGeometry TransGeom;
            TransGeom = Application.TransientGeometry;
            Segment.Geometry.Evaluator.GetParamExtents(MinParam, MaxParam);
            for (int i = 0, loopTo = SplitPrecision; i <= loopTo; i++)
            {
                try
                {
                    var pCoordinate = new double[2];
                    Segment.Geometry.Evaluator.GetPointAtParam(new[] { MinParam + i * (MaxParam - MinParam) / SplitPrecision }, pCoordinate);
                    pointList.Add(TransGeom.CreatePoint2d(pCoordinate[0], pCoordinate[1]));
                }
                catch (Exception e)
                {
                }

            }

            return pointList;
        }

        private double DistanceToViewRangeBox(Point2d TestPoint, DrawingView DView)
        {
            double shortest = _topLine.DistanceTo(TestPoint);
            double dist = _btmLine.DistanceTo(TestPoint);
            if (dist < shortest)
                shortest = dist;
            dist = _leftLine.DistanceTo(TestPoint);
            if (dist < shortest)
                shortest = dist;
            dist = _leftLine.DistanceTo(TestPoint);
            if (dist < shortest)
                shortest = dist;
            return shortest;
        }

        private double GetSegmentLength(DrawingCurveSegment Segment)
        {
            switch (Segment.GeometryType)
            {
                case var @case when @case == Curve2dTypeEnum.kLineCurve2d:
                    {
                        return Segment.EndPoint.DistanceTo(Segment.StartPoint);
                    }

                default:
                    {
                        return GetCurveLength(Segment.Geometry.Evaluator);
                    }
            }
        }

        private Point2d GetSegmentMidPoint(DrawingCurveSegment Segment)
        {
            Curve2dEvaluator CurveEval = Segment.Geometry.Evaluator;
            double minParam, maxParam, curveLength;
            double[] midParam = new double[1], midPointCoordinates = new double[2];
            CurveEval.GetParamExtents(out minParam, out maxParam);
            CurveEval.GetLengthAtParam(minParam, maxParam, out curveLength);
            CurveEval.GetParamAtLength(minParam, curveLength / 2d, out midParam[0]);
            CurveEval.GetPointAtParam(ref midParam, ref midPointCoordinates);
            return Application.TransientGeometry.CreatePoint2d(midPointCoordinates[0], midPointCoordinates[1]);
        }

        private double GetCurveLength(Curve2dEvaluator Eval)
        {
            var minParam = default(double);
            var maxParam = default(double);
            var curveLength = default(double);
            Eval.GetParamExtents(out minParam, out maxParam);
            Eval.GetLengthAtParam(minParam, maxParam, out curveLength);
            return curveLength;
        }

        private GeometryIntent GetAttachPoint(DrawingCurveSegment Segment)
        {
            if (Segment is null)
                return default;
            Sheet DrawingSheet = Segment.Parent.Parent.Parent;
            if (Segment.GeometryType == Curve2dTypeEnum.kCircleCurve2d | Segment.GeometryType == Curve2dTypeEnum.kEllipseFullCurve2d)
            {
                switch (GetQuadrant(Segment.Geometry.Center, Segment.Parent.Parent))
                {
                    case "Top":
                        {
                            return DrawingSheet.CreateGeometryIntent(Segment.Parent, PointIntentEnum.kCircularTopPointIntent);
                        }

                    case "Bottom":
                        {
                            return DrawingSheet.CreateGeometryIntent(Segment.Parent, PointIntentEnum.kCircularBottomPointIntent);
                        }

                    case "Left":
                        {
                            return DrawingSheet.CreateGeometryIntent(Segment.Parent, PointIntentEnum.kCircularLeftPointIntent);
                        }

                    case "Right":
                        {
                            return DrawingSheet.CreateGeometryIntent(Segment.Parent, PointIntentEnum.kCircularRightPointIntent);
                        }

                    case "Quadrant not Found":
                        {
                            return DrawingSheet.CreateGeometryIntent(Segment.Parent, PointIntentEnum.kCenterPointIntent);
                        }
                }
            }

            return DrawingSheet.CreateGeometryIntent(Segment.Parent, GetSegmentMidPoint(Segment));
        }
        #endregion
    }
}
