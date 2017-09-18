using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraficaSeñales
{
    class SeñalSenoidal : Señal
    {
        public double Amplitud { get; set; } //Senoidal
        public double Fase { get; set; } //Senoidal
        public double Frecuencia { get; set; } //Senoidal
        

        public SeñalSenoidal(double amplitud, double fase, double frecuencia,
            double tiempoInicial, double tiempoFinal, double frecuenciaMuestreo)
        {
            Amplitud = amplitud;
            Fase = fase;
            Frecuencia = frecuencia;
            TiempoInicial = tiempoInicial;
            TiempoFinal = tiempoFinal;
            FrecuenciaMuestreo = frecuenciaMuestreo;
            señal = new PointCollection();
            construirSeñal();
        }
        
        override public double evaluar(double t)
        {
            double resultado =
                Amplitud * Math.Sin((2 * Math.PI * Frecuencia * t) + Fase);
            return resultado;
        }

        
    }
}
