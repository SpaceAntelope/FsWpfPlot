namespace FsWpfPlot
open System
open System.Windows
open System.Windows.Controls
open System.Windows.Markup

module Main =
    
    open System.Reflection
    open FsWpfPlot.Models

    //let SurfacePlot (model: SurfacePlotModel) = 
    //    //printfn "ass: %s"  (Assembly.GetExecutingAssembly() .GetName().Name)
    //    //let resource =
    //    //    Assembly.GetExecutingAssembly().GetManifestResourceNames()
    //    //    |> Seq.find( fun res -> printfn "%A" res; res.EndsWith("xaml"))
    //    //    |> fun res -> sprintf "pack://application:,,,/FsWpfPlot;component/%s" res
    //    //    |> fun res -> printfn "%s" res; Uri(res, UriKind.Absolute)
        
    //    //printfn "%A %s %A"  resource (Assembly.GetExecutingAssembly().GetName().Name) resource

    //    //let win = Application.LoadComponent(new System.Uri("FsWpfPlot;component/Views/SurfacePlotContainer.xaml", UriKind.Relative)) :?> Window
    //    let k = System.IO.Packaging.PackUriHelper.UriSchemePack
    //    let resUri = System.Uri("/FsWpfPlot;component/Views/SurfacePlotContainer.xaml", System.UriKind.Relative)
    //    let win = Application.LoadComponent(resUri) :?> Window
    //    win.DataContext <- model
    //    Application().Run(win)
//    member this.X = "F#"
