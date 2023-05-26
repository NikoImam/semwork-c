using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ProgrammInterface;

namespace GraphPartitorApp
{
    public static class Algorithm
    {

        public static IPartiteGraph GetPartitior(string path)
        {
            var asm = Assembly.LoadFrom(path);
            var types = asm.GetTypes();
            Type? myType = null;
            foreach (var t in types)
            {
                if (t.GetInterfaces().Contains(typeof(IPartiteGraph)))
                {
                    myType = t;
                    break; //забытый break
                }
            }
            if (myType != null)
            {
                return (IPartiteGraph)Activator.CreateInstance(myType)!;
            }

            throw new Exception("Загрузите правильную сборку");
        }
    }
}
