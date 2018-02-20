using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Application
{
    public interface IRoutine
    {
    }
    public interface IInstallRoutine : IRoutine
    {
        void OnInstall(params string[] args);
    }
    public interface IInitializeRoutine : IRoutine
    {
        void OnInitialize(params string[] args);
    }
    public interface IOnStartRoutine : IRoutine
    {
        void OnStart();
    }
    public interface IOnStopRoutine : IRoutine
    {
        void OnStop();
    }

}
