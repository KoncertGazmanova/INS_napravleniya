using System;
using System.Linq;

namespace INS_napravleniya.NeuralNetwork
{
    public enum ActivationType
    {
        ReLU,
        Softmax
    }

    public class Layer
    {
        public Neuron[] Neurons { get; private set; }
        public double[] Outputs { get; private set; }
        public double[] Inputs { get; private set; }   // Храним входы, чтобы потом использовать в backprop
        public double[] NetInputs { get; private set; } // Суммы до активации (полезно для Softmax)
        public ActivationType Activation { get; private set; }

        // Конструктор слоя: neuronCount — количество нейронов, inputCount — размер входа, activation — тип активации
        public Layer(int neuronCount, int inputCount, ActivationType activation)
        {
            Neurons = new Neuron[neuronCount];
            Outputs = new double[neuronCount];
            NetInputs = new double[neuronCount];
            Activation = activation;

            // Создаем каждый нейрон
            for (int i = 0; i < neuronCount; i++)
            {
                Neurons[i] = new Neuron(inputCount);
            }
        }

        // Прямой проход по слою: умножаем входы на веса, суммируем, применяем функцию активации
        public double[] ProcessInputs(double[] inputs)
        {
            Inputs = inputs;

            // Считаем линейные суммы для каждого нейрона
            for (int i = 0; i < Neurons.Length; i++)
            {
                NetInputs[i] = Neurons[i].NetInput(inputs);
            }

            // Если ReLU — все отрицательные значения обнуляем
            if (Activation == ActivationType.ReLU)
            {
                for (int i = 0; i < Neurons.Length; i++)
                {
                    double val = NetInputs[i];
                    double activated = (val > 0) ? val : 0.0;
                    Neurons[i].Output = activated;
                    Outputs[i] = activated;
                }
            }
            else if (Activation == ActivationType.Softmax)
            {
                // Softmax: берём exp(net - maxVal), делим на сумму exp. 
                // Вычитаем maxVal для численной стабильности (чтобы не было слишком больших экспонент)
                double maxVal = NetInputs.Max();
                double sumExp = 0.0;
                for (int i = 0; i < NetInputs.Length; i++)
                {
                    sumExp += Math.Exp(NetInputs[i] - maxVal);
                }

                // Рассчитываем результат для каждого выхода
                for (int i = 0; i < NetInputs.Length; i++)
                {
                    double sm = Math.Exp(NetInputs[i] - maxVal) / sumExp;
                    Neurons[i].Output = sm;
                    Outputs[i] = sm;
                }
            }

            return Outputs;
        }

        // Обратное распространение ошибки
        // errors — это ошибка на выходе этого слоя
        // learningRate — шаг обучения
        public double[] Backpropagate(double[] errors, double learningRate)
        {
            // Убедимся, что у нас столько же ошибок, сколько нейронов
            if (errors.Length != Neurons.Length)
                throw new ArgumentException("Длина errors не совпадает с числом нейронов слоя");

            // Это массив, куда мы сложим ошибку, которую будем передавать дальше в предыдущий слой
            double[] propagated = new double[Neurons[0].Weights.Length];

            // Для каждого нейрона считаем delta (ошибку для этого нейрона),
            // и обновляем его веса
            for (int i = 0; i < Neurons.Length; i++)
            {
                double delta;
                if (Activation == ActivationType.ReLU)
                {
                    // Для ReLU, производная = 1, если выход > 0, иначе 0
                    double grad = (Neurons[i].Output > 0) ? 1.0 : 0.0;
                    delta = errors[i] * grad;
                }
                else // Softmax
                {
                    // При Softmax + CrossEntropy: d(Loss)/d(net) = (y_pred - y_true),
                    // так что у нас errors[i] уже = (y_pred - y_true)
                    delta = errors[i];
                }

                // Обновляем веса нейрона
                Neurons[i].UpdateWeights(Inputs, learningRate, delta);

                // Распространяем ошибку на каждый вход
                for (int w = 0; w < Neurons[i].Weights.Length; w++)
                {
                    propagated[w] += Neurons[i].Weights[w] * delta;
                }
            }
            return propagated;
        }
    }
}
