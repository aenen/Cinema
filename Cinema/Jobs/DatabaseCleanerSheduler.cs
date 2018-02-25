using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cinema.Jobs
{
    public class DatabaseCleanerSheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<DatabaseCleaner>().Build();

            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                            .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                            .StartNow()                            // запуск сразу после начала выполнения
                            .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                                .WithIntervalInMinutes(2)          // через 2 минуту
                                .RepeatForever())                   // бесконечное повторение
                            .Build();                               // создаем триггер

            scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
        }
    }
}