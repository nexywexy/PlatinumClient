namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class ReflectionObject
    {
        public ReflectionObject()
        {
            this.Members = new Dictionary<string, ReflectionMember>();
        }

        public static ReflectionObject Create(Type t, params string[] memberNames) => 
            Create(t, null, memberNames);

        public static ReflectionObject Create(Type t, MethodBase creator, params string[] memberNames)
        {
            ReflectionObject obj2 = new ReflectionObject();
            ReflectionDelegateFactory reflectionDelegateFactory = JsonTypeReflector.ReflectionDelegateFactory;
            if (creator != null)
            {
                obj2.Creator = reflectionDelegateFactory.CreateParameterizedConstructor(creator);
            }
            else if (ReflectionUtils.HasDefaultConstructor(t, false))
            {
                Func<object> ctor = reflectionDelegateFactory.CreateDefaultConstructor<object>(t);
                obj2.Creator = args => ctor();
            }
            foreach (string str in memberNames)
            {
                MethodInfo info2;
                MemberInfo[] source = t.GetMember(str, BindingFlags.Public | BindingFlags.Instance);
                if (source.Length != 1)
                {
                    throw new ArgumentException("Expected a single member with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture, str));
                }
                MemberInfo memberInfo = source.Single<MemberInfo>();
                ReflectionMember member = new ReflectionMember();
                MemberTypes types = memberInfo.MemberType();
                if (types != MemberTypes.Field)
                {
                    if (types == MemberTypes.Method)
                    {
                        goto Label_0107;
                    }
                    if (types != MemberTypes.Property)
                    {
                        throw new ArgumentException("Unexpected member type '{0}' for member '{1}'.".FormatWith(CultureInfo.InvariantCulture, memberInfo.MemberType(), memberInfo.Name));
                    }
                }
                if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
                {
                    member.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
                }
                if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
                {
                    member.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
                }
                goto Label_01F3;
            Label_0107:
                info2 = (MethodInfo) memberInfo;
                if (info2.IsPublic)
                {
                    ParameterInfo[] parameters = info2.GetParameters();
                    if ((parameters.Length == 0) && (info2.ReturnType != typeof(void)))
                    {
                        MethodCall<object, object> call = reflectionDelegateFactory.CreateMethodCall<object>(info2);
                        member.Getter = target => call(target, new object[0]);
                    }
                    else if ((parameters.Length == 1) && (info2.ReturnType == typeof(void)))
                    {
                        MethodCall<object, object> call1 = reflectionDelegateFactory.CreateMethodCall<object>(info2);
                        member.Setter = delegate (object target, object arg) {
                            object[] args = new object[] { arg };
                            call1(target, args);
                        };
                    }
                }
            Label_01F3:
                if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
                {
                    member.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
                }
                if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
                {
                    member.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
                }
                member.MemberType = ReflectionUtils.GetMemberUnderlyingType(memberInfo);
                obj2.Members[str] = member;
            }
            return obj2;
        }

        public Type GetType(string member) => 
            this.Members[member].MemberType;

        public object GetValue(object target, string member) => 
            this.Members[member].Getter(target);

        public void SetValue(object target, string member, object value)
        {
            this.Members[member].Setter(target, value);
        }

        public ObjectConstructor<object> Creator { get; private set; }

        public IDictionary<string, ReflectionMember> Members { get; private set; }
    }
}

