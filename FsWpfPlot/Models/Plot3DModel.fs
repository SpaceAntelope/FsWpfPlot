﻿namespace FsWpfPlot.Models

open System.ComponentModel
open System
open System.Windows
open System.Windows.Media.Media3D
open FsWpfPlot.Types
open FsWpfPlot.HelperFunctions
    
type Plot3DModel() = 
    // automatic get/setters won't cut it for dependency property binding
    let mutable funcZ : PlotFunction option = Some(fun x y -> (sin x*y) * 0.5) 
    let mutable data : Point3D[,] = null
    let mutable plotKind : PlotKind = PlotKind.Surface
    let mutable colorCoding : ColorCoding = ColorCoding.ByGradientY

    let propertyChangedHandler = new Event<PropertyChangedEventHandler,PropertyChangedEventArgs>()
    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member this.PropertyChanged: IEvent<PropertyChangedEventHandler,PropertyChangedEventArgs> = 
            propertyChangedHandler.Publish
    member this.raisePropertyChanged prop = propertyChangedHandler.Trigger(this, PropertyChangedEventArgs prop)
    
    member val Resolution = 91 with get,set
    member val MaxX = 5. with get,set
    member val MinX = 0. with get,set
    member val MaxY = 5. with get,set
    member val MinY = 0. with get,set
    //member val MaxZ = 1. with get,set
    //member val MinZ = 0. with get,set
    member this.FuncZ 
        with get() = funcZ
            and set(value) = 
                funcZ <- value
                this.raisePropertyChanged "FuncZ"
                this.raisePropertyChanged "Data"
        
    member this.setRangeX min max = this.MinX <- min ; this.MaxX <- max
    member this.getRangeX = this.MaxX - this.MinX
    member this.setRangeY min max = this.MinY <- min ; this.MaxY <- max
    member this.getRangeY = this.MaxY - this.MinY

    member this.MapToResolution row col =            
        let x' = this.MinX + (float col) / (float this.Resolution - 1.) * (this.MaxX - this.MinX)
        let y' = this.MinY + (float row) / (float this.Resolution - 1.) * (this.MaxY - this.MinY)
        Point(x',y')

    member this.Data 
        with get() = 
            match this.FuncZ with
            | Some(func) -> calculateDataAndMapToResolution (this.MapToResolution) func (this.Resolution)
            | None -> data
        and set(value) =
            data <- value
            this.raisePropertyChanged "Data"
        
    member this.ColorCoding
        with get() = colorCoding
        and set(value) = 
            colorCoding <- value
            this.raisePropertyChanged "PlotKind"

    member this.PlotKind 
        with get() = plotKind
        and  set(value: PlotKind) = 
                plotKind <- value
                this.raisePropertyChanged "PlotKind"

    member val PlotTitle = "3D Plot" with get, set