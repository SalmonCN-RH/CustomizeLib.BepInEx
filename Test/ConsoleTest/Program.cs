using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ConsoleTest
{
    public class Program
    {
        public delegate int MyFuncDelegate(int x);

        public static void Main(string[] args)
        {
            var m = ((Delegate)MyFunc).Method;
            RuntimeHelpers.PrepareMethod(m.MethodHandle);
            var ptr = m.MethodHandle.GetFunctionPointer();

            var d = Marshal.GetDelegateForFunctionPointer<MyFuncDelegate>(ptr);
            Console.WriteLine(d(0));

            foreach (var method in typeof(IMyInterface).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                Console.WriteLine($"{method.Name} {SubStringTail(method.Name, ".")}");

            Console.WriteLine($"{typeof(int).Assembly.GetName().Name} {typeof(int).Namespace} {typeof(int).Name}");
        }

        public static int MyFunc(int x)
        {
            Console.WriteLine(x);
            return x + 10;
        }

        public static string SubStringTail(string s, string sub) => s[(s.LastIndexOf(sub) + 1)..];
    }

    public interface IMyInterface
    {
        public void A();
        public void B();
    }

    public class MyClass : IMyInterface
    {
        void IMyInterface.A() { }

        public void B() { }
    }
}