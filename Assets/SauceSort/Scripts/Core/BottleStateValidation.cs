using System.Collections.Generic;
using System.Text;
using SauceSort.Core;

namespace Assets.SauceSort.Scripts.Core
{
    /// <summary>
    /// Small callable examples for checking the core pour rules without a scene
    /// or Unity test package. RunExamples returns each scenario's result.
    /// </summary>
    public static class BottleStateValidation
    {
        public static bool RunExamples(out string report)
        {
            var results = new List<string>();

            bool allPassed = true;

            BottleState emptySource = new BottleState();

            BottleState emptyDestination = new BottleState();

            allPassed &= Record(!emptySource.CanPourTo(emptyDestination), "An empty source cannot pour.", results);

            BottleState redSource = new BottleState(new[] { SauceType.Red });

            BottleState fullDestination = new BottleState(new[]
            {
                SauceType.Blue, SauceType.Blue, SauceType.Blue, SauceType.Blue
            });

            allPassed &= Record(!redSource.CanPourTo(fullDestination), "A full destination cannot receive a pour.", results);

            BottleState groupedSource = new BottleState(new[]
            {
                SauceType.Blue, SauceType.Red, SauceType.Red
            });

            BottleState matchingDestination = new BottleState(new[] { SauceType.Red });

            int moved;

            bool matchingPour = groupedSource.TryPourTo(matchingDestination, out moved);

            allPassed &= Record(
                matchingPour && moved == 2
                    && groupedSource.Layers.Count == 1
                    && matchingDestination.Layers.Count == 3,
                "A matching destination receives the entire two-layer top group.",
                results);

            BottleState sourceToEmpty = new BottleState(new[]
            {
                SauceType.Green, SauceType.Green
            });

            BottleState emptyTarget = new BottleState();

            bool emptyPour = sourceToEmpty.TryPourTo(emptyTarget, out moved);

            allPassed &= Record(emptyPour && moved == 2 && emptyTarget.GetTopSauce() == SauceType.Green,
                "An empty destination accepts the complete top group.",
                results);

            BottleState blueSource = new BottleState(new[] { SauceType.Blue });

            BottleState redTarget = new BottleState(new[] { SauceType.Red });

            allPassed &= Record(!blueSource.CanPourTo(redTarget), "A different top sauce rejects the pour.", results);

            BottleState twoRedSource = new BottleState(new[]
            {
                SauceType.Blue, SauceType.Red, SauceType.Red
            });
            BottleState oneSlotTarget = new BottleState(new[]
            {
                SauceType.Red, SauceType.Red, SauceType.Red
            });

            allPassed &= Record(!twoRedSource.TryPourTo(oneSlotTarget, out moved) && moved == 0 && twoRedSource.Layers.Count == 3 && oneSlotTarget.Layers.Count == 3,
                "An overlarge top group is rejected without partial changes.", results);

            var builder = new StringBuilder();
            for (int i = 0; i < results.Count; i++)
            {
                if (i > 0)
                {
                    builder.AppendLine();
                }

                builder.Append(results[i]);
            }

            report = builder.ToString();
            return allPassed;
        }

        private static bool Record(bool passed, string description, IList<string> results)
        {
            results.Add((passed ? "PASS: " : "FAIL: ") + description);
            return passed;
        }
    }
}
