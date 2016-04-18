using System;
using System.Data;
using System.Configuration;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace CardWarWEB
{
    public class Evaluator
    {
        private CSharpCodeProvider cCodeProder = null;
        private CompilerParameters compPars = null;
        private CompilerResults compResult = null;

        public Evaluator()
        {
            cCodeProder = new CSharpCodeProvider();
        }

        /// <summary>
        /// 执行指定的代码
        /// </summary>
        /// <param name="strCodes"></param>
        /// <returns></returns>
        public object Eval(string strCodes, string functionName, out string strErrText)
        {
            #region 编译代码
            strErrText = InitCompile(ref strCodes);
            if (strErrText != null) return null;
            try
            {
                compResult = cCodeProder.CompileAssemblyFromSource(compPars, new string[] { strCodes });
            }
            catch (NotImplementedException nie)
            {
                strErrText = nie.Message;
                return null;
            }

            if (compResult.Errors.HasErrors)
            {
                StringBuilder sbErrs = new StringBuilder(strCodes + System.Environment.NewLine);
                sbErrs.Append("语法错误：" + System.Environment.NewLine);
                foreach (CompilerError err in compResult.Errors)
                {
                    sbErrs.AppendFormat("{0}，{1}" + System.Environment.NewLine, err.ErrorNumber, err.ErrorText);
                }
                strErrText = sbErrs.ToString();
                return null;
            }
            #endregion

            Assembly assembly = compResult.CompiledAssembly;
            object prgInsl = assembly.CreateInstance("EvalClass._ClassEvaluatorCompiler");
            MethodInfo medInfo = prgInsl.GetType().GetMethod(functionName);
            try
            {
                object obj = medInfo.Invoke(prgInsl, null);
                return obj;
            }
            catch (Exception exMsg)
            {
                strErrText = exMsg.Message;
                strErrText += exMsg.StackTrace;
                return null;
            }
        }

        /// <summary>
        /// 预编译代码
        /// </summary>
        /// <param name="strCodes">待编译的源代码</param>
        /// <returns>如果错误则返回错误消息</returns>
        private string InitCompile(ref string strCodes)
        {
            List<string> lstRefs = new List<string>();//代码字符串中的include引用程序集------未使用

            List<string> lstUsings = new List<string>();//代码字符串中的using引用命名空间


            #region 分离引用的程序集与命名空间
            int point = 0;
            string strTemp;
            char[] cCodes = strCodes.ToCharArray();
            for (int i = 0; i < cCodes.Length; ++i)
            {
                if (cCodes[i] == '\n' || (cCodes[i] == '\r' && cCodes[i + 1] == '\n'))
                {
                    strTemp = strCodes.Substring(point, i - point);
                    if (strTemp.TrimStart(new char[] { ' ' }).StartsWith("using "))
                    {
                        strTemp = strTemp.Substring(6).Trim();
                        if (!lstUsings.Contains(strTemp))
                        {
                            lstUsings.Add(strTemp);
                        }
                        else
                        {
                            return "预编译失败，代码中不允许包含重复命名空间导入。" + System.Environment.NewLine + "using " + strTemp;
                        }
                        point = cCodes[i] == '\n' ? i + 1 : i + 2;
                        ++i;
                    }
                    else if (strTemp.TrimStart(new char[] { ' ' }).StartsWith("include "))
                    {
                        strTemp = strTemp.Substring(8).Trim().ToLower();
                        if (!lstRefs.Contains(strTemp))
                        {
                            lstUsings.Add(strTemp);
                        }
                        else
                        {
                            return "预编译失败，代码中不允许包含重复的程序集引用。" + System.Environment.NewLine + "include " + strTemp;
                        }
                        point = cCodes[i] == '\n' ? i + 1 : i + 2;
                        ++i;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            strCodes = strCodes.Substring(point);
            #endregion

            #region 初始化编译参数
            if (compPars == null)
            {
                compPars = new CompilerParameters();
                compPars.GenerateExecutable = false;
                compPars.GenerateInMemory = true;
            }

            compPars.ReferencedAssemblies.Clear();
            compPars.ReferencedAssemblies.Add("System.dll");
            compPars.ReferencedAssemblies.Add("System.Data.dll");
            compPars.ReferencedAssemblies.Add("System.Data.Linq.dll");
            compPars.ReferencedAssemblies.Add("System.Xml.dll");
            compPars.ReferencedAssemblies.Add("System.Web.dll");
            compPars.ReferencedAssemblies.Add(localConf.appEnv.ApplicationBasePath+ "/../../MainSpaceConfigs/bin/Release/MainSpaceConfigs.dll");

            foreach (string str in lstRefs)
            {
                compPars.ReferencedAssemblies.Add(str);
            }
            #endregion

            StringBuilder sbRetn = new StringBuilder();

            #region 生成代码模板

            /*拼接代码*/
            foreach (string str in lstUsings)
            {
                sbRetn.AppendLine("using " + str);
            }
            sbRetn.AppendLine("using System;");
            sbRetn.AppendLine("using System.Data;");
            sbRetn.AppendLine("using System.Collections.Generic;");
            sbRetn.AppendLine("using System.IO;");
            sbRetn.AppendLine("using System.Threading;");
            sbRetn.AppendLine("using System.Text;");
            sbRetn.AppendLine("using CardWarWEB;");
            sbRetn.AppendLine("namespace EvalClass{");
            sbRetn.AppendLine("public class _ClassEvaluatorCompiler{");
            sbRetn.AppendLine(strCodes);
            sbRetn.AppendLine("}");
            sbRetn.AppendLine("}");
            #endregion
            strCodes = sbRetn.ToString();
            return null;
        }
    }
}
