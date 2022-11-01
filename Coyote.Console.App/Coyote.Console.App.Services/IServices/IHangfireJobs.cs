using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.Services.IServices
{
    public interface IHangfireJobs
    {
        public void CallScheduler();
        public void CallTestScheduler();
    }
}
