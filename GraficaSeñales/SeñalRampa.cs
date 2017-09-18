using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraficaSeñales
{
    class SeñalRampa : Señal
    {
        public SeñalRampa(double tiempoInical, double tiempoFinal,
            double frecuenciaMuestreo)
        {
            TiempoInicial = tiempoInical;
            TiempoFinal = tiempoFinal;
            FrecuenciaMuestreo = frecuenciaMuestreo;
            señal = new PointCollection();
            construirSeñal();
        }

        public override double evaluar(double t)
        {
            double resultado = t;
            if (t < 0)
                resultado = 0;
            return resultado;

        }
    }
}
