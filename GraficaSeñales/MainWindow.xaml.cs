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
        double amplitudMaxima = 1;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void btnGraficar_Click(object sender, RoutedEventArgs e)
        {
            plnLineaGrafica.Points.Clear();
            double tiempoInicial =
                Double.Parse(txtTiempoInicial.Text);
            double tiempoFinal =
                Double.Parse(txtTiempoFinal.Text);
            double frecuenciaMuestreo =
                Double.Parse(txtFrecuenciaMuestreo.Text);
            switch (cbTipoSeñal.SelectedIndex)
            {
                case 0:
                    //Senoidal
                    double amplitud =
                        Double.Parse(txtAmplitud.Text);
                    double frecuencia =
                        Double.Parse(txtFrecuencia.Text);
                    double fase =
                        Double.Parse(txtFase.Text);
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
            if (señal != null)
            {
                //Aplicar operaciones
                señal.escalar
                    (Double.Parse(txtEscala.Text));
                //Graficar
                obtenerAmplitudMaxima();
                plnLineaGrafica.Points =
                    convertirSeñalAGrafica(señal.señal);
                lblMaxY.Text = Math.Round(amplitudMaxima).ToString();
                lblMinY.Text = Math.Round(-amplitudMaxima).ToString();
            }

            

            plnEjeX.Points.Clear();
            plnEjeX.Points.Add(new Point(tiempoInicial, 0));
            plnEjeX.Points.Add(new Point(tiempoFinal, 0));

            plnEjeY.Points.Clear();
            plnEjeY.Points.Add(new Point(0, amplitudMaxima));
            plnEjeY.Points.Add(new Point(0, -amplitudMaxima));

            plnEjeX.Points = convertirSeñalAGrafica(plnEjeX.Points);
            plnEjeY.Points = convertirSeñalAGrafica(plnEjeY.Points);
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
        }

        private PointCollection convertirSeñalAGrafica(PointCollection puntosSeñal)
        {
            PointCollection puntosGrafica = new PointCollection();
           
            double escalaX = scrContenedorGrafica.Width;
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
            if (grdConfiguracionSenoidal != null)
            {
                switch (cbTipoSeñal.SelectedIndex)
                {
                    case 0:
                        //Senoidal
                        grdConfiguracionSenoidal.Visibility =
                            Visibility.Visible;
                        break;
                    case 1:
                        //Rampa
                        grdConfiguracionSenoidal.Visibility =
                        Visibility.Hidden;
                        break;
                    case 2:
                        //Triangular
                        grdConfiguracionSenoidal.Visibility =
                        Visibility.Hidden;
                        break;
                    case 3:
                        //Rectangular
                        grdConfiguracionSenoidal.Visibility =
                        Visibility.Hidden;
                        break;
                }
            }
        }

        
    }
}
