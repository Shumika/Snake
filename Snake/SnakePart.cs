using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Семестровка
{
    public class SnakePart
    {
        public UIElement UiElement { get; set; } //Класс UIElement добавляет поддержку таких сущностей WPF, как компоновка 
                                                 //(layout), ввод (input), фокус (focus) и события (events)
        public Point Position { get; set; } //позиция элемента
        public bool IsHead { get; set; } //голова или нет?

        
    }
}
