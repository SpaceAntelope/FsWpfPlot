namespace FsWpfPlot.VisualElements

    open System.Windows.Media.Media3D
    open HelixToolkit.Wpf
    open System.Windows
    open FsWpfPlot.HelperFunctions
    open System.Windows.Media
    open System.Windows.Controls
    open Microsoft.Win32
    open FsWpfPlot.Types

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

        member this.UpdateModel() = visualChild.Content <- this.Points |> this.CreateModel

        member val IntervalX = 1. with get, set
        member val IntervalY = 1. with get, set
        member val IntervalZ = 0.25 with get, set
        member val FontSize = 0.06 with get, set
        member val LineThickness = 0.05 with get, set

        member this.CreateModel (points: Point3D[,]) = 
            let plotModel = Model3DGroup()
            let axesMeshBuilder = MeshBuilder()
            let summary = DataSetInfo.CreateFrom points
            
            for x in [ summary.MinX .. this.IntervalX ..summary.MaxX ] do
                let j = (x - summary.MinX) / summary.RangeX
                let path = [Point3D(x, summary.MinY, summary.MinZ) ] |> toList
                for i in [0 .. summary.Rows-1] do   
                    path.Add <| bilinearInterpolation points i (int j)
                path.Add <| Point3D(x, summary.MaxY, summary.MinZ)

                axesMeshBuilder.AddTube(path, this.LineThickness, 9, false)

                TextCreator.CreateTextLabelModel3D(
                    x.ToString(), Brushes.Black, true, this.FontSize,
                    Point3D((summary.MinX + summary.MaxX)* 0.5, summary.MinY - this.FontSize * 6., summary.MinZ),
                    Vector3D(1.,0.,0.), Vector3D(0.,1.,0.) )
                |> plotModel.Children.Add

            plotModel
  

