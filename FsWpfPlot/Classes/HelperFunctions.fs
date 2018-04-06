namespace FsWpfPlot

module HelperFunctions =
    open System.Windows.Media.Media3D
    open HelixToolkit.Wpf
    open System.Windows
    open System

    let findGradientY (data : Point3D[,]) =
        let rows,cols = data.GetUpperBound(0) + 1, data.GetUpperBound(0) + 1
        
        let inline index1 index max = if index + 1 < max then index + 1 else index
        let inline index0 index     = if index - 1 > 0   then index - 1 else index
        
        Array2D.init rows cols (fun row col -> 

            let p10 = data.[index1 row rows, index0 col]
            let p00 = data.[index0 row, index0 col]
            //let p11 = data.[index1 row rows, index1 col cols]
            //let p01 = data.[index0 row, index1 col cols]
            
            let dy = p10.Y - p00.Y
            let dz = p10.Z - p00.Z

            dz/dy)

    let bilinearInterpolation (points: Point3D[,]) row col =
        let rows,cols = points.GetUpperBound(0), points.GetUpperBound(0)

        let row0 = if row + 1 >= rows then rows - 2 else row
        let col0 = if col + 1 >= cols then cols - 2 else col

        let row' = if row < 0 then 0 else row
        let col' = if col < 0 then 0 else col

        let u = float (row' - row0)
        let v = float (col' - col0)

        let v00 = points.[row0, col0].ToVector3D()
        let v01 = points.[row0, col0 + 1].ToVector3D()
        let v10 = points.[row0 + 1, col0].ToVector3D()
        let v11 = points.[row0 + 1, col0 + 1].ToVector3D()
        let v0  = v00 * (1. - u) + v10 * u
        let v1  = v01 * (1. - u) + v11 * u
        
        (v0*(1.0 - v) + v1*v).ToPoint3D()
    

    let calculateDataAndMapToResolution (map : int->int->Point)  (f:float->float->float) resolution =
            Array2D.init resolution resolution (fun row col ->
                    let point = map row col
                    Point3D(point.X, point.Y, f point.X point.Y))


    let inline CGList<'T> (source: 'T seq) = System.Collections.Generic.List<'T>(source)
    let inline toDep source = source :> DependencyObject
    let inline roundTo1d z = Math.Round (z * 10.)/10.