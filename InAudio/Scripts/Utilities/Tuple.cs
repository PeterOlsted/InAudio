namespace InAudioSystem
{

    public static class Tuple
    {
        public static Tuple<T1> Create<T1>(T1 t1)
        {
            return new Tuple<T1>(t1);
        }

        public static Tuple<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
        {
            return new Tuple<T1, T2>(t1, t2);
        }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return new Tuple<T1, T2, T3>(t1, t2, t3);
        }

        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4);
        }

        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            return new Tuple<T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);
        }
    }

    public struct TupleS<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public TupleS(T1 t1, T2 t2)
        {
            this.Item1 = t1;
            this.Item2 = t2;
        }
    }


    public class Tuple<T1>
    {
        public T1 Item1;

        public Tuple(T1 t1)
        {
            this.Item1 = t1;
        }
    }

    public class Tuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public Tuple(T1 t1, T2 t2)
        {
            this.Item1 = t1;
            this.Item2 = t2;
        }
    }

    public class Tuple<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public Tuple(T1 t1, T2 t2, T3 t3)
        {
            this.Item1 = t1;
            this.Item2 = t2;
            this.Item3 = t3;
        }
    }

    public class Tuple<T1, T2, T3, T4>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public Tuple(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            this.Item1 = t1;
            this.Item2 = t2;
            this.Item3 = t3;
            this.Item4 = t4;
        }
    }

    public class Tuple<T1, T2, T3, T4, T5>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;

        public Tuple(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            this.Item1 = t1;
            this.Item2 = t2;
            this.Item3 = t3;
            this.Item4 = t4;
            this.Item5 = t5;
        }
    }
}