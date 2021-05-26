using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GroupCollectionTests
{
    public class GroupCollectionTests_AddFollowerToCollection_CorrectlyAddsFollowersToGroupCollection : ArrangeActAssertStrategy
    {
        private EvacuAgentPedestrianBase evacuAgentPedestrianBase;
        private GroupCollection groupCollection;
        private List<EvacuAgentPedestrianBase> groupToAddToGroupCollection;
        private List<EvacuAgentPedestrianBase> expectedGroupMembers;
        private List<EvacuAgentPedestrianBase> actualGroupMembers;
        private int expectedGroupCollectionMemberCount;

        [UnityTest]
        public override IEnumerator PerformTest()
        {
            Arrange();
            yield return null;
            Act();
            Assertion();
        }

        public override void Arrange()
        {
            evacuAgentPedestrianBase = SpawnFriendGroupOfEvacuAgentPedestrians(1).First();
            groupCollection = evacuAgentPedestrianBase.GroupCollection;

            groupToAddToGroupCollection = SpawnFriendGroupOfEvacuAgentPedestrians(3);

            // This is 4 as the group collection "group" contains the leader + the followers. So above we have 1 leader and 3 followers
            expectedGroupCollectionMemberCount = 4;

            expectedGroupMembers = new List<EvacuAgentPedestrianBase> { evacuAgentPedestrianBase };
            expectedGroupMembers.AddRange(groupToAddToGroupCollection);
        }

        public override void Act()
        {
            foreach(EvacuAgentPedestrianBase evacuAgentPedestrianBase in groupToAddToGroupCollection)
            {
                groupCollection.AddFollowerToCollection(evacuAgentPedestrianBase);
            }

            actualGroupMembers = groupCollection.GetGroupMembers();
        }

        public override void Assertion()
        {
            Assert.AreEqual(expectedGroupCollectionMemberCount, actualGroupMembers.Count);

            for(int index = 0; index < actualGroupMembers.Count; index++)
            {
                Assert.AreEqual(expectedGroupMembers[index], actualGroupMembers[index]);
            }
        }
    }

    public class GroupCollectionTests_HasNewFollowerBeenAdded_ResetHasNewFollowerBeenAdded_CorrectlyToggleHasFollowerBeenAdded : ArrangeActAssertStrategy
    {
        private List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases;
        private GroupCollection groupCollection;
        private EvacuAgentPedestrianBase followerToAddToCollection;
        private bool hasNewMemberBeenAddedInitial;
        private bool hasNewMemberBeenAddedAfterMemberIsAdded;
        private bool hasNewMemberBeenAddedAfterReset;

        [UnityTest]
        public override IEnumerator PerformTest()
        {
            Arrange();
            yield return null;
            Act();
            Assertion();
        }

        public override void Arrange()
        {
            evacuAgentPedestrianBases = SpawnFriendGroupOfEvacuAgentPedestrians(2);
            groupCollection = evacuAgentPedestrianBases[0].GroupCollection;
            followerToAddToCollection = evacuAgentPedestrianBases[1];
        }

        public override void Act()
        {
            // Inital
            groupCollection.ResetHasNewFollowerBeenAdded();
            hasNewMemberBeenAddedInitial = groupCollection.HasNewFollowerBeenAdded();

            // After a member is added
            groupCollection.AddFollowerToCollection(followerToAddToCollection);
            hasNewMemberBeenAddedAfterMemberIsAdded = groupCollection.HasNewFollowerBeenAdded();

            // After reset
            groupCollection.ResetHasNewFollowerBeenAdded();
            hasNewMemberBeenAddedAfterReset = groupCollection.HasNewFollowerBeenAdded();
        }

        public override void Assertion()
        {
            Assert.IsFalse(hasNewMemberBeenAddedInitial);
            Assert.IsTrue(hasNewMemberBeenAddedAfterMemberIsAdded);
            Assert.IsFalse(hasNewMemberBeenAddedAfterReset);
        }
    }

    public class GroupCollectionTests_UpdatePath_CorrectlyAddsNewPath_ClearingOldPath_AndSettingNewDestination : ArrangeActAssertStrategy
    {
        private List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases;
        private GroupCollection groupCollection;
        private List<Vector3> pathToAdd;
        private Vector3 expectedNewDestination;
        private Vector3 actualNewDestination;
        private int expectedInitialPathNodeCount;
        private int actualInitialPathNodeCount;
        private int expectedPathNodeCountAfterMethodCall;
        private int actualPathNodeCountAfterMethodCall;

        [UnityTest]
        public override IEnumerator PerformTest()
        {
            Arrange();
            yield return null;
            Act();
            Assertion();
        }

        public override void Arrange()
        {
            evacuAgentPedestrianBases = SpawnFriendGroupOfEvacuAgentPedestrians(2);
            groupCollection = evacuAgentPedestrianBases[0].GroupCollection;

            pathToAdd = new List<Vector3>
            {
                new Vector3(-5.25f, 0.25f, 3.5f),
                new Vector3(-10.5f, 0.25f, -2f),
                new Vector3(7.5f, 0.25f, 1.5f),
            };

            expectedInitialPathNodeCount = 0;
            expectedPathNodeCountAfterMethodCall = 3;

            expectedNewDestination = pathToAdd[0];
        }

        public override void Act()
        {
            actualInitialPathNodeCount = groupCollection.GetNumberOfPathNodes();
            groupCollection.UpdatePath(pathToAdd);
            actualPathNodeCountAfterMethodCall = groupCollection.GetNumberOfPathNodes();
            actualNewDestination = groupCollection.GroupDestination;
        }

        public override void Assertion()
        {
            Assert.AreEqual(expectedInitialPathNodeCount, actualInitialPathNodeCount);
            Assert.AreEqual(expectedPathNodeCountAfterMethodCall, actualPathNodeCountAfterMethodCall);
            AssertTwoVectorsAreEqualWithinTolerance(actualNewDestination, expectedNewDestination, floatingPointTolerance);
        }
    }

    public class GroupCollectionTests_HasGroupVisitedCurrentPathNode_MarkCurrentDestinationAsVisited_CorrectlyTogglesVisitedStateOfCurrentDestination : ArrangeActAssertStrategy
    {
        private List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases;
        private GroupCollection groupCollection;
        private List<Vector3> pathToAdd;
        private bool visitedStateOfCurrentDestinationInitial;
        private bool visitedStateOfCurrentDestinationAfterVisited;
        private bool visitedStateOfCurrentDestinationAfterUpdate;

        [UnityTest]
        public override IEnumerator PerformTest()
        {
            Arrange();
            yield return null;
            Act();
            Assertion();
        }

        public override void Arrange()
        {
            evacuAgentPedestrianBases = SpawnFriendGroupOfEvacuAgentPedestrians(2);
            groupCollection = evacuAgentPedestrianBases[0].GroupCollection;

            pathToAdd = new List<Vector3>
            {
                new Vector3(-5.25f, 0.25f, 3.5f),
                new Vector3(-10.5f, 0.25f, -2f),
                new Vector3(7.5f, 0.25f, 1.5f),
            };
        }

        public override void Act()
        {
            groupCollection.UpdatePath(pathToAdd);

            // Initial
            visitedStateOfCurrentDestinationInitial = groupCollection.HasGroupVisitedCurrentPathNode();

            // After current destination is visited
            groupCollection.MarkCurrentDestinationAsVisited();
            visitedStateOfCurrentDestinationAfterVisited = groupCollection.HasGroupVisitedCurrentPathNode();

            // After current destination is updated
            groupCollection.UpdateGroupDestination();
            visitedStateOfCurrentDestinationAfterUpdate = groupCollection.HasGroupVisitedCurrentPathNode();
        }

        public override void Assertion()
        {
            Assert.IsFalse(visitedStateOfCurrentDestinationInitial);
            Assert.IsTrue(visitedStateOfCurrentDestinationAfterVisited);
            Assert.IsFalse(visitedStateOfCurrentDestinationAfterUpdate);
        }
    }

    public class GroupCollectionTests_UpdateGroupDestination_CorrectlyUpdatesDestination_WhenPathHasNextNode : ArrangeActAssertStrategy
    {
        private List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases;
        private GroupCollection groupCollection;
        private List<Vector3> pathToAdd;
        private Vector3 actualNewDestination;
        private Vector3 expectedNewDestination;

        [UnityTest]
        public override IEnumerator PerformTest()
        {
            Arrange();
            yield return null;
            Act();
            Assertion();
        }

        public override void Arrange()
        {
            evacuAgentPedestrianBases = SpawnFriendGroupOfEvacuAgentPedestrians(2);
            groupCollection = evacuAgentPedestrianBases[0].GroupCollection;

            pathToAdd = new List<Vector3>
            {
                new Vector3(-5.25f, 0.25f, 3.5f),
                new Vector3(-10.5f, 0.25f, -2f),
                new Vector3(7.5f, 0.25f, 1.5f),
            };

            expectedNewDestination = pathToAdd[1];
        }

        public override void Act()
        {
            groupCollection.UpdatePath(pathToAdd);
            groupCollection.UpdateGroupDestination();
            actualNewDestination = groupCollection.GroupDestination;
        }

        public override void Assertion()
        {
            AssertTwoVectorsAreEqualWithinTolerance(actualNewDestination, expectedNewDestination, floatingPointTolerance);
        }
    }

    public class GroupCollectionTests_UpdateGroupDestination_SetsShouldUpdatePathToTrue_WhenPathcontainsNoMoreNodes : ArrangeActAssertStrategy
    {
        private List<EvacuAgentPedestrianBase> evacuAgentPedestrianBases;
        private GroupCollection groupCollection;
        private List<Vector3> pathToAdd;
        private bool shouldUpdatePathInitial;
        private bool shouldUpdatePathAfterUpdate;

        [UnityTest]
        public override IEnumerator PerformTest()
        {
            Arrange();
            yield return null;
            Act();
            Assertion();
        }

        public override void Arrange()
        {
            evacuAgentPedestrianBases = SpawnFriendGroupOfEvacuAgentPedestrians(2);
            groupCollection = evacuAgentPedestrianBases[0].GroupCollection;

            pathToAdd = new List<Vector3>
            {
                new Vector3(-5.25f, 0.25f, 3.5f)
            };
        }

        public override void Act()
        {
            groupCollection.UpdatePath(pathToAdd);
            shouldUpdatePathInitial = groupCollection.shouldUpdatePath;

            groupCollection.UpdateGroupDestination();
            shouldUpdatePathAfterUpdate = groupCollection.shouldUpdatePath;
        }

        public override void Assertion()
        {
            Assert.IsFalse(shouldUpdatePathInitial);
            Assert.IsTrue(shouldUpdatePathAfterUpdate);
        }
    }
}
