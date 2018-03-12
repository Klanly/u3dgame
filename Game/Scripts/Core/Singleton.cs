using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yifan.Core
{
    public class Singleton<T> where T : class, new()
    {
        private static T instancce = null;

        public static T Instance
        {
            get
            {
                if (instancce == null)
                {
                    instancce = new T();
                }

                return instancce;
            }
        }
    }
}