using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GraficaSeñales
{
    class SeñalTriangular : Señal
    {
        public SeñalTriangular(double tiempoInical, double tiempoFinal,
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
            double absT = Math.Abs(t);
            if (absT < 1.0)
            {
                resultado = 1 - absT;
            }


            return resultado;

        }
    }
}
