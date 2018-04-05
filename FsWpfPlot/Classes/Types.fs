namespace FsWpfPlot

module Types = 
    open System.Windows.Media.Media3D
    

    type ColorCoding = ByLights | ByGradientY

    type DataSetInfo = {
        MinX:float
        MinY:float
        MinZ:float
        MaxX:float
        MaxY:float
        MaxZ:float
        Rows:int
        Columns:int
    }
    with 
        static member CreateFrom (data: Point3D[,]) = 
            let rows,cols = data.GetUpperBound(0), data.GetUpperBound(1)
            let results = [|System.Double.MaxValue; System.Double.MaxValue; System.Double.MaxValue;
                            System.Double.MinValue; System.Double.MinValue; System.Double.MinValue;|]
                 
            data |> Array2D.iter (fun point ->                
                    results.[0] <- (max results.[0] point.X)
                    results.[1] <- (max results.[1] point.Y)
                    results.[2] <- (max results.[2] point.Z)
                    results.[3] <- (min results.[3] point.X)
                    results.[4] <- (min results.[4] point.Y)
                    results.[5] <- (min results.[5] point.Z) ) 
            
            { MinX = results.[0] 
              MinY = results.[1] 
              MinZ = results.[2] 
              MaxX = results.[3] 
              MaxY = results.[4] 
              MaxZ = results.[5] 
              Rows = rows
              Columns = cols }
        member this.RangeX = this.MaxX - this.MinX
        member this.RangeY = this.MaxY - this.MinY
        member this.RangeZ = this.MaxZ - this.MinZ