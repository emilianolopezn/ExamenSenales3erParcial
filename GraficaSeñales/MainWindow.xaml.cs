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
            grdConfiguracion.Children.Add(new ConfiguracionSenoidal());
            grdConfiguracion_2.Children.Add(new ConfiguracionSenoidal());
        }

        private void btnGraficar_Click(object sender, RoutedEventArgs e)
        {
            resultado = null;
            //Borra todos los puntos de la gráfica de la señal
            plnLineaGrafica.Points.Clear();
            plnLineaGrafica2.Points.Clear();

            double tiempoInicial =
                Double.Parse(txtTiempoInicial.Text);
            double tiempoFinal =
                Double.Parse(txtTiempoFinal.Text);
            double frecuenciaMuestreo =
                Double.Parse(txtFrecuenciaMuestreo.Text);

            //Evaluar valor del combo box que representa 
            //el tipo de señal
            switch (cbTipoSeñal.SelectedIndex)
            {
                case 0:
                    //Senoidal
                    double amplitud =
                        Double.Parse
                        (((ConfiguracionSenoidal)grdConfiguracion.Children[0]).
                        txtAmplitud.Text);
                    double frecuencia =
                        Double.Parse
                        (((ConfiguracionSenoidal)grdConfiguracion.Children[0]).
                        txtFrecuencia.Text);
                    double fase =
                        Double.Parse
                        (((ConfiguracionSenoidal)grdConfiguracion.Children[0]).
                        txtFase.Text);
                    señal =
                        new SeñalSenoidal(amplitud, fase, frecuencia,
                            tiempoInicial, tiempoFinal, frecuenciaMuestreo);
                    break;
                case 1:
                    //Rampa
                    señal =
                        new SeñalRampa(tiempoInicial, tiempoFinal,
                        frecuenciaMuestreo);
                    break;
                case 2:
                    //Triangular
                    señal =
                        new SeñalTriangular(tiempoInicial, tiempoFinal,
                        frecuenciaMuestreo);
                    break;
                case 3:
                    //Rectangular
                    señal =
                        new SeñalRectangular(tiempoInicial, tiempoFinal,
                        frecuenciaMuestreo);
                    break;
                default:
                    señal = null;
                    break;
            }
            switch(cbTipoSeñal_2.SelectedIndex)
            {
                case 0:
                    //Senoidal
                    double amplitud =
                        Double.Parse(
                            ((ConfiguracionSenoidal)(grdConfiguracion_2.Children[0]))
                            .txtAmplitud.Text);
                    double fase =
                        Double.Parse(
                            ((ConfiguracionSenoidal)(grdConfiguracion_2.Children[0]))
                            .txtFase.Text);
                    double frecuencia =
                        Double.Parse(
                            ((ConfiguracionSenoidal)(grdConfiguracion_2.Children[0]))
                            .txtFrecuencia.Text);
                    señalDos =
                        new SeñalSenoidal(amplitud, fase, frecuencia,
                        tiempoInicial, tiempoFinal, frecuenciaMuestreo);
                    break;
                case 1:
                    //Rampa
                    señalDos =
                        new SeñalRampa
                        (tiempoInicial, tiempoFinal, frecuenciaMuestreo);
                    break;
                case 2:
                    //Triangular
                    señalDos =
                        new SeñalTriangular
                        (tiempoInicial, tiempoFinal, frecuenciaMuestreo);
                    break;
                case 3:
                    //Rectangular
                    señalDos =
                        new SeñalRectangular
                        (tiempoInicial, tiempoFinal, frecuenciaMuestreo);
                    break;
                default:
                    señalDos = null;
                    break;
            }



            if (señal != null)
            {
                //Aplicar operaciones
                señal.escalar
                    (Double.Parse(txtEscala.Text));
                señal.desplazarEnTiempo(
                    Double.Parse(txtDesplazamientoTiempo.Text));
                señal.desplazarEnAmplitud(
                    Double.Parse(txtDesplazamientoAmplitud.Text));

                if (señalDos != null)
                {
                    //Aplicar operaciones
                    señalDos.
                        escalar(Double.Parse(txtEscala_2.Text));
                    señalDos.
                        desplazarEnTiempo(Double.Parse(txtDesplazamientoTiempo_2.Text));
                    señalDos.
                        desplazarEnAmplitud(Double.Parse(txtDesplazamientoAmplitud_2.Text));
                    //*********************Graficar***********
                    obtenerAmplitudMaxima();
                    plnLineaGrafica.Points =
                        convertirSeñalAGrafica(señal.señal);
                    plnLineaGrafica2.Points =
                        convertirSeñalAGrafica(señalDos.señal);

                    lblMaxY.Text = Math.Round(amplitudMaxima).ToString();
                    lblMinY.Text = Math.Round(-amplitudMaxima).ToString();
 
                }
                
            }

            

            

            plnEjeX.Points.Clear();
            plnEjeX.Points.Add(new Point(tiempoInicial, 0));
            plnEjeX.Points.Add(new Point(tiempoFinal, 0));

            plnEjeY.Points.Clear();
            plnEjeY.Points.Add(new Point(0, amplitudMaxima));
            plnEjeY.Points.Add(new Point(0, -amplitudMaxima));

            var ejeX = convertirSeñalAGrafica(plnEjeX.Points);
            var ejeY = convertirSeñalAGrafica(plnEjeY.Points);
            plnEjeX.Points = ejeX;
            plnEjeY.Points = ejeY;
            plnEjeX1.Points = ejeX;
            plnEjeY1.Points = ejeY;

            grdEtiquetas.Children.Clear();
            for (int i = Convert.ToInt32(tiempoInicial); i <= tiempoFinal; i++)
            {
                var etiqueta = new TextBlock();
                var posicion = new PointCollection();
                posicion.Add(new Point(i, 0));
                posicion = convertirSeñalAGrafica(posicion);
                etiqueta.Text = i.ToString();
                etiqueta.Margin = new Thickness(posicion[0].X, posicion[0].Y,0,0);
                etiqueta.Opacity = 0.5;
                grdEtiquetas.Children.Add(etiqueta);
            }
      

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
            foreach (Point punto in señalDos.señal)
            {
                if (Math.Abs(punto.Y) > amplitudMaxima)
                {
                    amplitudMaxima = Math.Abs(punto.Y);
                }
            }
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

        private PointCollection convertirSeñalAGrafica(PointCollection puntosSeñal)
        {
            PointCollection puntosGrafica = new PointCollection();
            double duracion = Double.Parse(txtTiempoFinal.Text) - Double.Parse(txtTiempoInicial.Text);
            double escalaX = scrContenedorGrafica.Width / duracion;
            double escalaY = (scrContenedorGrafica.Height - 20) / 2;
            
            foreach(Point muestra in puntosSeñal)
            {
                puntosGrafica.Add
                    (new Point(escalaX * (muestra.X - señal.TiempoInicial),
                    -(muestra.Y / amplitudMaxima) * escalaY + escalaY));
                
            }

            return puntosGrafica;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grdConfiguracion != null)
            {
                grdConfiguracion.Children.Clear();
                switch (cbTipoSeñal.SelectedIndex)
                {
                    case 0:
                        //Senoidal
                        grdConfiguracion.Children.Add(new ConfiguracionSenoidal());
                        break;
                    case 1:
                        //Rampa
                        break;
                    case 2:
                        //Triangular
                        break;
                    case 3:
                        //Rectangular
                        break;
                }
            }
        }

        private void cbTipoSeñal_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grdConfiguracion_2 != null)
            {
                grdConfiguracion_2.Children.Clear();
                switch (cbTipoSeñal_2.SelectedIndex)
                {
                    case 0:
                        //Senoidal
                        grdConfiguracion_2.Children.Add(new ConfiguracionSenoidal());
                        break;
                    case 1:
                        //Rampa
                        break;
                    case 2:
                        //Triangular
                        break;
                    case 3:
                        //Rectangular
                        break;
                }
            }
        }

        private void btnSumar_Click(object sender, RoutedEventArgs e)
        {
            if (señal != null && señalDos != null)
            {
                resultado = señal.sumar(señalDos);
                if (resultado != null)
                {
                    //Grafico
                    obtenerAmplitudMaxima();
                    plnLineaGraficaResultado.Points =
                        convertirSeñalAGrafica(resultado.señal);
                    lblMaxY_Resultado.Text = Math.Round(amplitudMaxima).ToString();
                    lblMinY_Resultado.Text = Math.Round(-amplitudMaxima).ToString();
                    grdEtiquetas2.Children.Clear();
                    for (int i = Convert.ToInt32(Double.Parse(txtTiempoInicial.Text)); i <= Double.Parse(txtTiempoFinal.Text); i++)
                    {
                        var etiqueta = new TextBlock();
                        var posicion = new PointCollection();
                        posicion.Add(new Point(i, 0));
                        posicion = convertirSeñalAGrafica(posicion);
                        etiqueta.Text = i.ToString();
                        etiqueta.Margin = new Thickness(posicion[0].X, posicion[0].Y, 0, 0);
                        etiqueta.Opacity = 0.5;
                        grdEtiquetas2.Children.Add(etiqueta);
                    }
                }
            }
        }

        private void btnMultiplicar_Click(object sender, RoutedEventArgs e)
        {
            if (señal != null && señalDos != null)
            {
                resultado = señal.multipllicar(señalDos);
                if (resultado != null)
                {
                    //Grafico
                    obtenerAmplitudMaxima();
                    plnLineaGraficaResultado.Points =
                        convertirSeñalAGrafica(resultado.señal);
                    lblMaxY_Resultado.Text = Math.Round(amplitudMaxima).ToString();
                    lblMinY_Resultado.Text = Math.Round(-amplitudMaxima).ToString();
                    grdEtiquetas2.Children.Clear();
                    for (int i = Convert.ToInt32(Double.Parse(txtTiempoInicial.Text)); i <= Double.Parse(txtTiempoFinal.Text); i++)
                    {
                        var etiqueta = new TextBlock();
                        var posicion = new PointCollection();
                        posicion.Add(new Point(i, 0));
                        posicion = convertirSeñalAGrafica(posicion);
                        etiqueta.Text = i.ToString();
                        etiqueta.Margin = new Thickness(posicion[0].X, posicion[0].Y, 0, 0);
                        etiqueta.Opacity = 0.5;
                        grdEtiquetas2.Children.Add(etiqueta);
                    }
                }
            }
        }
    }
}
