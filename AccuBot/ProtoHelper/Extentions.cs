using System.CodeDom.Compiler;
using System.Collections;
using System.Reflection;
using Google.Protobuf.WellKnownTypes;
using Spectre.Console;
using Type = System.Type;

namespace ProtoHelper;

//If no Spectre Console, https://stackoverflow.com/questions/34071716/c-sharp-conditional-compilation-if-assembly-exists

static public class Extentions
{

//#if SPECTRE
    static public Tree ToProtoTree(this object o)
    {
        var tree = new Tree(o.GetType().Name);
        return (Tree)tree.AddTreeObjects(o,0,true);
    }

    static public Tree ToClassTree(this object o)
    {
        var tree = new Tree($"[Red]{o.GetType().Name}[/]");
        return (Tree)tree.AddTreeObjects(o,0,false);
    }

    static public IHasTreeNodes AddTreeObjects(this IHasTreeNodes parent, object o, uint level, bool proto)
    {

        string[] levelColours = new[] { "green", "cyan" ,"blue"};
        string colour = levelColours[level < levelColours.Length ? level:levelColours.Length-1];

        Type t = o.GetType();
        PropertyInfo[] props = t.GetProperties(BindingFlags.Public|BindingFlags.Instance);
        foreach (PropertyInfo property in props)
        {
            var customAtt = property.GetCustomAttributes<GeneratedCodeAttribute>();
            if (proto && customAtt.All(x => x.Tool != "protoc")) continue;
            
            Type objType = property.PropertyType;
            var value = property?.GetValue(o);

            if (objType.IsPrimitive || objType.IsEnum || objType == typeof(string))
            {
                parent.AddNode($"[{colour}]{property?.Name}:[/] {value?.ToString() ?? ""}");
            }
            else if (typeof(Google.Protobuf.WellKnownTypes.Timestamp).IsAssignableFrom(objType))
            {
                var time = (Google.Protobuf.WellKnownTypes.Timestamp?)property?.GetValue(o);
                parent.AddNode($"[{colour}]{property?.Name}:[/] {time?.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss") ?? ""}");
            }
            else if (typeof(DateTime).IsAssignableFrom(objType))
            {
                parent.AddNode($"[{colour}]{property?.Name}:[/] {value:yyyy-MM-dd HH:mm:ss}");
            }
            else if (typeof(IEnumerable).IsAssignableFrom(objType))
            {
                if (objType.IsGenericType)
                {
                    Type[] at = objType.GetGenericArguments();
                    var genType = at.First<Type>();
                    if (genType.IsPrimitive || genType.IsEnum || genType == typeof(string))
                    {
                        var parentPlus = parent.AddNode($"[{colour}]{property?.Name}[/]");
                        foreach (object genObj in (IList)value)
                        {
                            parentPlus.AddNode($"{genObj?.ToString() ?? ""}");
                        }
                    }
                    else if (typeof(Google.Protobuf.WellKnownTypes.Timestamp).IsAssignableFrom(genType))
                    {
                        var parentPlus = parent.AddNode($"[{colour}]{property?.Name}[/]");
                        foreach (object genObj in (IList)value)
                        {
                            var time = (Google.Protobuf.WellKnownTypes.Timestamp)property.GetValue(genObj);
                            parentPlus.AddNode($"{time?.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss") ?? "null"}");
                        }
                    }
                    else
                    {
                        foreach (object genObj in (IList)value)
                        {
                            parent.AddNode(property.Name).AddTreeObjects(genObj,level+1, proto);
                        }
                    }
                }
            }
            else
            {
                parent.AddNode($"[{colour}]property.Name[/]").AddTreeObjects(property.GetValue(o),level+1, proto);
            }
        }

        return parent;
    }
//#endif
}