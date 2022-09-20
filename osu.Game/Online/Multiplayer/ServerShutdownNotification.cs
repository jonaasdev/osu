// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using Humanizer.Localisation;
using osu.Framework.Allocation;
using osu.Game.Overlays.Notifications;
using osu.Game.Utils;

namespace osu.Game.Online.Multiplayer
{
    public class ServerShutdownNotification : SimpleNotification
    {
        private readonly DateTimeOffset endDate;

        public ServerShutdownNotification(TimeSpan duration)
        {
            endDate = DateTimeOffset.UtcNow + duration;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            updateTime();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            Scheduler.Add(updateTimeWithReschedule);
        }

        private void updateTimeWithReschedule()
        {
            updateTime();

            // The remaining time on a countdown may be at a fractional portion between two seconds.
            // We want to align certain audio/visual cues to the point at which integer seconds change.
            // To do so, we schedule to the next whole second. Note that scheduler invocation isn't
            // guaranteed to be accurate, so this may still occur slightly late, but even in such a case
            // the next invocation will be roughly correct.
            double timeToNextSecond = endDate.Subtract(DateTimeOffset.UtcNow).TotalMilliseconds % 1000;

            Scheduler.AddDelayed(updateTimeWithReschedule, timeToNextSecond);
        }

        private void updateTime() => Text = $"The multiplayer server is restarting in {HumanizerUtils.Humanize(endDate.Subtract(DateTimeOffset.Now), precision: 2, minUnit: TimeUnit.Second)}.";
    }
}
