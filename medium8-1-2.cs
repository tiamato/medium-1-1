using System;
using System.Collections.Generic;

namespace Task
{
    static class Program
    {
        private static void Main()
        {
            var scene = CreateScene();

            while (true) scene.Update();
        }

        private static Scene CreateScene()
        {
            var scene = new Scene();
            scene.AddGameObject(new GameObject(5, 5, "1"));
            scene.AddGameObject(new GameObject(10, 10, "2"));
            scene.AddGameObject(new GameObject(15, 15, "3"));

            return scene;
        }
    }

    public struct Vector2
    {
        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public bool Equals(Vector2 vector) => X.Equals(vector.X) && Y.Equals(vector.Y);

        public static Vector2 operator +(Vector2 v1, Vector2 v2) => new Vector2(v1.X + v2.X, v1.Y + v2.Y);
    }

    public static class RandomExtension
    {
        public static Vector2 GetDirection(this Random random) => new Vector2(random.Next(-1, 1), random.Next(-1, 1)); 
    }

    public class GameObject
    {
        private readonly string _name;
        private Vector2 _position;

        public GameObject(int x, int y, string name)
        {
            Position = new Vector2(x, y);
            _name = name;
        }

        private Vector2 Position
        {
            get => _position;
            set => _position = new Vector2(value.X < 0 ? 0 : value.X, value.Y < 0 ? 0 : value.Y);
        }

        public void Move(Random random) => Position += random.GetDirection();

        public bool IsCollisionWith(GameObject gameObject) => gameObject != null && _position.Equals(gameObject._position);

        public void Print()
        {
            Console.SetCursorPosition(_position.X, _position.Y);
            Console.Write(_name);
        }
    }

    public class Scene
    {
        private readonly List<GameObject> _items = new List<GameObject>();
        private readonly Random _random = new Random();

        public void AddGameObject(GameObject gameObject) => _items.Add(gameObject);

        public void Update()
        {
            KillGameObjectsWithCollision();
            MoveAll();

            PrintAliveGameObjects();
        }

        private void PrintAliveGameObjects()
        {
            foreach (var gameObject in _items) gameObject.Print();
        }

        private void KillGameObjectsWithCollision()
        {
            var itemsToRemove = new HashSet<GameObject>();

            foreach (var item in _items)
            {
                foreach (var anotherItem in _items)
                {
                    if (!item.Equals(anotherItem) && item.IsCollisionWith(anotherItem))
                        itemsToRemove.Add(item);
                }
            }

            foreach (var item in itemsToRemove)
                _items.Remove(item);
        }

        private void MoveAll()
        {
            foreach (var gameObject in _items)
                gameObject.Move(_random);
        }
    }
}
