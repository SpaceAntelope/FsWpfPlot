﻿namespace FsWpfPlot.Types

open System.Windows.Media.Media3D
open System.Windows.Data
open System.Windows
    
type PlotFunction = float->float->float
type ChartFunction = float->float

type ColorCoding = ByLights=0 | ByGradientY=1

type PlotKind = Surface=0 | Points=1 | Linear=2 | Wireframe=3

type ChartKind = Line = 0 

type DataSummary2D = {
    MaxX:float
    MaxY:float
    MinX:float
    MinY:float
    Length: int
    Data: Point[]
} with 
    static member CreateFrom (data: Point[]) =             
        let limits = [| System.Double.MinValue; System.Double.MinValue;
                        System.Double.MaxValue; System.Double.MaxValue;|]
        data |> Array.iter (fun point ->
                limits.[0] <- (max limits.[0] point.X)
                limits.[1] <- (max limits.[1] point.Y)
                limits.[2] <- (min limits.[2] point.X)
                limits.[3] <- (min limits.[3] point.Y) )
        {   MaxX = limits.[0] 
            MaxY = limits.[1]               
            MinX = limits.[2] 
            MinY = limits.[3]               
            Length = Array.length data
            Data = data }
        
    member this.RangeX() = this.MaxX - this.MinX
    member this.RangeY() = this.MaxY - this.MinY

type DataSummary3D = {
    MaxX:float
    MaxY:float
    MaxZ:float
    MinX:float
    MinY:float
    MinZ:float
    Rows:int
    Columns:int
    Data: Point3D[,]
}
with 
    static member CreateFrom (data: Point3D[,]) = 
        let rows,cols = data.GetUpperBound(0), data.GetUpperBound(1)
        let results = [| System.Double.MinValue; System.Double.MinValue; System.Double.MinValue;
                            System.Double.MaxValue; System.Double.MaxValue; System.Double.MaxValue;|]
                 
        data |> Array2D.iter (fun point ->                
                results.[0] <- (max results.[0] point.X)
                results.[1] <- (max results.[1] point.Y)
                results.[2] <- (max results.[2] point.Z)
                results.[3] <- (min results.[3] point.X)
                results.[4] <- (min results.[4] point.Y)
                results.[5] <- (min results.[5] point.Z) ) 
            
        {   MaxX = results.[0] 
            MaxY = results.[1] 
            MaxZ = results.[2] 
            MinX = results.[3] 
            MinY = results.[4] 
            MinZ = results.[5] 
            Rows = rows
            Columns = cols
            Data = data }

    member this.RangeX() = this.MaxX - this.MinX
    member this.RangeY() = this.MaxY - this.MinY
    member this.RangeZ() = this.MaxZ - this.MinZ

type DummyConverter() = 
    interface IValueConverter with
        member this.Convert(value: obj, targetType: System.Type, parameter: obj, culture: System.Globalization.CultureInfo): obj = 
            printfn "converter hit: %A %A" value parameter
            printfn "is null: %b" (value = null)
            value

        member this.ConvertBack(value: obj, targetType: System.Type, parameter: obj, culture: System.Globalization.CultureInfo): obj = 
            raise (System.NotImplementedException())

//type EnumToBooleanConverter() =
//    interface IValueConverter with
//        member this.Convert(value: obj, targetType: System.Type, parameter: obj, culture: System.Globalization.CultureInfo): obj = 
//            if not isNull value && not isNull parameter 
//            then 
//                let isChecked = value :?> bool
//                let targetValue = parameter
                    

//        member this.ConvertBack(value: obj, targetType: System.Type, parameter: obj, culture: System.Globalization.CultureInfo): obj = 
//            raise (System.NotImplementedException()) 