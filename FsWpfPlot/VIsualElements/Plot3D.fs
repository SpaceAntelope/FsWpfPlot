namespace FsWpfPlot.VisualElements

    open System.Windows.Media.Media3D
    open HelixToolkit.Wpf
    open System.Windows
    open FsWpfPlot.HelperFunctions
    open System.Windows.Media
    open FsWpfPlot.Types
    open System
    open System.Windows.Controls

    type Plot3D() as this = 
        inherit ModelVisual3D()
        
        let visualChild = ModelVisual3D()
        do
            this.Children.Add visualChild
        
        static let PointsProperty : DependencyProperty = 
            DependencyProperty.Register(
                "Points", typeof<Point3D[,]>, typeof<Plot3D>,
                new UIPropertyMetadata(Array2D.zeroCreate<Point3D> 4 4, PropertyChangedCallback Plot3D.modelChanged) )

        static let ColorCodingProperty : DependencyProperty = 
            DependencyProperty.Register(
                "ColorCoding", typeof<ColorCoding>, typeof<Plot3D>,
                new UIPropertyMetadata(ColorCoding.ByGradientY, PropertyChangedCallback Plot3D.modelChanged) )

        static let PlotKindProperty : DependencyProperty = 
            DependencyProperty.Register(
                "PlotKind", typeof<PlotKind>, typeof<Plot3D>,
                new UIPropertyMetadata(PlotKind.Surface, PropertyChangedCallback Plot3D.modelChanged) )
        
        member this.Points 
            with get() = this.GetValue(PointsProperty) :?> Point3D[,]
            and  set(value: Point3D[,]) = this.SetValue(PointsProperty, value)
            
        member this.ColorCoding 
            with get() = this.GetValue(ColorCodingProperty) :?> ColorCoding
            and  set(value: ColorCoding) = this.SetValue(ColorCodingProperty, value)

        member this.PlotKind 
            with get() = this.GetValue(PlotKindProperty) :?> PlotKind
            and  set(value: PlotKind) = this.SetValue(PlotKindProperty, value)

        static member modelChanged sender e =             
            (sender :?> Plot3D).UpdateModel()

        member val IntervalX = 1. with get, set
        member val IntervalY = 1. with get, set
        member val IntervalZ = 0.5 with get, set
        member val FontSize = 0.2 with get, set
        member val LineThickness = 0.01 with get, set
        member val PointSize = 1. with get, set
        member val PointColor = Colors.Red with get, set

        member this.CreateLights colorCoding =
            let group = Model3DGroup()
            match colorCoding with 
            | ColorCoding.ByGradientY ->
                group.Children.Add(AmbientLight(Colors.White))
            | ColorCoding.ByLights -> 
                group.Children.Add(AmbientLight(Colors.Gray))
                group.Children.Add(PointLight(Colors.Red, Point3D(0.,-1000.,0.)))
                group.Children.Add(PointLight(Colors.Blue, Point3D(0.,0.,1000.)))
                group.Children.Add(PointLight(Colors.Green, Point3D(1000.,1000.,0.)))
            | x -> failwith (sprintf "%A is not a valid value for color coding" x)
            group
        
        member this.SurfaceBrush 
            with get() =
                match this.ColorCoding with 
                | ColorCoding.ByGradientY ->
                    BrushHelper.CreateGradientBrush(Colors.Red, Colors.White, Colors.Blue)
                    :> Brush
                | ColorCoding.ByLights -> 
                    Brushes.White 
                    :> Brush
                | x -> failwith (sprintf "%A is not a valid value for color coding" x)

        member this.UpdateModel() =          
            this.Children.Clear()
            this.Children.Add visualChild

            let group = Model3DGroup()
            let summary = this.Points |> DataSetInfo.CreateFrom 
            
            let axes() = 
                summary
                |> this.CreateAxes
                |> fun (textModel, axisGeometry) ->
                    group.Children.Add axisGeometry
                    textModel |> Seq.iter (this.Children.Add) 
            
            let wireframe() = 
                summary
                |> this.CreateWireFrame
                |> group.Children.Add
            
            let surface() = 
                summary
                |> this.CreateSurface (this.ColorCoding) (this.SurfaceBrush)
                |> group.Children.Add

            let lighting() = 
                this.ColorCoding
                |> this.CreateLights
                |> group.Children.Add
            
            let points() = 
                summary
                |> this.CreateScatterPlot this.PointColor
                |> this.Children.Add
            
            let linear() = 
                summary
                |> this.CreateLinear this.PointColor
                |> this.Children.Add

            match this.PlotKind with
            | PlotKind.Points -> 
                    axes()
                    lighting()
                    points()
            | PlotKind.Surface ->
                    axes()
                    lighting()
                    wireframe()
                    surface()
            | PlotKind.Wireframe ->
                    axes()
                    lighting()
                    wireframe()
            | PlotKind.Linear ->
                    axes()
                    lighting()
                    linear()
            | x -> failwith (sprintf "%A not a valid plot kind enum" x)
            
            visualChild.Content <- group           

        member this.CreateAxes (summary : DataSetInfo) =
            let { MinX=minX; MinY=minY; MinZ=minZ;
                  MaxX=maxX; MaxY=maxY; MaxZ=maxZ; } = summary
            
            let billBoard text point = 
                BillboardTextVisual3D(Text = text, FontWeight = FontWeight.FromOpenTypeWeight(1),
                    FontSize=15., FontFamily=new FontFamily("Arial"),
                    Background=Brushes.Transparent,
                    Position=point,
                    DepthOffset = 0.001)

            let xLabels = 
                [   yield! 
                        [ for x in minX .. this.IntervalX ..maxX do        
                            yield Point3D(x, minY - this.FontSize * 3., minZ - 2.5*this.FontSize)                
                            |> billBoard (string x) ]
                    
                    yield Point3D((minX + maxX)*0.5, minY - this.FontSize * 10., minZ - 6.0*this.FontSize)
                    |> billBoard "X"
                ]
             
            let yLabels = 
                [   yield! 
                        [ for y in minY .. this.IntervalY .. maxY do
                            yield Point3D(minX - this.FontSize * 3., y , minZ-2.5*this.FontSize)
                            |> billBoard (string y) ]
                    
                    yield Point3D(minX - this.FontSize * 10., (minY + maxX)* 0.5, minZ - 6.0*this.FontSize)
                    |> billBoard "Y"
                ]

            let zLabels =                
                let maxZ = roundTo1d maxZ
                [   yield! [ for z in minZ .. this.IntervalZ .. maxZ do      
                                yield Point3D(maxX + this.FontSize * 3., maxY + this.FontSize * 3., z)
                                |> billBoard (z |> roundTo1d |> string) ]

                    if  maxZ % this.IntervalZ > 0. then
                        yield Point3D(maxX + this.FontSize * 3., maxY + this.FontSize * 3., maxZ)
                        |> billBoard (string maxZ) 
                
                    yield Point3D(maxX + this.FontSize * 10., maxY + this.FontSize * 10., (minZ + maxZ)* 0.5)
                    |> billBoard "Z"
                ]   
        
            let boundingBox = Rect3D(minX, minY, minZ, summary.RangeX(), summary.RangeY(),  summary.RangeZ());
            let axesMeshBuilder = MeshBuilder()
            axesMeshBuilder.AddBoundingBox (boundingBox,this.LineThickness)
            
            xLabels@yLabels@zLabels, GeometryModel3D(axesMeshBuilder.ToMesh(), Materials.Black) 

        member this.CreateWireFrame (summary : DataSetInfo) =             
            let { MinX=minX; MinY=minY; MinZ=minZ;
                  MaxX=maxX; MaxY=maxY;
                  Columns=cols; Rows=rows; 
                  Data=points} = summary
            
            let meshBuilder = MeshBuilder()
                        
            for x in minX .. this.IntervalX ..maxX do
                let x' = (x - minX) / summary.RangeX() * (float cols - 1.)                        
                [ yield Point3D(x, minY, minZ)
                  yield! [ for row in 0..(rows-1) do yield bilinearInterpolation points row (int x') ]
                  yield Point3D(x, maxY, minZ) ]
                |> CGList
                |> fun plotSlice -> meshBuilder.AddTube(plotSlice, this.LineThickness, 9, false)            

            for y in minY .. this.IntervalY .. maxY do
                let y' = (y - minY) / summary.RangeY() * (float rows - 1.)
                [ yield Point3D(minX, y, minZ)
                  yield! [for col in 0..(cols - 1) do yield bilinearInterpolation points (int y') col ]
                  yield Point3D(maxX, y, minZ) ]
                |> CGList
                |> fun plotSlice -> meshBuilder.AddTube(plotSlice, this.LineThickness, 9, false)
            
            GeometryModel3D(meshBuilder.ToMesh(), Materials.Black)

        member this.CreateScatterPlot (color: Color) (summary : DataSetInfo) =
            PointsVisual3D(
                Color = color, Size=this.PointSize, 
                Points = Point3DCollection(summary.Data |> Seq.cast<Point3D>))
            
        member this.CreateLinear (color: Color) (summary : DataSetInfo) =
            LinesVisual3D(
                Color = color, 
                Points = Point3DCollection(summary.Data |> Seq.cast<Point3D>))


        member this.CreateSurface (colorCoding: ColorCoding) (brush : Brush) (summary: DataSetInfo) =
            let { MinZ=minZ; 
                  Columns=cols; Rows=rows; 
                  Data=points} = summary
            
            let colorValues = 
                match colorCoding with
                | ColorCoding.ByGradientY -> findGradientY summary.Data
                | ColorCoding.ByLights -> Array2D.zeroCreate 1 1
                | x -> failwith (sprintf "%A is not a valid value for color coding" x)
            
            let (minColorValue, maxColorValue) = 
                let values = [|Double.MaxValue;Double.MinValue|]
                colorValues
                |> Array2D.iter (fun colorValue -> 
                        values.[0] <- min values.[0] colorValue
                        values.[1] <- max values.[0] colorValue )
                
                // make color value 0 at texture coordinate 0.5
                if abs values.[0] < abs values.[1] 
                then values.[0] <- -values.[1]
                else values.[1] <- -values.[0]
                
                values.[0],values.[1]

            let textureCoordinates = 
                // set the texture coordinates by z-value or ColorValue
                Array2D.init (rows+1) (cols+1)
                    (fun row col ->                          
                        let z' = 
                            if colorValues.Length = 1 && colorValues.[0,0] = 0.
                            then (points.[row,col].Z - minZ) / summary.RangeZ()
                            else (colorValues.[row,col] - minColorValue) / (maxColorValue-minColorValue)
                        Point(z',z') )

            let meshBuilder = MeshBuilder()
            meshBuilder.AddRectangularMesh(points, textureCoordinates)
            let surfaceModel = GeometryModel3D(meshBuilder.ToMesh(), 
                                    MaterialHelper.CreateMaterial(brush, null, null, 1., 0.))
            surfaceModel.BackMaterial <- surfaceModel.Material
            surfaceModel
            
   

