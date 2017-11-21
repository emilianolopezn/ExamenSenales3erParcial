using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Numerics;

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

        public SeñalPersonalizada convolucionar(Señal operando)
        {
            //Inicializar resultado
            SeñalPersonalizada resultado =
                new SeñalPersonalizada();
            resultado.TiempoInicial =
                this.TiempoInicial + operando.TiempoInicial;
            resultado.TiempoFinal =
                this.TiempoFinal + operando.TiempoFinal;
            resultado.FrecuenciaMuestreo =
                this.FrecuenciaMuestreo;
            resultado.señal = new PointCollection();
            double tiempoTotal = 
                resultado.TiempoFinal - resultado.TiempoInicial;
            double numeroMuestras = 
                tiempoTotal * resultado.FrecuenciaMuestreo;
            double xActual = resultado.TiempoInicial;
            for (int n = 0; n < numeroMuestras; n++)
            {
                double muestra = 0;
                for (int k = 0; k < operando.señal.Count; k++)
                {
                    if ((n - k) >= 0 && (n - k) < operando.señal.Count)
                    {
                        muestra +=
                        (this.señal[k].Y * operando.señal[n - k].Y) * //altura
                        (1.0 / resultado.FrecuenciaMuestreo);         //base
                    }
                    
                }
                resultado.señal.Add(new Point(xActual, muestra));
                xActual += 1.0 / resultado.FrecuenciaMuestreo;
            }

                return resultado;
        }

        public SeñalPersonalizada correlacionar(Señal operando)
        {
            //Inicializar resultado
            SeñalPersonalizada resultado =
                new SeñalPersonalizada();
            //this es la A , operando es la B
            resultado.TiempoInicial =
                this.TiempoInicial - 
                (operando.TiempoFinal - operando.TiempoInicial);
            resultado.TiempoFinal =
                this.TiempoFinal;
            resultado.FrecuenciaMuestreo =
                this.FrecuenciaMuestreo;
            resultado.señal = new PointCollection();
            double tiempoTotal =
                resultado.TiempoFinal - resultado.TiempoInicial;
            double numeroMuestras =
                tiempoTotal * resultado.FrecuenciaMuestreo;
            double xActual = resultado.TiempoInicial;
            for (int n = 0; n < numeroMuestras; n++)
            {
                double muestra = 0;
                for (int k = 0; k < operando.señal.Count; k++)
                {
                    if ( n >= 0 && n < this.señal.Count &&
                        (n - k) >= 0 && (n - k) < operando.señal.Count)
                    {
                        muestra +=
                        (this.señal[n].Y * operando.señal[n - k].Y) * //altura
                        (1.0 / resultado.FrecuenciaMuestreo);         //base
                    }

                }
                resultado.señal.Add(new Point(xActual, muestra));
                xActual += 1.0 / resultado.FrecuenciaMuestreo;
            }

            return resultado;
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

        public SeñalPersonalizada transformar()
        {
            SeñalPersonalizada resultado =
                new SeñalPersonalizada();
            resultado.señal = new PointCollection();

            resultado.TiempoInicial = this.TiempoInicial;
            resultado.TiempoFinal = this.TiempoFinal;
            resultado.FrecuenciaMuestreo = this.FrecuenciaMuestreo;
            double tiempoActual = this.TiempoInicial;
            double intervaloMuestras = 1 / this.FrecuenciaMuestreo;

            for(int k=0; k < this.señal.Count; k++)
            {
                Complex muestra = 0;
                for (int n=0; n < this.señal.Count; n++)
                {
                    muestra +=
                        this.señal[n].Y *
                        Complex.Exp(-2 *
                        Math.PI *
                        Complex.ImaginaryOne *
                        k * n / this.señal.Count);
                }
                resultado.señal.Add(
                    new Point(tiempoActual, muestra.Magnitude));
                tiempoActual += intervaloMuestras;
            }

            return resultado;
        }
    }
}
