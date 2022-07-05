using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UDO.LOB.Extensions.Logging;

namespace UDO.LOB.Extensions 
{
    public class MethodInfo
    {
        public string Namespace { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        private MethodInfo CallerMethod { get; set; }

        public MethodInfo()
        {
        }

        /// <summary>
        /// MethodInfo Constructor: Create MethodInfo using specific values
        /// </summary>
        /// <param name="mNamespace">The Namespace</param>
        /// <param name="mClass">The Class name</param>
        /// <param name="mMethod">The Method name</param>
        public MethodInfo(string mNamespace, string mClass, string mMethod)
        {
            Namespace = mNamespace;
            Class = mClass;
            Method = mMethod;
        }

        /// <summary>
        /// MethodInfo Constructor: Create MethodInfo using Reflected Method
        /// </summary>
        /// <param name="method"></param>
        public MethodInfo(MethodBase method)
        {
            Namespace = method.ReflectedType.Namespace;
            Class = method.ReflectedType.Name;
            Method = method.Name;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}.{2}", Namespace, Class, Method);
        }

        public string ToString(bool ignoreNamespace)
        {
            if (ignoreNamespace) return String.Format("{0}.{1}", Class, Method);
            return this.ToString();
        }

        public string GetClassCallerPath(bool ignoreParentNameSpace, bool ignoreChildNameSpace)
        {
            var parent = this.ToString(ignoreParentNameSpace);
            if (CallerMethod == null) return parent;
            var child = CallerMethod.ToString(ignoreChildNameSpace);
            return parent+":"+child;
        }

        /// <summary>
        /// ShareNamespaceAndClass: Determine if the method info passed shares the namespace and class
        /// </summary>
        /// <param name="mi">MethodInfo to compare</param>
        /// <returns></returns>
        public bool SharesNamespaceAndClass(MethodInfo mi)
        {
            if (mi == null) return false;
            if (this.Namespace != mi.Namespace ||
                this.Class != mi.Class) return false;
            return true;
        }

        /// <summary>
        /// ShareNamespaceAndClass: Determine if the two methods share the same namespace and class
        /// </summary>
        /// <param name="methodInfo1"></param>
        /// <param name="methodInfo2"></param>
        /// <returns></returns>
        public static bool SharesNamespaceAndClass(MethodInfo methodInfo1, MethodInfo methodInfo2)
        {
            if (methodInfo1 == null && methodInfo2 == null) return true;
            if (methodInfo1 == null || methodInfo2 == null) return false;
            return methodInfo1.Equals(methodInfo2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var mi = obj as MethodInfo;
            if (mi == null || this.Method == null || mi.Method == null || !SharesNamespaceAndClass(mi) || this.Method != mi.Method) return false;
            return true;
        }

        /// <summary>
        /// GetThisMethod: Get this method's Namespace, Class, and MethodName
        /// </summary>
        /// <param name="position">position: the position on the callstack.  Default is 1, the method calling this method.</param>
        /// <returns></returns>
        public static MethodInfo GetThisMethod(int position=1)
        {
            try
            {
                var stackTrace = new StackTrace();
                MethodBase method = stackTrace.GetFrame(position).GetMethod();
                return new MethodInfo(method);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("UDO", Guid.Empty, "GetThisMethod", ex.Message);
                return new MethodInfo("Namespace", "Class", "Method");
            }
        }

        /// <summary>
        /// GetCallingMethod: Get the calling method information (Namespace, Class, and Method Name)
        /// if outsideOfNameSpaceAndClass is specified as true, it will traverse the Call Stack
        /// to get the method that called the first method in the class calling GetCallingMethod.
        /// 
        /// Example: Calling this method from class Child would return the Parent class method that called
        /// the Child class method because Parent class is not the same class or namespace as the Child method.
        /// </summary>
        /// <param name="outsideOfNameSpaceAndClass">If true, get the Parent class method that called the Child class method (that calls this method)</param>
        /// <returns></returns>
        public static MethodInfo GetCallingMethod(bool outsideOfNameSpaceAndClass = false)
        {
            try
            {
                var stackTrace = new StackTrace();

                if (!outsideOfNameSpaceAndClass)
                {
                    return new MethodInfo(stackTrace.GetFrame(2).GetMethod());
                }

                var callerMethod = new MethodInfo(stackTrace.GetFrame(1).GetMethod());

                for (int stackPos = 2; stackPos < stackTrace.FrameCount; stackPos++)
                {
                    var currentMethod = new MethodInfo(stackTrace.GetFrame(stackPos).GetMethod());
                    if (!callerMethod.SharesNamespaceAndClass(currentMethod))
                    {
                        currentMethod.CallerMethod = callerMethod;
                        return currentMethod;
                    }
                }
                return null;//no caller found
            }
            catch (Exception ex)
            {
                LogHelper.LogError("UDO", Guid.Empty, "GetCallingMethod", ex.Message);
                return new MethodInfo("Namespace", "Class", "Method");
            }
        }
    }
}
