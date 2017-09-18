using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraficaSeñales
{
    abstract class Señal
    {
        public double TiempoInicial { get; set; }
        public double TiempoFinal { get; set; }
        public double FrecuenciaMuestreo { get; set; }
        public PointCollection señal;

        abstract public double evaluar(double t);

        public void construirSeñal()
        {
            señal.Clear();
            double intervaloMuestreo = 1 / FrecuenciaMuestreo;
            for (double t = TiempoInicial;
                t <= TiempoFinal;
                t += intervaloMuestreo)
            {
                señal.Add(
                    new Point(t, evaluar(t)));
            }
        }

        public void escalar(double factor)
        {
            PointCollection señalEscalada = 
                new PointCollection();
            foreach(Point muestra in señal)
            {
                señalEscalada.Add(
                    new Point(muestra.X,
                    muestra.Y * factor));
            }
            señal = señalEscalada;
        }
    }
}
