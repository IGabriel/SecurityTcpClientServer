using System;

namespace ClientServerLibrary
{
    public class ExceptionHandler : LogObject
    {
        public Action<Exception> CatchAction { get; set; }
        public Action FinallyAction { get; set; }
        public bool ThrowOut { get; set;}

        public void Execute(Action tryToDo)
        {
            Execute(tryToDo, ThrowOut);
        }

        public void Execute(Action tryToDo, bool throwOut)
        {
            Execute(tryToDo, throwOut, CatchAction, FinallyAction);
        }

        public void Execute(Action tryToDo, bool throwOut, Action<Exception> catchAction, Action finallyAction)
        {
            if (tryToDo == null)
            {
                throw new ArgumentNullException(paramName: nameof(tryToDo));
            }

            try
            {
                tryToDo();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Rise exception in method: {0}.", tryToDo.Method.Name), ex);

                if (catchAction != null)
                {
                    catchAction(ex);
                }                

                if (throwOut)
                {
                    throw;
                }

                if (catchAction == null && !throwOut)
                {
                    Logger.Warn("The exception is ignored!");
                }
            }
            finally
            {
                if (finallyAction != null)
                {
                    finallyAction();
                }
            }
        }
    }
}