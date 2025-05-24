using StardewValleyFOMO.Lib;
using System.Collections.Generic;

namespace StardewValleyFOMO.Lib.Test
{
    public class BundleTests
    {
        [Test]
        public void Constructor_WithValidBundleData_ParsesCorrectly()
        {
            // Arrange
            var bundleData = new KeyValuePair<string, string>("Pantry/0", "Spring Crops/O 495 1/495 1 0 496 1 0 497 1 0 498 1 0/0/1//Spring Crops");

            // Act
            var bundle = new Bundle(bundleData);

            // Assert
            Assert.That(bundle.AreaId, Is.EqualTo(0));
            Assert.That(bundle.BundleKey, Is.EqualTo("Pantry")); // This is based on the current implementation (keyParts[0])
            Assert.That(bundle.BundleDefinition, Is.EqualTo("Spring Crops/O 495 1/495 1 0 496 1 0 497 1 0 498 1 0/0/1//Spring Crops"));
            Assert.That(bundle.ItemCountToComplete, Is.EqualTo(1));
            Assert.That(bundle.Items, Has.Count.EqualTo(4));
            Assert.Multiple(() =>
            {
                Assert.That(bundle.Items[0].Id, Is.EqualTo(495));
                Assert.That(bundle.Items[0].Count, Is.EqualTo(1));
                Assert.That(bundle.Items[0].MinQuality, Is.EqualTo(0));

                Assert.That(bundle.Items[1].Id, Is.EqualTo(496));
                Assert.That(bundle.Items[1].Count, Is.EqualTo(1));
                Assert.That(bundle.Items[1].MinQuality, Is.EqualTo(0));

                Assert.That(bundle.Items[2].Id, Is.EqualTo(497));
                Assert.That(bundle.Items[2].Count, Is.EqualTo(1));
                Assert.That(bundle.Items[2].MinQuality, Is.EqualTo(0));

                Assert.That(bundle.Items[3].Id, Is.EqualTo(498));
                Assert.That(bundle.Items[3].Count, Is.EqualTo(1));
                Assert.That(bundle.Items[3].MinQuality, Is.EqualTo(0));
            });
        }

        [Test]
        public void Constructor_WithAdventurersBundleData_ParsesCorrectly()
        {
            // Arrange
            // Bundle Key: "Boiler Room/22" -> Definition: "Adventurer's/R 518 1/766 99 0 767 10 0 768 1 0 769 10/1/2//Adventurer's"
            var bundleData = new KeyValuePair<string, string>("Boiler Room/22", "Adventurer's/R 518 1/766 99 0 767 10 0 768 1 0 769 10/1/2//Adventurer's");

            // Act
            var bundle = new Bundle(bundleData);

            // Assert
            Assert.That(bundle.AreaId, Is.EqualTo(22));
            Assert.That(bundle.BundleKey, Is.EqualTo("Boiler Room")); // This is based on the current implementation (keyParts[0])
            Assert.That(bundle.BundleDefinition, Is.EqualTo("Adventurer's/R 518 1/766 99 0 767 10 0 768 1 0 769 10/1/2//Adventurer's"));
            Assert.That(bundle.ItemCountToComplete, Is.EqualTo(2));
            Assert.That(bundle.Items, Has.Count.EqualTo(4));
            Assert.Multiple(() =>
            {
                Assert.That(bundle.Items[0].Id, Is.EqualTo(766));
                Assert.That(bundle.Items[0].Count, Is.EqualTo(99));
                Assert.That(bundle.Items[0].MinQuality, Is.EqualTo(0));

                Assert.That(bundle.Items[1].Id, Is.EqualTo(767));
                Assert.That(bundle.Items[1].Count, Is.EqualTo(10));
                Assert.That(bundle.Items[1].MinQuality, Is.EqualTo(0));

                Assert.That(bundle.Items[2].Id, Is.EqualTo(768));
                Assert.That(bundle.Items[2].Count, Is.EqualTo(1));
                Assert.That(bundle.Items[2].MinQuality, Is.EqualTo(0));

                Assert.That(bundle.Items[3].Id, Is.EqualTo(769));
                Assert.That(bundle.Items[3].Count, Is.EqualTo(10));
                Assert.That(bundle.Items[3].MinQuality, Is.EqualTo(0));
            });
        }

        [Test]
        public void Constructor_ItemCountToCompleteIsDifferent_ParsesCorrectly()
        {
            // Arrange
            var bundleData = new KeyValuePair<string, string>("Pantry/1", "Quality Crops/O 24 5/24 5 1 26 5 1 28 5 1/0/3//Quality Crops");

            // Act
            var bundle = new Bundle(bundleData);

            // Assert
            Assert.That(bundle.ItemCountToComplete, Is.EqualTo(3));
            Assert.That(bundle.Items, Has.Count.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(bundle.Items[0].Id, Is.EqualTo(24));
                Assert.That(bundle.Items[0].Count, Is.EqualTo(5));
                Assert.That(bundle.Items[0].MinQuality, Is.EqualTo(1));

                Assert.That(bundle.Items[1].Id, Is.EqualTo(26));
                Assert.That(bundle.Items[1].Count, Is.EqualTo(5));
                Assert.That(bundle.Items[1].MinQuality, Is.EqualTo(1));

                Assert.That(bundle.Items[2].Id, Is.EqualTo(28));
                Assert.That(bundle.Items[2].Count, Is.EqualTo(5));
                Assert.That(bundle.Items[2].MinQuality, Is.EqualTo(1));
            });
        }
    }
}
