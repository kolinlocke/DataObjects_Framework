using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DataObjects_Framework.Objects
{
    public class ClsThreadStarter
    {
        #region _Events

        public delegate void Ds_Error(String Msg);
        public event Ds_Error Ev_Error;

        #endregion

        #region _Variables

        Thread mThread;
        Delegate mMethod;
        Object[] mArgs;
        Object mTargetClass;

        #endregion

        #region _Methods

        public void Setup(Delegate MethodObj, Object[] Args, Object TargetClass)
        {
            this.mMethod = MethodObj;
            this.mArgs = Args;
            this.mTargetClass = TargetClass;

            this.mThread = new Thread(new ThreadStart(ExecuteMethod));
            this.mThread.Name = @"ClsThreadStarter_" + this.mMethod.Method.Name;
        }

        public void Start()
        {
            if (this.mThread != null)
            { this.mThread.Start(); }
        }

        void ExecuteMethod()
        {
            try
            { this.mMethod.Method.Invoke(this.mTargetClass, System.Reflection.BindingFlags.InvokeMethod, null, this.mArgs, null); }
            catch (Exception Ex)
            {
                if (this.Ev_Error != null)
                {
                    try { this.Ev_Error(@"Error Occured (" + this.mMethod.Method.Name + "): " + Ex.InnerException.Message); }
                    catch { throw new Exception("ClsThreadStarter: Unexpected error happened."); }
                }
            }
        }

        #endregion

        #region _Properties

        public Thread pThread
        {
            get { return this.mThread; }
        }

        public Object pTargetClass
        {
            get { return this.mTargetClass; }
        }

        #endregion
    }
}
