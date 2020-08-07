using System;
using System.Collections.Generic;

namespace Task
{
    static class Program
    {
        private static void Main()
        {
            GameObjectList gameObjectList = new GameObjectList();
            gameObjectList.Add(new GameObject(5, 5, "1", true));
            gameObjectList.Add(new GameObject(10, 10, "2", true));
            gameObjectList.Add(new GameObject(15, 15, "3", true));

            while (true)
            {
                gameObjectList.KillAllWithCollision();
                gameObjectList.MoveAll();
                gameObjectList.PrintAllAlive();
            }
        }
    }

    public class Coordinate
    {
        public int Value { private set; get; }

        public Coordinate(int coordinate)
        {
            Value = coordinate;
        }

        public void RandomStep(Random random)
        {
            Value += random.Next(-1, 1);
            if (Value < 0)
            {
                Value = 0;
            }
        }

        public bool Equals(Coordinate coordinate)
        {
            return Value == coordinate.Value;
        }
    }

    public class GameObject
    {
        private readonly Coordinate _x;
        private readonly Coordinate _y;
        private readonly string _name;

        public bool IsAlive { private set; get; }

        public GameObject(int x, int y, string name, bool isAlive)
        {
            _x = new Coordinate(x);
            _y = new Coordinate(y);
            _name = name;
            IsAlive = isAlive;
        }

        public void Kill()
        {
            IsAlive = false;
        }

        public void RandomStep(Random random)
        {
            _x.RandomStep(random);
            _y.RandomStep(random);
        }

        public bool IsCollisionWith(GameObject gameObject)
        {
            return _x.Equals(gameObject._x) && _y.Equals(gameObject._y);
        }

        public void Print()
        {
            Console.SetCursorPosition(_x.Value, _y.Value);
            Console.Write(_name);
        }
    }

    public class GameObjectList
    {
        private readonly List<GameObject> _items = new List<GameObject>();
        private readonly Random _random = new Random();

        public void Add(GameObject gameObject)
        {
            _items.Add(gameObject);
        }

        public void MoveAll()
        {
            foreach (var gameObject in _items)
            {
                gameObject.RandomStep(_random);
            }
        }

        public void KillAllWithCollision()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                for (int j = i + 1; j < _items.Count; j++)
                {
                    if (_items[i].IsCollisionWith(_items[j]))
                    {
                        _items[i].Kill();
                        _items[j].Kill();
                    }
                }
            }
        }

        public void PrintAllAlive()
        {
            foreach (var gameObject in _items)
            {
                if (gameObject.IsAlive)
                {
                    gameObject.Print();
                }
            }
        }
    }
}