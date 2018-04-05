namespace FsWpfPlot.VisualElements

    open System.Windows.Media.Media3D
    open HelixToolkit.Wpf
    open System.Windows
    open FsWpfPlot.HelperFunctions
    open System.Windows.Media
    open System.Windows.Controls
    open Microsoft.Win32
    open FsWpfPlot.Types
    open System.Net.WebSockets
    open System
    open System.Windows.Media
    open System

    type SurfacePlotAxes3D() as this = 
        inherit ModelVisual3D()
        
        let visualChild = ModelVisual3D()
        do
            this.Children.Add visualChild
        

        static let PointsProperty : DependencyProperty = 
            DependencyProperty.Register(
                "Points", typeof<Point3D[,]>, typeof<SurfacePlotAxes3D>,
                new UIPropertyMetadata(Array2D.zeroCreate<Point3D> 4 4, PropertyChangedCallback SurfacePlotAxes3D.modelChanged) )

        member this.Points 
            with get() = this.GetValue(PointsProperty) :?> Point3D[,]
            and  set(value: Point3D[,]) = this.SetValue(PointsProperty, value)

        static member modelChanged sender e = (sender :?> SurfacePlotAxes3D).UpdateModel()

        member this.UpdateModel() = 
            //visualChild.Content <- this.Points |> this.CreateModel
            let (textModel, axisGeometry) = this.Points |> DataSetInfo.CreateFrom |> this.CreateAxes
            visualChild.Content <- axisGeometry
            textModel |> Seq.iter (this.Children.Add)

        member val IntervalX = 1. with get, set
        member val IntervalY = 1. with get, set
        member val IntervalZ = 1. with get, set
        member val FontSize = 0.2 with get, set
        member val LineThickness = 0.025 with get, set

        member this.CreateAxes (summary : DataSetInfo) =
            let axesMeshBuilder = MeshBuilder()
            let textModel = System.Collections.Generic.List<Visual3D>()

            for x in [ summary.MinX .. this.IntervalX ..summary.MaxX ] do        
                BillboardTextVisual3D(Text = x.ToString(),         
                    FontSize=15., FontFamily=new FontFamily("Consolas"),
                    Background=Brushes.Transparent,
                    Position=Point3D(x, summary.MinY - this.FontSize * 3., summary.MinZ),
                    DepthOffset = 0.001)
                |> textModel.Add              
            
            for y in [summary.MinY .. this.IntervalY .. summary.MaxY] do
                BillboardTextVisual3D(Text = y.ToString(),         
                    FontSize=15., FontFamily=new FontFamily("Consolas"),
                    Background=Brushes.Transparent,
                    Position=Point3D(summary.MinX - this.FontSize * 3., y, summary.MinZ), 
                    DepthOffset = 0.001)
                |> textModel.Add              

            for z in [summary.MinZ .. this.IntervalZ .. summary.MaxZ] do
                let z' = Math.Round (z * 10.)/10.
                BillboardTextVisual3D(Text = z'.ToString(),         
                    FontSize=15., FontFamily=new FontFamily("Consolas"),
                    Background=Brushes.Transparent,
                    Position=Point3D(summary.MaxX + this.FontSize * 3., summary.MaxY + this.FontSize * 3., z), 
                    DepthOffset = 0.001)
                |> textModel.Add              
            
            BillboardTextVisual3D(Text = "X",         
                    FontSize=15., FontFamily=new FontFamily("Consolas"),
                    Background=Brushes.Transparent,
                    Position=Point3D((summary.MinX + summary.MaxX)*0.5, summary.MinY - this.FontSize * 10., summary.MinZ),
                    DepthOffset = 0.001)
            |> textModel.Add              

            BillboardTextVisual3D(Text = "Y",         
                    FontSize=15., FontFamily=new FontFamily("Consolas"),
                    Background=Brushes.Transparent,
                    Position=Point3D(summary.MinX - this.FontSize * 10., (summary.MinY + summary.MaxX)* 0.5, summary.MinZ),
                    DepthOffset = 0.001)
            |> textModel.Add              

            BillboardTextVisual3D(Text = "Z",         
                    FontSize=15., FontFamily=new FontFamily("Consolas"),
                    Background=Brushes.Transparent,
                    Position=Point3D(summary.MaxX + this.FontSize * 10., summary.MaxY + this.FontSize * 10., (summary.MinZ + summary.MaxZ)* 0.5),
                    DepthOffset = 0.001)
            |> textModel.Add              

            let boundingBox = Rect3D(summary.MinX, summary.MinY, summary.MinZ, summary.RangeX(), summary.RangeY(),  summary.RangeZ());
            axesMeshBuilder.AddBoundingBox (boundingBox,this.LineThickness)
            
            textModel, GeometryModel3D(axesMeshBuilder.ToMesh(), Materials.Black) 

        member this.CreateModel (points: Point3D[,]) = 
            let plotModel = Model3DGroup()
            let axesMeshBuilder = MeshBuilder()
            let summary = DataSetInfo.CreateFrom points

            for x in [ summary.MinX .. this.IntervalX ..summary.MaxX ] do
                //let j = (x - summary.MinX) / summary.RangeX() * (float summary.Columns - 1.)
                //let path = [Point3D(x, summary.MinY, summary.MinZ) ] |> toList
                //for i in [0 .. summary.Rows-1] do   
                //    path.Add <| bilinearInterpolation points i (int j)
                //path.Add <| Point3D(x, summary.MaxY, summary.MinZ)

                //axesMeshBuilder.AddTube(path, this.LineThickness, 9, false)
                ()
            
            for y in [summary.MinY .. this.IntervalY .. summary.MaxY] do
                ()                

            for z in [summary.MinZ .. this.IntervalZ .. summary.MaxZ] do
                ()
            
            
            plotModel
   

