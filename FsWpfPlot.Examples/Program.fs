open FsWpfPlot.Models
open FsWpfPlot.Types
open System
open System.Windows.Shapes
open System.Windows
open System.Windows.Media.Media3D




let plot3d (f: PlotFunction) = 
    let model = Plot3DModel(Resolution = 200)
    model.setRangeX -1. 1.
    model.setRangeY -1. 1.
    model.FuncZ <- Some(f)
        
    let plot3d = FsWpfPlot.Views.SurfacePlotContainerProxy(model)
    printfn "%A" <| plot3d.Show()

let plot2d (f : ChartFunction) =
    let model = Plot2DModel(Resolution = 100)
    model.setRangeX -3. 3.
    model.Func <- Some(f)

    let plot2d = FsWpfPlot.Views.Plot2DContainerProxy(model)
    printfn "%A" <| plot2d.Show()

[<EntryPoint>]
[<STAThread>]
let main argv = 

    let inline signf x = (sign x) |> float

    let radial : PlotFunction = fun x y -> sqrt (x**2. + y**2.)
    let windmill : PlotFunction = fun x y -> signf(x*y)*signf(1. - (x*9.)**2. + (y*9.)**2.)/9.
    let fences = fun x y -> 0.75 * exp(((x*5.)**2. * (y*5.)**2.)**2.)
    let ripple : PlotFunction = fun x y -> sin ( 10.*(x**2. + y**2. )/10.) 
    let ripple' : PlotFunction = fun x y -> sin ( 10.*(x**2. + y**2.)/10.) + tanh(x*y) - radial x y
    let torus : PlotFunction = fun x y -> (0.4**2. - (0.6-(x**2. + y**2.)**0.5)**2.)**0.5
    let tube : PlotFunction = fun x y -> 1.0/(15.*(x**2. + y**2.))
    
    //plot3d tube
    plot2d (fun x -> sin x)

    printfn "Done."
    
    0
    