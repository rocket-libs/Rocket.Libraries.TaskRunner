using Rocket.Libraries.TaskRunner.Schedules;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Rocket.Libraries.TaskRunner.OnDemandQueuing
{
    public class OnDemandQueueManager<TIdentifier> : IOnDemandQueueManager<TIdentifier>
    {
        private readonly object queueLock = new object();

        private ImmutableList<TIdentifier> onDemandQueue;

        private ImmutableList<TIdentifier> OnDemandQueue
        {
            get
            {
                if (onDemandQueue == null)
                {
                    onDemandQueue = ImmutableList<TIdentifier>.Empty;
                }
                return onDemandQueue;
            }
        }

        public ImmutableList<TIdentifier> Queued
        {
            get
            {
                lock (queueLock)
                {
                    return OnDemandQueue;
                }
            }
        }

        public void DeQueue(TIdentifier taskDefinitionId)
        {
            lock (queueLock)
            {
                onDemandQueue = onDemandQueue.Remove(taskDefinitionId);
            }
        }

        public void Queue(TIdentifier taskDefinitionId)
        {
            lock (queueLock)
            {
                var alreadyQueued = OnDemandQueue.Any(candidateTaskDefinitionId => EqualityComparer<TIdentifier>.Default.Equals(candidateTaskDefinitionId, taskDefinitionId));
                if (alreadyQueued)
                {
                    return;
                }
                else
                {
                    onDemandQueue = onDemandQueue.Add(taskDefinitionId);
                }
            }
        }

        public ImmutableList<ISchedule<TIdentifier>> GetOnDemandSchedulesFlagged(ImmutableList<ISchedule<TIdentifier>> schedules, ImmutableList<TIdentifier> onDemandTaskDefinitionIds)
        {
            foreach (var taskDefinitionId in onDemandTaskDefinitionIds)
            {
                var targetSchedule = schedules.First(candidateSchedule => EqualityComparer<TIdentifier>.Default.Equals(taskDefinitionId, candidateSchedule.TaskDefinitionId));
                targetSchedule.IsOnDemand = true;
            }
            return schedules;
        }

        public void DeQueueAll()
        {
            lock (queueLock)
            {
                var queued = Queued;
                foreach (var taskDefinitionId in queued)
                {
                    DeQueue(taskDefinitionId);
                }
            }
        }
    }
}