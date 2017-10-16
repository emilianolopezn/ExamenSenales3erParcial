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

        public void desplazarEnAmplitud(double cantidadDesplazamiento)
        {
            PointCollection señalEscalada =
                new PointCollection();
            foreach (Point muestra in señal)
            {
                señalEscalada.Add(
                    new Point(muestra.X,
                    muestra.Y + cantidadDesplazamiento));
            }
            señal = señalEscalada;
        }

        public void desplazarEnTiempo(double tiempo)
        {
            //f(t) = f(t + t0);
            PointCollection señalDesplazada =
                new PointCollection();
            foreach (Point muestra in señal)
            {
                int indice = (int)
                    (((muestra.X - TiempoInicial) * FrecuenciaMuestreo) +
                    (tiempo * FrecuenciaMuestreo));
                
                if (indice >= 0 && 
                    indice < (FrecuenciaMuestreo *
                    (TiempoFinal - TiempoInicial)))
                {
                    //Indice estra en el rango válido
                    señalDesplazada.Add(
                    new Point(muestra.X,
                    señal[indice].Y));
                }
                else
                {
                    //Indice está fuera rango
                    señalDesplazada.Add(
                    new Point(muestra.X,
                    0));
                }

                
            }
            señal = señalDesplazada;
        }

        public SeñalPersonalizada sumar(Señal sumando)
        {
            if (this.TiempoInicial == sumando.TiempoInicial &&
                this.TiempoFinal == sumando.TiempoFinal &&
                this.FrecuenciaMuestreo == sumando.FrecuenciaMuestreo )
            {
                SeñalPersonalizada resultado =
                    new SeñalPersonalizada();
                resultado.TiempoInicial = this.TiempoInicial;
                resultado.TiempoFinal = this.TiempoFinal;
                resultado.FrecuenciaMuestreo = this.FrecuenciaMuestreo;
                resultado.señal = new PointCollection();
                for (int i = 0; i < this.señal.Count; i++)
                {
                    Point muestraResultante = new Point();
                    muestraResultante.X = this.señal[i].X;
                    muestraResultante.Y =
                        this.señal[i].Y +
                        sumando.señal[i].Y;
                    resultado.señal.Add(muestraResultante);
                }
                return resultado;
            }
            return null;
        }

        public SeñalPersonalizada multipllicar(Señal multiplicando)
        {
            if (this.TiempoInicial == multiplicando.TiempoInicial &&
                this.TiempoFinal == multiplicando.TiempoFinal &&
                this.FrecuenciaMuestreo == multiplicando.FrecuenciaMuestreo)
            {
                SeñalPersonalizada resultado =
                    new SeñalPersonalizada();
                resultado.TiempoInicial = this.TiempoInicial;
                resultado.TiempoFinal = this.TiempoFinal;
                resultado.FrecuenciaMuestreo = this.FrecuenciaMuestreo;
                resultado.señal = new PointCollection();
                for (int i = 0; i < this.señal.Count; i++)
                {
                    Point muestraResultante = new Point();
                    muestraResultante.X = this.señal[i].X;
                    muestraResultante.Y =
                        this.señal[i].Y *
                        multiplicando.señal[i].Y;
                    resultado.señal.Add(muestraResultante);
                }
                return resultado;
            }
            return null;
        }
    }
}
