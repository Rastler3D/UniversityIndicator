using Microsoft.Extensions.DependencyInjection;
using Python.Deployment;
using Python.Runtime;
using System;
using System.Collections.Generic;

namespace Univer.Models
{
    public static class PythonService
    {
        public static IServiceCollection AddPython(this IServiceCollection service)
        {
            InitializePython();
            return service.AddSingleton(x=> Py.GIL());

        }
        public static IServiceCollection WithModules(this IServiceCollection service, params string[] moduleNames)
        {
            Array.ForEach(moduleNames, moduleName => InstallModule(moduleName));
            return service;
        }
        public static void InitializePython()
        {
            Installer.SetupPython();
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
            
            using (Py.GIL())
            {
                dynamic path = Py.Import("os");
                dynamic sys = Py.Import("sys");
                sys.path.append(path.getcwd());
            }
            
        }
        public static void InstallModule(string moduleName)
        {
            Installer.TryInstallPip();
            Installer.PipInstallModule(moduleName);
            PythonEngine.Initialize();
        }
        public static PyDict ToPython(this Dictionary<int,List<int>> dictionary)
        {
            PyDict dict = new PyDict();
            foreach (var entry in dictionary)
            {
                dict.SetItem(entry.Key.ToPython(), entry.Value.ToPython().ToPython());
            }
            return dict;
        }
        public static PyList ToPython<T>(this List<T> list)
        {
            PyList pyList = new PyList();
            foreach (var entry in list)
            {
                pyList.Append(entry.ToPython());
            }
            return pyList;
        }

    }
}
