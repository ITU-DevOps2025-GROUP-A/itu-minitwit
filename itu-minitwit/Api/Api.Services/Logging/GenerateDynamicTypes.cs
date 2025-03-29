using System.Reflection;
using System.Reflection.Emit;

namespace Api.Services.LogDecorator;

public abstract class GenerateDynamicTypes
{
    public static object? CreateDynamicInstance(string[] args)
    {
        var kvPairs = new Dictionary<string, object>();

        foreach (var item in args)
        {
            var parts = item.Split([':'], 2);
            if (parts.Length != 2) continue;
            
            var value = parts[1].Trim();
            if (string.IsNullOrWhiteSpace(value)) value = "null";
                
            kvPairs[parts[0].Trim()] = value;
        }

        var dynamicType = CreateTypeWithProperties(kvPairs.Keys);
        var dynamicInstance = Activator.CreateInstance(dynamicType);

        foreach (var kv in kvPairs)
        {
            dynamicType.GetProperty(kv.Key)?.SetValue(dynamicInstance, kv.Value);
        }

        return dynamicInstance;
    }
    private static Type CreateTypeWithProperties(IEnumerable<string> propertyNames)
    {
        var assemblyName = new AssemblyName("DynamicArgumentsAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var typeBuilder = moduleBuilder.DefineType(
            "DynamicArgumentsType",
            TypeAttributes.Public | TypeAttributes.Class
        );

        foreach (var propName in propertyNames)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + propName, typeof(string), FieldAttributes.Private);

            var propBuilder = typeBuilder.DefineProperty(propName, PropertyAttributes.HasDefault, typeof(string), null);

            // Getter
            var getter = typeBuilder.DefineMethod("get_" + propName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(string), Type.EmptyTypes);

            var getterIL = getter.GetILGenerator();
            getterIL.Emit(OpCodes.Ldarg_0);
            getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getterIL.Emit(OpCodes.Ret);

            // Setter
            var setter = typeBuilder.DefineMethod("set_" + propName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null, new[] { typeof(string) });

            var setterIL = setter.GetILGenerator();
            setterIL.Emit(OpCodes.Ldarg_0);
            setterIL.Emit(OpCodes.Ldarg_1);
            setterIL.Emit(OpCodes.Stfld, fieldBuilder);
            setterIL.Emit(OpCodes.Ret);

            // Hook up getter/setter to property
            propBuilder.SetGetMethod(getter);
            propBuilder.SetSetMethod(setter);
        }

        return typeBuilder.CreateType();
    }
}