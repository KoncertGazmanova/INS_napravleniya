using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace INS_napravleniya
{
    public partial class DatasetForm : Form
    {
        private string datasetPath = "Dataset";

        public DatasetForm()
        {
            InitializeComponent();
            LoadDataset();
        }

        // Загрузка данных в TreeView
        private void LoadDataset()
        {
            if (!Directory.Exists(datasetPath))
            {
                MessageBox.Show("Выборка пуста.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            treeViewDataset.Nodes.Clear();

            foreach (var labelDir in Directory.GetDirectories(datasetPath))
            {
                string label = Path.GetFileName(labelDir);

                var node = new TreeNode(label);
                node.Tag = labelDir; // Сохраняем путь к папке
                treeViewDataset.Nodes.Add(node);
            }

            treeViewDataset.AfterSelect += TreeViewDataset_AfterSelect;
        }

        // Обработка выбора узла в TreeView
        private void TreeViewDataset_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string labelPath = e.Node.Tag.ToString();
            DisplayImages(labelPath);
        }

        // Отображение рисунков с кнопками удаления
        private void DisplayImages(string labelPath)
        {
            panelImages.Controls.Clear(); // Очищаем панель перед добавлением новых элементов

            foreach (var file in Directory.GetFiles(labelPath, "*.json"))
            {
                // Загружаем массив пикселей из JSON
                var pixelData = JsonConvert.DeserializeObject<int[,]>(File.ReadAllText(file));
                int width = pixelData.GetLength(0);
                int height = pixelData.GetLength(1);
                Bitmap bitmap = new Bitmap(width, height);

                // Конвертируем массив в изображение (0 → белый, 1 → чёрный)
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int colorValue = pixelData[x, y] == 1 ? 0 : 255;
                        bitmap.SetPixel(x, y, Color.FromArgb(colorValue, colorValue, colorValue));
                    }
                }

                // Создаём контейнер для изображения и кнопки
                var container = new FlowLayoutPanel
                {
                    Width = 120,
                    Height = 140,
                    FlowDirection = FlowDirection.TopDown,
                    Margin = new Padding(10)
                };

                // Создаём PictureBox для изображения
                var pictureBox = new PictureBox
                {
                    Width = 100,
                    Height = 100,
                    BorderStyle = BorderStyle.FixedSingle,
                    Image = bitmap,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                // Создаём кнопку удаления
                var deleteButton = new Button
                {
                    Text = "Удалить",
                    Width = 100,
                    Tag = file // Сохраняем путь к файлу в Tag
                };

                deleteButton.Click += (s, e) =>
                {
                    string filePath = deleteButton.Tag.ToString();
                    DeleteImage(filePath, labelPath);
                };

                // Добавляем элементы в контейнер
                container.Controls.Add(pictureBox);
                container.Controls.Add(deleteButton);

                // Добавляем контейнер в панель
                panelImages.Controls.Add(container);
            }

            // Принудительное обновление Panel (если потребуется)
            panelImages.Refresh();
        }

        // Удаление рисунка
        private void DeleteImage(string filePath, string labelPath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath); // Удаляем файл
                MessageBox.Show("Рисунок удалён!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayImages(labelPath); // Обновляем интерфейс
            }
            else
            {
                MessageBox.Show("Файл не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
