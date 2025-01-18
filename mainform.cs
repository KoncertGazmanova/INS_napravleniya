using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;       
using System.Windows.Forms;
using INS_napravleniya.NeuralNetwork;

namespace INS_napravleniya
{
    public partial class mainform : Form
    {
        private Bitmap canvas; // Основной холст для рисования
        private Stack<Bitmap> undoStack = new(); // Стек для хранения предыдущих состояний (для кнопки "Отмена")
        private Graphics graphics; // Графический объект для рисования на холсте
        private string datasetPath = "Dataset"; // Путь к папке с нашим датасетом
        private INS_napravleniya.NeuralNetwork.NeuralNetwork network; // сама нейросеть
        private List<double[]> trainingInputs = new(); // Список входных данных для обучения
        private List<double[]> trainingOutputs = new(); // Список выходных (цель) для обучения
        private List<string> labels = new(); // Список меток классов (названия папок)

        public mainform()
        {
            InitializeComponent(); // Инициализация компонентов формы.

            // Создаем новый чистый битмап размером под pictureBox
            canvas = new Bitmap(pictureBoxCanvas.Width, pictureBoxCanvas.Height);

            // Создаем объект графикс, который будет использоваться для рисования на битмап
            graphics = Graphics.FromImage(canvas);

            ClearCanvas(); // Заполняем холст белым цветом, чтобы изначально был пустой фон.
                           // 
            pictureBoxCanvas.Image = canvas; // Устанавливаем созданный Bitmap как изображение для PictureBox, чтобы он стал видимым в интерфейсе

            // Привязываем обработчики событий мыши для рисования на PictureBox
            pictureBoxCanvas.MouseDown += PictureBoxCanvas_MouseDown; // Событие нажатия кнопки мыши.
            pictureBoxCanvas.MouseMove += PictureBoxCanvas_MouseMove; // Событие движения мыши.

            // Привязываем обработчики событий для кнопок интерфейса.
            buttonSave.Click += ButtonSave_Click; // Кнопка "Сохранить"
            buttonVisualize.Click += ButtonVisualize_Click; // Кнопка "Просмотр выборки" открывает форму для визуализации.
            buttonCancel.Click += ButtonCancel_Click; // Кнопка "Отмена" восстанавливает предыдущий холст.
            buttonRefreshDataset.Click += ButtonRefreshDataset_Click; // Кнопка "Обновить датасет" обновляет выборку.
            LoadDataset();// Загружаем датасет при запуске приложения, чтобы иметь доступ к обучающим данным.
        }


        
        private void ButtonRefreshDataset_Click(object sender, EventArgs e)// Кнопка "Обновить датасет" — считываем все данные из папок по новой
        {
            // Проверка и вызов вызываем LoadDataset(), чтобы вдруг появились новые классы или файлы
            LoadDataset();
            MessageBox.Show("Датасет обновлен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Обработчик кнопки "Обучить"
        private void ButtonTrain_Click(object sender, EventArgs e)
        {
            // Проверяем, что у нас вообще есть какие-то данные
            if (trainingInputs.Count == 0 || trainingOutputs.Count == 0)
            {
                MessageBox.Show("Нет данных для обучения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Если сеть еще не создана или вдруг размер последнего слоя не совпадает с числом классов, то создаем новую сеть (784 -> 64 -> 32 -> кол-во классов)
   
            if (network == null || network.Layers[^1].Neurons.Length != labels.Count)
            {
                //  выбрал число 784 т.к это 28x28 пикселей (размер холста), а 64 и 32 —  мощности скрытых слоев
                // 
                network = new INS_napravleniya.NeuralNetwork.NeuralNetwork(
                    new int[] { 784, 64, 32, labels.Count }
                );
            }

            // Теперь обучаем. В данном примере я выбрал 390 эпох и шаг 0.005, вроде работает.
            // Но можно поднять эпох до 700 тогда сеть будет дольше учиться, но возможно точность станет выше (зависит от данных, не факт).
            network.Train(
                trainingInputs.ToArray(),
                trainingOutputs.ToArray(),
                epochs: 390,        // здесь можно поставить 1000, если хочется прям капитально обучить
                learningRate: 0.005 // шаг. Если поставить 0.01, то градиенты будут обновляться пошустрее
            );

            // Печать весов в отладку — на случай, если хочется посмотреть, что там внутри для отладки делал
            network.PrintWeights();

            // Маленькая проверка качества на тех же данных (для демонстрации) чтобы проверить, как вообще сетка обучается
            for (int i = 0; i < trainingInputs.Count; i++)
            {
                double[] predicted = network.Compute(trainingInputs[i]);
                int predictedIndex = Array.IndexOf(predicted, predicted.Max());
                int targetIndex = Array.IndexOf(trainingOutputs[i], trainingOutputs[i].Max());

                System.Diagnostics.Debug.WriteLine(
                    $"Пример {i + 1}: Цель = {labels[targetIndex]}, " +
                    $"Предсказание = {labels[predictedIndex]}"
                );
            }

            MessageBox.Show("Обучение завершено!", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Метод, который собирает обучающую выборку из папок
        private void LoadDataset()
        {
            trainingInputs.Clear();
            trainingOutputs.Clear();
            labels.Clear();

            // Если папки нет - ошибка
            if (!Directory.Exists(datasetPath))
            {
                MessageBox.Show($"Папка '{datasetPath}' не найдена!",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 1) Смотрим все подпапки внутри "Dataset"
            var subDirs = Directory.GetDirectories(datasetPath);

            // 2) Для каждой подпапки проверяем, есть ли там json-файлы. Если есть — добавляем название в labels
            foreach (var dir in subDirs)
            {
                var jsonFiles = Directory.GetFiles(dir, "*.json");
                if (jsonFiles.Length > 0)
                {
                    string labelName = Path.GetFileName(dir);
                    labels.Add(labelName);
                }
            }

            // Если ничего не нашлось, значит у нас нет данных
            if (labels.Count == 0)
            {
                MessageBox.Show($"В папке '{datasetPath}' нет подпапок с *.json!",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3) Делаем второй проход: теперь реально читаем все файлы и формируем input/outputs
            for (int labelIndex = 0; labelIndex < labels.Count; labelIndex++)
            {
                string labelDir = Path.Combine(datasetPath, labels[labelIndex]);
                var jsonFiles = Directory.GetFiles(labelDir, "*.json");

                foreach (var file in jsonFiles)
                {
                    // Десериализация массивов пикселей из JSON
                    int[,] pixelData = JsonConvert.DeserializeObject<int[,]>(File.ReadAllText(file));

                    // Превращаем его в плоский массив (double), чтобы подавать на вход сети
                    double[] inputVector = new double[pixelData.Length];
                    int idx = 0;
                    for (int x = 0; x < pixelData.GetLength(0); x++)
                    {
                        for (int y = 0; y < pixelData.GetLength(1); y++)
                        {
                            inputVector[idx++] = pixelData[x, y];
                        }
                    }

                    // Формируем выход (one-hot): если у нас N классов, то длина = N, 
                    // и у текущего класса ставим 1.0
                    double[] outputVector = new double[labels.Count];
                    outputVector[labelIndex] = 1.0;

                    // Запихиваем в общий список
                    trainingInputs.Add(inputVector);
                    trainingOutputs.Add(outputVector);
                }
            }

            // Немножко отладочной информации: что нашли
            Console.WriteLine($"labels.Count = {labels.Count}");
            for (int i = 0; i < labels.Count; i++)
                Console.WriteLine($"  label[{i}] = {labels[i]}");

            Console.WriteLine($"Всего обучающих примеров: {trainingInputs.Count}");
            for (int i = 0; i < trainingOutputs.Count; i++)
            {
                Console.WriteLine($"  outputVector[{i}] size = {trainingOutputs[i].Length}");
            }
        }

        

        private double[] ConvertCanvasToVector() // Превращаем картинку на экране в вектор из 28*28 = 784 значений
        // Каждое значение в диапазоне [0..1], где 1 — это темный пиксель
        {
            // Сжимаем холст до 28х28 (вроде как MNIST-стандарт)
            using (Bitmap scaledCanvas = ResizeBitmap(canvas, 28, 28))
            {
                double[] vector = new double[scaledCanvas.Width * scaledCanvas.Height];
                int index = 0;

                // Пробегаемся по пикселям
                for (int x = 0; x < scaledCanvas.Width; x++)
                {
                    for (int y = 0; y < scaledCanvas.Height; y++)
                    {
                        Color color = scaledCanvas.GetPixel(x, y);
                        // Тут "1.0 - (color.R / 255.0)", чтобы черный пиксель (R=0) превращался в 1.0
                        // а белый (R=255) в 0.0 
                        vector[index++] = 1.0 - (color.R / 255.0);
                    }
                }

                return vector;
            }
        }

        // Кнопка "Распознать"
        private void ButtonRecognize_Click(object sender, EventArgs e)
        {
            if (network == null)
            {
                MessageBox.Show("Сначала обучите сеть!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Берем текущее изображение с холста и делаем из него вектор
            double[] inputVector = ConvertCanvasToVector();

            // Считаем выход сети
            double[] output = network.Compute(inputVector);

            // Определяем, какой класс (индекс) имеет самый большой выход
            int predictedIndex = Array.IndexOf(output, output.Max());
            string predictedLabel = labels[predictedIndex];

            // Показываем результат
            MessageBox.Show($"Распознанный класс: {predictedLabel}", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Событие нажатия мышью по холсту — начинаем рисовать, если ЛКМ
        private void PictureBoxCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SaveCanvasState(); // Сохраняем текущее состояние, чтобы можно было отменить
                Draw(e.X, e.Y);
            }
        }

        // Событие движения мыши — если ЛКМ зажата, то рисуем черной кистью
        private void PictureBoxCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Draw(e.X, e.Y);
            }
        }

        // Собственно рисование — закрашиваем небольшую окружность
        private void Draw(int x, int y)
        {
            using (var brush = new SolidBrush(Color.Black))
            {
                graphics.FillEllipse(brush, x - 5, y - 5, 10, 10);
                pictureBoxCanvas.Invalidate(); // Обновить отрисовку на экране
            }
        }

        // Кнопка "Сохранить" — сохраняем нарисованную цифру/символ в датасет
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            string label = textBoxLabel.Text.Trim(); // Получаем введенную метку (название класса).

            // Если метка не введена, показываем сообщение об ошибке.
            if (string.IsNullOrWhiteSpace(label))
            {
                MessageBox.Show("Введите метку для рисунка.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Создаем папку для метки, если ее еще нет.
            string labelPath = Path.Combine(datasetPath, label);
            if (!Directory.Exists(labelPath))
            {
                Directory.CreateDirectory(labelPath);
            }

            using (Bitmap scaledCanvas = ResizeBitmap(canvas, 28, 28)) // Сжимаем холст до 28x28 пикселей.
            {
                // Определяем имя нового файла: берем максимальный номер среди существующих файлов + 1.
                int sampleId = Directory
                    .GetFiles(labelPath, "*.json") // Получаем все файлы с расширением .json в папке.
                    .Select(f => int.Parse(Path.GetFileNameWithoutExtension(f))) // Берем номера файлов.
                    .DefaultIfEmpty(0) // Если папка пустая, возвращаем 0.
                    .Max() + 1; // Увеличиваем номер на 1.

                string filePath = Path.Combine(labelPath, $"{sampleId}.json"); // Полный путь к новому файлу.

                // Преобразуем изображение в двумерный массив пикселей.
                int[,] pixelData = new int[scaledCanvas.Width, scaledCanvas.Height];
                for (int x = 0; x < scaledCanvas.Width; x++)
                {
                    for (int y = 0; y < scaledCanvas.Height; y++)
                    {
                        Color color = scaledCanvas.GetPixel(x, y); // Получаем цвет пикселя.
                        pixelData[x, y] = (color.R < 200) ? 1 : 0; // Если цвет темный, записываем 1, иначе 0.
                    }
                }

                // Сохраняем массив пикселей в файл в формате JSON.
                File.WriteAllText(filePath, JsonConvert.SerializeObject(pixelData));

                // Показываем сообщение об успешном сохранении.
                MessageBox.Show($"Рисунок сохранен под меткой '{label}'.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // После сохранения очищаем холст.
            ClearCanvas();
        }


        // Кнопка "Просмотр выборки" — открывает форму со списком данных (DatasetForm)
        private void ButtonVisualize_Click(object sender, EventArgs e)
        {
            var datasetForm = new DatasetForm();
            datasetForm.ShowDialog();
        }

        // Кнопка "Отмена" — восстанавливает предыдущий холст из undoStack
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 0)
            {
                canvas = undoStack.Pop();
                graphics = Graphics.FromImage(canvas);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                pictureBoxCanvas.Image = canvas;
                pictureBoxCanvas.Invalidate();
            }
            else
            {
                MessageBox.Show("Нет действий для отмены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Просто очищаем холст, заливаем белым
        private void ClearCanvas()
        {
            graphics.Clear(Color.White);
            pictureBoxCanvas.Invalidate();
        }

        // Сохраняем текущее состояние холста в стек
        private void SaveCanvasState()
        {
            undoStack.Push(new Bitmap(canvas));
        }

        // Уменьшаем (масштабируем) Bitmap до указанных width/height
        // Метод для изменения размера Bitmap
        private Bitmap ResizeBitmap(Bitmap original, int width, int height)
        {
            // Создаем новый Bitmap с указанными размерами.
            Bitmap resized = new Bitmap(width, height);

            // Создаем объект Graphics, чтобы "перерисовать" изображение в новый размер.
            using (Graphics g = Graphics.FromImage(resized))
            {
                // Устанавливаем режим интерполяции "NearestNeighbor" (ближайший сосед).
                // Этот метод сохраняет четкие границы пикселей при уменьшении изображения.
                // Важный момент: этот метод подходит для изображений с пиксельной графикой, чтобы избежать размытия.
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                // Рисуем оригинальное изображение на новом Bitmap с новыми размерами.
                // Это фактически изменяет размер исходного изображения.
                g.DrawImage(original, 0, 0, width, height);
            }

            // Возвращаем измененное изображение.
            return resized;
        }

    }
}
