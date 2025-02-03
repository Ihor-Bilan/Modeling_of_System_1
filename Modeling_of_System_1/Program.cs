using System;
using System.Threading;

abstract class Fish
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Age { get; set; }

    public Fish(int x, int y)
    {
        X = x;
        Y = y;
        Age = 0;
    }

    public abstract void Move(Fish[,] pond);
}

class Crucian : Fish
{
    private static Random rand = new Random();
    private const int ReproductionAge = 5;

    public Crucian(int x, int y) : base(x, y) { }

    public override void Move(Fish[,] pond)
    {
        Age++;
        int newX, newY;
        do
        {
            newX = X + rand.Next(-1, 2);
            newY = Y + rand.Next(-1, 2);
        } while (!IsValidMove(pond, newX, newY));

        pond[X, Y] = null;
        X = newX;
        Y = newY;
        pond[X, Y] = this;

        if (Age % ReproductionAge == 0)
        {
            Spawn(pond);
        }
    }

    private void Spawn(Fish[,] pond)
    {
        int newX, newY;
        do
        {
            newX = rand.Next(pond.GetLength(0));
            newY = rand.Next(pond.GetLength(1));
        } while (pond[newX, newY] != null);

        pond[newX, newY] = new Crucian(newX, newY);
    }

    private bool IsValidMove(Fish[,] pond, int x, int y)
    {
        return x >= 0 && x < pond.GetLength(0) && y >= 0 && y < pond.GetLength(1) && pond[x, y] == null;
    }
}

class Pike : Fish
{
    private static Random rand = new Random();
    private const int MaxHunger = 5;
    private int hunger = MaxHunger;

    public Pike(int x, int y) : base(x, y) { }

    public override void Move(Fish[,] pond)
    {
        Age++;
        int preyX = -1, preyY = -1;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int newX = X + dx;
                int newY = Y + dy;
                if (IsValidPrey(pond, newX, newY))
                {
                    preyX = newX;
                    preyY = newY;
                }
            }
        }

        if (preyX != -1)
        {
            pond[preyX, preyY] = null;
            hunger = MaxHunger;
            MoveTo(pond, preyX, preyY);
        }
        else
        {
            hunger--;
            if (hunger <= 0)
            {
                pond[X, Y] = null;
                return;
            }
            int newX, newY;
            do
            {
                newX = X + rand.Next(-1, 2);
                newY = Y + rand.Next(-1, 2);
            } while (!IsValidMove(pond, newX, newY));
            MoveTo(pond, newX, newY);
        }
    }

    private void MoveTo(Fish[,] pond, int newX, int newY)
    {
        pond[X, Y] = null;
        X = newX;
        Y = newY;
        pond[X, Y] = this;
    }

    private bool IsValidPrey(Fish[,] pond, int x, int y)
    {
        return x >= 0 && x < pond.GetLength(0) && y >= 0 && y < pond.GetLength(1) && pond[x, y] is Crucian;
    }

    private bool IsValidMove(Fish[,] pond, int x, int y)
    {
        return x >= 0 && x < pond.GetLength(0) && y >= 0 && y < pond.GetLength(1) && pond[x, y] == null;
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Write("Введіть розмір ставка: ");
        int size = int.Parse(Console.ReadLine());
        Console.Write("Введіть кількість карасів: ");
        int numCrucians = int.Parse(Console.ReadLine());
        Console.Write("Введіть кількість щук: ");
        int numPikes = int.Parse(Console.ReadLine());
        Console.Write("Введіть кількість циклів: ");
        int cycles = int.Parse(Console.ReadLine());

        Simulation sim = new Simulation(size, numCrucians, numPikes);
        sim.Run(cycles);
    }
}

class Simulation
{
    private Fish[,] pond;
    private int size;
    private static Random rand = new Random();

    public Simulation(int size, int numCrucians, int numPikes)
    {
        this.size = size;
        pond = new Fish[size, size];
        InitializePond(numCrucians, numPikes);
    }

    private void InitializePond(int numCrucians, int numPikes)
    {
        for (int i = 0; i < numCrucians; i++)
            AddFish(new Crucian(rand.Next(size), rand.Next(size)));
        for (int i = 0; i < numPikes; i++)
            AddFish(new Pike(rand.Next(size), rand.Next(size)));
    }

    private void AddFish(Fish fish)
    {
        while (pond[fish.X, fish.Y] != null)
        {
            fish.X = rand.Next(size);
            fish.Y = rand.Next(size);
        }
        pond[fish.X, fish.Y] = fish;
    }

    public void Run(int cycles)
    {
        for (int i = 0; i < cycles; i++)
        {
            Update();
            DisplayPond();
            Thread.Sleep(1000);
        }
        Console.WriteLine("Симуляція завершена.");
    }

    private void Update()
    {
        foreach (var fish in pond)
        {
            fish?.Move(pond);
        }
    }

    private void DisplayPond()
    {
        Console.Clear();
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (pond[i, j] is Crucian) Console.Write("К ");
                else if (pond[i, j] is Pike) Console.Write("Щ ");
                else Console.Write(". ");
            }
            Console.WriteLine();
        }
    }
}