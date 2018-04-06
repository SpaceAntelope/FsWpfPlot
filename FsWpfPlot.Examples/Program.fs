open FsWpfPlot
open System.Windows
open FsWpfPlot.Models
open System
open System.Windows.Media.Media3D
open System.Windows.Input

type PlotFunction = float->float->float

[<EntryPoint>]
[<STAThread>]
let main argv = 


    let model = SurfacePlotModel()
    let inline sq x = pown x 2

    let signf x = (sign x) |> float

    let radial : PlotFunction = fun x y -> sqrt ((pown x 2) + (pown y 2))
    let windmill : PlotFunction = fun x y -> signf(x*y)*signf(1. - sq(x*9.) + sq(y*9.))/9.
    let fences = fun x y -> 0.75 * exp(sq(sq(x*5.)*sq(y*5.)))
    let ripple : PlotFunction = fun x y -> sin ( 10.*(sq x + (sq y) )/10.) 
    let ripple' : PlotFunction = fun x y -> sin ( 10.*(sq x + (sq y) )/10.) + tanh(x*y)

    model.Resolution <- 200
    model.setRangeX -6. 6.
    model.setRangeY -6. 6.
    model.FuncZ <- Some(ripple')
        
    let plot = FsWpfPlot.Views.SurfacePlotContainerProxy(model)
    printfn "%A" <| plot.Show()

    printfn "Done."
    
    0
    