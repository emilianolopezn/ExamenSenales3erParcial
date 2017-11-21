using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GraficaSeñales
{
    class SeñalExponencial : Señal
    {
        public double Alpha { get; set; } //Senoidal

        public SeñalExponencial(double alpha,
            double tiempoInicial, double tiempoFinal, double frecuenciaMuestreo)
        {
            Alpha = alpha;
            TiempoInicial = tiempoInicial;
            TiempoFinal = tiempoFinal;
            FrecuenciaMuestreo = frecuenciaMuestreo;
            señal = new PointCollection();
            construirSeñal();
        }

        override public double evaluar(double t)
        {
            double resultado =
                Math.Exp(Alpha * t);
            return resultado;
        }
    }
}
