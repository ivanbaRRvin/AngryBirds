using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProjectileSimulator
{
    public partial class MainWindow : Window
    {
        const double g = 9.81;
        DispatcherTimer timer;

        double x, y, vx, vy, dt;
        double mass = 1.0;
        double scale = 5.0;
        double kAir = 0;
        Point prevPoint;
        bool isRunning = false;

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += OnTimerTick;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                timer.Stop();
                StartButton.Content = "Старт";
                isRunning = false;
                return;
            }

            SimulationCanvas.Children.Clear();

            var culture = CultureInfo.InvariantCulture;

            if (!double.TryParse(VelocityBox.Text, NumberStyles.Float, culture, out double v0) || v0 <= 0 ||
                !double.TryParse(AngleBox.Text, NumberStyles.Float, culture, out double angle) ||
                !double.TryParse(TimeStepBox.Text, NumberStyles.Float, culture, out dt) || dt <= 0 ||
                !double.TryParse(DragBox.Text, NumberStyles.Float, culture, out double k) || k < 0 ||
                !double.TryParse(MassBox.Text, NumberStyles.Float, culture, out double m) || m <= 0)
            {
                MessageBox.Show("Ошибка вычислений. Проверьте вводимые значения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            kAir = k;
            mass = m;

            double angleRad = angle * Math.PI / 180.0;
            vx = v0 * Math.Cos(angleRad);
            vy = v0 * Math.Sin(angleRad);
            x = 0;
            y = 0;

            prevPoint = new Point(0, SimulationCanvas.Height);

            timer.Start();
            StartButton.Content = "Стоп";
            isRunning = true;
        }



        private void OnTimerTick(object sender, EventArgs e)
        {
            if (y < 0)
            {
                timer.Stop();
                StartButton.Content = "Старт";
                isRunning = false;
                return;
            }

            double v = Math.Sqrt(vx * vx + vy * vy);
            double ax = (-kAir * v * vx) / mass;
            double ay = -g + (-kAir * v * vy) / mass;

            vx += ax * dt;
            vy += ay * dt;
            x += vx * dt;
            y += vy * dt;

            double xScaled = x * scale;
            double yScaled = SimulationCanvas.Height - y * scale;

            Point newPoint = new Point(xScaled, yScaled);

            Line line = new Line
            {
                X1 = prevPoint.X,
                Y1 = prevPoint.Y,
                X2 = newPoint.X,
                Y2 = newPoint.Y,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            SimulationCanvas.Children.Add(line);
            prevPoint = newPoint;
        }

    }
}
