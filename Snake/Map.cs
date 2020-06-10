using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Threading;

namespace Семестровка
{
    class Map
    {
        const int constantSize = 20;
        const int SnakeStartSpeed = 400;
        const int minimumSpeedSnake = 100;
        private DispatcherTimer gameTickTimer = new DispatcherTimer(); //таймер
        public int CurrentScore = 0; //текущий счёт
        public Snake Snake = new Snake();
        private Random rnd = new Random();//для положения еды
        public double GameSpeed => gameTickTimer.Interval.TotalMilliseconds;
        
        public void AddEventHandler (EventHandler action)
        {
            gameTickTimer.Tick += action;
        }

        public void ResetParameters()
        {
            Snake.SnakeParts.Clear();  //чистим лист частей
            //сбрасываем значеия до дефолтных 
            CurrentScore = 0;
            Snake.ResetParam(new Point(constantSize * 8, constantSize * 8));
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(SnakeStartSpeed);
            gameTickTimer.IsEnabled = true;
        }

        public Point GetNextFoodPosition(int width, int height ) //находит следующую позицию для еды         ?????????????????????????
        {
            int maxX = width;
            int maxY = height;
            int foodX = rnd.Next(0, maxX) * constantSize;
            int foodY = rnd.Next(0, maxY) * constantSize;

            foreach (SnakePart snakePart in Snake.SnakeParts) //бежим по списку частей змеи
            {
                if ((snakePart.Position.X == foodX) && (snakePart.Position.Y == foodY)) //если позиция еды = позиции части змеи
                    return GetNextFoodPosition(width, height); //заново вызываем этот метод
            }

            return new Point(foodX, foodY); //если позиция ок, создаем еду
        }
        public void EatSnakeFood()
        {
            Snake.SnakeLength++; //увеличени длины змеи
            CurrentScore++; //увеличение счёта (за еду)  //нижняя граница скорости змеи        //всего тиков - текущий счет*2
            int timerInterval = Math.Max(minimumSpeedSnake, (int)gameTickTimer.Interval.TotalMilliseconds - (CurrentScore * 3)); //уменьшаем интервал тиков 
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(timerInterval); //увеличение количества тиков для увеличение скорости змеи
        }

        public void DisableTimer()
        {
            gameTickTimer.IsEnabled = false;
        }
    }
}
