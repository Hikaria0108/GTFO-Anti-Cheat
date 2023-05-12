using System;
using System.Reflection;
using HarmonyLib;
using System.Collections.Generic;

namespace Hikaria.GTFO_Anti_Cheat.Utils
{
    internal abstract class Patch
    {
        public virtual void Initialize()
        {
        }

        protected internal Harmony Harmony { get; set; }

        public virtual bool Enabled
        {
            get
            {
                return true;
            }
        }

        public virtual string Name { get; }

        public abstract void Execute();

        public void PatchConstructor<TClass>(PatchType patchType, string prefixMethodName = null, string postfixMethodName = null) where TClass : class
        {
            this.PatchConstructor<TClass>(null, patchType, prefixMethodName, postfixMethodName);
        }

        public void PatchConstructor<TClass>(Type[] parameters, PatchType patchType, string prefixMethodName = null, string postfixMethodName = null) where TClass : class
        {
            ConstructorInfo methodBase = AccessTools.Constructor(typeof(TClass), parameters, false);
            this.PatchMethod<TClass>(methodBase, patchType, prefixMethodName, postfixMethodName);
        }

        public void PatchMethod<TClass>(string methodName, PatchType patchType, Type[] generics = null, string prefixMethodName = null, string postfixMethodName = null) where TClass : class
        {
            this.PatchMethod<TClass>(methodName, null, patchType, generics, prefixMethodName, postfixMethodName);
        }

        public void PatchMethod<TClass>(string methodName, Type[] parameters, PatchType patchType, Type[] generics = null, string prefixMethodName = null, string postfixMethodName = null) where TClass : class
        {
            MethodInfo methodBase = AccessTools.Method(typeof(TClass), methodName, parameters, generics);
            this.PatchMethod<TClass>(methodBase, patchType, prefixMethodName, postfixMethodName);
        }

        public void PatchMethod<TClass>(MethodBase methodBase, PatchType patchType, string prefixMethodName = null, string postfixMethodName = null) where TClass : class
        {
            this.PatchMethod(typeof(TClass), methodBase, patchType, prefixMethodName, postfixMethodName);
        }

        public void PatchMethod(Type classType, string methodName, PatchType patchType, Type[] generics = null, string prefixMethodName = null, string postfixMethodName = null)
        {
            this.PatchMethod(classType, methodName, null, patchType, generics, prefixMethodName, postfixMethodName);
        }

        public void PatchMethod(Type classType, string methodName, Type[] parameters, PatchType patchType, Type[] generics = null, string prefixMethodName = null, string postfixMethodName = null)
        {
            MethodInfo methodBase = AccessTools.Method(classType, methodName, parameters, generics);
            this.PatchMethod(classType, methodBase, patchType, prefixMethodName, postfixMethodName);
        }

        public void PatchMethod(Type classType, MethodBase methodBase, PatchType patchType, string prefixMethodName = null, string postfixMethodName = null)
        {
            string str = classType.Name.Replace("`", "__");
            string arg = methodBase.ToString();
            string str2 = methodBase.IsConstructor ? "ctor" : methodBase.Name;
            MethodInfo methodInfo = null;
            MethodInfo methodInfo2 = null;
            if ((patchType & PatchType.Prefix) > (PatchType)0)
            {
                try
                {
                    methodInfo2 = AccessTools.Method(base.GetType(), prefixMethodName ?? (str + "__" + str2 + "__Prefix"), null, null);
                }
                catch (Exception arg2)
                {
                    Logs.LogFatal(string.Format("未能获得 ({0}) 的前缀补丁方法: {1}", arg, arg2));
                }
            }
            if ((patchType & PatchType.Postfix) > (PatchType)0)
            {
                try
                {
                    methodInfo = AccessTools.Method(base.GetType(), postfixMethodName ?? (str + "__" + str2 + "__Postfix"), null, null);
                }
                catch (Exception arg3)
                {
                    Logs.LogFatal(string.Format("未能获得 ({0}) 的后缀补丁方法: {1}", arg, arg3));
                }
            }
            try
            {
                if (methodInfo2 != null && methodInfo != null)
                {
                    this.Harmony.Patch(methodBase, new HarmonyMethod(methodInfo2), new HarmonyMethod(methodInfo), null, null, null);
                }
                else if (methodInfo2 != null)
                {
                    this.Harmony.Patch(methodBase, new HarmonyMethod(methodInfo2), null, null, null, null);
                }
                else if (methodInfo != null)
                {
                    this.Harmony.Patch(methodBase, null, new HarmonyMethod(methodInfo), null, null, null);
                }
            }
            catch (Exception arg4)
            {
                Logs.LogError(string.Format("无法为 {0} 方法打补丁: {1}", arg, arg4));
            }
        }

        [Flags]
        internal enum PatchType : byte
        {
            Prefix = 1,
            Postfix = 2,
            Both = 3
        }

        internal static void RegisterPatch<T>() where T : Patch, new()
        {
            if (EntryPoint.s_harmonyInstance == null)
            {
                EntryPoint.s_harmonyInstance = new Harmony(PluginInfo.PLUGIN_GUID);
            }
            if (RegisteredPatches.ContainsKey(typeof(T)))
            {
                Logs.LogMessage(string.Format(EntryPoint.Language.IGNORE_REPEAT_PATCH, typeof(T).Name));
                return;
            }
            T t = Activator.CreateInstance<T>();
            t.Harmony = EntryPoint.s_harmonyInstance;
            T t2 = t;
            t2.Initialize();
            if (t2.Enabled)
            {
                Logs.LogMessage(string.Format(EntryPoint.Language.PATCHING, t2.Name));
                t2.Execute();
            }
            RegisteredPatches[typeof(T)] = t2;
        }

        internal protected static readonly Dictionary<Type, Patch> RegisteredPatches = new Dictionary<Type, Patch>();
    }
}
