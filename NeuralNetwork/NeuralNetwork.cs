using System;
using System.Linq;

namespace INS_napravleniya.NeuralNetwork
{
    public class NeuralNetwork
    {
        public Layer[] Layers { get; private set; } // массив слоев, которые составляют сеть
        private Random rng = new Random(); // генератор случайных чисел, нужен для перемешивания датасета

        // конструктор принимает структуру сети (например, {784, 64, 32, 8}) 
        // и на основе этого создает нужное количество слоев
        public NeuralNetwork(int[] layerStructure)
        {
            // минимум 2 слоя должно быть
            if (layerStructure.Length < 2)
                throw new ArgumentException("Нужно минимум 2 слоя (вход и выход)");

            // инициализируем массив слоев, их всегда на 1 меньше, чем элементов в layerStructure
            Layers = new Layer[layerStructure.Length - 1];

            // создаем все слои по очереди
            for (int i = 0; i < Layers.Length; i++)
            {
                // для последнего слоя обычно используется Softmax, чтобы делать классификацию
                // для всех остальных слоев — ReLU, чтобы быстрее сходилась сеть
                ActivationType activation = (i == Layers.Length - 1)
                    ? ActivationType.Softmax
                    : ActivationType.ReLU;

                // создаем слой, указываем число нейронов в этом слое и в предыдущем
                Layers[i] = new Layer(
                    neuronCount: layerStructure[i + 1],
                    inputCount: layerStructure[i],
                    activation: activation
                );
            }
        }

        // по идем по всем слоям, передавая данные дальше
        public double[] Compute(double[] inputs)
        {
            double[] outputs = inputs; // на входе данные которые нужно обработать
            foreach (var layer in Layers)
            {
                outputs = layer.ProcessInputs(outputs); // передаем данные через слой
            }
            return outputs; // возвращаем выход сети
        }

        // обучение сети с помощью функции потерь и обратного распространения ошибки
        public void Train(double[][] inputSet, double[][] targetSet, int epochs, double learningRate)
        {
            // проверяем, чтобы входов и целей было одинаковое количество, иначе это ошибка
            if (inputSet.Length != targetSet.Length)
                throw new ArgumentException("Число входов и целей не совпадает!");

            // запускаем обучение на заданное количество эпох
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                ShuffleDataset(inputSet, targetSet); // перемешиваем данные, чтобы сеть не запоминала порядок

                double totalLoss = 0.0; // тут будем хранить сумму всех ошибок за эпоху

                for (int i = 0; i < inputSet.Length; i++)
                {
                    // считаем выход сети для текущего примера
                    double[] predicted = Compute(inputSet[i]);

                    // считаем ошибку (функцию потерьy)
                    double sampleLoss = 0.0;
                    for (int j = 0; j < predicted.Length; j++)
                    {
                        double p = Math.Max(predicted[j], 1e-12); // защищаемся от log(0), берем минимум 1e-12
                        sampleLoss -= targetSet[i][j] * Math.Log(p); // формула CE = -y_true * log(y_pred)
                    }
                    totalLoss += sampleLoss; // копим ошибку для всей эпохи

                    // считаем ошибки для выходного слоя (y_pred - y_true)
                    double[] outputErrors = new double[predicted.Length];
                    for (int j = 0; j < predicted.Length; j++)
                    {
                        outputErrors[j] = predicted[j] - targetSet[i][j];
                    }

                    // теперь идем назад по слоям, передавая ошибки (backpropagation)
                    double[] errors = outputErrors;
                    for (int layerIndex = Layers.Length - 1; layerIndex >= 0; layerIndex--)
                    {
                        errors = Layers[layerIndex].Backpropagate(errors, learningRate); // считаем ошибки и обновляем веса
                    }
                }

                // средняя ошибка за эпоху
                double avgLoss = totalLoss / inputSet.Length;
                System.Diagnostics.Debug.WriteLine($"[Эпоха {epoch + 1}/{epochs}] CE = {avgLoss:F6}");
            }
        }

        // перемешиваем данные (входы и цели), чтобы примеры подавались в случайном порядке
        private void ShuffleDataset(double[][] inputs, double[][] targets)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                int swapIndex = rng.Next(i, inputs.Length); // выбираем случайный индекс

                // меняем местами входы
                var tmpIn = inputs[i];
                inputs[i] = inputs[swapIndex];
                inputs[swapIndex] = tmpIn;

                // меняем местами цели
                var tmpOut = targets[i];
                targets[i] = targets[swapIndex];
                targets[swapIndex] = tmpOut;
            }
        }

        // выводим веса сети для отладки, просто чтобы посмотреть, что происходит
        public void PrintWeights()
        {
            for (int layerIndex = 0; layerIndex < Layers.Length; layerIndex++)
            {
                var layer = Layers[layerIndex];
                System.Diagnostics.Debug.WriteLine($"=== Layer {layerIndex} ({layer.Activation}) ===");
                for (int neuronIndex = 0; neuronIndex < layer.Neurons.Length; neuronIndex++)
                {
                    var neuron = layer.Neurons[neuronIndex];
                    System.Diagnostics.Debug.WriteLine($"   Neuron[{neuronIndex}].Bias = {neuron.Bias:F4}");
                    var wStr = string.Join(", ", neuron.Weights.Select(w => w.ToString("F4")));
                    System.Diagnostics.Debug.WriteLine($"   Weights: [{wStr}]");
                }
            }
        }
    }
}
