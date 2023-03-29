namespace Common
{
    public class BaseManager<T> where T: new()
    {
        private static T m_instance;
        private static object obj = new object();

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (obj)
                    {
                        m_instance = new T();
                    }
                }

                return m_instance;
            }
        }
    }
}