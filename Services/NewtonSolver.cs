using System;

namespace six.Services
{
    public class NewtonSolver
    {
        
        public (double Root, int Iterations) Solve(
            Func<double, double> f,
            Func<double, double> df,
            double x0,
            double epsilon,
            int maxIterations = 1000)
        {
            double x = x0;

            for (int i = 0; i < maxIterations; i++)
            {
                double fx = f(x);
                double dfx = df(x);

                if (Math.Abs(dfx) < 1e-12)
                    throw new InvalidOperationException(
                        "Производная близка к нулю — метод Ньютона расходится");

                double xNext = x - fx / dfx;

                if (Math.Abs(xNext - x) < epsilon)
                    return (xNext, i + 1);

                x = xNext;
            }

            throw new InvalidOperationException(
                $"Метод не сошёлся за {maxIterations} итераций");
        }
    }
}