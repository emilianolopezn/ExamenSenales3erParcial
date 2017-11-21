using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using Microsoft.Win32;

using NAudio.Wave;


namespace GraficaSeñales
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Señal señal;
        Señal señalDos;
        SeñalPersonalizada resultado;

        double amplitudMaxima = 1;

        public MainWindow()
        {
            InitializeComponent();

        }

        //Ahora es el analizar 
        private void btnGraficar_Click(object sender, RoutedEventArgs e)
        {
            resultado = null;
            //Borra todos los puntos de la gráfica de la señal
            plnLineaGrafica.Points.Clear();
            plnLineaGrafica2.Points.Clear();

            

            //Aquí se va a construir la señal
            //(Leer el audio)
            var ruta = txtAudio.Text;
            AudioFileReader reader = new AudioFileReader(ruta);

            double tiempoInicial = 0;
            double tiempoFinal = reader.TotalTime.Milliseconds / 1000.0;
            double frecuenciaMuestreo = reader.WaveFormat.SampleRate;

            txtTiempoInicial.Text = tiempoInicial.ToString();
            txtTiempoFinal.Text = tiempoFinal.ToString();
            txtFrecuenciaMuestreo.Text = frecuenciaMuestreo.ToString();

            double duracion =
                tiempoFinal - tiempoInicial;

            señal = new SeñalPersonalizada();
            señal.señal = new PointCollection();
            señal.TiempoInicial = tiempoInicial;
            señal.TiempoFinal = tiempoFinal;
            señal.FrecuenciaMuestreo = frecuenciaMuestreo;

            var readBuffer = new float[reader.WaveFormat.Channels];
            int muestrasLeidas = 1;
            double intervaloMuestreo = 1 / frecuenciaMuestreo;
            double tiempoActual = tiempoInicial;
            do
            {
                muestrasLeidas = 
                    reader.Read(readBuffer, 0, reader.WaveFormat.Channels);
                if (muestrasLeidas > 0)
                {
                    double max = readBuffer.Take(muestrasLeidas).Max();
                    señal.señal.Add(new Point(tiempoActual, max));
                }
                tiempoActual += intervaloMuestreo;
            } while (muestrasLeidas > 0);

            plnLineaGrafica.Points =
                convertirSeñalAGrafica(señal.señal, duracion, tiempoInicial);

            //Graficar ejes
            plnEjeX.Points.Clear();
            plnEjeX.Points.Add(new Point(tiempoInicial, 0));
            plnEjeX.Points.Add(new Point(tiempoFinal, 0));

            plnEjeY.Points.Clear();
            plnEjeY.Points.Add(new Point(0, amplitudMaxima));
            plnEjeY.Points.Add(new Point(0, -amplitudMaxima));


            var ejeX = convertirSeñalAGrafica(plnEjeX.Points,duracion, tiempoInicial);
            var ejeY = convertirSeñalAGrafica(plnEjeY.Points,duracion, tiempoInicial);
            plnEjeX.Points = ejeX;
            plnEjeY.Points = ejeY;
            
            //plnEjeX1.Points = ejeX;
            //plnEjeY1.Points = ejeY;

            grdEtiquetas.Children.Clear();
            for (int i = Convert.ToInt32(tiempoInicial); i <= tiempoFinal; i++)
            {
                var etiqueta = new TextBlock();
                var posicion = new PointCollection();
                posicion.Add(new Point(i, 0));
                posicion = convertirSeñalAGrafica(posicion, duracion, tiempoInicial);
                etiqueta.Text = i.ToString();
                etiqueta.Margin = new Thickness(posicion[0].X, posicion[0].Y,0,0);
                etiqueta.Opacity = 0.5;
                grdEtiquetas.Children.Add(etiqueta);
            }

            transformar();
        }

        private void obtenerAmplitudMaxima() {
            amplitudMaxima=1;
            foreach(Point punto in señal.señal)
            {
                if (Math.Abs(punto.Y) > amplitudMaxima)
                {
                    amplitudMaxima = Math.Abs(punto.Y);
                }
            }
            /*foreach (Point punto in señalDos.señal)
            {
                if (Math.Abs(punto.Y) > amplitudMaxima)
                {
                    amplitudMaxima = Math.Abs(punto.Y);
                }
            }*/
            if (resultado != null)
            {
                foreach (Point punto in resultado.señal)
                {
                    if (Math.Abs(punto.Y) > amplitudMaxima)
                    {
                        amplitudMaxima = Math.Abs(punto.Y);
                    }
                }
            }
            

        }

        private PointCollection convertirSeñalAGrafica(PointCollection puntosSeñal, 
            double duracion, double tiempoInicial)
        {
            PointCollection puntosGrafica = new PointCollection();
            
            double escalaX = scrContenedorGrafica.Width / duracion;
            double escalaY = (scrContenedorGrafica.Height - 20) / 2;
            
            foreach(Point muestra in puntosSeñal)
            {
                puntosGrafica.Add
                    (new Point(escalaX * (muestra.X - tiempoInicial),
                    -(muestra.Y / amplitudMaxima) * escalaY + escalaY));
                
            }

            return puntosGrafica;
        }

       


        private void sumar()
        {
            if (señal != null && señalDos != null)
            {
                double tiempoInicial =
                    Double.Parse(txtTiempoInicial.Text);
                double duracion =
                    Double.Parse(txtTiempoFinal.Text) -
                    Double.Parse(txtTiempoInicial.Text);
                resultado = señal.sumar(señalDos);
                if (resultado != null)
                {
                    //Grafico
                    obtenerAmplitudMaxima();
                    plnLineaGraficaResultado.Points =
                        convertirSeñalAGrafica(resultado.señal,duracion, tiempoInicial);
                    lblMaxY_Resultado.Text = Math.Round(amplitudMaxima).ToString();
                    lblMinY_Resultado.Text = Math.Round(-amplitudMaxima).ToString();
                    grdEtiquetas2.Children.Clear();
                    for (int i = Convert.ToInt32(Double.Parse(txtTiempoInicial.Text)); i <= Double.Parse(txtTiempoFinal.Text); i++)
                    {
                        var etiqueta = new TextBlock();
                        var posicion = new PointCollection();
                        posicion.Add(new Point(i, 0));
                        posicion = convertirSeñalAGrafica(posicion,duracion, tiempoInicial);
                        etiqueta.Text = i.ToString();
                        etiqueta.Margin = new Thickness(posicion[0].X, posicion[0].Y, 0, 0);
                        etiqueta.Opacity = 0.5;
                        grdEtiquetas2.Children.Add(etiqueta);
                    }
                    graficarEjesResultado(resultado.TiempoInicial,
                        resultado.TiempoFinal);
                }
            }
        }

        private void multiplicar()
        {
            if (señal != null && señalDos != null)
            {
                double tiempoInicial =
                    Double.Parse(txtTiempoInicial.Text);
                double duracion =
                    Double.Parse(txtTiempoFinal.Text) -
                    Double.Parse(txtTiempoInicial.Text);
                resultado = señal.multipllicar(señalDos);
                if (resultado != null)
                {
                    //Grafico
                    obtenerAmplitudMaxima();
                    plnLineaGraficaResultado.Points =
                        convertirSeñalAGrafica(resultado.señal, duracion, tiempoInicial);
                    lblMaxY_Resultado.Text = Math.Round(amplitudMaxima).ToString();
                    lblMinY_Resultado.Text = Math.Round(-amplitudMaxima).ToString();
                    grdEtiquetas2.Children.Clear();
                    for (int i = Convert.ToInt32(Double.Parse(txtTiempoInicial.Text)); i <= Double.Parse(txtTiempoFinal.Text); i++)
                    {
                        var etiqueta = new TextBlock();
                        var posicion = new PointCollection();
                        posicion.Add(new Point(i, 0));
                        posicion = convertirSeñalAGrafica(posicion, duracion, tiempoInicial);
                        etiqueta.Text = i.ToString();
                        etiqueta.Margin = new Thickness(posicion[0].X, posicion[0].Y, 0, 0);
                        etiqueta.Opacity = 0.5;
                        grdEtiquetas2.Children.Add(etiqueta);
                    }
                    graficarEjesResultado(resultado.TiempoInicial,
                        resultado.TiempoFinal);
                }
            }
        }

        private void convolucionar()
        {
            if (señal != null && señalDos != null)
            {
                double tiempoInicial = 
                    Double.Parse(txtTiempoInicial.Text);
                double tiempoFinal =
                    Double.Parse(txtTiempoFinal.Text);
                double tiempoInicialConvolucion =
                    tiempoInicial + tiempoInicial;
                double tiempoFinalConvolucion =
                    tiempoFinal + tiempoFinal;
                double duracion =
                     (tiempoFinal + tiempoFinal) -
                     (tiempoInicial + tiempoInicial);
                resultado = señal.convolucionar(señalDos);
                if (resultado != null)
                {
                    //Grafico
                    obtenerAmplitudMaxima();
                    plnLineaGraficaResultado.Points =
                        convertirSeñalAGrafica(resultado.señal,duracion, tiempoInicialConvolucion);
                    lblMaxY_Resultado.Text = Math.Round(amplitudMaxima).ToString();
                    lblMinY_Resultado.Text = Math.Round(-amplitudMaxima).ToString();

                    grdEtiquetas2.Children.Clear();
                    for (int i = Convert.ToInt32(tiempoInicialConvolucion); i <= tiempoFinalConvolucion; i++)
                    {
                        var etiqueta = new TextBlock();
                        var posicion = new PointCollection();
                        posicion.Add(new Point(i, 0));
                        posicion = convertirSeñalAGrafica(posicion, duracion, tiempoInicialConvolucion);
                        etiqueta.Text = i.ToString();
                        etiqueta.Margin = new Thickness(posicion[0].X, posicion[0].Y, 0, 0);
                        etiqueta.Opacity = 0.5;
                        grdEtiquetas2.Children.Add(etiqueta);
                    }
                    graficarEjesResultado(resultado.TiempoInicial,
                        resultado.TiempoFinal);
                }
            }
        }

        private void correlacionar() {
            if (señal != null && señalDos != null) {
                double tiempoInicial =
                    Double.Parse(txtTiempoInicial.Text);
                double tiempoFinal =
                    Double.Parse(txtTiempoFinal.Text);

                double duracion =
                     tiempoFinal - tiempoInicial;

                resultado = señal.correlacionar(señalDos);
                if (resultado != null) {
                    //Grafico
                    obtenerAmplitudMaxima();
                    plnLineaGraficaResultado.Points =
                        convertirSeñalAGrafica(resultado.señal, duracion, tiempoInicial);
                    lblMaxY_Resultado.Text = Math.Round(amplitudMaxima).ToString();
                    lblMinY_Resultado.Text = Math.Round(-amplitudMaxima).ToString();

                    grdEtiquetas2.Children.Clear();
                    for (int i = Convert.ToInt32(tiempoInicial); i <= tiempoFinal; i++) {
                        var etiqueta = new TextBlock();
                        var posicion = new PointCollection();
                        posicion.Add(new Point(i, 0));
                        posicion = convertirSeñalAGrafica(posicion, duracion, tiempoInicial);
                        etiqueta.Text = i.ToString();
                        etiqueta.Margin = new Thickness(posicion[0].X, posicion[0].Y, 0, 0);
                        etiqueta.Opacity = 0.5;
                        grdEtiquetas2.Children.Add(etiqueta);
                    }
                    graficarEjesResultado(resultado.TiempoInicial,
                        resultado.TiempoFinal);
                }
            }
        }

        private void transformar()
        {
            if (señal != null)
            {
                double tiempoInicial =
                    Double.Parse(txtTiempoInicial.Text);
                double tiempoFinal =
                    Double.Parse(txtTiempoFinal.Text);
                double frecuenciaMuestreo =
                      Double.Parse(txtFrecuenciaMuestreo.Text);
                double duracion =
                     tiempoFinal - tiempoInicial;
                resultado = señal.transformar();
                if (resultado != null)
                {
                    int indiceValorMaximo = 0;
                    double valorMaximo = 0.0;
                    for (int n = 0; n < resultado.señal.Count / 2; n++)
                    {
                        if (resultado.señal[n].Y > valorMaximo)
                        {
                            valorMaximo = resultado.señal[n].Y;
                            indiceValorMaximo = n;
                        }
                    }
                    double frecuenciaFundamental =
                        indiceValorMaximo * frecuenciaMuestreo /
                        señal.señal.Count;
                    
                    
                        lblFrecuenciaFundamental.Text =
                            frecuenciaFundamental.ToString() + " Hz";
                    
                    if(frecuenciaFundamental> 261 && frecuenciaFundamental < 293)
                    {
                        lblFrecuenciaFundamental.Text = "Do";
                    }
                    else if(frecuenciaFundamental> 293 && frecuenciaFundamental < 329)
                    {
                        lblFrecuenciaFundamental.Text = "Re";
                    }
                    else if(frecuenciaFundamental >329 && frecuenciaFundamental < 349)
                    {
                        lblFrecuenciaFundamental.Text = "Mi";
                    }
                    else if(frecuenciaFundamental > 349 && frecuenciaFundamental < 392)
                    {
                        lblFrecuenciaFundamental.Text = "Fa";
                    }
                    else if(frecuenciaFundamental >392 && frecuenciaFundamental < 440)
                    {
                        lblFrecuenciaFundamental.Text = "Sol";
                    }
                    else if (frecuenciaFundamental > 440 && frecuenciaFundamental < 493)
                    {
                        lblFrecuenciaFundamental.Text = "La";
                    }
                    else if(frecuenciaFundamental >493 && frecuenciaFundamental < 523)
                    {
                        lblFrecuenciaFundamental.Text = "SI";
                    }

                }

                if (resultado != null)
                {
                    //Grafico
                    obtenerAmplitudMaxima();
                    plnLineaGraficaResultado.Points =
                        convertirSeñalAGrafica(resultado.señal, duracion, tiempoInicial);
                    lblMaxY_Resultado.Text = Math.Round(amplitudMaxima).ToString();
                    lblMinY_Resultado.Text = Math.Round(-amplitudMaxima).ToString();

                    grdEtiquetas2.Children.Clear();
                    for (int i = Convert.ToInt32(tiempoInicial); i <= tiempoFinal; i++)
                    {
                        var etiqueta = new TextBlock();
                        var posicion = new PointCollection();
                        posicion.Add(new Point(i, 0));
                        posicion = convertirSeñalAGrafica(posicion, duracion, tiempoInicial);
                        etiqueta.Text = i.ToString();
                        etiqueta.Margin = new Thickness(posicion[0].X, posicion[0].Y, 0, 0);
                        etiqueta.Opacity = 0.5;
                        //grdEtiquetas2.Children.Add(etiqueta);
                    }
                    graficarEjesResultado(resultado.TiempoInicial,
                        resultado.TiempoFinal);
                }
            }
        }

      

        void graficarEjesResultado(double tiempoInicial,
            double tiempoFinal)
        {
            double duracion = tiempoFinal - tiempoInicial;

            //Graficar ejes
            plnEjeX1.Points.Clear();
            plnEjeX1.Points.Add(new Point(tiempoInicial, 0));
            plnEjeX1.Points.Add(new Point(tiempoFinal, 0));

            plnEjeY1.Points.Clear();
            plnEjeY1.Points.Add(new Point(0, amplitudMaxima));
            plnEjeY1.Points.Add(new Point(0, -amplitudMaxima));


            var ejeX = convertirSeñalAGrafica(plnEjeX1.Points, duracion, tiempoInicial);
            var ejeY = convertirSeñalAGrafica(plnEjeY1.Points, duracion, tiempoInicial);
            plnEjeX1.Points = ejeX;
            plnEjeY1.Points = ejeY;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtAudio.Text = openFileDialog.FileName;
            }
        }
    }
}
