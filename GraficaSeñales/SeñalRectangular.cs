using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraficaSeñales
{
    class SeñalRectangular : Señal
    {
        public SeñalRectangular(double tiempoInical, double tiempoFinal,
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
            
            double resultado = 0;
            if (Math.Abs(t) < 0.5)
            {
                resultado = 1;
            }
            else if (Math.Abs(t) == 0.5)
            {
                resultado = 0.5;
            }

            return resultado;
        }
    }
}
