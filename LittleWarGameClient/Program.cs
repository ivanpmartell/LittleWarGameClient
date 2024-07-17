using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LittleWarGameClient
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                PreJITMethods(assembly);
                ForceLoadAll(assembly);
            }
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        static void PreJITMethods(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            foreach (Type curType in types)
            {
                MethodInfo[] methods = curType.GetMethods(
                        BindingFlags.DeclaredOnly |
                        BindingFlags.NonPublic |
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.Static);

                foreach (MethodInfo curMethod in methods)
                {
                    if (curMethod.IsAbstract ||
                        curMethod.ContainsGenericParameters)
                        continue;
                    try
                    {
                        RuntimeHelpers.PrepareMethod(curMethod.MethodHandle);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        static void ForceLoadAll(Assembly assembly)
        {
            ForceLoadAll(assembly, new HashSet<Assembly>());
        }

        private static void ForceLoadAll(Assembly assembly,
                                         HashSet<Assembly> loadedAssmblies)
        {
            bool alreadyLoaded = !loadedAssmblies.Add(assembly);
            if (alreadyLoaded)
                return;

            AssemblyName[] refrencedAssemblies =
                assembly.GetReferencedAssemblies();

            foreach (AssemblyName curAssemblyName in refrencedAssemblies)
            {
                try {
                    Assembly nextAssembly = Assembly.Load(curAssemblyName);
                    if (nextAssembly.GlobalAssemblyCache)
                        continue;

                    ForceLoadAll(nextAssembly, loadedAssmblies);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}