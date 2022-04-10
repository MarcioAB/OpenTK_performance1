# GameWindow, GLControl and GLWpfControl performance compare
The idea of this solution is to compare the performance by single render of several millions of triangles using 3 differente UI user interfaces: GameWindow, WinForms GLControl and GLWpfControl. The 3 UI share the same model and the same GL basic stuff in 2 libraries ... work in progress
Result on my Radeon RX550/550 Series is: GLControl is a bit faster than GameWindow that is a bit faster of GLWpfControl.
