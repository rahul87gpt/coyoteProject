using Coyote.Console.App.Services.IServices;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.Services
{
    public class HangfireJobs : IHangfireJobs
    {

        ISchedulerService scheduler=null;

        public HangfireJobs(ISchedulerService services)
        {
            scheduler = services;
        }
        public void CallScheduler()
        {
         
            RecurringJob.AddOrUpdate(
                () => scheduler.GetSchedulerInQueue(),
#pragma warning disable CS0618 // Type or member is obsolete
                Cron.MinuteInterval(5));
#pragma warning restore CS0618 // Type or member is obsolete
        }


        public void CallTestScheduler()
        {
            RecurringJob.AddOrUpdate(
                () => scheduler.TestHangfire(),
#pragma warning disable CS0618 // Type or member is obsolete
                Cron.MinuteInterval(5));
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
