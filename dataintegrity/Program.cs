using System;

namespace dataintegrity
{
    //целостность данных
    public class Statistics
    {
        public int SuccessCount; //количество успеха
        public int TotalCount; //количество испытаний
        public void Print()
        {
            Console.WriteLine("{0}%", (SuccessCount * 100) / TotalCount);
        }
    }
    public class Program
    {
        public static void Main()
        {
            var rnd = new Random();
            var stat = new Statistics();
            for (int i = 0; i < 1000; i++)
            {
                if (rnd.Next(3) > 1) stat.SuccessCount++;
                stat.TotalCount++;
            }
            stat.Print();

            //проблема в том, что никто нам не помешает написать вот так и это не правильно:
            //stat.TotalCount = 100;
            //stat.SuccessCount = 146;
            //stat.Print();
        }
    }

    //изменено и защищено
    public class Statistics1
    {
        private int totalCount;
        private int successCount;
        public void AccountCase(bool isSuccess)
        {
            if (isSuccess) successCount++;
            totalCount++;
        }
        public void Print()
        {
            Console.WriteLine("{0}%", (successCount * 100) / totalCount);
        }
    }
    public class Program1
    {
        public static void Main()
        {
            var rnd = new Random();
            var stat = new Statistics();
            for (int i = 0; i < 1000; i++)
                stat.AccountCase(rnd.Next(3) > 1);
            stat.Print();
            //Теперь так сделать нельзя: доступ к приватным полям возможен только изнутри класса
            //stat.totalCount = 100;
            //stat.successCount = 146;
        }
    }


    //Конструкторы
    public class TournirInfo0
    {
        public int TeamsCount { get; set; }
        public string[] TeamsNames { get; set; }
        public double[,] Scores { get; set; }
        //Все эти три поля связаны друг с другом, и нужно обеспечивать их целостность.
    }
    public class TournirInfo1
    {
        public int TeamsCount { get; private set; }
        public string[] TeamsNames { get; private set; }
        public double[,] Scores { get; private set; }
        public void Initialize(int teamsCount)
        {
            TeamsCount = teamsCount;
            TeamsNames = new string[teamsCount];
            Scores = new double[teamsCount, teamsCount];
        }
        //Теперь эти поля будут согласованы.
        //Но не очень хорошо, что между созданием класса var info=new TournirInfo1(); и его инициализацией info.Initialize(5); можно ошибочно использовать объект, к которому он не готов.
    }
    public class TournirInfo
    {
        public int TeamsCount { get; private set; }
        public string[] TeamsNames { get; private set; }
        public double[,] Scores { get; private set; }
        //Это конструктор - дополнительные действия по инициализации объекта
        public TournirInfo(int teamsCount)
        {
            TeamsCount = teamsCount;
            TeamsNames = new string[teamsCount];
            Scores = new double[teamsCount, teamsCount];
        }
        //может быть несколько конструкторов, по аналогии с перегруженными методами
        public TournirInfo(params string[] teamNames)
        {
            TeamsCount = teamNames.Length;
            TeamsNames = teamNames;
            Scores = new double[TeamsCount, TeamsCount];
        }
        //public TournirInfo()
        //{
        //    TeamsCount = 4;
        //    TeamsNames = new string[TeamsCount];
        //    Scores = new double[TeamsCount, TeamsCount];
        //}
        //Так делать плохо: DRY
        // Лучше делать так: конструктор вызывает другой конструктор
        public TournirInfo()
            : this(4)
        { }
    }
    class Program3
    {
        public static void Main()
        {
            var info = new TournirInfo(5);
            //теперь объект сразу будет инициализирован, не может возникнуть ситуация, при которой он может быть создан, но не готов к использованию
        }
    }

    //статический конструктор
    class Test
    {
        public static readonly DateTime Readonly;
        public readonly int Number; //Статические конструкторы всегда без параметров
        static Test()
        {
            Readonly = DateTime.Now;
        }
        //это динамический конструктор
        public Test()
        {
            Number = 3;
        }
    }
    class Program7
    {
        public static void Main()
        {
            var test = new Test(); //сначала вызовется статический конструктор (настройка типа) /и только после этого - динамический
        }
    }

    //Вектор
    class Program4
    {
        public static void Check()
        {
            Vector vector = new Vector(3, 4);
            Console.WriteLine(vector.ToString()); //(3, 4) with length: 5

            vector.X = 0;
            vector.Y = -1;
            Console.WriteLine(vector.ToString()); //(0, -1) with length: 1

            vector = new Vector(9, 40);
            Console.WriteLine(vector.ToString()); //(9, 40) with length: 41

            Console.WriteLine(new Vector(0, 0).ToString()); //(0, 0) with length: 0
        }
    }
    public class Vector //поля этого класса инициализируются в конструкторе
    {
        public double X;
        public double Y;
        public double Length { get { return Math.Sqrt(X * X + Y * Y); } } // поле - вычисляемое свойство
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return string.Format("({0}, {1}) with length: {2}", X, Y, Length);
        }
    }


    //Поля readonly
    public class TournirInfo2
    {
        //readonly поле - это еще более сильное ограничение целостности
        //такие поля можно присваивать и изменять только в конструкторе
        public readonly int TeamsCount;
        public readonly string[] TeamsNames;
        public readonly double[,] Scores;
        public TournirInfo2(int teamsCount)
        {
            TeamsCount = teamsCount;
            TeamsNames = new string[teamsCount];
            Scores = new double[teamsCount, teamsCount];
        }
        public void SomeMethod()
        {
            // TeamsCount = 4; //так писать нельзя, хотя мы внутри класса
        }
    }
    public class Program5
    {
        public static void Main()
        {
            var info = new TournirInfo2(4);
            // info.TeamsCount = 5; //так тоже нельзя, хотя поле public
        }
    }
    class Test1
    {
        public readonly DateTime Time = DateTime.Now; //Вызов Now произойдет при создании объекта в неявно созданном конструкторе
    }

    //Дробь
    class Program6
    {
        public static void Check(int num, int den)
        {
            var ratio = new Ratio(num, den);
            Console.WriteLine("{0}/{1} = {2}", //1/2 = 0.5   -10/5 = -2   ArgumentException   ArgumentException
                ratio.Numerator, ratio.Denominator,
                ratio.Value.ToString(CultureInfo.InvariantCulture)); 
        }
    }
    public class Ratio //создания объекта Ratio нет возможности его изменить, то есть поменять поля Numerator, Denominator или Value.
    {
        public Ratio(int num, int den)
        {
            if (den <= 0)
                throw new ArgumentException(); //усл, что знаменатель всегда больше нуля - исключение ArgumentException при попытке установить неверное значение знаменателя
            Numerator = num;
            Denominator = den;
        }
        //Numerator, Denominator и Value должны остаться полями класса Ratio
        public readonly int Numerator; //числитель
        public readonly int Denominator; //знаменатель
        public double Value { get { return (double)Numerator / Denominator; } } //значение дроби Numerator / Denominator
    }
}
