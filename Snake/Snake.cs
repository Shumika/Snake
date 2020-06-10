using System.Collections.Generic;
using System.Windows;

namespace Семестровка
{
    public class Snake
    {
        public List<SnakePart> SnakeParts = new List<SnakePart>(); //лист частей змеи
        public SnakeDirection Direction = SnakeDirection.Right; //сохранение текущего направления
        public int SnakeLength; //длина змеи
        public enum SnakeDirection { Left, Right, Up, Down };
        const int SnakeStartLength = 3;

        public void ResetParam(Point point)
        {
            SnakeLength = SnakeStartLength;
            Direction = SnakeDirection.Right;
            SnakeParts.Add(new SnakePart() { Position = point });
        }

        public void MoveSnake(int constantSize)
        {
            // Определите, в каком направлении нужно развернуть змею, исходя из текущего направления
            SnakePart snakeHead = SnakeParts[SnakeParts.Count - 1]; //голова = последнему элементу в списке частей
            double nextX = snakeHead.Position.X; //следующая координата Х
            double nextY = snakeHead.Position.Y; //следующая кордината У
            switch (Direction) //перемещение змеи в нужном направление
            {  //  <  \/
                case SnakeDirection.Left:
                    nextX -= constantSize;
                    break;
                case SnakeDirection.Right:
                    nextX += constantSize;
                    break;
                case SnakeDirection.Up:
                    nextY -= constantSize;
                    break;
                case SnakeDirection.Down:
                    nextY += constantSize;
                    break;
            }

            // Теперь добавьте новую голову в наш список частей змеи.
            SnakeParts.Add(new SnakePart() //добавляем в список новый элемент
            {
                Position = new Point(nextX, nextY),
                IsHead = true //пометка, что глова есть
            });

        }

    }
}
