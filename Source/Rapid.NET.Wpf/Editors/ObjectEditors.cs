using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Rapid.NET.Wpf.Editors
{

    public interface IObjectEditor
    {
        void SetValue(object newValue);

        object GetValue();
    }

    public interface IArgEditor
    {
        string GetString(object value);
        object GetValue(string text);
    }



    public static class ObjectEditors
    {
        public enum EditorType
        {
            String, Enum, Numeric, Array, Dictionary, Bool,
        }

        private static readonly Type[] NUMERIC_TYPES = new Type[] { typeof(int), typeof(float), typeof(double) };
        private static readonly Type[] NULLABLE_NUMERIC_TYPES = new Type[] { typeof(int?), typeof(float?), typeof(double?) };

        public static void TrySetValueFromArgs(
            this IObjectEditor editor, string args, Type objectType)
        {
            if (args == null)
                return;

            try
            {
                object value = JsonObject.Deserialize(args, objectType);
                editor.SetValue(value);
            }
            catch (Exception ex)
            {
                ScriptMethods.Warn("Could not set args for "
                    + objectType + ": " + args + ": " + ex);
            }
        }

        public static bool TryGetValue(this IObjectEditor editor, out object ret)
        {
            ret = null;
            try
            {
                ret = editor.GetValue();
                return true;
            }
            catch (Exception ex)
            {
                ScriptMethods.Warn("Could not get args from form input: " + ex);
                return false;
            }
        }

        public static UserControl MakeEditor(Type type)
        {


            type.IsNullable(out Type underlying);
                //return new ArgEditor(type);

            EditorType et = GetEditorType(underlying);
            switch (et)
            {
                case EditorType.Dictionary:
                    return new DictionaryEditor(type);
                case EditorType.Enum:
                    return new EnumEditor(type);
                case EditorType.Bool:
                    return new EnumEditor(type);
                default:
                    return new ArgEditor(type);
        }
            //if (type == typeof(string))
            //{
            //    return new ArgEditor(type);
            //    //return new StringEditor2();
            //}
            //else if (type.IsEnum)
            //{
            //    return new EnumEditor(type);
            //}
            //else if (type == typeof(bool))
            //{
            //    return new StringEditor(type); //TODO
            //}
            //else if (type == typeof(bool?))
            //{
            //    return new StringEditor(type); //TODO
            //}
            //else if (NUMERIC_TYPES.Contains(type))
            //{
            //    return new NumericEditor(type);
            //}
            //else if (NULLABLE_NUMERIC_TYPES.Contains(type))
            //{
            //    return new StringEditor(type); //TODO
            //}
            //else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            //{
            //    //return new ArrayEditor(type);
            //    return new ArgEditor(type);
            //}
            //else if (GetEnumerableType(type) != null)
            //{
            //    return new ArgEditor(type);
            //    //return new ArrayEditor(type);
            //}
            //else if (GetParser(type) != null)
            //{
            //    return new StringEditor(type);
            //}
            //else
            //{
            //    try
            //    {
            //        return new DictionaryEditor(type);
            //    }
            //    catch (Exception)
            //    {
            //        return new StringEditor(type);
            //    }
            //}
        }


        public static EditorType GetEditorType(Type type)
        {
            if (type.IsNullable(out Type underlying))
                return GetEditorType(underlying);

            if (type == typeof(string))
                return EditorType.String;
            else if (type.IsEnum)
                return EditorType.Enum;
            else if (type == typeof(bool))
                return EditorType.Bool;
            else if (NUMERIC_TYPES.Contains(type))
                return EditorType.Numeric;
            else if (NULLABLE_NUMERIC_TYPES.Contains(type))
                return EditorType.String;
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return EditorType.Array;
            else if (GetEnumerableType(type) != null)
                return EditorType.Array;
            else if (GetParser(type) != null)
                return EditorType.String;
            else
            {
                try
                {
                    return EditorType.Dictionary;
                }
                catch (Exception)
                {
                    return EditorType.String;
                }
            }
        }


        /// <summary>
        /// Get a function that converts a string to an object of the specified type
        /// </summary>
        public static Func<string, object> GetParser(Type type)
        {
            if (type == typeof(string))
                return s => s;
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return GetParser(type.GetGenericArguments()[0]);

            MethodInfo parse = type.GetMethod("Parse", 
                BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);

            if (parse == null)
                return null;
            else
                return s => parse.Invoke(null, new object[] { s });
        }

        /// <summary>
        /// For a type that inherits generic IEnumerable, get the type of each item in that IEnumerable
        /// </summary>
        public static Type GetEnumerableType(Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return intType.GetGenericArguments()[0];
            }
            return null;
        }


        public static Func<object> GetDefaultMaker(Type type)
        {
            ConstructorInfo ci = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                null, new Type[] { }, null);
            if (ci == null)
                return () => FormatterServices.GetUninitializedObject(type);
            else
                return () => ci.Invoke(new object[] { });
        }

        public static bool IsNullable(this Type t, out Type underlying)
        {
            underlying = t;
            if (t == null)
                return false;

            bool ret = t.IsGenericType 
                && t.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (ret)
                underlying = t.GetGenericArguments()[0];

            return ret;
        }

        public static string NiceName(this Type t, bool ignoreNest = false)
        {
            if (t.IsDefined(typeof(CompilerGeneratedAttribute), false))
                return t.Name;
            else if (t.FullName != null && map.ContainsKey(t.FullName))
                return map[t.FullName];
            else if (t.IsNested && !ignoreNest && !t.IsGenericParameter)
                return t.DeclaringType.NiceName() + "." + NiceName(t, true);
            else if (t.IsGenericType)
            {
                Type[] gen = t.GetGenericArguments();

                if (t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return gen.First().NiceName() + "?";

                string ret = t.Name;
                if (ret.Contains("`"))
                    ret = ret.Substring(0, t.Name.IndexOf("`")) + "<";
                foreach (Type g in gen)
                {
                    ret += g.NiceName() + ", ";
                }

                return ret.Substring(0, ret.Length - 2) + ">";
            }
            else
                return t.Name;
        }

        private static Dictionary<string, string> map =
            new Dictionary<string, string>() {
                {"System.Boolean","bool"},
                {"System.Byte","byte"},
                {"System.SByte","sbyte"},
                {"System.Char","char"},
                {"System.Decimal","decimal"},
                {"System.Double","double"},
                {"System.Single","float"},
                {"System.Int32","int"},
                {"System.UInt32","uint"},
                {"System.Int64","long"},
                {"System.UInt64","ulong"},
                {"System.Object","object"},
                {"System.Int16","short"},
                {"System.UInt16","ushort"},
                {"System.String","string"},
                {"System.Void","void"},
            };



    }
}
