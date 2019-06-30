using System.Collections.Generic;
using NitroxClient.GameLogic.InitialSync.Base;
using NitroxModel.Packets;
using Story;

namespace NitroxClient.GameLogic.InitialSync
{
    public class StoryGoalInitialSyncProcessor : InitialSyncProcessor
    {
        
        public override void Process(InitialPlayerSync packet)
        {
            SetCompletedStoryGoals(packet.StoryGoalData.CompletedGoals);
            SetRadioQueue(packet.StoryGoalData.RadioQueue);
        }

        private void SetRadioQueue(List<string> radioQueue)
        {
            StoryGoalManager.main.pendingRadioMessages.AddRange(radioQueue);
            StoryGoalManager.main.PulsePendingMessages();
        }
        
        private void SetCompletedStoryGoals(List<string> storyGoalData)
        {
            // StoryGoalManager.main.OnSceneObjectsLoaded() has already called.
            // So make a new hashset whihc contains only uninitialized goals.
            HashSet<string> uninitializedCompletedGoals = new HashSet<string>();
            foreach (string completedGoal in storyGoalData)
            {
                // Each key must have "OnPlay" prefix.
                // See StoryGoalManager.ExecutePendingRadioMessage().
                string key = "OnPlay" + completedGoal;
               if(!StoryGoalManager.main.completedGoals.Contains(key))
                {
                    uninitializedCompletedGoals.Add(key);
                }
            }

            // Call notifications.
            StoryGoalManager.main.compoundGoalTracker.NotifyGoalComplete(uninitializedCompletedGoals);
            foreach (string completedGoal in uninitializedCompletedGoals)
            {
                StoryGoalManager.main.onGoalUnlockTracker.NotifyGoalComplete(completedGoal);
            }
        }
    }
}
