using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using six.Commands;
using six.Database;
using six.Models;
using six.Services;

namespace six.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly EquationRepository _repository;
        private readonly NewtonSolver _solver = new NewtonSolver();

        private const string EquationName = "x² − 4 = 0";

        private double _x0 = 3.0;
        private double _epsilon = 0.0001;
        private double _resultRoot;
        private int _iterations;
        private string _errorMessage = "";

        public MainViewModel()
        {
            var db = new DatabaseHelper();
            _repository = new EquationRepository(db);

            SolveCommand = new RelayCommand(async _ => await SolveAsync());
            ClearHistoryCommand = new RelayCommand(async _ => await ClearAsync());

            _ = LoadHistoryAsync();
        }

        public string EquationText => EquationName;

        public double X0
        {
            get => _x0;
            set { _x0 = value; OnPropertyChanged(); }
        }

        public double Epsilon
        {
            get => _epsilon;
            set { _epsilon = value; OnPropertyChanged(); }
        }

        public double ResultRoot
        {
            get => _resultRoot;
            set { _resultRoot = value; OnPropertyChanged(); }
        }

        public int Iterations
        {
            get => _iterations;
            set { _iterations = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ObservableCollection<EquationResult> History { get; } = new();

        public ICommand SolveCommand { get; }
        public ICommand ClearHistoryCommand { get; }

        private async Task SolveAsync()
        {
            try
            {
                ErrorMessage = "";

                if (Epsilon <= 0)
                    throw new ArgumentException("Точность ε должна быть больше нуля");

                if (Epsilon >= 0.1)
                    throw new ArgumentException("Точность ε должна быть меньше 0.1 ");
                
                if (Math.Abs(2 * X0) < 1e-12)
                    throw new ArgumentException("Начальное приближение x₀ = 0 недопустимо - производная равна нулю");

                // f(x)  = x² − 4
                // f'(x) = 2x
                Func<double, double> f = x => x * x - 4;
                Func<double, double> df = x => 2 * x;

                var (root, iterations) = _solver.Solve(f, df, X0, Epsilon);

                ResultRoot = root;
                Iterations = iterations;

                await _repository.SaveAsync(new EquationResult
                {
                    X0 = X0,
                    Epsilon = Epsilon,
                    Root = root,
                    Iterations = iterations,
                    Timestamp = DateTime.Now,
                    EquationText = EquationName
                });

                await LoadHistoryAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        private async Task LoadHistoryAsync()
        {
            var all = await _repository.GetAllAsync();
            History.Clear();
            foreach (var item in all)
                History.Add(item);
        }

        private async Task ClearAsync()
        {
            try
            {
                await _repository.ClearAllAsync();
                await LoadHistoryAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}