using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ClimbingCompetition.WPF
{
    public class DataRoutedEventArgs<T> : RoutedEventArgs
    {
        private readonly T data;

        public DataRoutedEventArgs(T data) { this.data = data; }

        public DataRoutedEventArgs(T data, RoutedEvent routedEvent)
            : base(routedEvent)
        { this.data = data; }

        public DataRoutedEventArgs(T data, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        { this.data = data; }

        public T Data
        {
            get
            {
                return this.data;
            }
        }
    }

    public delegate void DataRoutedEventHandler<T>(object sender, DataRoutedEventArgs<T> e);
}
