﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UntisNotifier.Abstractions.Models;

namespace UntisNotifier
{
    class Program
    {
        public static string WeekDate { get; private set; }
        static async Task Main(string[] args)
        {
            if (args.Count() > 0)
            {
                WeekDate = args[0].ToString();
            }

            var configurator = new Configurator("UntisNotifier.json");

            var client = configurator.GetWebUntisClient();
            var isLoggedIn = await client.LoginAsync();
            if (isLoggedIn)
            {
                var lessons = await client.GetWeeklyLessonsAsync();

                if (String.IsNullOrWhiteSpace(WeekDate))
                {
                    lessons = lessons.Where(c => c.StartTime.Date >= DateTime.Today.Date).ToList();
                }


                var todayLessons = lessons.Any(c => c.StartTime.Date == DateTime.Today.Date);

                List<Lesson> abnormalLessons;
                if (todayLessons)
                {
                    //Get all changes
                    abnormalLessons = await client.GetAbnormalLessons(lessons);
                }
                else
                {
                    //Get only new changes
                    abnormalLessons = await client.GetAbnormalLessons(lessons, true);
                }


                if (abnormalLessons?.Count > 0)
                {
                    var tasks = new List<Task>();
                    configurator.Notifiers.ForEach(n => tasks.Add(Task.Factory.StartNew(() => n.Notify(abnormalLessons))));
                    await Task.WhenAll(tasks.ToArray());
                }
            }
        }
    }
}