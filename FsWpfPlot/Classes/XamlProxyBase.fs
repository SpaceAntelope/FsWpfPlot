namespace FsWpfPlot

module Classes =

    open System
    open System.Windows
    open System.Windows.Controls
    open System.Windows.Markup    
    open System.ComponentModel

    let (?) (this : Control) (prop : string) : 'T = 
        let ctrl = this.FindName(prop) :?> 'T 
        if isNull ctrl then raise (System.Exception(sprintf "Could not find property with name %s" prop))
        else ctrl            

    type XamlProxyBase(resourcePath : string, model : INotifyPropertyChanged) = 
        let resUri = System.Uri(resourcePath, System.UriKind.Relative)
        let win = Application.LoadComponent(resUri) :?> Window
        do
            win.DataContext <- model
            
        member this.Window with get() = win
        member this.Show() = Application().Run(win)
