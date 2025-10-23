using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WinFormsApp4_registration_
{
    public partial class StudentsForm : Form
    {
        private List<(string Student, int Grade)> students = new List<(string, int)>();

        private Label labelTitle;
        private Label labelName;
        private Label labelGrade;
        private Label labelSearch;
        private TextBox textBoxName;
        private TextBox textBoxGrade;
        private TextBox textBoxSearch;
        private Button buttonAdd;
        private Button buttonFind;
        private Button buttonSave;
        private Button buttonLoad;
        private Button buttonClear;
        private Label labelAverage;
        private DataGridView dataGridView1;
        private Chart chartGrades;

        public StudentsForm()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            // --- Основные настройки формы ---
            this.Text = "Учёт студентов";
            this.BackColor = System.Drawing.Color.WhiteSmoke;   
            this.Font = new System.Drawing.Font("Segoe UI", 10);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 920;
            this.Height = 620;

            // --- Заголовок ---
            labelTitle = new Label()
            {
                Text = "📘 Учёт студентов и оценок",
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.DarkSlateGray,
                Height = 60
            };
            this.Controls.Add(labelTitle);

            // --- Поля ввода ---
            labelName = new Label() { Text = "Имя студента:", Left = 30, Top = 80, Width = 120 };
            textBoxName = new TextBox() { Left = 160, Top = 80, Width = 200 };

            labelGrade = new Label() { Text = "Оценка:", Left = 380, Top = 80, Width = 70 };
            textBoxGrade = new TextBox() { Left = 450, Top = 80, Width = 60 };

            buttonAdd = CreateButton("Добавить", 530, 75, ButtonAdd_Click);
            this.Controls.AddRange(new Control[] { labelName, textBoxName, labelGrade, textBoxGrade });

            // --- Поиск ---
            labelSearch = new Label() { Text = "Поиск по имени:", Left = 30, Top = 120, Width = 130 };
            textBoxSearch = new TextBox() { Left = 160, Top = 120, Width = 200 };
            buttonFind = CreateButton("🔍 Найти", 370, 115, ButtonFind_Click);
            this.Controls.AddRange(new Control[] { labelSearch, textBoxSearch, buttonFind });

            // --- Таблица ---
            dataGridView1 = new DataGridView()
            {
                Top = 160,
                Left = 30,
                Width = 500,
                Height = 280,
                AllowUserToAddRows = false,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dataGridView1.Columns.Add("Student", "Студент");
            dataGridView1.Columns.Add("Grade", "Оценка");
            this.Controls.Add(dataGridView1);

            // --- Диаграмма ---
            chartGrades = new Chart()
            {
                Left = 550,
                Top = 160,
                Width = 320,
                Height = 280,
                BackColor = System.Drawing.Color.White
            };
            ChartArea chartArea = new ChartArea("Grades");
            chartGrades.ChartAreas.Add(chartArea);
            Series series = new Series("Оценки")
            {
                ChartType = SeriesChartType.Column,
                Color = System.Drawing.Color.CornflowerBlue
            };
            chartGrades.Series.Add(series);
            this.Controls.Add(chartGrades);

            // --- Средний балл ---
            labelAverage = new Label()
            {
                Text = "Средний балл: —",
                Left = 30,
                Top = 460,
                Width = 400,
                Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.MediumSlateBlue
            };
            this.Controls.Add(labelAverage);

            // --- Кнопки ---
            buttonSave = CreateButton("💾 Сохранить", 30, 500, ButtonSave_Click);
            buttonLoad = CreateButton("📂 Загрузить", 180, 500, ButtonLoad_Click);
            buttonClear = CreateButton("🧹 Очистить", 330, 500, ButtonClear_Click);
        }

        private Button CreateButton(string text, int left, int top, EventHandler handler)
        {
            var btn = new Button()
            {
                Text = text,
                Left = left,
                Top = top,
                Width = 120,
                Height = 35,
                BackColor = System.Drawing.Color.CornflowerBlue,
                FlatStyle = FlatStyle.Flat,
                ForeColor = System.Drawing.Color.White
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += handler;
            this.Controls.Add(btn);
            return btn;
        }

        // === Добавление студента ===
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            string name = textBoxName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите имя студента!");
                return;
            }

            if (!int.TryParse(textBoxGrade.Text, out int grade) || grade < 1 || grade > 5)
            {
                MessageBox.Show("Введите корректную оценку от 1 до 5!");
                return;
            }

            students.Add((name, grade));
            dataGridView1.Rows.Add(name, grade);

            textBoxName.Clear();
            textBoxGrade.Clear();

            UpdateAverage();
            UpdateChart();
        }

        // === Обновление среднего ===
        private void UpdateAverage()
        {
            if (students.Count == 0)
            {
                labelAverage.Text = "Средний балл: —";
                return;
            }

            double avg = students.Average(s => s.Grade);
            labelAverage.Text = $"Средний балл: {avg:F2}";
        }

        // === Поиск ===
        private void ButtonFind_Click(object sender, EventArgs e)
        {
            string filter = textBoxSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filter))
            {
                MessageBox.Show("Введите имя для поиска!");
                return;
            }

            var filtered = students.Where(s => s.Student.ToLower().Contains(filter)).ToList();

            dataGridView1.Rows.Clear();
            foreach (var s in filtered)
                dataGridView1.Rows.Add(s.Student, s.Grade);

            if (filtered.Count == 0)
                MessageBox.Show("Ничего не найдено!");
        }

        // === Обновление диаграммы ===
        private void UpdateChart()
        {
            chartGrades.Series["Оценки"].Points.Clear();
            foreach (var s in students)
                chartGrades.Series["Оценки"].Points.AddXY(s.Student, s.Grade);
        }

        // === Сохранение ===
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            File.WriteAllLines("students.csv", students.Select(s => $"{s.Student},{s.Grade}"));
            MessageBox.Show("✅ Данные сохранены в файл students.csv");
        }

        // === Загрузка ===
        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            if (!File.Exists("students.csv"))
            {
                MessageBox.Show("Файл students.csv не найден!");
                return;
            }

            students.Clear();
            dataGridView1.Rows.Clear();

            foreach (var line in File.ReadAllLines("students.csv"))
            {
                var parts = line.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[1], out int g))
                {
                    students.Add((parts[0], g));
                    dataGridView1.Rows.Add(parts[0], g);
                }
            }

            UpdateAverage();
            UpdateChart();
            MessageBox.Show("📂 Данные успешно загружены!");
        }

        // === Очистка ===
        private void ButtonClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Очистить все данные?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                students.Clear();
                dataGridView1.Rows.Clear();
                chartGrades.Series["Оценки"].Points.Clear();
                UpdateAverage();
            }
        }
    }
}

