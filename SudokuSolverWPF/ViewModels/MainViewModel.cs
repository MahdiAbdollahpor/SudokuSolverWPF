using SudokuSolverWPF.Converters;
using SudokuSolverWPF.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuSolverWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SudokuSolver _solver = new SudokuSolver();
        private ObservableCollection<SudokuCell> _cells;
        private bool _isSolving;
        private string _statusMessage;
        private Brush _statusColor;

        public MainViewModel()
        {
            InitializeEmptyPuzzle();
            SolveCommand = new RelayCommand(Solve, CanSolve);
            ClearCommand = new RelayCommand(Clear);
            StatusMessage = "Ready";
            StatusColor = Brushes.Black;
        }

        public ObservableCollection<SudokuCell> Cells
        {
            get => _cells;
            set
            {
                _cells = value;
                OnPropertyChanged(nameof(Cells));
            }
        }

        public bool IsSolving
        {
            get => _isSolving;
            set
            {
                _isSolving = value;
                OnPropertyChanged(nameof(IsSolving));
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public Brush StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        public ICommand SolveCommand { get; }
        public ICommand ClearCommand { get; }

        private void InitializeEmptyPuzzle()
        {
            Cells = new ObservableCollection<SudokuCell>();
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Cells.Add(new SudokuCell
                    {
                        Value = 0,
                        Row = row,
                        Column = col,
                        IsFixed = false
                    });
                }
            }
        }

        private void Solve(object parameter)
        {
            IsSolving = true;
            StatusMessage = "Solving...";
            StatusColor = Brushes.Blue;

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    int[,] puzzle = new int[9, 9];
                    foreach (var cell in Cells)
                    {
                        puzzle[cell.Row, cell.Column] = cell.IsFixed ? cell.Value : 0;
                    }

                    var solution = _solver.Solve(puzzle);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        for (int row = 0; row < 9; row++)
                        {
                            for (int col = 0; col < 9; col++)
                            {
                                var cell = Cells[row * 9 + col];
                                cell.Value = solution[row, col];
                            }
                        }
                    });

                    StatusMessage = "Solution found!";
                    StatusColor = Brushes.Green;
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error: {ex.Message}";
                    StatusColor = Brushes.Red;
                }
                finally
                {
                    IsSolving = false;
                }
            });
        }

        private bool CanSolve(object parameter) => !IsSolving;

        private void Clear(object parameter)
        {
            InitializeEmptyPuzzle();
            StatusMessage = "Cleared";
            StatusColor = Brushes.Black;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SudokuCell : INotifyPropertyChanged
    {
        private int _value;
        private bool _isFixed;

        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public int Row { get; set; }
        public int Column { get; set; }

        public bool IsFixed
        {
            get => _isFixed;
            set
            {
                if (_isFixed != value)
                {
                    _isFixed = value;
                    OnPropertyChanged(nameof(IsFixed));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}