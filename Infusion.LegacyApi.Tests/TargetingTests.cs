﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.LegacyApi.Tests.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class TargetingTests
    {
        [TestMethod]
        public void Can_wait_for_target_When_no_last_action()
        {
            var testProxy = new InfusionTestProxy();
            var task = Task.Run(() => testProxy.Api.WaitForTarget(TimeSpan.MaxValue));
            testProxy.Api.WaitForTargetStartedEvent.WaitOne(100).Should().BeTrue();
            testProxy.ServerPacketHandler.HandlePacket(TargetCursorPackets.TargetCursor);

            task.Wait(100).Should().BeTrue();
        }

        [TestMethod]
        public void Can_wait_for_target_after_last_action_When_TargetCursor_arrives_before_waiting_starts()
        {
            var testProxy = new InfusionTestProxy();

            testProxy.Api.NotifyAction(DateTime.UtcNow.AddMilliseconds(-1));
            testProxy.ServerPacketHandler.HandlePacket(TargetCursorPackets.TargetCursor);

            bool waitResult = false;
            var task = Task.Run(() => waitResult = testProxy.Api.WaitForTarget(TimeSpan.MaxValue));

            task.Wait(100).Should().BeTrue();
            waitResult.Should().BeTrue();
        }

        [TestMethod]
        public void Can_wait_for_target_after_last_action_When_TargetCursor_arrives_before_waiting_starts_and_failure_messages_are_specified()
        {
            var testProxy = new InfusionTestProxy();

            testProxy.Api.NotifyAction(DateTime.UtcNow.AddMilliseconds(-1));
            testProxy.ServerPacketHandler.HandlePacket(TargetCursorPackets.TargetCursor);

            bool waitResult = false;
            var task = Task.Run(() => waitResult = testProxy.Api.WaitForTarget(TimeSpan.MaxValue, "some", "failure", "messages"));

            task.Wait(100).Should().BeTrue();
            waitResult.Should().BeTrue();
        }

        [TestMethod]
        public void Timeouts_when_waiting_for_target_and_TargetCursor_arrives_before_last_action()
        {
            var testProxy = new InfusionTestProxy();

            testProxy.ServerPacketHandler.HandlePacket(TargetCursorPackets.TargetCursor);
            testProxy.Api.NotifyAction(DateTime.UtcNow.AddMilliseconds(1));

            var task = Task.Run(() => testProxy.Api.WaitForTarget(TimeSpan.MaxValue));

            task.Wait(100).Should().BeFalse();
        }

        [TestMethod]
        public void Can_terminate_before_target_because_fail_message_received()
        {
            var testProxy = new InfusionTestProxy();

            testProxy.Api.NotifyAction(DateTime.UtcNow.AddMilliseconds(-1));

            testProxy.ServerPacketHandler.HandlePacket(SpeechPackets.FailureMessageFromServer);

            bool waitResult = true;
            var task = Task.Run(() => waitResult = testProxy.Api.WaitForTarget(TimeSpan.MaxValue, "failure message"));
            task.Wait(100).Should().BeTrue();

            waitResult.Should().BeFalse();
        }
    }
}