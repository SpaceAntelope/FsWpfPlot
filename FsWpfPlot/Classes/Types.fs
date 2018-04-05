namespace FsWpfPlot.Types
    open System.Windows.Media.Media3D
    open System.Windows.Data
    

    type ColorCoding = ByLights | ByGradientY

    type DataSetInfo = {
        MaxX:float
        MaxY:float
        MaxZ:float
        MinX:float
        MinY:float
        MinZ:float
        Rows:int
        Columns:int
    }
    with 
        static member CreateFrom (data: Point3D[,]) = 
            let rows,cols = data.GetUpperBound(0), data.GetUpperBound(1)
            let results = [| System.Double.MinValue; System.Double.MinValue; System.Double.MinValue;
                             System.Double.MaxValue; System.Double.MaxValue; System.Double.MaxValue;|]
                 
            data |> Array2D.iteri (fun x y point ->                
                    results.[0] <- (max results.[0] point.X)
                    results.[1] <- (max results.[1] point.Y)
                    results.[2] <- (max results.[2] point.Z)
                    results.[3] <- (min results.[3] point.X)
                    results.[4] <- (min results.[4] point.Y)
                    results.[5] <- (min results.[5] point.Z) 
                    ) 
            
            { MaxX = results.[0] 
              MaxY = results.[1] 
              MaxZ = results.[2] 
              MinX = results.[3] 
              MinY = results.[4] 
              MinZ = results.[5] 
              Rows = rows
              Columns = cols }
        member this.RangeX() = this.MaxX - this.MinX
        member this.RangeY() = this.MaxY - this.MinY
        member this.RangeZ() = this.MaxZ - this.MinZ

    type LightConverter() = 
        interface IValueConverter with
            member this.Convert(value: obj, targetType: System.Type, parameter: obj, culture: System.Globalization.CultureInfo): obj = 
                printfn "converter hit: %A %A" value parameter
                printfn "is null: %b" (value = null)
                value

            member this.ConvertBack(value: obj, targetType: System.Type, parameter: obj, culture: System.Globalization.CultureInfo): obj = 
                raise (System.NotImplementedException())