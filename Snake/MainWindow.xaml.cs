using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Семестровка.Snake;

namespace Семестровка
{
    public partial class MainWindow : Window
    {
        const int constantSize = 20;
        private UIElement snakeFood = null; //ссылка на еду
        private SolidColorBrush colorFood = Brushes.Crimson; //цвет еды
        private SolidColorBrush colorBody = Brushes.Goldenrod;
        private SolidColorBrush colorHead = Brushes.Chocolate;
        private Map map = new Map();
        private void StartNewGame()
        {
            foreach (SnakePart snakeBodyPart in map.Snake.SnakeParts) //пробегаемся по списку частей змеи
            {
                if (snakeBodyPart.UiElement != null) //если список не пустой
                    GameArea.Children.Remove(snakeBodyPart.UiElement); //удаляем с поля все части змеи
            }

            if (snakeFood != null) //если ссылка на еду не пустая
                GameArea.Children.Remove(snakeFood);  //удаляем еду с поля

            map.ResetParameters();
            UpdateGameStatus();
            DrawSnake();
            DrawSnakeFood();
        }

        private void PressTheKey(object sender, KeyEventArgs e) //KeyEventArgs-события нажатия клавиш
        {
            SnakeDirection originalSnakeDirection = map.Snake.Direction; //сохраняем тукущее направление движения для проверки
            switch (e.Key)
            {
                case Key.Up:
                    if (map.Snake.Direction != SnakeDirection.Down) //меняет направление, но не может поворачиваться на 180 градусов
                        map.Snake.Direction = SnakeDirection.Up; //поэтому делаем проверку на != противоположному направлению
                    break;
                case Key.Down:
                    if (map.Snake.Direction != SnakeDirection.Up)
                        map.Snake.Direction = SnakeDirection.Down;
                    break;
                case Key.Left:
                    if (map.Snake.Direction != SnakeDirection.Right)
                        map.Snake.Direction = SnakeDirection.Left;
                    break;
                case Key.Right:
                    if (map.Snake.Direction != SnakeDirection.Left)
                        map.Snake.Direction = SnakeDirection.Right;
                    break;
                case Key.Space:  //при нажатии калвиши "пробел" запускает новую игру
                    StartNewGame();
                    break;
            }
            if (map.Snake.Direction != originalSnakeDirection) //если направление движения изменилось, мы вызываем метод движения змеи
                RedrawSnake();
        }

        private void DrawSnake()
        {
            foreach (SnakePart snakePart in map.Snake.SnakeParts) //пробeгаемся по списку частей змеи
            {
                if (snakePart.UiElement == null)  //если элемента не существует
                {
                    snakePart.UiElement = new Ellipse() //создание нового элемента
                    {
                        Width = constantSize,
                        Height = constantSize,
                        Fill = snakePart.IsHead ? colorHead : colorBody //проверяем голова или тело и красим
                    };
                    GameArea.Children.Add(snakePart.UiElement); //добавляем часть на канву (в игру). Часть змеи является дочерним элементом
                    Canvas.SetTop(snakePart.UiElement, snakePart.Position.Y); // элемент змеи помещает сверху на величину У
                    Canvas.SetLeft(snakePart.UiElement, snakePart.Position.X); // элемент змеи помещает слева на величину Х
                }
            }
        }
        private void WindowContentRendered(object sender, EventArgs e) //визуализация содержимого окна
        {
            DrawGameArea();
        }

        private void DrawGameArea()
        {
            bool filledInField = false;
            int nextX = 0, nextY = 0;
            int rowCounter = 0;
            bool wasOliveDrab = false;

            while (filledInField == false) //пока не заполнится всё поле
            {
                Ellipse ellipse = new Ellipse //создание нового круга на поле
                {
                    Width = constantSize,
                    Height = constantSize,                   //Color.FromRGB

                    Fill = wasOliveDrab ? Brushes.YellowGreen : Brushes.OliveDrab //проверяет прошлый цвет и ставит соотв.
                };
                GameArea.Children.Add(ellipse);//добавляет дочерний элемент - круг на поле
                Canvas.SetTop(ellipse, nextY); //расположения круга псверху на У    
                Canvas.SetLeft(ellipse, nextX); //расположение круга слева на Х

                wasOliveDrab = !wasOliveDrab; //меняем цвет
                nextX += constantSize; //передвигаем Х на следующую клетку
                if (nextX >= GameArea.ActualWidth) //если по Х  дошли к концу поля
                {
                    nextX = 0; //обнуляем Х
                    nextY += constantSize;//сдвигаемся по У на следующую полосу 
                    rowCounter++; //счетчик полос увеличиваем
                    wasOliveDrab = (rowCounter % 2 != 0); //проверяем какой первый цвет (чётная или нечётная полоса)
                }

                if (nextY >= GameArea.ActualHeight) //если по У дошли к концу
                    filledInField = true; //поле заполнено
            }
        }

