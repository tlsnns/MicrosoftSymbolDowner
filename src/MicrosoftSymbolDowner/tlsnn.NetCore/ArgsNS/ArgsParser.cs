using System.Collections.Generic;

namespace tlsnn.NetCore.ArgsNS.ArgsNS
{
    static public class ArgsParser
    {
        static public void Parse(string[] args, ArgRouter argRouter)
        {
            string Option = null;
            List<string> listValue = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (IsOptionPrefix(args[i]))
                {
                    argRouter.Route(args[i], null);
                }
                else if (IsKeyPrefix(args[i]))
                {
                    Option = args[i];
                }
                else
                {
                    listValue.Add(args[i]);
                    if (i == args.Length - 1 || IsKeyPrefix(args[i + 1]))
                    {
                        argRouter.Route(Option, listValue.ToArray());
                        listValue.Clear();
                    }
                }
            }
        }
        static bool IsOptionPrefix(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }
            if (s.Length > 2 && s[0] == '-' && s[1] == '-')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static bool IsKeyPrefix(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }
            if (s[0] == '-')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
