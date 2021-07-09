﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace BOT_Marvin___Counter_Strike_Discord_Bot.Viewers.Modifiers
{ 
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewerModifierAttribute : Attribute
    {
        public static List<ViewerModifier> GetAllModifiers()
        {
            List<ViewerModifier> mods = new List<ViewerModifier>();

            foreach(Type t in Assembly.GetEntryAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(ViewerModifier)) && t.GetCustomAttribute(typeof(ViewerModifierAttribute)) != null)
                {
                    try
                    {
                        ConstructorInfo inf = t.GetConstructor(new Type[0]);
                        if (inf == null)
                            throw new InvalidOperationException("A viewer modifier has been found, but there is no Constructor taking 0 Arguments!!!");
                        mods.Add((ViewerModifier)inf.Invoke(null));
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }


            return mods;
        }
    }
}
