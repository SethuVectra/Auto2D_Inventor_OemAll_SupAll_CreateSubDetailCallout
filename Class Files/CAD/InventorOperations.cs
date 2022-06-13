using Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig;
using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CAD
{
    public class InventorOperations : ICadOperations
    {
        private Inventor.Application _application;
        private PartsListLevelEnum _partsListLevelEnum;
        private TransientGeometry _transGeom;

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
                LogWriter.LogWrite("Unable to Delete Partlist");
                return false;
            }
        }

        private Sheet _activeSheet;
        private Sheet ActiveSheet => _activeSheet ?? (_activeSheet = (_application.ActiveDocument as DrawingDocument).ActiveSheet);

        public List<string> GetDrawingFilePaths(string path)
        {
            List<string> files = new List<string>();
            foreach (string item in System.IO.Directory.GetFiles(path, "*.iam", System.IO.SearchOption.AllDirectories))
            {
                if (IsWeldedAssembly(item))
                    files.Add(item);
            }

            return files;
        }

        public bool IsWeldedAssembly(Document assembly)
        {
            if (assembly is AssemblyDocument)
            {
                if (GetCustomIProperty(assembly, StaticVariables.PartType).Equals(StaticVariables.WeldedAssembly))
                    return true;
                //assembly.Close();
            }

            return false;
        }

        public string GetCustomIProperty(Inventor.Document doc, string propertyName)
        {
            string iPropertyValue = string.Empty;
            // Get the custom property set. 
            // Watch out for the wrapped line.
            Inventor.PropertySet customPropSet;
            customPropSet = doc.PropertySets["Inventor User Defined Properties"];

            // Get the existing property, if it exists.
            Inventor.Property prop = null;
            try
            {
                prop = customPropSet[propertyName];
                iPropertyValue = prop.Value as string;
            }
            catch (Exception ex)
            {
                return iPropertyValue;
            }

            return iPropertyValue;
        }

        public bool IsWeldedAssembly(string filepath)
        {
            Document assembly = _application.Documents.Open(filepath, false);
            return IsWeldedAssembly(assembly);
        }

        private DrawingView _view;
        public DrawingView View
        {
            get
            {
                if (_view == null)
                    foreach (DrawingView item in ActiveSheet.DrawingViews)
                        if (item.Name == StaticVariables.ViewName)
                        {
                            _view = item;
                            if (_view.IsRasterView)
                                _view.IsRasterView = false;
                        }

                if (_view == null)
                {
                    LogWriter.LogWrite("B_LCS View is not placed. Please place view with the name B_LCS before proceed");
                    return null;
                }

                return _view;
            }

        }

        public bool PlaceBalloonCallouts(object partList, VctBalloons vctBalloons)
        {
            //    StartApplication();   

            //    DrawingView view = (DrawingView)GetView();
            //    if (view == null)
            //    {
            //        LogWriter.LogWrite("B_LCS View is not placed. Please place view with the name B_LCS before proceed");
            //        return false;
            //    } 
            //    AddBallonToView(view);
            //    foreach (VctBalloon vctBalloon in vctBalloons.BalloonCallouts)
            //    {
            //        ObjectCollection pointCollection = _application.TransientObjects.CreateObjectCollection();
            //        pointCollection.Add(vctBalloon.LeaderPoint);
            //        pointCollection.Add(ActiveSheet.CreateGeometryIntent(vctBalloon.DrawingCurve));
            //        Balloon balloon = ActiveSheet.Balloons.Add(pointCollection);
            //    }
            return true;
        }

        public object PlacePartList(string partListName, double positionX, double positionY, TableSettings tableSettings)
        {
            if (View == null) return null;
            partsList = ActiveSheet.PartsLists.Add(View, _application.TransientGeometry.CreatePoint2d(positionX, positionY));

            foreach (PartsListStyle item in (ActiveSheet.Parent as DrawingDocument).StylesManager.PartsListStyles)
            {
                if (item.Name.Equals(partListName, StringComparison.InvariantCultureIgnoreCase))
                {
                    partsList.Style = item;
                    break;
                }
            }

            bool isAutoWrap = Convert.ToBoolean(tableSettings.AutomaticWrap);
            partsList.WrapAutomatically = isAutoWrap;
            partsList.WrapLeft = tableSettings.TextWrapingDirection == "Left";
            if (isAutoWrap)
            {
                partsList.MaximumRows = tableSettings.MaximumRows;
                partsList.NumberOfSections = tableSettings.NumberOfSections;
            }

            return partsList;
        }

        public List<string> GetOpenedModels()
        {
            List<string> drawings = new List<string>();
            foreach (_Document document in _application.Documents)
            {
                if (IsWeldedAssembly(document))
                    drawings.Add(document.FullDocumentName);
            }

            return drawings;
        }
        public bool StartApplication()
        {
            try
            {
                _application = System.Runtime.InteropServices.Marshal.GetActiveObject("Inventor.Application") as Inventor.Application;
            }
            catch
            {
                try
                {
                    _application = System.Activator.CreateInstance(System.Type.GetTypeFromProgID("Inventor.Application")) as Inventor.Application;
                }
                catch
                {
                    LogWriter.LogWrite("Unable to Start the Inventor Application");
                    return false;
                }
            }

            if (_application != null)
            {
                _transGeom = _application.TransientGeometry;
                _application.Visible = true;
            }

            return true;
        }
        public bool UpdateSettings(string filepath, InputType calloutDetails)
        {
            Inventor.Document oDocument = _application.Documents.ItemByName[filepath];
            AssemblyDocument oAssemblyDocument = oDocument as AssemblyDocument;
            if (oAssemblyDocument == null) return false;

            TableSettings tableSettings = calloutDetails.TableSettings;
            BOM oBom = oAssemblyDocument.ComponentDefinition.BOM;



            if (tableSettings.BomView == "Parts Only")
            {
                oBom.PartsOnlyViewEnabled = true;
                oBom.PartsOnlyViewNumberingScheme = tableSettings.Numbering == "Numeric" ? NumberingSchemeEnum.kNumericNumbering : NumberingSchemeEnum.kUppercaseAlphaNumbering;
                oBom.PartsOnlyViewMinimumDigits = tableSettings.MinDigits;
                _partsListLevelEnum = PartsListLevelEnum.kPartsOnly;
            }
            else if (tableSettings.BomView == "Structured")
            {
                oBom.StructuredViewEnabled = true;
                oBom.StructuredViewFirstLevelOnly = tableSettings.Levels == "First Level";
                oBom.StructuredViewMinimumDigits = tableSettings.MinDigits;
                if (tableSettings.Levels == "First Level")
                    _partsListLevelEnum = PartsListLevelEnum.kStructured;
                else
                    _partsListLevelEnum = PartsListLevelEnum.kStructuredAllLevels;
            }

            DrawingDocument oDrawingDocument = FindDrawingFile(filepath);

            if (oDrawingDocument == null) return false;

            return true;
        }

        private DrawingDocument FindDrawingFile(string fullFilename)
        {
            DrawingDocument oDrawingDocument = null;
            string path = System.IO.Path.GetFullPath(fullFilename);

            string filename = System.IO.Path.GetFileNameWithoutExtension(fullFilename);
            string drawingFilename = _application.DesignProjectManager.ResolveFile(path, filename + ".dwg");

            if (drawingFilename == "")
            {
                drawingFilename = _application.DesignProjectManager.ResolveFile(path, filename + ".idw");
            }

            if (drawingFilename != "")
            {
                oDrawingDocument = _application.Documents.ItemByName[drawingFilename] as DrawingDocument;
                oDrawingDocument = oDrawingDocument ?? _application.Documents.Open(drawingFilename) as DrawingDocument;
            }
            return oDrawingDocument;
        }


        #region Auto Balloon
        private double _blnViewMargin = 2d;
        private LineSegment2d TopLine { get; set; }
        private LineSegment2d BtmLine { get; set; }
        private LineSegment2d RightLine { get; set; }
        private LineSegment2d LeftLine { get; set; }

        private Vector2d XAxis { get; set; }

        private PartsList partsList;
        public bool IsPartslistPlaced => partsList != null;
        internal List<VctBalloon> AddBallonToView()
        {
            List<VctBalloon> Balloons = new List<VctBalloon>();

            InitialiseViewBoundingBox();
            DeleteBalloons();

            foreach (PartsListRow row in partsList.PartsListRows)
            {
                if (!row.Ballooned)
                {
                    VctBalloon vctBalloon = CreateRowItemBalloon(row);
                    if (vctBalloon != null)
                        Balloons.Add(vctBalloon);
                }
            }
            return Balloons;
        }

        private void InitialiseViewBoundingBox()
        {
            double offset = 1.5;
            Point2d topLeft = _transGeom.CreatePoint2d(View.Left - offset, View.Top + offset);
            Point2d topRight = _transGeom.CreatePoint2d(View.Left + View.Width + offset, View.Top + offset);
            Point2d btmLeft = _transGeom.CreatePoint2d(View.Left - offset, View.Top - View.Height - offset);
            Point2d btmRight = _transGeom.CreatePoint2d(View.Left + View.Width + offset, View.Top - View.Height - offset);

            TopLine = _transGeom.CreateLineSegment2d(topLeft, topRight);
            BtmLine = _transGeom.CreateLineSegment2d(btmLeft, btmRight);
            LeftLine = _transGeom.CreateLineSegment2d(topLeft, btmLeft);
            RightLine = _transGeom.CreateLineSegment2d(topRight, btmRight);

            XAxis = View.Center.VectorTo(_transGeom.CreatePoint2d(View.Left + View.Width + offset - View.Center.Y));
        }

        private void DeleteBalloons()
        {
            foreach (Balloon balloon in View.Parent.Balloons)
            {
                balloon.Delete();
            }
        }

        private VctBalloon CreateRowItemBalloon(PartsListRow item)
        {
            Point2d balloonPositionPt = GetBalloonAttachGeometry(item, View, out GeometryIntent curveAttachPt, out VctBalloonZone quadrant); ;
            ObjectCollection leaderPoints = _application.TransientObjects.CreateObjectCollection();

            if (balloonPositionPt == null) return null;
            leaderPoints.Add(balloonPositionPt);
            leaderPoints.Add(curveAttachPt);

            Balloon balloon = View.Parent.Balloons.Add(leaderPoints);
            //UpdateBalloonItem(balloon);
            BalloonValueSet balloonValueSet = balloon.BalloonValueSets[1];

            return new VctBalloon()
            {
                BalloonPosition = new double[] { balloonPositionPt.X, balloonPositionPt.Y },
                CircleSize = 0,
                Zone = quadrant,
                DrawingCurve = null,
                LeaderPoint = new double[] { curveAttachPt.PointOnSheet.X, curveAttachPt.PointOnSheet.Y },
                PartNumber = balloonValueSet.ItemNumber
            };

        }
        public void UpdateBalloonItem(Balloon balloon)
        {
            BalloonValueSet balloonValueSet = balloon.BalloonValueSets[1];
            //string str = System.IO.Path.GetFileNameWithoutExtension(balloonValueSet.ReferencedRow.BOMRow.ReferencedFileDescriptor.FullFileName);
            //balloonValueSet.ReferencedRow.BOMRow.ItemNumber = str.Substring(str.Length - 1);
            //balloonValueSet.OverrideValue = overridevalue.ToString();
            //overridevalue++;
        }

        private Point2d GetBalloonPosition(Point2d attachPoint, DrawingView view, out VctBalloonZone quadrant)
        {
            Point2d leaderPoint = attachPoint.Copy();
            quadrant = GetQuadrant(leaderPoint, view);

            LineSegment2d balloonPtLine = null;
            if (quadrant == VctBalloonZone.Top)
                balloonPtLine = TopLine;
            else if (quadrant == VctBalloonZone.Left)
                balloonPtLine = LeftLine;
            else if (quadrant == VctBalloonZone.Bottom)
                balloonPtLine = BtmLine;
            else if (quadrant == VctBalloonZone.Right)
                balloonPtLine = RightLine;

            // Line AB represented as a1x + b1y = c1 
            if (balloonPtLine != null)
            {
                double a1 = balloonPtLine.EndPoint.Y - balloonPtLine.StartPoint.Y;
                double b1 = balloonPtLine.StartPoint.X - balloonPtLine.EndPoint.X;
                double c1 = a1 * balloonPtLine.StartPoint.X + b1 * balloonPtLine.StartPoint.Y;

                // Line CD represented as a2x + b2y = c2 
                double a2 = leaderPoint.Y - view.Center.Y;
                double b2 = view.Center.X - leaderPoint.X;
                double c2 = a2 * view.Center.X + b2 * view.Center.Y;

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
                    return _transGeom.CreatePoint2d(x, y);
                }
            }

            #region Getquadrant

            //Vector2d secondAxis = view.Center.VectorTo(leaderPoint);
            //double angleRadial = XAxis.AngleTo(secondAxis);
            //double angle = angleRadial * (180 / Math.PI);
            //switch (GetQuadrant(attachPoint, view))
            //{
            //    case "Top":
            //        {
            //            leaderPoint.Y = view.Top + _blnViewMargin;
            //            translationRatio = (leaderPoint.Y - view.Center.Y) / (attachPoint.Y - view.Center.Y);
            //            leaderPoint.X = view.Center.X + (attachPoint.X - view.Center.X) * translationRatio;
            //            break;
            //        }

            //    case "Bottom":
            //        {
            //            leaderPoint.Y = view.Top - view.Height - _blnViewMargin;
            //            translationRatio = (leaderPoint.Y - view.Center.Y) / (attachPoint.Y - view.Center.Y);
            //            leaderPoint.X = view.Center.X + (attachPoint.X - view.Center.X) * translationRatio;
            //            break;
            //        }

            //    case "Left":
            //        {
            //            leaderPoint.X = view.Left - _blnViewMargin;
            //            translationRatio = (leaderPoint.X - view.Center.X) / (attachPoint.X - view.Center.X);
            //            leaderPoint.Y = view.Center.Y + (attachPoint.Y - view.Center.Y) * translationRatio;
            //            break;
            //        }

            //    case "Right":
            //        {
            //            leaderPoint.X = view.Left + view.Width + _blnViewMargin;
            //            translationRatio = (leaderPoint.X - view.Center.X) / (attachPoint.X - view.Center.X);
            //            leaderPoint.Y = view.Center.Y + (attachPoint.Y - view.Center.Y) * translationRatio;
            //            break;
            //        }

            //    case "Quadrant not Found":
            //        {
            //            return default;
            //        }
            //}
            #endregion
            return leaderPoint;
        }

        private VctBalloonZone GetQuadrant(Point2d attachPoint, DrawingView view)
        {
            double cornerAngle = Math.Atan2(view.Height, view.Width);
            double pointAngle = Math.Atan2(attachPoint.Y - view.Center.Y, attachPoint.X - view.Center.X);
            if (pointAngle < cornerAngle && pointAngle > -cornerAngle)
                return VctBalloonZone.Right;
            if (pointAngle > cornerAngle && pointAngle < Math.PI - cornerAngle)
                return VctBalloonZone.Top;
            if (pointAngle > Math.PI - cornerAngle | pointAngle < -Math.PI + cornerAngle)
                return VctBalloonZone.Left;
            if (pointAngle > -Math.PI + cornerAngle && pointAngle < -cornerAngle)
                return VctBalloonZone.Bottom;
            return VctBalloonZone.NotFound;
        }

        private Point2d GetBalloonAttachGeometry(PartsListRow item, DrawingView view, out GeometryIntent intent, out VctBalloonZone quadrant)
        {
            ComponentOccurrencesEnumerator itemOccurrences = view.ReferencedDocumentDescriptor.ReferencedDocument.ComponentDefinition.Occurrences.AllReferencedOccurrences(item.ReferencedFiles[1].DocumentDescriptor);

            GetPeripheralCurves(GetCurvesFromOcc(itemOccurrences, view));
            if (_peripheralCurves.Items.Count == 0)
            {
                LogWriter.LogWrite("Unable to find the drawing curve for part " + itemOccurrences[1].Name);
            }

            Point2d optimalPoint = GetoptimalCurve(view, out intent, out quadrant);
            if (optimalPoint == null)
            {
                LogWriter.LogWrite("Unable to find a position to place Balloon for " + itemOccurrences[1].Name);
            }
            return optimalPoint;
            //return GetAttachPoint(GetBestSegmentFromOccurrence(OccurrencesCurves));
        }

        private Point2d GetoptimalCurve(DrawingView view, out GeometryIntent intent, out VctBalloonZone quadrant)
        {
            quadrant = VctBalloonZone.NotFound;
            intent = null;
            foreach (var item in _peripheralCurves.Items.OrderBy(a => a.Distance))
            {
                bool overlap = false;
                intent = GetAttachPoint(item.Segment);
                Point2d result = GetBalloonPosition(intent.PointOnSheet, view, out quadrant);
                if (view.Parent.Balloons.Count == 0)
                    return result;
                
                foreach (Balloon balloon in view.Parent.Balloons)
                {
                    double balloonRadius = balloon.Style.BalloonDiameter / 2;
                    if (DoOverlap(balloon.Position.X - balloonRadius,
                        balloon.Position.Y + balloonRadius,
                        balloon.Position.X + balloonRadius,
                        balloon.Position.Y - balloonRadius,
                        result.X - balloonRadius,
                        result.Y + balloonRadius,
                        result.X + balloonRadius,
                        result.Y - balloonRadius))
                    {
                        overlap = true;
                        break;
                    }
                }

                if (!overlap)
                    return result;
            }
            return null;
        }

        static bool DoOverlap(double l1X, double l1Y, double r1X, double r1Y,
            double l2X, double l2Y, double r2X, double r2Y)
        {
            if (l1X >= r2X || l2X >= r1X)
                return false;

            if (r1Y >= l2Y || r2Y >= l1Y)
                return false;

            return true;
        }

        #endregion
        #region Curves analysis methods
        private List<DrawingCurve> GetCurvesFromOcc(ComponentOccurrencesEnumerator occurrences, DrawingView view)
        {
            var curves = new List<DrawingCurve>();
            foreach (ComponentOccurrence occ in occurrences)
            {
                if (!occ.Suppressed)
                {
                    foreach (DrawingCurve curve in view.DrawingCurves[occ])
                    {
                        if (!curves.Contains(curve))
                            curves.Add(curve);
                    }
                }
            }

            return curves;
        }

        private Dictionary<Point2d, DrawingCurveSegment> GetPeripheralSegments(List<DrawingCurve> occurrenceCurves, bool considerAllSegments = false)
        {
            Dictionary<Point2d, DrawingCurveSegment> peripheralSegments = new Dictionary<Point2d, DrawingCurveSegment>();

            foreach (DrawingCurve curve in occurrenceCurves)
            {
                foreach (DrawingCurveSegment segment in curve.Segments)
                {
                    Sheet drawingSheet = segment.Parent.Parent.Parent;
                    if (considerAllSegments)
                    {
                        if (segment.GeometryType == Curve2dTypeEnum.kCircleCurve2d ||
                            segment.GeometryType == Curve2dTypeEnum.kEllipseFullCurve2d)
                        {
                            peripheralSegments.Add(
                                drawingSheet
                                    .CreateGeometryIntent(segment.Parent, PointIntentEnum.kCircularTopPointIntent)
                                    .PointOnSheet, segment);
                            peripheralSegments.Add(
                                drawingSheet
                                    .CreateGeometryIntent(segment.Parent, PointIntentEnum.kCircularBottomPointIntent)
                                    .PointOnSheet, segment);
                            peripheralSegments.Add(
                                drawingSheet
                                    .CreateGeometryIntent(segment.Parent, PointIntentEnum.kCircularLeftPointIntent)
                                    .PointOnSheet, segment);
                            peripheralSegments.Add(
                                drawingSheet
                                    .CreateGeometryIntent(segment.Parent, PointIntentEnum.kCircularRightPointIntent)
                                    .PointOnSheet, segment);
                        }
                        else if (segment.GeometryType == Curve2dTypeEnum.kEllipticalArcCurve2d || segment.GeometryType == Curve2dTypeEnum.kBSplineCurve2d)
                        {
                            peripheralSegments.Add(GetSegmentMidPoint(segment), segment);
                        }
                        else
                        {
                            dynamic geometry = segment.Geometry;
                            peripheralSegments.Add(geometry.MidPoint, segment);
                        }
                    }
                    else if (segment.GeometryType == Curve2dTypeEnum.kLineSegmentCurve2d && DistancebtTwoPoints(segment) > 0.5)
                    {
                        dynamic geometry = segment.Geometry;
                        peripheralSegments.Add(geometry.MidPoint, segment);
                    }
                }
            }
            return peripheralSegments;
        }


        private double DistancebtTwoPoints(DrawingCurveSegment drawingCurve)
        {
            if (drawingCurve.GeometryType == Curve2dTypeEnum.kLineSegmentCurve2d)
                return DistancebtTwoPoints(drawingCurve.Geometry.StartPoint, drawingCurve.Geometry.EndPoint);
            return 0;
        }
        private double DistancebtTwoPoints(double[] startpoint, double[] endpoint)
        {
            return DistancebtTwoPoints(_application.TransientGeometry.CreatePoint2d(startpoint[0], startpoint[1]),
                _application.TransientGeometry
                    .CreatePoint2d(endpoint[0], endpoint[1]));
        }
        private double DistancebtTwoPoints(Point2d startpoint, Point2d endpoint)
        {
            return startpoint.DistanceTo(endpoint);// Math.Sqrt(Math.Pow((Endpoint.X - Startpoint.X), 2) + Math.Pow((Endpoint.Y - Startpoint.Y), 2));
        }

        private PeripheralCurves _peripheralCurves;
        private void GetPeripheralCurves(List<DrawingCurve> occurrenceCurves)
        {
            _peripheralCurves = new PeripheralCurves(TopLine, BtmLine);
            var allSegments = GetPeripheralSegments(occurrenceCurves);
            if (allSegments.Count == 0)
                allSegments = GetPeripheralSegments(occurrenceCurves, true);

            if (allSegments.Count == 0)
            {
                return;
            }
            _peripheralCurves.Add(allSegments);
        }
        private Point2d GetSegmentMidPoint(DrawingCurveSegment segment)
        {
            Curve2dEvaluator curveEval = segment.Geometry.Evaluator;
            double minParam, maxParam, curveLength;
            double[] midParam = new double[1], midPointCoordinates = new double[2];
            curveEval.GetParamExtents(out minParam, out maxParam);
            curveEval.GetLengthAtParam(minParam, maxParam, out curveLength);
            curveEval.GetParamAtLength(minParam, curveLength / 2d, out midParam[0]);
            curveEval.GetPointAtParam(ref midParam, ref midPointCoordinates);
            return _application.TransientGeometry.CreatePoint2d(midPointCoordinates[0], midPointCoordinates[1]);
        }

        private GeometryIntent GetAttachPoint(DrawingCurveSegment segment)
        {
            if (segment is null) return null;

            if (segment.GeometryType == Curve2dTypeEnum.kCircleCurve2d | segment.GeometryType == Curve2dTypeEnum.kEllipseFullCurve2d)
            {
                switch (GetQuadrant(segment.Geometry.Center, segment.Parent.Parent))
                {
                    case VctBalloonZone.Top:
                        return ActiveSheet.CreateGeometryIntent(segment.Parent, PointIntentEnum.kCircularTopPointIntent);

                    case VctBalloonZone.Bottom:
                        return ActiveSheet.CreateGeometryIntent(segment.Parent, PointIntentEnum.kCircularBottomPointIntent);

                    case VctBalloonZone.Left:
                        return ActiveSheet.CreateGeometryIntent(segment.Parent, PointIntentEnum.kCircularLeftPointIntent);

                    case VctBalloonZone.Right:
                        return ActiveSheet.CreateGeometryIntent(segment.Parent, PointIntentEnum.kCircularRightPointIntent);

                    case VctBalloonZone.NotFound:
                        return ActiveSheet.CreateGeometryIntent(segment.Parent, PointIntentEnum.kCenterPointIntent);
                }
            }
            return ActiveSheet.CreateGeometryIntent(segment.Parent, GetSegmentMidPoint(segment));
        }

        public object PlacePartList()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