        private void RedrawSnake()
        {
            // Удаляем последнюю часть змеи, при подготовке новой части добавленной ниже
            while (map.Snake.SnakeParts.Count >= map.Snake.SnakeLength) //если кол-во частей змеи >= длине змеи
            {
                GameArea.Children.Remove(map.Snake.SnakeParts[0].UiElement); //удаляем голову на поле
                map.Snake.SnakeParts.RemoveAt(0); //удаляем голову (элемент с индексом 0) в списке элементов
            }
            // Далее мы добавим новый элемент к змее, который будет представлять собой (новую) голову
            // Поэтому мы помечаем все существующие детали как элементы без головы (тела)
            foreach (SnakePart snakePart in map.Snake.SnakeParts) //бежим по списку частей
            {
                (snakePart.UiElement as Ellipse).Fill = colorBody; //рисует элемент тела и закрашивает в соотв. цвет тела
                snakePart.IsHead = false; //помечаем отсутствие головы
            }
            map.Snake.MoveSnake(constantSize);
            DrawSnake();
            CheckForCollisions();       
        }

        private void DrawSnakeFood()
        {
            Point foodPosition = map.GetNextFoodPosition((int)(GameArea.ActualWidth / constantSize),(int)(GameArea.ActualHeight / constantSize));
            snakeFood = new Ellipse() //создаем еду
            {
                Width = constantSize,
                Height = constantSize,
                Fill = colorFood
            };
            GameArea.Children.Add(snakeFood); //размещаем еду на поле
            Canvas.SetTop(snakeFood, foodPosition.Y);
            Canvas.SetLeft(snakeFood, foodPosition.X);
        }

        private void UpdateGameStatus()
        {       //изменяе скорость и счёт в игре
           Title = "Snake - Score: " + map.CurrentScore + " - Game speed: " + map.GameSpeed;
        }

        private void EndGame()
        {
            map.DisableTimer();
            MessageBox.Show("Оооопс, кажется, ты умер", "Snake");//меседж бокс о поражении
        }                                

        private void CheckForCollisions()
        {
            SnakePart snakeHead = map.Snake.SnakeParts[map.Snake.SnakeParts.Count - 1]; //голова последняя  в списке

            if ((snakeHead.Position.X == Canvas.GetLeft(snakeFood)) && (snakeHead.Position.Y == Canvas.GetTop(snakeFood)))
            {                                        //если голова змеи встретила еду
                map.EatSnakeFood();
                GameArea.Children.Remove(snakeFood); //удаление съеденной еды
                DrawSnakeFood();  //отрисовка новой еды
                UpdateGameStatus();
                return;
            }

            if ((snakeHead.Position.Y < 0) || (snakeHead.Position.Y >= GameArea.ActualHeight) ||
            (snakeHead.Position.X < 0) || (snakeHead.Position.X >= GameArea.ActualWidth))  //если произошёл выход за границы поля
            {
                EndGame();
            }
            //извлекает все элементы из списка частей змеи и робегается по каждой части
            foreach (SnakePart snakeBodyPart in map.Snake.SnakeParts.Take(map.Snake.SnakeParts.Count - 1))
            {                                          //если текущее положение головы==положению хоть одной части змеи игра закончена
                if ((snakeHead.Position.X == snakeBodyPart.Position.X) && (snakeHead.Position.Y == snakeBodyPart.Position.Y))
                    EndGame();
            }
        }

        private void GameTickTimerTick(object sender, EventArgs e)
        {
            RedrawSnake();
        }

        private void Window_Content(object sender, EventArgs e)
        {
            DrawGameArea();
            StartNewGame();
        }

        public MainWindow()
        {
            InitializeComponent();
            map.AddEventHandler(GameTickTimerTick);
        }
    }
}
