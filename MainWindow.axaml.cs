using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace sistema_acceso;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void BtnCapturar_Click(object sender, RoutedEventArgs e)
    {
        // Referencias a los controles
        var btn = (Button)sender;
        var txt = this.FindControl<TextBlock>("txtStatus");

        btn.IsEnabled = false;
        txt.Text = "Esperando dedo en el escáner...";

        // Ejecución en segundo plano para no congelar la ventana
        byte[]? data = await Task.Run(() =>
        {
            byte[]? result = null;

            // Bucle bloqueante: no sale hasta que el escáner devuelve datos
            while (result == null)
            {
                result = Scanner.GetRawImage();

                // Pausa breve para no consumir el 100% del CPU
                Thread.Sleep(100);
            }
            return result;
        });

        // Guardar el archivo una vez terminada la lectura
        string path = "huella.bin";
        await File.WriteAllBytesAsync(path, data);

        txt.Text = "¡Huella capturada y guardada!";
        btn.IsEnabled = true;
    }
}
