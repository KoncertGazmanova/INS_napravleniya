using System;

namespace INS_napravleniya.NeuralNetwork
{
    public class Neuron
    {
        // массив весов для каждого входа нейрона
        public double[] Weights { get; private set; }

        // смещение, которое добавляется ко входам
        public double Bias { get; private set; }

        // выход нейрона после активации
        public double Output { get; set; }

        // рандом для генерации случайных чисел
        private Random random = new Random();

        // конструктор создаёт веса для входов и задаёт смещение
        public Neuron(int inputCount)
        {
            Weights = new double[inputCount];// создаём массив под веса
            InitializeWeights();// задаём всем весам рандомные значения
            Bias = (random.NextDouble() - 0.5) * 0.1;// рандомное смещение, чтобы нейрон стартовал с чего-то
        }

        // инициализация весов случайными числами
        private void InitializeWeights()
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = (random.NextDouble() - 0.5) * 0.1;// задаём весу рандомное число в небольшом диапазоне
            }
        }

        // вычисляем взвешенную сумму входов по формуле W * X + B
        public double NetInput(double[] inputs)
        {
            if (inputs.Length != Weights.Length) // проверяем, чтобы входов было столько же, сколько весов
                throw new ArgumentException("Число входов не совпадает с числом весов");

            double sum = 0.0; // тут будем хранить итоговую сумму
            for (int i = 0; i < inputs.Length; i++)
            {
                sum += inputs[i] * Weights[i];// умножаем каждый вход на его вес и добавляем к сумме
            }
            sum += Bias;// прибавляем смещение к сумме

            return sum;// возвращаем итоговый результат
        }

        // обновляем веса после обучения, тут используется градиентный спуск
        public void UpdateWeights(double[] inputs, double learningRate, double delta)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] -= learningRate * delta * inputs[i]; // корректируем вес в зависимости от ошибки, входа и скорости обучения
            }
            Bias -= learningRate * delta; // смещение обновляем по тому же принципу
        }
    }
}
