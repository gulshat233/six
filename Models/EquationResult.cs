using System;

namespace six.Models
{
    public class EquationResult
    {
        public int Id { get; set; }
        public double X0 { get; set; }
        public double Epsilon { get; set; }
        public double Root { get; set; }
        public int Iterations { get; set; }
        public DateTime Timestamp { get; set; }
        public string EquationText { get; set; }
    }
}