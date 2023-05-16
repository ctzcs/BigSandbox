namespace Box1.BoxUnbox
{
    public class BoxBox<T> where T:class
    {
        private T[] a;
        private int count = 0;

        void Add(T item)
        {
            a[count] = item;
        }
    }
}