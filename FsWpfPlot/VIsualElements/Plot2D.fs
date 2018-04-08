namespace FsWpfPlot.VisualElements

    open System.Windows.Media.Media3D
    open HelixToolkit.Wpf
    open System.Windows
    open FsWpfPlot.HelperFunctions
    open System.Windows.Media
    open FsWpfPlot.Types
    open System
    open System.Windows.Controls

    type Plot2D() as this = 
        inherit ModelVisual3D()
        
        let visualChild = ModelVisual3D()
        do
            this.Children.Add visualChild
        
        static let PointsProperty : DependencyProperty = 
            DependencyProperty.Register(
                "Points", typeof<Point[]>, typeof<Plot2D>,
                new UIPropertyMetadata(Array.zeroCreate<Point> 4,  PropertyChangedCallback Plot2D.modelChanged) )
        
        static let PlotKindProperty : DependencyProperty = 
            DependencyProperty.Register(
                "PlotKind", typeof<PlotKind>, typeof<Plot2D>,
                new UIPropertyMetadata(PlotKind.Surface, PropertyChangedCallback Plot2D.modelChanged) )
        
        member this.Points 
            with get() = this.GetValue(PointsProperty) :?> Point[]
            and  set(value: Point[]) = this.SetValue(PointsProperty, value)                    

        member this.PlotKind 
            with get() = this.GetValue(PlotKindProperty) :?> PlotKind
            and  set(value: PlotKind) = this.SetValue(PlotKindProperty, value)

        static member modelChanged sender e =             
            (sender :?> Plot2D).UpdateModel()

        member val IntervalX = 1. with get, set
        member val IntervalY = 0.25 with get, set
        member val FontSize = 0.1 with get, set
        member val LineThickness = 0.015 with get, set
        member val PointSize = 1. with get, set
        member val PointColor = Colors.Red with get, set

        //member this.SurfaceBrush 
        //    with get() =
        //        match this.ColorCoding with 
        //        | ColorCoding.ByGradientY ->
        //            BrushHelper.CreateGradientBrush(Colors.Red, Colors.White, Colors.Blue)
        //            :> Brush
        //        | ColorCoding.ByLights -> 
        //            Brushes.White 
        //            :> Brush
        //        | x -> failwith (sprintf "%A is not a valid value for color coding" x)

        member this.UpdateModel() =          
            this.Children.Clear()
            this.Children.Add visualChild

            let group = Model3DGroup()
            let summary = this.Points |> DataSummary2D.CreateFrom 
            
            let axes() = 
                summary
                |> this.CreateAxes
                |> fun (textModel, gridLines) ->
                    this.Children.Add gridLines
                    textModel |> Seq.iter (group.Children.Add) 
        
        //let surface() = 
            //    summary
            //    |> this.CreateSurface (this.ColorCoding) (this.SurfaceBrush)
            //    |> group.Children.Add

            
            let points() = 
                summary
                |> this.CreateScatterPlot this.PointColor
                |> this.Children.Add
            
            let linear() = 
                summary
                |> this.CreateLinear this.PointColor
                |> group.Children.Add

            let lighting() = 
                this.CreateLights()
                |> group.Children.Add
            //match this.PlotKind with
            //| PlotKind.Points -> 
            //        axes()
            //        points()
            //| PlotKind.Surface ->
            //        axes()
            //        //wireframe()
            //        //surface()            
            //| PlotKind.Linear ->
            //        axes()
            //        linear()
            //| x -> failwith (sprintf "%A not a valid plot kind enum" x)
            
            axes()
            lighting()
            linear()
            visualChild.Content <- group           

        member this.CreateLights() =
            let group = Model3DGroup()
            group.Children.Add(AmbientLight(Colors.White))
            //match colorCoding with 
            //| ColorCoding.ByGradientY ->
            //    group.Children.Add(AmbientLight(Colors.White))
            //| ColorCoding.ByLights -> 
            //    group.Children.Add(AmbientLight(Colors.Gray))
            //    group.Children.Add(PointLight(Colors.Red, Point3D(0.,-1000.,0.)))
            //    group.Children.Add(PointLight(Colors.Blue, Point3D(0.,0.,1000.)))
            //    group.Children.Add(PointLight(Colors.Green, Point3D(1000.,1000.,0.)))
            //| x -> failwith (sprintf "%A is not a valid value for color coding" x)
            group

        member this.CreateScatterPlot (color: Color) (summary : DataSummary2D) =
            PointsVisual3D(
                Color = color, Size=this.PointSize, 
                Points = Point3DCollection(summary.Data |> Array.map pointToPoint3D) )

        member this.CreateAxes (summary : DataSummary2D) =
            let { MinX=minX; MinY=minY; 
                  MaxX=maxX; MaxY=maxY; Length = length} = summary

            let label text point = 
                TextCreator.CreateTextLabelModel3D(text, Brushes.Black, true, this.FontSize, 
                    point, Vector3D(1., 0., 0.), Vector3D(0., 1., 0.))
                    
            let xLabels = 
                [   yield! 
                        [ for x in minX .. this.IntervalX ..maxX do        
                            yield Point3D(x, minY - this.FontSize * 3., 0.)
                            |> label (string x) ]
                    
                    yield Point3D((minX + maxX)*0.5, minY - this.FontSize * 7.5, 0.)
                    |> label "X"
                ]
             
            let yLabels = 
                let maxY' = roundTo1d maxY
                let minY' = roundTo1d minY
                [   yield! 
                        [ for y in minY' .. this.IntervalY .. maxY' do
                            yield Point3D(minX - this.FontSize * 3., y, 0.)
                            |> label (y |> roundTo1d |> string) ]                                     

                    yield Point3D(minX - this.FontSize * 7.5, minY + summary.RangeY()* 0.5, 0.)
                    |> label "Y"
                ]

            let gridLines  = 
                let rangeX = summary.RangeX()
                let rangeY = summary.RangeY()
                GridLinesVisual3D(Center=Point3D(minX + rangeX/2.,minY + rangeY/2.,0.), MajorDistance = this.IntervalX, MinorDistance=this.IntervalY, Length= rangeX, Width=rangeY, Thickness=0.01)
                    //Normal=Vector3D(0.,1.,0.), LengthDirection=Vector3D(1.,0.,0.), Length=100.)
                       
            //let boundingBox = Rect3D(minX, minY, 0., summary.RangeX(), summary.RangeY(), 0.);
            //let axesMeshBuilder = MeshBuilder()            
            //axesMeshBuilder.AddBoundingBox (boundingBox,this.LineThickness)
            
            xLabels@yLabels, gridLines // GeometryModel3D(axesMeshBuilder.ToMesh(), Materials.Black) 

            
        member this.CreateLinear (color: Color) (summary : DataSummary2D) =
            let meshBuilder = MeshBuilder()
            summary.Data 
            |> Array.map (fun pt -> Point3D(pt.X, pt.Y, 0.)) 
            |> (fun pts ->
                    meshBuilder.AddTube(pts, this.LineThickness, 9, false))
            
            GeometryModel3D(meshBuilder.ToMesh(), Materials.Red)
            //LinesVisual3D(
            //    Color = color, Thickness = 0.1,
            //    Points = Point3DCollection(summary.Data |> Array.mapi (fun i pt -> Point3D(pt.X, pt.Y, float(i % 2)/10.))) )


        
            
   

