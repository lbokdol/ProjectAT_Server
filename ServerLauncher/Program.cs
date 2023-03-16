using System;
using System.Reflection;

Assembly a = Assembly.Load("Session");

Type myType = a.GetType("Session.Program");

MethodInfo myMethod = myType.GetMethod("Main");

object obj = Activator.CreateInstance(myType);

myMethod.Invoke(obj, new string[1]);