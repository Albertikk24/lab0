using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CombinedTasks
{
    // ==========================================
    // КЛАССЫ ДЛЯ ЗАДАНИЯ №2
    // ==========================================
    public class Animal
    {
        public string Name { get; set; }        // Наименование
        public double Weight { get; set; }      // Вес животного
        public double FoodWeight { get; set; }  // Вес потребляемой пищи за сутки
        public string FoodType { get; set; }    // Тип пищи (мясо, трава)
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Настройка консоли для корректного отображения кириллицы
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================================");
                Console.WriteLine("           ВЫБЕРИТЕ ЗАДАНИЕ ДЛЯ ВЫПОЛНЕНИЯ       ");
                Console.WriteLine("=================================================");
                Console.WriteLine("1 - Задание №1 (Расчет объемов бруска и цилиндра)");
                Console.WriteLine("2 - Задание №2 (Зоопарк: JSON и анализ данных)");
                Console.WriteLine("0 - Выход");
                Console.WriteLine("=================================================");
                Console.Write("Ваш выбор: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunTask1();
                        break;
                    case "2":
                        RunTask2();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некорректный ввод. Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ==========================================
        // РЕАЛИЗАЦИЯ ЗАДАНИЯ №1
        // ==========================================
        static void RunTask1()
        {
            Console.Clear();
            Console.WriteLine("=== Задание №1: Расчет объемов и отходов ===\n");

            // Ввод данных
            double a = ReadDouble("Введите ширину бруска (a): ");
            double b = ReadDouble("Введите высоту бруска (b): ");
            double L = ReadDouble("Введите длину бруска (L): ");

            double r = ReadDouble("Введите радиус цилиндра (r): ");
            double l = ReadDouble("Введите длину цилиндра (l): ");

            // Проверка условий (согласно заданию r <= a/2, r <= b/2, l <= L)
            if (r > a / 2 || r > b / 2 || l > L)
            {
                Console.WriteLine("\nОшибка: Размеры цилиндра превышают размеры бруска!");
                Pause();
                return;
            }

            // Расчеты
            double vBar = a * b * L; // Объем бруска
            double vCyl = Math.PI * Math.Pow(r, 2) * l; // Объем цилиндра (используем Math.PI)
            double vWaste = vBar - vCyl; // Отходы
            double percentWaste = (vWaste / vBar) * 100; // Процент отходов

            // Вывод результатов
            Console.WriteLine("\n--- Результаты расчета ---");
            Console.WriteLine($"Объем бруска: {vBar:F3}");
            Console.WriteLine($"Объем цилиндра: {vCyl:F3}");
            Console.WriteLine($"Объем отходов: {vWaste:F3}");
            Console.WriteLine($"Процент материала, ушедшего в отходы: {percentWaste:F2}%");

            Pause();
        }

        // ==========================================
        // РЕАЛИЗАЦИЯ ЗАДАНИЯ №2
        // ==========================================
        static void RunTask2()
        {
            Console.Clear();
            string filePath = "animals.json";

            // --- ЧАСТЬ А: Генерация данных и сохранение ---
            Console.WriteLine("=== Часть А: Генерация и сохранение данных ===");

            List<Animal> animals = new List<Animal>()
            {
                new Animal { Name = "Лев", Weight = 200, FoodWeight = 10, FoodType = "мясо" },
                new Animal { Name = "Тигр", Weight = 250, FoodWeight = 15, FoodType = "мясо" },
                new Animal { Name = "Слон", Weight = 4000, FoodWeight = 150, FoodType = "трава" },
                new Animal { Name = "Заяц", Weight = 5, FoodWeight = 1.5, FoodType = "трава" },
                new Animal { Name = "Волк", Weight = 50, FoodWeight = 5, FoodType = "мясо" },
                new Animal { Name = "Корова", Weight = 500, FoodWeight = 20, FoodType = "трава" }
            };

            // Сохранение в JSON
            try
            {
                string jsonString = JsonSerializer.Serialize(animals, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonString);
                Console.WriteLine($"Данные успешно сгенерированы и сохранены в файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
                Pause();
                return;
            }

            // --- ЧАСТЬ Б: Считывание и обработка ---
            Console.WriteLine("\n=== Часть Б: Считывание и анализ данных ===");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден.");
                Pause();
                return;
            }

            // Считывание из JSON
            string jsonFromFile = File.ReadAllText(filePath);
            List<Animal> loadedAnimals = JsonSerializer.Deserialize<List<Animal>>(jsonFromFile);

            if (loadedAnimals == null || loadedAnimals.Count == 0)
            {
                Console.WriteLine("Данные не загружены или список пуст.");
                Pause();
                return;
            }

            // 1. Вывод животных, поедающих мясо
            Console.WriteLine("\n1. Животные, поедающие мясо:");
            var meatEaters = loadedAnimals.Where(a => a.FoodType.ToLower() == "мясо");

            foreach (var animal in meatEaters)
            {
                Console.WriteLine($" - {animal.Name} (вес: {animal.Weight} кг)");
            }

            // 2. Наименование животного с максимальным количеством пищи на 1 кг веса
            // Формула: FoodWeight / Weight
            Console.WriteLine("\n2. Животное с максимальным потреблением пищи на 1 кг веса:");

            var topAnimal = loadedAnimals
                .OrderByDescending(a => a.FoodWeight / a.Weight)
                .FirstOrDefault();

            if (topAnimal != null)
            {
                double ratio = topAnimal.FoodWeight / topAnimal.Weight;
                Console.WriteLine($" - {topAnimal.Name}");
                Console.WriteLine($"   Потребление: {topAnimal.FoodWeight} кг пищи на {topAnimal.Weight} кг веса.");
                Console.WriteLine($"   Коэффициент: {ratio:F4} (пищи/вес)");
            }

            Pause();
        }

        // ==========================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // ==========================================

        // Метод для ввода числа с проверкой (для Задания 1)
        static double ReadDouble(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (double.TryParse(Console.ReadLine(), out double result))
                {
                    return result;
                }
                Console.WriteLine("Некорректный ввод. Попробуйте снова.");
            }
        }

        // Метод для паузы перед очисткой экрана
        static void Pause()
        {
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }
}