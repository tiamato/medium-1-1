using System;
using System.Collections.Generic;

namespace Task
{
    static class Program
    {
        private static void Main()
        {
            var scene = CreateScene();
            var sceneLoop = new SceneLoop(scene);

            sceneLoop.StartConsoleGameCycle();
        }

        private static Scene CreateScene()
        {
            var stepRangeRestrictions = new StepRangeRestrictions(-1, 1, -1, 1);

            var scene = new Scene();
            scene.AddGameObject(new GameObject(5, 5, "1", stepRangeRestrictions));
            scene.AddGameObject(new GameObject(10, 10, "2", stepRangeRestrictions));
            scene.AddGameObject(new GameObject(15, 15, "3", stepRangeRestrictions));

            return scene;
        }
    }

    public class Vector2
    {
        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public bool Equals(Vector2 vector)
        {
            if (vector == null)
            {
                return false;
            }

            return X.Equals(vector.X) && Y.Equals(vector.Y);
        }
    }

    public class StepRangeRestrictions
    {
        public readonly int MinX;
        public readonly int MaxX;
        public readonly int MinY;
        public readonly int MaxY;

        public StepRangeRestrictions(int minX, int maxX, int minY, int maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }
    }

    public class GameObject
    {
        public readonly string Name;
        private readonly StepRangeRestrictions _stepRangeRestrictions;

        public GameObject(int x, int y, string name, StepRangeRestrictions stepRangeRestrictions)
        {
            Position = new Vector2(x, y);
            Name = name;

            _stepRangeRestrictions = stepRangeRestrictions;
        }

        public Vector2 Position { get; private set; }

        public void Move(Random random)
        {
            Position = new Vector2(
                RandomStep(Position.X, random, _stepRangeRestrictions.MinX, _stepRangeRestrictions.MaxX),
                RandomStep(Position.Y, random, _stepRangeRestrictions.MinY, _stepRangeRestrictions.MaxY)
            );
        }

        public bool IsCollisionWith(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }

            return Position.Equals(gameObject.Position);
        }

        private static int RandomStep(int value, Random random, int stepMinValue, int stepMaxValue)
        {
            var result = value + random.Next(stepMinValue, stepMaxValue);

            if (result < 0)
            {
                result = 0;
            }

            return result;
        }
    }

    public class Scene
    {
        private readonly List<GameObject> _items = new List<GameObject>();
        private readonly Random _random = new Random();

        public IEnumerable<GameObject> Items => _items;

        public event Action OnSceneUpdated;

        public void AddGameObject(GameObject gameObject)
        {
            _items.Add(gameObject);
        }

        public void UpdateScene()
        {
            KillGameObjectsWithCollision();
            MoveAll();

            OnSceneUpdated?.Invoke();
        }

        private void KillGameObjectsWithCollision()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var currentInCollision = false;

                for (int j = i + 1; j < _items.Count; j++)
                {
                    if (_items[i].IsCollisionWith(_items[j]))
                    {
                        currentInCollision = true;
                        _items.Remove(_items[j--]);
                    }
                }

                if (currentInCollision)
                {
                    _items.Remove(_items[i--]);
                }
            }
        }

        private void MoveAll()
        {
            foreach (var gameObject in _items)
            {
                gameObject.Move(_random);
            }
        }
    }

    public class SceneLoop
    {
        private readonly Scene _scene;

        public SceneLoop(Scene scene)
        {
            _scene = scene;
        }

        public void StartConsoleGameCycle()
        {
            var consoleView = new ConsoleView(_scene);

            while (true)
            {
                _scene.UpdateScene();
            }
        }
    }

    public class ConsoleView
    {
        private readonly Scene _scene;

        public ConsoleView(Scene scene)
        {
            _scene = scene;
            _scene.OnSceneUpdated += PrintAliveGameObjects;
        }

        private static void PrintGameObject(GameObject gameObject)
        {
            Console.SetCursorPosition(gameObject.Position.X, gameObject.Position.Y);
            Console.Write(gameObject.Name);
        }

        private void PrintAliveGameObjects()
        {
            foreach (var gameObject in _scene.Items)
            {
                PrintGameObject(gameObject);
            }
        }
    }
}
