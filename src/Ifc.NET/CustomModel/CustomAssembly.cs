using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

using System.Globalization;
using System.Threading;


namespace Ifc4.CustomModel
{

	public class CustomAssembly
	{
		private Hashtable m_EnumTypeHashtable;
		private ModuleBuilder m_ModuleBuilder = null;

		private const string NOTAVAILABLE = "n/a";
		private int m_TextIndex = 0;

        public CustomAssembly()
		{
			m_EnumTypeHashtable = new Hashtable();
		}

        private Type GetEnumType(ref string enumName, CustomModel.CustomProperty customProperty)
		{
			object enumValue;
			string enumDesc;
			Type enumType;

			bool defined;

			if(!m_EnumTypeHashtable.ContainsKey(enumName))
				return null;

			System.Collections.IEnumerator enumer;
			System.Collections.DictionaryEntry entry;

			enumer = m_EnumTypeHashtable.GetEnumerator();
			while(enumer.MoveNext())
			{
				entry = (System.Collections.DictionaryEntry)enumer.Current;
				enumType = entry.Value as Type;
				if(enumType == null)
					continue;

				if(entry.Key.ToString().StartsWith(enumName) == false)
					continue;

                //Array enumArray = System.Enum.GetValues(enumType);
                //defined = false;
                //for(int i=0; i<enumArray.Length; i++)
                //{
                //    defined = false;
                //    enumValue = enumArray.GetValue(i);

                //    foreach (CustomModel.CustomPropertyStandardValue pei in customProperty.ComboboxValues)
                //    {
                //        //if(pei.Browsable == false)
                //        //    continue;

                //        //enumDesc = pei.DisplayText;
                //        //enumDesc = VerifyEnumDesc(enumDesc, enumValue);

                //        if ((int)enumValue == pei.Index && enumValue.ToString() == enumDesc)
                //        {
                //            defined = true;
                //            break;
                //        }
                //    }

                //    if(!defined)
                //        break;
                //}
                //if(defined)
                //{
                //    //Console.WriteLine(enumType.Name);
                //    return enumType;
                //}

                return enumType;
			}

			enumName += "_" + m_EnumCounter++;
		    return null;
		}
			
		private long m_EnumCounter = 0;

		private ModuleBuilder GetDynamicModule()
		{
			if(m_ModuleBuilder != null)
				return m_ModuleBuilder;

			string strDefineModuleName = "enumTmp";
			// Create a name for the assembly.
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "DynamicEnum";

			AppDomain appDomain = Thread.GetDomain();
			AssemblyBuilderAccess access =  AssemblyBuilderAccess.Run;
			AssemblyBuilder assembly = appDomain.DefineDynamicAssembly(assemblyName, access);
			ModuleBuilder module;
			module = assembly.DefineDynamicModule(strDefineModuleName);

			m_ModuleBuilder = module;
			return m_ModuleBuilder;
		}

        public Type CreateDynamicEnum(CustomModel.CustomProperty customProperty)
		{
			try
			{
				ModuleBuilder module;
				object enumValue;
				string enumDesc;
				FieldBuilder fbe;
				Type tEnum;
				EnumBuilder eb;
				string enumName;

				tEnum = null;
				fbe = null;

                if (customProperty.ComboboxValues == null)
					return tEnum;

				module = GetDynamicModule();
                enumName = String.Format("enum{0}", customProperty.Key);

                tEnum = this.GetEnumType(ref enumName, customProperty);
				if(tEnum != null)
					return tEnum;

				eb = module.DefineEnum(enumName, TypeAttributes.Public, typeof(int));
                int index = 0;
                foreach (CustomModel.CustomPropertyStandardValue pei in customProperty.ComboboxValues)
				{
					try
					{
                        enumValue = index++;
						enumDesc = pei.DisplayText;
						enumDesc = VerifyEnumDesc(enumDesc, enumValue);
					    pei.DisplayText = enumDesc;
						fbe = eb.DefineLiteral(enumDesc, enumValue);
					}
					catch(Exception exc)
					{
						continue;
					}
					finally
					{
					}
				}

				tEnum = eb.CreateType();

				if(!this.m_EnumTypeHashtable.ContainsKey(enumName))
					this.m_EnumTypeHashtable.Add(enumName, tEnum);

				return tEnum;
			}
			catch(Exception exc)
			{
				return null;			}
		}

		private string VerifyEnumDesc(string enumDesc, object enumValue)
		{
            // only secure that desc is filled
            string newEnumDesc = enumDesc.Trim().Replace(",", " ");
			if (enumDesc == "")
                newEnumDesc = "E" + enumValue.ToString().Trim().Replace(",", " ");

			return enumDesc;
		}

	}


}

