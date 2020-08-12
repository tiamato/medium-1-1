using System;
using System.Collections.Generic;
using System.Linq;

namespace Task
{
    static class Program
    {
        private static void Main()
        {
            var scene = InitScene();
            var sceneController = new SceneController(scene);

            sceneController.StartConsoleGameCycle();
        }

        private static Scene InitScene()
        {
            var defaultStepRange = new StepRange(-1, 1, -1, 1);

            var scene = new Scene();
            scene.AddGameObject(new GameObject(5, 5, "1", true, defaultStepRange));
            scene.AddGameObject(new GameObject(10, 10, "2", true, defaultStepRange));
            scene.AddGameObject(new GameObject(15, 15, "3", true, defaultStepRange));

            return scene;
        }
    }

    public class Coordinate
    {
        public int Value { private set; get; }

        public Coordinate(int coordinate)
        {
            Value = coordinate;
        }

        public void RandomStep(Random random, int stepMinValue, int stepMaxValue)
        {
            Value += random.Next(stepMinValue, stepMaxValue);
            if (Value < 0)
            {
                Value = 0;
            }
        }
    }

    public class StepRange
    {
        public int MinX { get; }
        public int MaxX { get; }
        public int MinY { get; }
        public int MaxY { get; }

        public StepRange(int minX, int maxX, int minY, int maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }
    }

    public class GameObject
    {
        public readonly Coordinate XCoordinate;
        public readonly Coordinate YCoordinate;
        public readonly string Name;
        public bool IsAlive { private set; get; }
        private readonly StepRange _stepRange;

        public GameObject(int x, int y, string name, bool isAlive, StepRange stepRange)
        {
            XCoordinate = new Coordinate(x);
            YCoordinate = new Coordinate(y);
            Name = name;
            IsAlive = isAlive;

            _stepRange = stepRange;
        }

        public void Die()
        {
            IsAlive = false;
        }

        public void RandomStep(Random random)
        {
            XCoordinate.RandomStep(random, _stepRange.MinX, _stepRange.MaxX);
            YCoordinate.RandomStep(random, _stepRange.MinY, _stepRange.MaxY);
        }
    }

    public interface IScene
    {
        IEnumerable<GameObject> GetAliveItems();

        event Action OnSceneUpdated;

        void UpdateScene();
    }

    public class Scene : IScene
    {
        private readonly List<GameObject> _items = new List<GameObject>();
        private readonly Random _random = new Random();

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
            var itemsWithCollision = _items
                .GroupBy(gameObject => new { X = gameObject.XCoordinate.Value, Y = gameObject.YCoordinate.Value })
                .Where(grouping => grouping.Count() > 1)
                .SelectMany(grouping => grouping);

            foreach (var gameObject in itemsWithCollision)
            {
                gameObject.Die();
            }
        }

        private void MoveAll()
        {
            foreach (var gameObject in _items)
            {
                gameObject.RandomStep(_random);
            }
        }

        public IEnumerable<GameObject> GetAliveItems()
        {
            return _items.Where(item => item.IsAlive);
        }
    }

    public class SceneController
    {
        private readonly IScene _scene;

        public SceneController(IScene scene)
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
        private readonly IScene _scene;

        public ConsoleView(IScene scene)
        {
            _scene = scene;
            _scene.OnSceneUpdated += PrintAliveGameObjects;
        }

        private static void PrintGameObject(GameObject gameObject)
        {
            Console.SetCursorPosition(gameObject.XCoordinate.Value, gameObject.YCoordinate.Value);
            Console.Write(gameObject.Name);
        }

        private void PrintAliveGameObjects()
        {
            foreach (var gameObject in _scene.GetAliveItems())
            {
                PrintGameObject(gameObject);
            }
        }
    }
}
