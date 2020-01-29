namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class DynamicReflectionDelegateFactory : ReflectionDelegateFactory
    {
        public static DynamicReflectionDelegateFactory Instance = new DynamicReflectionDelegateFactory();

        public override Func<T> CreateDefaultConstructor<T>(Type type)
        {
            DynamicMethod method1 = CreateDynamicMethod("Create" + type.FullName, typeof(T), ReflectionUtils.EmptyTypes, type);
            method1.InitLocals = true;
            ILGenerator iLGenerator = method1.GetILGenerator();
            this.GenerateCreateDefaultConstructorIL(type, iLGenerator);
            return (Func<T>) method1.CreateDelegate(typeof(Func<T>));
        }

        private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
            if (owner.IsInterface())
            {
                return new DynamicMethod(name, returnType, parameterTypes, owner.Module, true);
            }
            return new DynamicMethod(name, returnType, parameterTypes, owner, true);
        }

        public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
        {
            if (fieldInfo.IsLiteral)
            {
                object constantValue = fieldInfo.GetValue(null);
                return o => constantValue;
            }
            Type[] parameterTypes = new Type[] { typeof(object) };
            DynamicMethod method1 = CreateDynamicMethod("Get" + fieldInfo.Name, typeof(T), parameterTypes, fieldInfo.DeclaringType);
            ILGenerator iLGenerator = method1.GetILGenerator();
            this.GenerateCreateGetFieldIL(fieldInfo, iLGenerator);
            return (Func<T, object>) method1.CreateDelegate(typeof(Func<T, object>));
        }

        public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
        {
            Type[] parameterTypes = new Type[] { typeof(T) };
            DynamicMethod method1 = CreateDynamicMethod("Get" + propertyInfo.Name, typeof(object), parameterTypes, propertyInfo.DeclaringType);
            ILGenerator iLGenerator = method1.GetILGenerator();
            this.GenerateCreateGetPropertyIL(propertyInfo, iLGenerator);
            return (Func<T, object>) method1.CreateDelegate(typeof(Func<T, object>));
        }

        public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
        {
            Type[] parameterTypes = new Type[] { typeof(object), typeof(object[]) };
            DynamicMethod method1 = CreateDynamicMethod(method.ToString(), typeof(object), parameterTypes, method.DeclaringType);
            ILGenerator iLGenerator = method1.GetILGenerator();
            this.GenerateCreateMethodCallIL(method, iLGenerator, 1);
            return (MethodCall<T, object>) method1.CreateDelegate(typeof(MethodCall<T, object>));
        }

        public override ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
        {
            Type[] parameterTypes = new Type[] { typeof(object[]) };
            DynamicMethod method1 = CreateDynamicMethod(method.ToString(), typeof(object), parameterTypes, method.DeclaringType);
            ILGenerator iLGenerator = method1.GetILGenerator();
            this.GenerateCreateMethodCallIL(method, iLGenerator, 0);
            return (ObjectConstructor<object>) method1.CreateDelegate(typeof(ObjectConstructor<object>));
        }

        public override Action<T, object> CreateSet<T>(FieldInfo fieldInfo)
        {
            Type[] parameterTypes = new Type[] { typeof(T), typeof(object) };
            DynamicMethod method1 = CreateDynamicMethod("Set" + fieldInfo.Name, null, parameterTypes, fieldInfo.DeclaringType);
            ILGenerator iLGenerator = method1.GetILGenerator();
            GenerateCreateSetFieldIL(fieldInfo, iLGenerator);
            return (Action<T, object>) method1.CreateDelegate(typeof(Action<T, object>));
        }

        public override Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
        {
            Type[] parameterTypes = new Type[] { typeof(T), typeof(object) };
            DynamicMethod method1 = CreateDynamicMethod("Set" + propertyInfo.Name, null, parameterTypes, propertyInfo.DeclaringType);
            ILGenerator iLGenerator = method1.GetILGenerator();
            GenerateCreateSetPropertyIL(propertyInfo, iLGenerator);
            return (Action<T, object>) method1.CreateDelegate(typeof(Action<T, object>));
        }

        private void GenerateCreateDefaultConstructorIL(Type type, ILGenerator generator)
        {
            if (type.IsValueType())
            {
                generator.DeclareLocal(type);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Box, type);
            }
            else
            {
                ConstructorInfo con = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, ReflectionUtils.EmptyTypes, null);
                if (con == null)
                {
                    throw new ArgumentException("Could not get constructor for {0}.".FormatWith(CultureInfo.InvariantCulture, type));
                }
                generator.Emit(OpCodes.Newobj, con);
            }
            generator.Return();
        }

        private void GenerateCreateGetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
        {
            if (!fieldInfo.IsStatic)
            {
                generator.PushInstance(fieldInfo.DeclaringType);
                generator.Emit(OpCodes.Ldfld, fieldInfo);
            }
            else
            {
                generator.Emit(OpCodes.Ldsfld, fieldInfo);
            }
            generator.BoxIfNeeded(fieldInfo.FieldType);
            generator.Return();
        }

        private void GenerateCreateGetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
        {
            MethodInfo getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod == null)
            {
                throw new ArgumentException("Property '{0}' does not have a getter.".FormatWith(CultureInfo.InvariantCulture, propertyInfo.Name));
            }
            if (!getMethod.IsStatic)
            {
                generator.PushInstance(propertyInfo.DeclaringType);
            }
            generator.CallMethod(getMethod);
            generator.BoxIfNeeded(propertyInfo.PropertyType);
            generator.Return();
        }

        private void GenerateCreateMethodCallIL(MethodBase method, ILGenerator generator, int argsIndex)
        {
            ParameterInfo[] parameters = method.GetParameters();
            Label label = generator.DefineLabel();
            generator.Emit(OpCodes.Ldarg, argsIndex);
            generator.Emit(OpCodes.Ldlen);
            generator.Emit(OpCodes.Ldc_I4, parameters.Length);
            generator.Emit(OpCodes.Beq, label);
            generator.Emit(OpCodes.Newobj, typeof(TargetParameterCountException).GetConstructor(ReflectionUtils.EmptyTypes));
            generator.Emit(OpCodes.Throw);
            generator.MarkLabel(label);
            if (!method.IsConstructor && !method.IsStatic)
            {
                generator.PushInstance(method.DeclaringType);
            }
            int arg = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo info = parameters[i];
                Type parameterType = info.ParameterType;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                    LocalBuilder local = generator.DeclareLocal(parameterType);
                    if (!info.IsOut)
                    {
                        generator.PushArrayInstance(argsIndex, i);
                        if (parameterType.IsValueType())
                        {
                            Label label2 = generator.DefineLabel();
                            Label label3 = generator.DefineLabel();
                            generator.Emit(OpCodes.Brtrue_S, label2);
                            generator.Emit(OpCodes.Ldloca_S, local);
                            generator.Emit(OpCodes.Initobj, parameterType);
                            generator.Emit(OpCodes.Br_S, label3);
                            generator.MarkLabel(label2);
                            generator.PushArrayInstance(argsIndex, i);
                            generator.UnboxIfNeeded(parameterType);
                            generator.Emit(OpCodes.Stloc, arg);
                            generator.MarkLabel(label3);
                        }
                        else
                        {
                            generator.UnboxIfNeeded(parameterType);
                            generator.Emit(OpCodes.Stloc, arg);
                        }
                    }
                    generator.Emit(OpCodes.Ldloca_S, local);
                    arg++;
                }
                else if (parameterType.IsValueType())
                {
                    generator.PushArrayInstance(argsIndex, i);
                    Label label4 = generator.DefineLabel();
                    Label label5 = generator.DefineLabel();
                    generator.Emit(OpCodes.Brtrue_S, label4);
                    LocalBuilder local = generator.DeclareLocal(parameterType);
                    generator.Emit(OpCodes.Ldloca_S, local);
                    generator.Emit(OpCodes.Initobj, parameterType);
                    generator.Emit(OpCodes.Ldloc, arg);
                    generator.Emit(OpCodes.Br_S, label5);
                    generator.MarkLabel(label4);
                    generator.PushArrayInstance(argsIndex, i);
                    generator.UnboxIfNeeded(parameterType);
                    generator.MarkLabel(label5);
                    arg++;
                }
                else
                {
                    generator.PushArrayInstance(argsIndex, i);
                    generator.UnboxIfNeeded(parameterType);
                }
            }
            if (method.IsConstructor)
            {
                generator.Emit(OpCodes.Newobj, (ConstructorInfo) method);
            }
            else
            {
                generator.CallMethod((MethodInfo) method);
            }
            Type type2 = method.IsConstructor ? method.DeclaringType : ((MethodInfo) method).ReturnType;
            if (type2 != typeof(void))
            {
                generator.BoxIfNeeded(type2);
            }
            else
            {
                generator.Emit(OpCodes.Ldnull);
            }
            generator.Return();
        }

        internal static void GenerateCreateSetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
        {
            if (!fieldInfo.IsStatic)
            {
                generator.PushInstance(fieldInfo.DeclaringType);
            }
            generator.Emit(OpCodes.Ldarg_1);
            generator.UnboxIfNeeded(fieldInfo.FieldType);
            if (!fieldInfo.IsStatic)
            {
                generator.Emit(OpCodes.Stfld, fieldInfo);
            }
            else
            {
                generator.Emit(OpCodes.Stsfld, fieldInfo);
            }
            generator.Return();
        }

        internal static void GenerateCreateSetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
        {
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);
            if (!setMethod.IsStatic)
            {
                generator.PushInstance(propertyInfo.DeclaringType);
            }
            generator.Emit(OpCodes.Ldarg_1);
            generator.UnboxIfNeeded(propertyInfo.PropertyType);
            generator.CallMethod(setMethod);
            generator.Return();
        }
    }
}

