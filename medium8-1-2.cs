using System;
using System.Collections.Generic;

namespace Task
{
    static class Program
    {
        private static void Main()
        {
            var scene = CreateScene();

            while (true) scene.UpdateScene();
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
    }

    public class GameObject
    {
        private readonly string _name;

        public GameObject(int x, int y, string name)
        {
            Position = new Vector2(x, y);
            _name = name;
        }

        private Vector2 Position { get; set; }

        public void Move(Random random)
        {
            Position = new Vector2(
                RandomStep(Position.X, random),
                RandomStep(Position.Y, random)
            );
        }

        public bool IsCollisionWith(GameObject gameObject)
        {
            return gameObject != null && Position.Equals(gameObject.Position);
        }

        private static int RandomStep(int value, Random random)
        {
            var result = value + random.Next(-1, 1);

            if (result < 0)
                result = 0;

            return result;
        }

        public void Print()
        {
            Console.SetCursorPosition(Position.X, Position.Y);
            Console.Write(_name);
        }

    }

    public class Scene
    {
        private readonly List<GameObject> _items = new List<GameObject>();
        private readonly Random _random = new Random();

        public void AddGameObject(GameObject gameObject) => _items.Add(gameObject);

        public void UpdateScene()
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
