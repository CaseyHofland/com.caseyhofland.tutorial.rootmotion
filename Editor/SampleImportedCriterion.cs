using System;
using System.Linq;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace Tutorials.RootMotion
{
    public class SampleImportedCriterion : Criterion
    {
        // !!! THIS CODE IS UNTESTED !!!

        [field: SerializeField] public string packageName { get; set; }
        [field: SerializeField] public string packageVersion { get; set; }
        [field: SerializeField] public string sampleName { get; set; }

        protected override bool EvaluateCompletion() => TryGetSample(out var sample) && sample.isImported;

        public override bool AutoComplete()
        {
            if (!TryGetSample(out var sample))
            {
                return false;
            }

            sample.Import();
            AssetDatabase.SaveAssets();
            return true;
        }

        private bool TryGetSample(out Sample sample)
        {
            try
            {
                var samples = Sample.FindByPackage(packageName, packageVersion);
                sample = samples.Single(sample => sample.displayName == sampleName);
                return true;
            }
            catch (InvalidOperationException)
            {
                sample = default;
                return false;
            }
        }
    }
}
