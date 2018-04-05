// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open FsWpfPlot
open System.Windows
open FsWpfPlot.Models
open System
open System.Windows.Media.Media3D

[<EntryPoint>]
[<STAThread>]
let main argv = 

    let model = SurfacePlotModel(MinX = 6., MinY = 8.)
    model.Data <- model.FuncZ |> model.DataFromFunction
    let plot = FsWpfPlot.Views.SurfacePlotContainerProxy(model)
    printfn "%A" <| plot.Show()

    printfn "Done."
    
    //Console.ReadLine() |> ignore
    (Array2D.create<Point3D> 4 4 ).GetType().Name
    0
    