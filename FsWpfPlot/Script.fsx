// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
#r "WindowsBase"
#r "PresentationCore"
#r "PresentationFramework"
#r @"bin/Debug/FsWpfPlot.dll"
#r @"bin/Debug/HelixToolkit.Wpf.dll"


open FsWpfPlot
open System.Windows
open FsWpfPlot.Models
open System

let model = SurfacePlotModel(MinX = 6., MinY = 8.)
let plot = FsWpfPlot.Views.SurfacePlotContainerProxy(model)
plot.Show()

