namespace FsWpfPlot

module Classes =

    open System
    open System.Windows
    open System.Windows.Controls
    open System.Windows.Markup    

    let (?) (this : Control) (prop : string) : 'T = this.FindName(prop) :?> 'T

    type XamlProxyBase(resourcePath : string) = 
        let resUri = System.Uri(resourcePath, System.UriKind.Relative)
        let win = Application.LoadComponent(resUri) :?> Window

        member this.Window with get() = win
        member this.Show() = Application().Run(win)
