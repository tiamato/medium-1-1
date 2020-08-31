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
            var defaultStepRange = new StepRangeHolder(-1, 1, -1, 1);

            var scene = new Scene();
            scene.AddGameObject(new GameObject(5, 5, "1", defaultStepRange));
            scene.AddGameObject(new GameObject(10, 10, "2", defaultStepRange));
            scene.AddGameObject(new GameObject(15, 15, "3", defaultStepRange));

            return scene;
        }
    }

    public class Vector2 : IComparable<Vector2>
    {
        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public int CompareTo(Vector2 vector)
        {
            if (vector == null)
            {
                return 1;
            }

            var compareX = X.CompareTo(vector.X);

            if (compareX != 0)
            {
                return compareX;
            }

            return Y.CompareTo(vector.Y);
        }
    }

    public class StepRangeHolder
    {
        public readonly int MinX;
        public readonly int MaxX;
        public readonly int MinY;
        public readonly int MaxY;

        public StepRangeHolder(int minX, int maxX, int minY, int maxY)
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
        private readonly StepRangeHolder _stepRange;

        public GameObject(int x, int y, string name, StepRangeHolder stepRange)
        {
            Position = new Vector2(x, y);
            Name = name;

            _stepRange = stepRange;
        }

        public Vector2 Position { get; private set; }

        public static int PositionComparison(GameObject x, GameObject y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return x.Position.CompareTo(y.Position);
        }

        public void Move(Random random)
        {
            Position = new Vector2(
                RandomStep(Position.X, random, _stepRange.MinX, _stepRange.MaxX),
                RandomStep(Position.Y, random, _stepRange.MinY, _stepRange.MaxY)
            );
        }

        public bool IsCollisionWith(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }

            return Position.CompareTo(gameObject.Position) == 0;
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
            _items.Sort(GameObject.PositionComparison);

            for (int i = 1; i < _items.Count; i++)
            {
                if (_items[i - 1].IsCollisionWith(_items[i]))
                {
                    do
                    {
                        _items.Remove(_items[i]);
                    }
                    while (i < _items.Count && _items[i - 1].IsCollisionWith(_items[i]));

                    _items.Remove(_items[i - 1]);

                    i--;
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
